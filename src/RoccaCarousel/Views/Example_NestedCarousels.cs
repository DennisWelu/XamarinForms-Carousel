﻿using System;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RoccaCarousel
{
	public class Example_NestedCarousels : ContentPage
	{
		FlexCarouselView _carousel;

		public Example_NestedCarousels ()
		{
			#region Carousel Code

			// Initialise a new Carousel layout
			_carousel = new FlexCarouselView {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			var nestedCarousel = GenerateCarousel(); // Generate our Nested Carousel
			AddPageToParentCarousel(_carousel, nestedCarousel); // Add the generated 
			_carousel.Children.Add (CreatePage (Color.Maroon, Color.White, new Label() { Text = "Parent Carousel\nPage 2:\n" + ExampleStrings.ILikeDogs, TextColor = Color.White }, _carousel));

			#endregion

			Title = "Nested Carousels";

			// Finally, assign the carousel as the page content.
			Content = _carousel;
		}

		public FlexCarouselView GenerateCarousel() {
			FlexCarouselView carousel = new FlexCarouselView {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand
			};

			AddPagesToNestedCarousel(carousel);

			return carousel;
		}

		public void AddPagesToNestedCarousel(FlexCarouselView carousel) {
			// Add content pages to the carousel (in this instance, the buttons are nested within the carousel)
			carousel.Children.Add (CreatePage (Color.Maroon, Color.White, new Label() { Text = "Nested Page 1\n" + ExampleStrings.ILikeDogs, TextColor = Color.White }, carousel));
			carousel.Children.Add (CreatePage (Color.Navy, Color.White, new Label() { Text = "Nested Page 2\n" + ExampleStrings.WaterMovesFast, TextColor = Color.White }, carousel));
			carousel.Children.Add (CreatePage (Color.White, Color.Black, new Label() { Text = "Nested Page 3\n" + ExampleStrings.LysineContingency, TextColor = Color.Black }, carousel));
		}

		public void AddPageToParentCarousel(FlexCarouselView carousel, FlexCarouselView nestedCarousel) {
			// Add content pages to the carousel (in this instance, the buttons are nested within the carousel)
			carousel.Children.Add (CreatePage (Color.Maroon, Color.White, nestedCarousel, carousel));
		}
			
		// Here we create basic pages for the views, only we specify the content to display in the main area
		// We also specify which CarouselView we wish to manipulate by passing it in.
		public Grid CreatePage(Color bgColor, Color textColor, View content, FlexCarouselView eventTarget) {

			Grid layout = new Grid {
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

			layout.Children.Add (goBack, 0, 1, 3, 4);
			layout.Children.Add (goForward, 2, 3, 3, 4);
			layout.Children.Add (content, 0, 3, 0, 3);

			return layout;
		}
	}
}

