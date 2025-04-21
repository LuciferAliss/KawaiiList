using HandyControl.Controls;
using System.Windows;

namespace KawaiiList.Controls
{
    public class ExtendedCarousel : Carousel
    {
        public static readonly DependencyProperty IndexPageProperty = DependencyProperty.Register(
            nameof(IndexPage), typeof(int), typeof(ExtendedCarousel),
            new PropertyMetadata(0, OnIndexPageChanged));

        public int IndexPage
        {
            get => (int)GetValue(IndexPageProperty);
            set => SetValue(IndexPageProperty, value);
        }

        private static void OnIndexPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ExtendedCarousel carousel && e.NewValue is int newIndex)
            {
                carousel.PageIndex = newIndex;
            }
        }

        public new void UpdatePageButtons(int index = -1)
        {
            base.UpdatePageButtons(index);

            // Обновляем IndexPage при изменении PageIndex
            if (index >= 0 && index != IndexPage)
            {
                IndexPage = index;
            }
        }
    }
}