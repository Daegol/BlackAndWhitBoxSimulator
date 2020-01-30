using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace DiagramDesigner
{
    public class ToolboxItem : ContentControl
    {
        private Point? dragStartPoint = null;

        static ToolboxItem()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ToolboxItem), new FrameworkPropertyMetadata(typeof(ToolboxItem)));
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            this.dragStartPoint = new Point?(e.GetPosition(this));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton != MouseButtonState.Pressed)
                this.dragStartPoint = null;

            if (!this.dragStartPoint.HasValue) return;
            var xamlString = XamlWriter.Save(this.Content);
            var dataObject = new DragObject();
            dataObject.Xaml = xamlString;

            if (VisualTreeHelper.GetParent(this) is WrapPanel panel)
            {
                const double scale = 1.3;
                dataObject.DesiredSize = new Size(panel.ItemWidth * scale, panel.ItemHeight * scale);
            }

            DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);

            e.Handled = true;
        }
    }

    public class DragObject
    {
        public string Xaml { get; set; }

        public Size? DesiredSize { get; set; }
    }
}
