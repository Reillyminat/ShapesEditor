using System.Windows;
using System.Windows.Controls;

namespace ShapesEditor
{
    public static class ScrollViewerBinding
    {
        private static readonly DependencyProperty VerticalScrollBindingProperty =
            DependencyProperty.RegisterAttached("VerticalScrollBinding", typeof(bool?), typeof(ScrollViewerBinding));
        private static readonly DependencyProperty HorizontalScrollBindingProperty =
            DependencyProperty.RegisterAttached("HorizontalScrollBinding", typeof(bool?), typeof(ScrollViewerBinding));

        public static readonly DependencyProperty VerticalOffsetProperty =
    DependencyProperty.RegisterAttached("VerticalOffset", typeof(double),
    typeof(ScrollViewerBinding), new FrameworkPropertyMetadata(double.NaN,
        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        OnVerticalOffsetPropertyChanged));
        public static double GetVerticalOffset(DependencyObject depObj)
        {
            return (double)depObj.GetValue(VerticalOffsetProperty);
        }
        public static void SetVerticalOffset(DependencyObject depObj, double value)
        {
            depObj.SetValue(VerticalOffsetProperty, value);
        }
        private static void OnVerticalOffsetPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = d as ScrollViewer;
            if (scrollViewer == null)
                return;

            BindVerticalOffset(scrollViewer);
            scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
        }

        public static void BindVerticalOffset(ScrollViewer scrollViewer)
        {
            if (scrollViewer.GetValue(VerticalScrollBindingProperty) != null)
                return;

            scrollViewer.SetValue(VerticalScrollBindingProperty, true);
            scrollViewer.ScrollChanged += (s, se) =>
            {
                if (se.VerticalChange == 0)
                    return;
                SetVerticalOffset(scrollViewer, se.VerticalOffset);
            };
        }


        public static readonly DependencyProperty HorizontalOffsetProperty =
DependencyProperty.RegisterAttached("HorizontalOffset", typeof(double),
typeof(ScrollViewerBinding), new FrameworkPropertyMetadata(double.NaN,
FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
OnHorizontalOffsetPropertyChanged));
        public static double GetHorizontalOffset(DependencyObject depObj)
        {
            return (double)depObj.GetValue(HorizontalOffsetProperty);
        }
        public static void SetHorizontalOffset(DependencyObject depObj, double value)
        {
            depObj.SetValue(HorizontalOffsetProperty, value);
        }
        private static void OnHorizontalOffsetPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = d as ScrollViewer;
            if (scrollViewer == null)
                return;

            BindHorizontalOffset(scrollViewer);
            scrollViewer.ScrollToHorizontalOffset((double)e.NewValue);
        }

        public static void BindHorizontalOffset(ScrollViewer scrollViewer)
        {
            if (scrollViewer.GetValue(HorizontalScrollBindingProperty) != null)
                return;

            scrollViewer.SetValue(HorizontalScrollBindingProperty, true);
            scrollViewer.ScrollChanged += (s, se) =>
            {
                if (se.HorizontalChange == 0)
                    return;
                SetHorizontalOffset(scrollViewer, se.HorizontalOffset);
            };
        }
    }
}
