using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using EvoTransitions.iOS.Extensions;
using Foundation;
using UIKit;

namespace EvoTransitions.iOS.Transitions
{
    public class NavigationTransition : UIViewControllerAnimatedTransitioning
    {
        public NavigationTransition(List<UIView> fromView, List<UIView> toView, UINavigationControllerOperation operation)
        {
            _fromView = fromView;
            _toView = toView;
            _operation = operation;
        }

        private readonly List<UIView> _fromView;
        private readonly List<UIView> _toView;
        private readonly UINavigationControllerOperation _operation;

        public override async void AnimateTransition(IUIViewControllerContextTransitioning transitionContext)
        {
            var containerView = transitionContext.ContainerView;
            var fromViewController = transitionContext.GetViewControllerForKey(UITransitionContext.FromViewControllerKey);
            var toViewController = transitionContext.GetViewControllerForKey(UITransitionContext.ToViewControllerKey);

            var viewAnimated = false;

            // This needs to be added to the view hierarchy for the destination frame to be correct,
            // but we don't want it visible yet.
            containerView.InsertSubview(toViewController.View, 0);
            toViewController.View.Alpha = 0;

            //iterate to destination views, just to be sure to dont start transitions with views only in the start controller
            for (var i = 0; i < _toView.Count; i++)
            {
                if (_toView[i] == null || _fromView.Count == 0)
                    break;

                UIView fromView = _fromView.FirstOrDefault(x => x.Tag == _toView[i].Tag);

                //No matching views in the start controller :(
                if (fromView == null)
                    break;

                viewAnimated = true;

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

            //No views to animate, use the standard slide animation
            if (!viewAnimated)
            {
                var toViewInitialX = _operation == UINavigationControllerOperation.Pop
                    ? -toViewController.View.Frame.Width
                    : toViewController.View.Frame.Width;

                toViewController.View.Frame = new CGRect(toViewInitialX, fromViewController.View.Frame.Y,
                    toViewController.View.Frame.Width, toViewController.View.Frame.Height);
            }

            UIView.Animate(TransitionDuration(transitionContext), 0, UIViewAnimationOptions.CurveEaseInOut, () =>
            {
                //No views to animate, use the standard slide animation
                if (!viewAnimated)
                {
                    toViewController.View.Frame = new CGRect(
                        fromViewController.View.Frame.X,
                        fromViewController.View.Frame.Y,
                        toViewController.View.Frame.Width,
                        toViewController.View.Frame.Height);
                }
                else
                {
                    fromViewController.View.Alpha = 0;
                }

                toViewController.View.Alpha = 1;
            }, () => { transitionContext.CompleteTransition(!transitionContext.TransitionWasCancelled); });

        }

        public override double TransitionDuration(IUIViewControllerContextTransitioning transitionContext) => .250;


    }
}