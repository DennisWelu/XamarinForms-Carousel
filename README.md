# Xamarin.Forms FlexCarouselView
This is a fork of the RoccaCarousel / ManualCarouselView project here: https://github.com/roccacreative/XamarinForms-Carousel.

The controls has been significantly reworked with these notable changes:

- support for vertical orientation
- configurable properties for wrap-around, animation time, and easing method
- support for usage in XAML

The sample project has the same behavior plus one demo to illustrate XAML usage.

In addition, the container hierarchy has been simplified a bit.

## Why another carousel control?
After looking at various carousel implementations for Xamarin Forms, including the upcoming
CarouselView being added to Xamarin Forms (to replace CarouselPage), I liked the appraoch
RoccaCarousel took the best - NOT using a scrollview for virtual content, and using a 
container to hold the position of the content in a parent layout. That seemed to be a simple
and robust way to co-exist in various types of control hierarchies while allowing circular/infinite
wraparound navigation from last-to-first item and back (using a ScrollView with a specific virtual
ContentSize makes that feature harder).

## What's not there yet?
After fiddling with this for a day decided not to use it anyway. :-) But publishing it here for grins.
Why not? I forgot/didn't realize that Swipe gestures in Xamarin Forms were fairly limiting at this point.
Wanted easy swipe detection with inertia and gravity like the Alliance Carousel has, which I've used previously.
Also the lack of cool transition animations ended up being a bigger loss of experience than anticipated.

So it's good for what it does, but not the final answer (for me).
