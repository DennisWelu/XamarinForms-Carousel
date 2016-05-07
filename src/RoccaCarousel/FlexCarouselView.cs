using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace RoccaCarousel
{
    public interface IFlexCarouselItemView
    {
        void OnItemViewAppearing();
        void OnItemViewDisappearing();

        void OnItemViewAppeared();
        void OnItemViewDisappeared();
    }

    /// <summary>
    /// A carousel view control with support for:
    /// - horizontal or vertical navigation
    /// - wrap around navigation
    /// - easing and animation time configuration
    /// - xaml usage, to some extent
    /// </summary>
    [ContentProperty("Children")]
    public class FlexCarouselView : ContentView
    {
        Grid _rootLayout;
        ContentView _container1;
        ContentView _container2;
        readonly ObservableCollection<View> _children;

        bool _animationInProgress;
        bool _swappedContainers;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public FlexCarouselView()
        {
            _children = new ObservableCollection<View>();
            _children.CollectionChanged += ChildrenChanged;

            _container1 = new ContentView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            _container2 = new ContentView
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            _rootLayout = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection {
                    new ColumnDefinition{ Width = new GridLength (1, GridUnitType.Star) }
                },
                RowDefinitions = new RowDefinitionCollection {
                    new RowDefinition { Height = new GridLength (1, GridUnitType.Star) }
                },
                ColumnSpacing = 0,
                RowSpacing = 0,
                IsClippedToBounds = true
            };

            _rootLayout.Children.Add(_container2, 0, 1, 0, 1);
            _rootLayout.Children.Add(_container1, 0, 1, 0, 1);

            Content = _rootLayout;
        }

        #region Children and selected index/item

        public IList<View> Children
        {
            get { return _children; }
        }

        void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SelectedIndex == -1 && Children.Any())
                SelectedIndex = 0;
            else if (SelectedIndex > -1 && !Children.Any())
                SelectedIndex = -1;
        }

        // SelectedIndex
        public static readonly BindableProperty SelectedIndexProperty =
            BindableProperty.Create("SelectedIndex", typeof(int), typeof(FlexCarouselView), -1, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) => { ((FlexCarouselView)bindable).SelectedIndexChanged((int)oldValue, (int)newValue); });

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        void SelectedIndexChanged(int oldvalue, int newvalue)
        {
            var needsMove = oldvalue == -1 || newvalue == -1;

            SelectedItemView = newvalue == -1 ? null : Children[newvalue];

            if (needsMove)
                MoveToItem(newvalue, 0);
        }

        // SelectedItemView
        public static readonly BindableProperty SelectedItemViewProperty =
            BindableProperty.Create("SelectedItemView", typeof(View), typeof(FlexCarouselView), null, BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) => { ((FlexCarouselView)bindable).SelectedItemViewChanged(); });

        public View SelectedItemView
        {
            get { return (View)GetValue(SelectedItemViewProperty); }
            set { SetValue(SelectedItemViewProperty, value); }
        }

        void SelectedItemViewChanged()
        {
            SelectedIndex = SelectedItemView == null ? -1 : Children.IndexOf(SelectedItemView);
        }

        #endregion

        #region Other bindable properties

        // Orientation
        public static readonly BindableProperty OrientationProperty =
            BindableProperty.Create("Orientation", typeof(ScrollOrientation), typeof(FlexCarouselView), ScrollOrientation.Horizontal);

        public ScrollOrientation Orientation
        {
            get { return (ScrollOrientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // AnimationTime
        public static readonly BindableProperty AnimationTimeProperty =
            BindableProperty.Create("AnimationTime", typeof(uint), typeof(FlexCarouselView), (uint)250);

        public uint AnimationTime
        {
            get { return (uint)GetValue(AnimationTimeProperty); }
            set { SetValue(AnimationTimeProperty, value); }
        }

        // AnimationEasing
        public static readonly BindableProperty AnimationEasingProperty =
            BindableProperty.Create("AnimationEasing", typeof(Easing), typeof(FlexCarouselView), Easing.CubicInOut);

        public Easing AnimationEasing
        {
            get { return (Easing)GetValue(AnimationEasingProperty); }
            set { SetValue(AnimationEasingProperty, value); }
        }

        // WrapsAround
        public static readonly BindableProperty WrapsAroundProperty =
            BindableProperty.Create("WrapsAround", typeof(bool), typeof(FlexCarouselView), true);

        public bool WrapsAround
        {
            get { return (bool)GetValue(WrapsAroundProperty); }
            set { SetValue(WrapsAroundProperty, value); }
        }

        #endregion

        /// <summary>
        /// Move to the next item with wrap around if configured.
        /// </summary>
        public void NextItem()
        {
            if (_animationInProgress)
                return;
            
            var nextIndex = SelectedIndex + 1;
            if (WrapsAround && nextIndex > Children.Count - 1)
                nextIndex = 0;

            SelectedIndex = nextIndex;
            MoveToItem(nextIndex, 1);
        }

        /// <summary>
        /// Move to the previous item with wrap around if configured.
        /// </summary>
        public void PreviousItem()
        {
            if (_animationInProgress)
                return;

            var nextIndex = SelectedIndex - 1;
            if (WrapsAround && nextIndex < 0)
                nextIndex = Children.Count - 1;

            SelectedIndex = nextIndex;
            MoveToItem(nextIndex, -1);
        }

        /// <summary>
        /// This is the key method that causes viewable content to be put in place.
        /// </summary>
        public void MoveToItem(int itemIndex, int direction)
        {
            Debug.Assert(itemIndex >= -1 && itemIndex < Children.Count);

            // Prepare references to the pages to prepare for animation.
            var currentContainer = _swappedContainers ? _container2 : _container1;
            var nextContainer = _swappedContainers ? _container1 : _container2;
            _swappedContainers = !_swappedContainers;

            // Prepare view dimensions each time the page changes so layout alterations are not then fixed.
            var dimen = new Rectangle(currentContainer.X, currentContainer.Y, currentContainer.Width, currentContainer.Height);

            // Prepare next page
            var isInitialContent = !Children.Contains(currentContainer.Content) && !Children.Contains(nextContainer.Content);
            nextContainer.Content = itemIndex >= 0 ? Children[itemIndex] : new ContentView();
            _rootLayout.RaiseChild(nextContainer);

            // Notify the page content of start/stop change events
            var nextItem = nextContainer.Content as IFlexCarouselItemView;
            var currentItem = currentContainer.Content as IFlexCarouselItemView;
            nextItem?.OnItemViewAppearing();
            currentItem?.OnItemViewDisappearing();

            // No need to animate the first one (or the removal of all items), just put in place and we're done.
            // Also, a direciton of 0 means just pop into place.
            if (isInitialContent || itemIndex == -1 || direction == 0)
            {
                nextContainer.Layout(dimen);
                currentItem?.OnItemViewDisappeared();
                nextItem?.OnItemViewAppeared();
                return;
            }

            // Inform the animation framework that we have a batch of animations to perform
            _animationInProgress = true;
            currentContainer.BatchBegin();
            nextContainer.BatchBegin();

            if (Orientation == ScrollOrientation.Horizontal)
            {
                if (direction > 0)
                    MoveRightToLeft(dimen, currentContainer, nextContainer);
                else
                    MoveLeftToRight(dimen, currentContainer, nextContainer);
            }
            else
            {
                if (direction > 0)
                    MoveBottomToTop(dimen, currentContainer, nextContainer);
                else
                    MoveTopToBottom(dimen, currentContainer, nextContainer);
            }

            nextContainer.LayoutTo(new Rectangle(dimen.X, dimen.Y, dimen.Width, dimen.Height), AnimationTime, Easing.CubicInOut).ContinueWith(_ =>
            {
                _animationInProgress = false;
                currentItem?.OnItemViewDisappeared();
                nextItem?.OnItemViewAppeared();
            });

            // Make it so
            currentContainer.BatchCommit();
            nextContainer.BatchCommit();
        }

        void MoveRightToLeft(Rectangle dimen, View currentContainer, View nextContainer)
        {
            nextContainer.Layout(new Rectangle(dimen.X + dimen.Width, dimen.Y, dimen.Width, dimen.Height));
            currentContainer.LayoutTo(new Rectangle(dimen.X - dimen.Width, dimen.Y, dimen.Width, dimen.Height), AnimationTime, AnimationEasing);
        }

        void MoveLeftToRight(Rectangle dimen, View currentContainer, View nextContainer)
        {
            nextContainer.Layout(new Rectangle(dimen.X - dimen.Width, dimen.Y, dimen.Width, dimen.Height));
            currentContainer.LayoutTo(new Rectangle(dimen.X + dimen.Width, dimen.Y, dimen.Width, dimen.Height), AnimationTime, AnimationEasing);
        }

        void MoveBottomToTop(Rectangle dimen, View currentContainer, View nextContainer)
        {
            nextContainer.Layout(new Rectangle(dimen.X, dimen.Y + dimen.Height, dimen.Width, dimen.Height));
            currentContainer.LayoutTo(new Rectangle(dimen.X, dimen.Y - dimen.Height, dimen.Width, dimen.Height), AnimationTime, AnimationEasing);
        }

        void MoveTopToBottom(Rectangle dimen, View currentContainer, View nextContainer)
        {
            nextContainer.Layout(new Rectangle(dimen.X, dimen.Y - dimen.Height, dimen.Width, dimen.Height));
            currentContainer.LayoutTo(new Rectangle(dimen.X, dimen.Y + dimen.Height, dimen.Width, dimen.Height), AnimationTime, AnimationEasing);
        }
    }
}
