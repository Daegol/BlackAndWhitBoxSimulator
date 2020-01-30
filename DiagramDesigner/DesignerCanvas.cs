using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;
using DiagramDesigner.Simulator;

namespace DiagramDesigner
{
    public partial class DesignerCanvas : Canvas
    {
        private Point? rubberbandSelectionStartPoint = null;

        private SelectionService selectionService;
        private TestManeger testManeger = TestManeger.Instance;
        internal SelectionService SelectionService => selectionService ?? (selectionService = new SelectionService(this));

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (!Equals(e.Source, this)) return;
            this.rubberbandSelectionStartPoint = new Point?(e.GetPosition(this));
            SelectionService.ClearSelection();
            Focus();
            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton != MouseButtonState.Pressed)
                this.rubberbandSelectionStartPoint = null;

            if (this.rubberbandSelectionStartPoint.HasValue)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    var adorner = new RubberbandAdorner(this, rubberbandSelectionStartPoint);
                    adornerLayer.Add(adorner);
                }
            }
            e.Handled = true;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            if (!(e.Data.GetData(typeof(DragObject)) is DragObject dragObject) ||
                string.IsNullOrEmpty(dragObject.Xaml)) return;
            var content = XamlReader.Load(XmlReader.Create(new StringReader(dragObject.Xaml)));

            if (content != null)
            {
                var c = (Grid)content;
                Guid id = Guid.NewGuid();
                testManeger.AddComponent(id,c.Tag.ToString());
                var newItem = new DesignerItem(id,c.Tag.ToString()) {Content = content};
                var position = e.GetPosition(this);

                if (dragObject.DesiredSize.HasValue)
                {
                    var desiredSize = dragObject.DesiredSize.Value;
                    newItem.Width = desiredSize.Width;
                    newItem.Height = desiredSize.Height;

                    DesignerCanvas.SetLeft(newItem, Math.Max(0, position.X - newItem.Width / 2));
                    DesignerCanvas.SetTop(newItem, Math.Max(0, position.Y - newItem.Height / 2));
                }
                else
                {
                    DesignerCanvas.SetLeft(newItem, Math.Max(0, position.X));
                    DesignerCanvas.SetTop(newItem, Math.Max(0, position.Y));
                }

                Canvas.SetZIndex(newItem, this.Children.Count);
                this.Children.Add(newItem);                    
                SetConnectorDecoratorTemplate(newItem);

                this.SelectionService.SelectItem(newItem);
                newItem.Focus();
            }

            e.Handled = true;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var size = new Size();

            foreach (UIElement element in this.InternalChildren)
            {
                var left = Canvas.GetLeft(element);
                var top = Canvas.GetTop(element);
                left = double.IsNaN(left) ? 0 : left;
                top = double.IsNaN(top) ? 0 : top;

                element.Measure(constraint);

                var desiredSize = element.DesiredSize;
                if (double.IsNaN(desiredSize.Width) || double.IsNaN(desiredSize.Height)) continue;
                size.Width = Math.Max(size.Width, left + desiredSize.Width);
                size.Height = Math.Max(size.Height, top + desiredSize.Height);
            }
            size.Width += 10;
            size.Height += 10;
            return size;
        }

        private static void SetConnectorDecoratorTemplate(ContentControl item)
        {
            if (!item.ApplyTemplate() || !(item.Content is UIElement)) return;
            var template = DesignerItem.GetConnectorDecoratorTemplate(item.Content as UIElement);
            if (item.Template.FindName("PART_ConnectorDecorator", item) is Control decorator && template != null)
                decorator.Template = template;
        }
    }
}
