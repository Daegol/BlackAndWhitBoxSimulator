using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace DiagramDesigner
{
    public class RubberbandAdorner : Adorner
    {
        private Point? startPoint;
        private Point? endPoint;
        private readonly Pen rubberbandPen;

        private readonly DesignerCanvas designerCanvas;

        public RubberbandAdorner(DesignerCanvas designerCanvas, Point? dragStartPoint)
            : base(designerCanvas)
        {
            this.designerCanvas = designerCanvas;
            this.startPoint = dragStartPoint;
            rubberbandPen = new Pen(Brushes.LightSlateGray, 1) {DashStyle = new DashStyle(new double[] {2}, 1)};
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!this.IsMouseCaptured)
                    this.CaptureMouse();

                endPoint = e.GetPosition(this);
                UpdateSelection();
                this.InvalidateVisual();
            }
            else
            {
                if (this.IsMouseCaptured) this.ReleaseMouseCapture();
            }

            e.Handled = true;
        }

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured) this.ReleaseMouseCapture();

            var adornerLayer = AdornerLayer.GetAdornerLayer(this.designerCanvas);
            adornerLayer?.Remove(this);

            e.Handled = true;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));

            if (this.startPoint.HasValue && this.endPoint.HasValue)
                dc.DrawRectangle(Brushes.Transparent, rubberbandPen, new Rect(this.startPoint.Value, this.endPoint.Value));
        }

        private void UpdateSelection()
        {
            designerCanvas.SelectionService.ClearSelection();

            var rubberBand = new Rect(startPoint.Value, endPoint.Value);
            foreach (Control item in designerCanvas.Children)
            {
                var itemRect = VisualTreeHelper.GetDescendantBounds(item);
                var itemBounds = item.TransformToAncestor(designerCanvas).TransformBounds(itemRect);

                if (!rubberBand.Contains(itemBounds)) continue;
                if (item is Connection)
                    designerCanvas.SelectionService.AddToSelection(item as ISelectable);
                else
                {
                    var di = item as DesignerItem;
                    if (di != null && di.ParentID == Guid.Empty)
                        designerCanvas.SelectionService.AddToSelection(di);
                }
            }
        }
    }
}
