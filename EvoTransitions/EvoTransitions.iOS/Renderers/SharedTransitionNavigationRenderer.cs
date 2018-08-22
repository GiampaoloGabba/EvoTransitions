using System.Collections.Generic;
using System.ComponentModel;
using EvoTransitions.Controls;
using EvoTransitions.Enums;
using EvoTransitions.iOS.Extensions;
using EvoTransitions.iOS.Renderers;
using EvoTransitions.iOS.Transitions;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(SharedTransitionNavigationPage), typeof(SharedTransitionNavigationRenderer))]

namespace EvoTransitions.iOS.Renderers
{
    public class SharedTransitionNavigationRenderer : NavigationRenderer, IUINavigationControllerDelegate, IUIGestureRecognizerDelegate
    {
        UIPercentDrivenInteractiveTransition _percentDrivenInteractiveTransition;
        public double SharedTransitionDuration { get; set; }
        public BackgroundTransition BackgroundTransition { get; set; }

        Page _current;
        public Page Current
        {
            get => _current;
            set
            {
                if (_current == value)
                    return;

                if (_current != null)
                    _current.PropertyChanged -= HandleChildPropertyChanged;

                _current = value;

                if (_current != null)
                    _current.PropertyChanged += HandleChildPropertyChanged;

                UpdateBackgroundTransition();
                UpdateSharedTransitionDuration();
            }
        }

        NavigationPage NavPage => Element as NavigationPage;

        public SharedTransitionNavigationRenderer() : base()
        {
            Delegate = this;
        }

        [Export("navigationController:animationControllerForOperation:fromViewController:toViewController:")]
        public IUIViewControllerAnimatedTransitioning GetAnimationControllerForOperation(UINavigationController navigationController, UINavigationControllerOperation operation, UIViewController fromViewController, UIViewController toViewController)
        {
            var toView = toViewController.View.GetSubviewsWithTag();
            var fromView = new List<UIView>();

            //Filter all the sourceviews with the same tags
            foreach (var view in toView)
                fromView.Add(fromViewController.View.ViewWithTag(view.Tag));

            return new NavigationTransition(fromView, toView, operation, BackgroundTransition, SharedTransitionDuration);
        }

        [Export("navigationController:interactionControllerForAnimationController:")]
        public IUIViewControllerInteractiveTransitioning GetInteractionControllerForAnimationController(UINavigationController navigationController, IUIViewControllerAnimatedTransitioning animationController)
        {
            return _percentDrivenInteractiveTransition;
        }

        public override UIViewController PopViewController(bool animated)
        {
            //We need to take the transition configuration from the destination page
            //So we need to anticipate it in the Current property before pop occour
            var pageCount = Element.Navigation.NavigationStack.Count;
            if (pageCount > 1)
                Current = Element.Navigation.NavigationStack[pageCount - 2];

            return base.PopViewController(animated);
        }

        public override void PushViewController(UIViewController viewController, bool animated)
        {
            base.PushViewController(viewController, animated);
            Current = NavPage.CurrentPage;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Add pangesture on left edge to POP page
            var interactiveTransitionRecognizer = new UIScreenEdgePanGestureRecognizer();
            interactiveTransitionRecognizer.AddTarget(() => InteractiveTransitionRecognizerAction(interactiveTransitionRecognizer));
            interactiveTransitionRecognizer.Edges = UIRectEdge.Left;
            View.AddGestureRecognizer(interactiveTransitionRecognizer);
        }

        void InteractiveTransitionRecognizerAction(UIScreenEdgePanGestureRecognizer sender)
        {
            var percent = sender.TranslationInView(sender.View).X / sender.View.Frame.Width;

            switch (sender.State)
            {
                case UIGestureRecognizerState.Began:
                    _percentDrivenInteractiveTransition = new UIPercentDrivenInteractiveTransition();
                    PopViewController(true);
                    break;

                case UIGestureRecognizerState.Changed:
                    _percentDrivenInteractiveTransition.UpdateInteractiveTransition(percent);
                    break;

                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Failed:
                    _percentDrivenInteractiveTransition.CancelInteractiveTransition();
                    _percentDrivenInteractiveTransition = null;
                    break;

                case UIGestureRecognizerState.Ended:
                    if (percent > 0.5 || sender.VelocityInView(sender.View).X > 300)
                    {
                        _percentDrivenInteractiveTransition.FinishInteractiveTransition();
                    }
                    else
                    {
                        _percentDrivenInteractiveTransition.CancelInteractiveTransition();
                    }
                    _percentDrivenInteractiveTransition = null;
                    break;
            }
        }

        void HandleChildPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == SharedTransitionNavigationPage.BackgroundTransitionProperty.PropertyName)
            {
                UpdateBackgroundTransition();
                MessagingCenter.Send(this, "UpdateBackgroundTransition");
            }
            else if (e.PropertyName == SharedTransitionNavigationPage.SharedTransitionDurationProperty.PropertyName)
            {
                UpdateSharedTransitionDuration();
                MessagingCenter.Send(this, "UpdateSharedTransitionDuration");
            }    
        }

        void UpdateBackgroundTransition()
        {
            BackgroundTransition = SharedTransitionNavigationPage.GetBackgroundTransition(Current);
        }

        void UpdateSharedTransitionDuration()
        {
            SharedTransitionDuration = (double) SharedTransitionNavigationPage.GetSharedTransitionDurationProperty(Current) / 1000;
        }
    }
}
