﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using DiagramDesigner.Simulator;
using Microsoft.Win32;

namespace DiagramDesigner
{
    public partial class DesignerCanvas
    {
        public static RoutedCommand SelectAll = new RoutedCommand();
        public static RoutedCommand StartSimulation = new RoutedCommand();

        public DesignerCanvas()
        {
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.New, New_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, Open_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, Save_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Print, Print_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, Cut_Executed, Cut_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, Copy_Executed, Copy_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, Paste_Executed, Paste_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, Delete_Executed, Delete_Enabled));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.SelectAll, SelectAll_Executed));
            this.CommandBindings.Add(new CommandBinding(DesignerCanvas.StartSimulation, StartSim));
            SelectAll.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Control));
            this.AllowDrop = true;
            Clipboard.Clear();
        }

        #region New Command

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Children.Clear();
            this.SelectionService.ClearSelection();
        }

        #endregion

        #region Open Command

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            XElement root = LoadSerializedDataFromFile();

            if (root == null)
                return;

            this.Children.Clear();
            this.SelectionService.ClearSelection();

            IEnumerable<XElement> itemsXML = root.Elements("DesignerItems").Elements("DesignerItem");
            foreach (XElement itemXML in itemsXML)
            {
                Guid id = new Guid(itemXML.Element("ID").Value);
                DesignerItem item = DeserializeDesignerItem(itemXML, id, 0, 0);
                this.Children.Add(item);
                SetConnectorDecoratorTemplate(item);
            }

            this.InvalidateVisual();

            IEnumerable<XElement> connectionsXML = root.Elements("Connections").Elements("Connection");
            foreach (XElement connectionXML in connectionsXML)
            {
                Guid sourceID = new Guid(connectionXML.Element("SourceID").Value);
                Guid sinkID = new Guid(connectionXML.Element("SinkID").Value);

                String sourceConnectorName = connectionXML.Element("SourceConnectorName").Value;
                String sinkConnectorName = connectionXML.Element("SinkConnectorName").Value;

                Connector sourceConnector = GetConnector(sourceID, sourceConnectorName);
                Connector sinkConnector = GetConnector(sinkID, sinkConnectorName);

                Connection connection = new Connection(sourceConnector, sinkConnector);
                Canvas.SetZIndex(connection, Int32.Parse(connectionXML.Element("zIndex").Value));
                this.Children.Add(connection);
            }
        }

        #endregion

        #region Save Command

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IEnumerable<DesignerItem> designerItems = this.Children.OfType<DesignerItem>();
            IEnumerable<Connection> connections = this.Children.OfType<Connection>();

            XElement designerItemsXML = SerializeDesignerItems(designerItems);
            XElement connectionsXML = SerializeConnections(connections);

            XElement root = new XElement("Root");
            root.Add(designerItemsXML);
            root.Add(connectionsXML);

            SaveFile(root);
        }

        #endregion

        #region Print Command

        private void Print_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectionService.ClearSelection();

            PrintDialog printDialog = new PrintDialog();

            if (true == printDialog.ShowDialog())
            {
                printDialog.PrintVisual(this, "WPF Diagram");
            }
        }

        #endregion

        #region Copy Command

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CopyCurrentSelection();
        }

        private void Copy_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionService.CurrentSelection.Count() > 0;
        }

        #endregion

        #region Paste Command

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            XElement root = LoadSerializedDataFromClipBoard();

            if (root == null)
                return;

            // create DesignerItems
            Dictionary<Guid, Guid> mappingOldToNewIDs = new Dictionary<Guid, Guid>();
            List<ISelectable> newItems = new List<ISelectable>();
            IEnumerable<XElement> itemsXML = root.Elements("DesignerItems").Elements("DesignerItem");

            double offsetX = Double.Parse(root.Attribute("OffsetX").Value, CultureInfo.InvariantCulture);
            double offsetY = Double.Parse(root.Attribute("OffsetY").Value, CultureInfo.InvariantCulture);

            foreach (XElement itemXML in itemsXML)
            {
                Guid oldID = new Guid(itemXML.Element("ID").Value);
                Guid newID = Guid.NewGuid();
                mappingOldToNewIDs.Add(oldID, newID);
                DesignerItem item = DeserializeDesignerItem(itemXML, newID, offsetX, offsetY);
                this.Children.Add(item);
                SetConnectorDecoratorTemplate(item);
                newItems.Add(item);
            }

            // update group hierarchy
            SelectionService.ClearSelection();
            foreach (DesignerItem el in newItems)
            {
                if (el.ParentID != Guid.Empty)
                    el.ParentID = mappingOldToNewIDs[el.ParentID];
            }


            foreach (DesignerItem item in newItems)
            {
                if (item.ParentID == Guid.Empty)
                {
                    SelectionService.AddToSelection(item);
                }
            }

            // create Connections
            IEnumerable<XElement> connectionsXML = root.Elements("Connections").Elements("Connection");
            foreach (XElement connectionXML in connectionsXML)
            {
                Guid oldSourceID = new Guid(connectionXML.Element("SourceID").Value);
                Guid oldSinkID = new Guid(connectionXML.Element("SinkID").Value);

                if (mappingOldToNewIDs.ContainsKey(oldSourceID) && mappingOldToNewIDs.ContainsKey(oldSinkID))
                {
                    Guid newSourceID = mappingOldToNewIDs[oldSourceID];
                    Guid newSinkID = mappingOldToNewIDs[oldSinkID];

                    String sourceConnectorName = connectionXML.Element("SourceConnectorName").Value;
                    String sinkConnectorName = connectionXML.Element("SinkConnectorName").Value;

                    Connector sourceConnector = GetConnector(newSourceID, sourceConnectorName);
                    Connector sinkConnector = GetConnector(newSinkID, sinkConnectorName);

                    Connection connection = new Connection(sourceConnector, sinkConnector);
                    Canvas.SetZIndex(connection, Int32.Parse(connectionXML.Element("zIndex").Value));
                    this.Children.Add(connection);

                    SelectionService.AddToSelection(connection);
                }
            }


            root.Attribute("OffsetX").Value = (offsetX + 10).ToString();
            root.Attribute("OffsetY").Value = (offsetY + 10).ToString();
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Xaml, root);
        }

        private void Paste_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsData(DataFormats.Xaml);
        }

        #endregion

        #region Delete Command

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteCurrentSelection();
        }

        private void Delete_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.SelectionService.CurrentSelection.Count() > 0;
        }

        #endregion

        #region Cut Command

        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CopyCurrentSelection();
            DeleteCurrentSelection();
        }

        private void Cut_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.SelectionService.CurrentSelection.Count() > 0;
        }

        #endregion

        #region SelectAll Command

        private void SelectAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SelectionService.SelectAll();
        }

        #endregion

        #region Helper Methods

        private XElement LoadSerializedDataFromFile()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Designer Files (*.xml)|*.xml|All Files (*.*)|*.*";

            if (openFile.ShowDialog() == true)
            {
                try
                {
                    return XElement.Load(openFile.FileName);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace, e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return null;
        }

        void SaveFile(XElement xElement)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Files (*.xml)|*.xml|All Files (*.*)|*.*";
            if (saveFile.ShowDialog() == true)
            {
                try
                {
                    xElement.Save(saveFile.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.StackTrace, ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private XElement LoadSerializedDataFromClipBoard()
        {
            if (Clipboard.ContainsData(DataFormats.Xaml))
            {
                String clipboardData = Clipboard.GetData(DataFormats.Xaml) as String;

                if (String.IsNullOrEmpty(clipboardData))
                    return null;
                try
                {
                    return XElement.Load(new StringReader(clipboardData));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.StackTrace, e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return null;
        }

        private XElement SerializeDesignerItems(IEnumerable<DesignerItem> designerItems)
        {
            XElement serializedItems = new XElement("DesignerItems",
                                       from item in designerItems
                                       let contentXaml = XamlWriter.Save(((DesignerItem)item).Content)
                                       select new XElement("DesignerItem",
                                                  new XElement("Left", Canvas.GetLeft(item)),
                                                  new XElement("Top", Canvas.GetTop(item)),
                                                  new XElement("Width", item.Width),
                                                  new XElement("Height", item.Height),
                                                  new XElement("ID", item.ID),
                                                  new XElement("zIndex", Canvas.GetZIndex(item)),
                                                  new XElement("IsGroup", item.IsGroup),
                                                  new XElement("ParentID", item.ParentID),
                                                  new XElement("Content", contentXaml)
                                              )
                                   );

            return serializedItems;
        }

        private XElement SerializeConnections(IEnumerable<Connection> connections)
        {
            var serializedConnections = new XElement("Connections",
                           from connection in connections
                           select new XElement("Connection",
                                      new XElement("SourceID", connection.Source.ParentDesignerItem.ID),
                                      new XElement("SinkID", connection.Sink.ParentDesignerItem.ID),
                                      new XElement("SourceConnectorName", connection.Source.Name),
                                      new XElement("SinkConnectorName", connection.Sink.Name),
                                      new XElement("SourceArrowSymbol", connection.SourceArrowSymbol),
                                      new XElement("SinkArrowSymbol", connection.SinkArrowSymbol),
                                      new XElement("zIndex", Canvas.GetZIndex(connection))
                                     )
                                  );

            return serializedConnections;
        }

        private static DesignerItem DeserializeDesignerItem(XElement itemXML, Guid id, double OffsetX, double OffsetY)
        {
            var tag = itemXML.Value.Substring(itemXML.Value.IndexOf("Tag"),20);
            int startI = 0;
            int stopI = 0;
            for (int i = 0; i < tag.Length; i++)
            {
                if (tag[i] == '=')
                {
                    startI = i+2;
                }

                if (startI != 0 && tag[i] == '"' &&i!=startI-1)
                {
                    stopI = i;
                    break;
                }
            }

            var t = tag.Substring(startI, stopI-startI);
            TestManeger.Instance.AddComponent(id,t);
            DesignerItem item = new DesignerItem(id, "Default");
            item.Width = Double.Parse(itemXML.Element("Width").Value, CultureInfo.InvariantCulture);
            item.Height = Double.Parse(itemXML.Element("Height").Value, CultureInfo.InvariantCulture);
            item.ParentID = new Guid(itemXML.Element("ParentID").Value);
            item.IsGroup = Boolean.Parse(itemXML.Element("IsGroup").Value);
            Canvas.SetLeft(item, Double.Parse(itemXML.Element("Left").Value, CultureInfo.InvariantCulture) + OffsetX);
            Canvas.SetTop(item, Double.Parse(itemXML.Element("Top").Value, CultureInfo.InvariantCulture) + OffsetY);
            Canvas.SetZIndex(item, Int32.Parse(itemXML.Element("zIndex").Value));
            Object content = XamlReader.Load(XmlReader.Create(new StringReader(itemXML.Element("Content").Value)));
            item.Content = content;
            return item;
        }

        private void CopyCurrentSelection()
        {
            IEnumerable<DesignerItem> selectedDesignerItems =
                this.SelectionService.CurrentSelection.OfType<DesignerItem>();

            List<Connection> selectedConnections =
                this.SelectionService.CurrentSelection.OfType<Connection>().ToList();

            foreach (Connection connection in this.Children.OfType<Connection>())
            {
                if (!selectedConnections.Contains(connection))
                {
                    DesignerItem sourceItem = (from item in selectedDesignerItems
                                               where item.ID == connection.Source.ParentDesignerItem.ID
                                               select item).FirstOrDefault();

                    DesignerItem sinkItem = (from item in selectedDesignerItems
                                             where item.ID == connection.Sink.ParentDesignerItem.ID
                                             select item).FirstOrDefault();

                    if (sourceItem != null &&
                        sinkItem != null &&
                        BelongToSameGroup(sourceItem, sinkItem))
                    {
                        selectedConnections.Add(connection);
                    }
                }
            }

            XElement designerItemsXML = SerializeDesignerItems(selectedDesignerItems);
            XElement connectionsXML = SerializeConnections(selectedConnections);

            XElement root = new XElement("Root");
            root.Add(designerItemsXML);
            root.Add(connectionsXML);

            root.Add(new XAttribute("OffsetX", 10));
            root.Add(new XAttribute("OffsetY", 10));

            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Xaml, root);
        }

        private void DeleteCurrentSelection()
        {
            foreach (Connection connection in SelectionService.CurrentSelection.OfType<Connection>())
            {
                testManeger.RemoveConnection(connection.Source.ParentDesignerItem.ID,connection.Sink.ParentDesignerItem.ID);
                this.Children.Remove(connection);
            }

            foreach (DesignerItem item in SelectionService.CurrentSelection.OfType<DesignerItem>())
            {
                Control cd = item.Template.FindName("PART_ConnectorDecorator", item) as Control;
                testManeger.RemoveComponent(item.ID);
                List<Connector> connectors = new List<Connector>();
                GetConnectors(cd, connectors);

                foreach (Connector connector in connectors)
                {
                    foreach (Connection con in connector.Connections)
                    {
                        this.Children.Remove(con);
                    }
                }
                this.Children.Remove(item);
            }

            SelectionService.ClearSelection();
            UpdateZIndex();
        }

        private void UpdateZIndex()
        {
            List<UIElement> ordered = (from UIElement item in this.Children
                                       orderby Canvas.GetZIndex(item as UIElement)
                                       select item as UIElement).ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                Canvas.SetZIndex(ordered[i], i);
            }
        }

        private static Rect GetBoundingRectangle(IEnumerable<DesignerItem> items)
        {
            double x1 = Double.MaxValue;
            double y1 = Double.MaxValue;
            double x2 = Double.MinValue;
            double y2 = Double.MinValue;

            foreach (DesignerItem item in items)
            {
                x1 = Math.Min(Canvas.GetLeft(item), x1);
                y1 = Math.Min(Canvas.GetTop(item), y1);

                x2 = Math.Max(Canvas.GetLeft(item) + item.Width, x2);
                y2 = Math.Max(Canvas.GetTop(item) + item.Height, y2);
            }

            return new Rect(new Point(x1, y1), new Point(x2, y2));
        }

        private void GetConnectors(DependencyObject parent, List<Connector> connectors)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is Connector)
                {
                    connectors.Add(child as Connector);
                }
                else
                    GetConnectors(child, connectors);
            }
        }

        private Connector GetConnector(Guid itemID, String connectorName)
        {
            DesignerItem designerItem = (from item in this.Children.OfType<DesignerItem>()
                                         where item.ID == itemID
                                         select item).FirstOrDefault();

            Control connectorDecorator = designerItem.Template.FindName("PART_ConnectorDecorator", designerItem) as Control;
            connectorDecorator.ApplyTemplate();

            return connectorDecorator.Template.FindName(connectorName, connectorDecorator) as Connector;
        }

        private bool BelongToSameGroup(IGroupable item1, IGroupable item2)
        {
            IGroupable root1 = SelectionService.GetGroupRoot(item1);
            IGroupable root2 = SelectionService.GetGroupRoot(item2);

            return (root1.ID == root2.ID);
        }

        #endregion

        private void StartSim(object sender, ExecutedRoutedEventArgs e)
        {
            testManeger.StartSim();
        }
    }
}
