using System;
using System.Collections.Generic;
using System.Linq;

namespace DiagramDesigner
{
    internal class SelectionService
    {
        private readonly DesignerCanvas designerCanvas;

        private List<ISelectable> currentSelection;
        internal List<ISelectable> CurrentSelection => currentSelection ?? (currentSelection = new List<ISelectable>());

        public SelectionService(DesignerCanvas canvas)
        {
            this.designerCanvas = canvas;
        }

        internal void SelectItem(ISelectable item)
        {
            this.ClearSelection();
            this.AddToSelection(item);
        }

        internal void AddToSelection(ISelectable item)
        {
            if (item is IGroupable groupable)
            {
                var groupItems = GetGroupMembers(groupable);

                foreach (ISelectable groupItem in groupItems)
                {
                    groupItem.IsSelected = true;
                    CurrentSelection.Add(groupItem);
                }
            }
            else
            {
                item.IsSelected = true;
                CurrentSelection.Add(item);
            }
        }

        internal void RemoveFromSelection(ISelectable item)
        {
            if (item is IGroupable)
            {
                var groupItems = GetGroupMembers(item as IGroupable);

                foreach (ISelectable groupItem in groupItems)
                {
                    groupItem.IsSelected = false;
                    CurrentSelection.Remove(groupItem);
                }
            }
            else
            {
                item.IsSelected = false;
                CurrentSelection.Remove(item);
            }
        }

        internal void ClearSelection()
        {
            CurrentSelection.ForEach(item => item.IsSelected = false);
            CurrentSelection.Clear();
        }

        internal void SelectAll()
        {
            ClearSelection();
            CurrentSelection.AddRange(designerCanvas.Children.OfType<ISelectable>());
            CurrentSelection.ForEach(item => item.IsSelected = true);
        }

        internal List<IGroupable> GetGroupMembers(IGroupable item)
        {
            var list = designerCanvas.Children.OfType<IGroupable>();
            var rootItem = GetRoot(list, item);
            return GetGroupMembers(list, rootItem);
        }

        internal IGroupable GetGroupRoot(IGroupable item)
        {
            var list = designerCanvas.Children.OfType<IGroupable>();
            return GetRoot(list, item);
        }

        private static IGroupable GetRoot(IEnumerable<IGroupable> list, IGroupable node)
        {
            if (node == null || node.ParentID == Guid.Empty)
            {
                return node;
            }
            else
            {
                return (from item in list where item.ID == node.ParentID select GetRoot(list, item)).FirstOrDefault();
            }
        }

        private static List<IGroupable> GetGroupMembers(IEnumerable<IGroupable> list, IGroupable parent)
        {
            var groupMembers = new List<IGroupable>();
            groupMembers.Add(parent);

            var children = list.Where(node => node.ParentID == parent.ID);

            foreach (var child in children)
            {
                groupMembers.AddRange(GetGroupMembers(list, child));
            }

            return groupMembers;
        }
    }
}
