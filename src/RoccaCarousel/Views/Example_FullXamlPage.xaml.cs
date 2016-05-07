using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace RoccaCarousel
{
    public partial class Example_FullXamlPage : ContentPage
    {
        public Example_FullXamlPage()
        {
            BindingContext = this;
            BackCommand = new Command(() => Carousel.PreviousItem());
            NextCommand = new Command(() => Carousel.NextItem());

            InitializeComponent();
        }

        public string ILikeDogs { get { return "Page 1\n" + ExampleStrings.ILikeDogs; } }
        public string WaterMovesFast { get { return "Page 2\n" + ExampleStrings.WaterMovesFast; } }
        public string LysineContingency { get { return "Page 3\n" + ExampleStrings.LysineContingency; } }

        public Command BackCommand { get; }
        public Command NextCommand { get; }
    }
}
