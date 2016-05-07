using System;

using Xamarin.Forms;

namespace RoccaCarousel
{
    public class ExamplesList : ContentPage
    {
        string[] listitems = {
            "Full Page",
            "Full Page XAML",
            "Nested Carousels",
            "Externally Controlled",
            "Live-Tiles"
        };
        public ExamplesList()
        {
            ListView list = new ListView()
            {
                ItemsSource = listitems
            };
            list.ItemSelected += (object sender, SelectedItemChangedEventArgs e) =>
            {
                list.SelectedItem = null;

                if ((string)e.SelectedItem == listitems[0])
                {
                    Navigation.PushAsync(new Example_FullPage());
                }
                else if ((string)e.SelectedItem == listitems[1])
                {
                    Navigation.PushAsync(new Example_FullXamlPage());
                }
                else if ((string)e.SelectedItem == listitems[2])
                {
                    Navigation.PushAsync(new Example_NestedCarousels());
                }
                else if ((string)e.SelectedItem == listitems[3])
                {
                    Navigation.PushAsync(new Example_ExternalControl());
                }
                else if ((string)e.SelectedItem == listitems[4])
                {
                    Navigation.PushAsync(new Example_LiveTiles());
                }
            };

            Title = "Flex Carousel Examples";
            Content = list;
        }
    }
}


