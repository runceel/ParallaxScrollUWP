using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 を参照してください

namespace App23
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var listViewVisual = ElementCompositionPreview.GetElementVisual(this.ListView);
            var scrollViewer = FindVisualTree<ScrollViewer>(this.ListView);
            var scrollViewerPropertySet = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);
            var random = new Random();

            for (int i = 0; i < 1000; i++)
            {
                var argb = new byte[4];
                random.NextBytes(argb);
                var rect = new Border
                {
                    Width = random.Next(10, 100),
                    Height = random.Next(10, 100),
                    Background = new SolidColorBrush(Color.FromArgb(argb[0], argb[1], argb[2], argb[3])),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };
                Canvas.SetTop(rect, random.NextDouble() * scrollViewer.ScrollableHeight);
                Canvas.SetLeft(rect, random.NextDouble() * scrollViewer.ActualWidth);
                this.ParallaxBackground.Children.Add(rect);

                var rectVisual = ElementCompositionPreview.GetElementVisual(rect);
                var offsetY = rectVisual.Offset.Y;
                var expression = listViewVisual.Compositor.CreateExpressionAnimation($"offsetY + scrollViewer.Translation.Y * {random.NextDouble()}");
                expression.SetScalarParameter("offsetY", offsetY);
                expression.SetReferenceParameter("scrollViewer", scrollViewerPropertySet);
                rectVisual.StartAnimation("Offset.Y", expression);
            }
        }

        private static T FindVisualTree<T>(DependencyObject element)
            where T : DependencyObject
        {
            var childCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                if (child is T) { return (T)child; }
                var result = FindVisualTree<T>(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}
