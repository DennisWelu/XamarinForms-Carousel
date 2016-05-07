using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RoccaCarousel
{
	public class Example_FullPage : ContentPage
	{
		FlexCarouselView _carousel;

		public Example_FullPage ()
		{
			#region Carousel Code

			// Initialise a new Carousel layout
			_carousel = new FlexCarouselView {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			AddPagesToCarousel (_carousel);

			#endregion

			// Set the page title
			Title = "Full Page Carousel";

			// Finally, assign the carousel as the page content.
			Content = _carousel;
		}

		public void AddPagesToCarousel(FlexCarouselView carousel) {
			// Add content pages to the carousel (in this instance, the buttons are nested within the carousel)
			carousel.Children.Add (CreatePage (Color.Maroon, Color.White, "Page 1\n" + ExampleStrings.ILikeDogs, carousel ));
			carousel.Children.Add (CreatePage (Color.Navy, Color.White, "Page 2\n" + ExampleStrings.WaterMovesFast, carousel ));
			carousel.Children.Add (CreatePage (Color.White, Color.Black, "Page 3\n" + ExampleStrings.LysineContingency, carousel ));
		}

		public Grid CreatePage(Color bgColor, Color textColor, string text, FlexCarouselView eventTarget) {
			Grid content = new Grid {
				ColumnDefinitions = new ColumnDefinitionCollection {
					new ColumnDefinition{ Width = new GridLength (1, GridUnitType.Star) },
					new ColumnDefinition{ Width = new GridLength (1, GridUnitType.Star) },
					new ColumnDefinition{ Width = new GridLength (1, GridUnitType.Star) }
				},
				RowDefinitions = new RowDefinitionCollection{
					new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
					new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
					new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
					new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
				},
				BackgroundColor = bgColor
			};
					
			Button goBack = new Button () {
				Text = "Back",
				TextColor = textColor,
				Command = new Command(() => {
					Device.BeginInvokeOnMainThread(() => {
                        eventTarget.PreviousItem();
					});
				})
			};

			Button goForward = new Button () {
				Text = "Next",
				TextColor = textColor,
				Command = new Command(() => {
					Device.BeginInvokeOnMainThread(() => {
                        eventTarget.NextItem();
					});
				})
			};

			Label textcontent = new Label () {
				TextColor = textColor,
				Text = text,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
			};

			content.Children.Add (goBack, 0, 1, 3, 4);
			content.Children.Add (goForward, 2, 3, 3, 4);
			content.Children.Add (textcontent, 0, 3, 0, 3);

			return content;
		}
	}
}

