using System;
using System.Windows.Input;
using System.Windows;
using GongSolutions.Wpf.DragDrop;

namespace MultipleFilesRename
{
    public partial class MainWindow
    {
        static Cursor CustomCursor1 = new Cursor(Application.GetResourceStream(new Uri("Images/Cursor1.cur", UriKind.Relative)).Stream);
        static Cursor CustomCursor2 = new Cursor(Application.GetResourceStream(new Uri("Images/Cursor2.cur", UriKind.Relative)).Stream);

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            DragAndDropRules(dropInfo);
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            DragAndDropRules(dropInfo);
        }

        void DragAndDropRules(IDropInfo dropInfo)
        {
            RuleView? sourceRule = dropInfo.Data as RuleView;
            RuleView? destRule = dropInfo.TargetItem as RuleView;

            if (sourceRule != null && destRule != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;

                int sourcePos = nameListBox.Items.IndexOf(sourceRule);
                int destPos = nameListBox.Items.IndexOf(destRule);

                var temp = _rules[sourcePos];
                _rules[sourcePos] = _rules[destPos];
                _rules[destPos] = temp;

                UpdateResult();
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 3);
        }

        private void ScrollToBottom_Click(object sender, RoutedEventArgs e)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + 40);
        }

        private void ScrollBottom(object sender, RoutedEventArgs e)
        {
            scrollViewer.ScrollToBottom();
        }

        private void ScrollTop(object sender, RoutedEventArgs e)
        {
            scrollViewer.ScrollToTop();
        }

        private void ScrollToTop(object sender, RoutedEventArgs e)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - 40);
        }
    }
}
