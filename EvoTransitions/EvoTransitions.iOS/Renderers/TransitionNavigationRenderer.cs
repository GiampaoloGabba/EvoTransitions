using System.Collections.Generic;
using EvoTransitions.iOS.Extensions;
using EvoTransitions.iOS.Renderers;
using EvoTransitions.iOS.Transitions;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly:ExportRenderer(typeof(NavigationPage), typeof(TransitionNavigationRenderer))]

namespace EvoTransitions.iOS.Renderers
{
    public class TransitionNavigationRenderer : NavigationRenderer, IUINavigationControllerDelegate, IUIGestureRecognizerDelegate
    {
        private UIPercentDrivenInteractiveTransition _percentDrivenInteractiveTransition;

        public TransitionNavigationRenderer() : base()
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

            return new NavigationTransition(fromView, toView, operation);
        }

        [Export("navigationController:interactionControllerForAnimationController:")]
        public IUIViewControllerInteractiveTransitioning GetInteractionControllerForAnimationController(UINavigationController navigationController, IUIViewControllerAnimatedTransitioning animationController)
        {
            return _percentDrivenInteractiveTransition;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Add panegesture on left edge to POP page
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

    }

}
