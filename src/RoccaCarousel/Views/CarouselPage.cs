using System;
using Xamarin.Forms;

namespace RoccaCarousel
{
	public class CarouselPage : ContentView, IFlexCarouselItemView
	{
		public event Action ItemViewAppearing;
        public event Action ItemViewDisappearing;
        public event Action ItemViewAppeared;
        public event Action ItemViewDisappeared;

        public void OnItemViewAppearing()
		{
            ItemViewAppearing?.Invoke();
		}
        public void OnItemViewDisappearing()
		{
            ItemViewDisappearing?.Invoke();
		}
        public void OnItemViewAppeared()
		{
            ItemViewAppeared?.Invoke();
		}
        public void OnItemViewDisappeared()
		{
            ItemViewDisappeared.Invoke();
		}
	}
}
