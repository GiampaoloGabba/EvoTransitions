using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using EvoTransitions.Enums;
using EvoTransitions.iOS.Extensions;
using EvoTransitions.iOS.Renderers;
using Foundation;
using UIKit;
using Xamarin.Forms;

namespace EvoTransitions.iOS.Transitions
{
    public class NavigationTransition : UIViewControllerAnimatedTransitioning
    {
        BackgroundTransition _backgroundTransition;
        double _sharedTransitionDuration;

        public NavigationTransition(List<UIView> fromView, List<UIView> toView, UINavigationControllerOperation operation, BackgroundTransition backgroundTransition, double sharedTransitionDuration)
        {
            _fromView = fromView;
            _toView = toView;
            _operation = operation;

            _backgroundTransition = backgroundTransition;
            _sharedTransitionDuration = sharedTransitionDuration;

            MessagingCenter.Subscribe<SharedTransitionNavigationRenderer>(this, "UpdateBackgroundTransition",
                sender => { _backgroundTransition = sender.BackgroundTransition; });

            MessagingCenter.Subscribe<SharedTransitionNavigationRenderer>(this, "UpdateSharedTransitionDuration",
                sender => { _sharedTransitionDuration = sender.SharedTransitionDuration; });
        }

        readonly List<UIView> _fromView;
        readonly List<UIView> _toView;
        readonly UINavigationControllerOperation _operation;

        public override async void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
        {
            var containerView = transitionContext.ContainerView;
            var fromViewController = transitionContext.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);
            var toViewController = transitionContext.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);

            // This needs to be added to the view hierarchy for the destination frame to be correct,
            // but we don't want it visible yet.
            containerView.InsertSubview(toViewController.View, 0);
            toViewController.View.Alpha = 0;

            //iterate to destination views, just to be sure to dont start transitions with views only in the start controller
            for (var i = 0; i < _toView.Count; i++)
            {
                if (_toView[i] == null || _fromView.Count == 0)
                    break;

                UIView fromView = null;
                try
                {
                    fromView = _fromView.FirstOrDefault(x => x.Tag == _toView[i].Tag);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                //No matching views in the start controller
                if (fromView == null)
                    break;

                UIView fromViewSnapshot;
                CGRect fromViewFrame = fromView.Frame;

                if (fromView is UILabel || fromView is UIButton)
                {
                    //For buttons and labels just copy the view to preserve a good transition
                    //Using normal snapshot with labels and buttons may cause streched and deformed images
                    fromViewSnapshot = (UIView)NSKeyedUnarchiver.UnarchiveObject(NSKeyedArchiver.ArchivedDataWithRootObject(fromView));                    
                }
                else if (fromView is UIImageView fromImageView)
                {
                    //Get the snapshot based on the real image size, not his containing frame!
                    //This is needed to avoid deformations with image aspectfit
                    //where the container frame can a have different size from the contained image
                    fromViewFrame = fromImageView.GetImageFrameWithAspectRatio();
                    fromViewSnapshot = fromView.ResizableSnapshotView(fromViewFrame, false, UIEdgeInsets.Zero);
                }
                else
                {
                    fromViewSnapshot = fromView.SnapshotView(false);
                }

                //minor perf gain
                fromViewSnapshot.Opaque = true;
                containerView.AddSubview(fromViewSnapshot);
                fromViewSnapshot.Frame = fromView.ConvertRectToView(fromViewFrame, containerView);

                // Without this, the snapshots will include the following "recent" changes
                // Needed only on push. So pop can use the interaction (pangesture)
                if (_operation == UINavigationControllerOperation.Push)
                    await Task.Yield();

                _toView[i].Alpha = 0;
                fromView.Alpha = 0;

                //If uimage, preserve aspect ratio on destination frame
                CGRect toFrame = _toView[i] is UIImageView toImageView
                    ? toImageView.ConvertRectToView(toImageView.GetImageFrameWithAspectRatio(), containerView)
                    : _toView[i].ConvertRectToView(_toView[i].Frame, containerView);

                //Animate views,
                //saving the counter in an external variable is needed to avoid unexpected behaviours
                var viewCont = i;
                UIView.Animate(TransitionDuration(transitionContext), 0, UIViewAnimationOptions.CurveEaseInOut, () =>
                {
                    fromViewSnapshot.Frame = toFrame;
                    fromViewSnapshot.Alpha = 1;

                }, () =>
                {
                    _toView[viewCont].Alpha = 1;
                    _fromView[viewCont].Alpha = 1;

                    fromViewSnapshot.RemoveFromSuperview();
                });
            }

            containerView.InsertSubview(toViewController.View, 1);

            if (_backgroundTransition == BackgroundTransition.None)
            {
                UIView.Animate(0, 0, UIViewAnimationOptions.TransitionNone, () =>
                {
                    toViewController.View.Alpha = 1;
                }, () => { transitionContext.CompleteTransition(!transitionContext.TransitionWasCancelled); });
            }
            else
            {
                UIView.Animate(TransitionDuration(transitionContext), 0, UIViewAnimationOptions.CurveLinear, () =>
                {
                    toViewController.View.Alpha = 1;
                    fromViewController.View.Alpha = 0;
                }, () => { transitionContext.CompleteTransition(!transitionContext.TransitionWasCancelled); });
            }
        }

        public override double TransitionDuration(IUIViewControllerContextTransitioning transitionContext)
        {
            return _sharedTransitionDuration;
        }

        protected override void Dispose(bool disposing)
        {
            MessagingCenter.Unsubscribe<SharedTransitionNavigationRenderer>(this, "UpdateBackgroundTransition");
            MessagingCenter.Unsubscribe<SharedTransitionNavigationRenderer>(this, "UpdateSharedTransitionDuration");

            base.Dispose(disposing);
        }
    }
}