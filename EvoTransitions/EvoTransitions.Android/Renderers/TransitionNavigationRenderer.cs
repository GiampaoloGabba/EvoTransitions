using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Support.Transitions;
using Android.Support.V7.Widget;
using EvoTransitions.Droid.Extensions;
using EvoTransitions.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using View = Android.Views.View;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(TransitionNavigationRenderer))]

namespace EvoTransitions.Droid.Renderers
{
    public class TransitionNavigationRenderer : NavigationPageRenderer
    {
        private readonly FragmentManager _fragmentManager;
        private List<View> _viewsToLeaveWithTransitionName;
        private bool _transitionNameUpdated;
        
        //cache fragment's views with transitionName
        //Also used to animate same elements in multiple pages beyond the first animation
        private Dictionary<Fragment, List<View>> _viewsToLeaveStack;

        public TransitionNavigationRenderer(Context context) : base(context)
        {
            _fragmentManager = ((FormsAppCompatActivity)Context).SupportFragmentManager;
            _viewsToLeaveStack = new Dictionary<Fragment, List<View>>();

        }

        protected override void SetupPageTransition(FragmentTransaction transaction, bool isPush)
        {
            var fragments = _fragmentManager.Fragments;
            var fragmentToLeave = fragments.Last();

            UpdateViewList(fragmentToLeave, isPush);
            UpdateTransitionName(fragments, isPush);

            foreach (var transitionView in _viewsToLeaveWithTransitionName)
            {
                transaction.AddSharedElement(transitionView, transitionView.TransitionName);
            }

            //This is needed to make shared transitions works with hide & add fragments instead of .replace
            transaction.SetReorderingAllowed(true);

            //Change the default transition for non shared elements.
            transaction.SetTransition((int)FragmentTransit.None);
            
        }

        public override void AddView(View child)
        {
            if (!(child is Toolbar))
            {
                var fragments = _fragmentManager.Fragments;

                //set transitions only when we have at least 2 fragments (push)
                if (fragments.Count > 1 && Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    var fragmentToPop = fragments[fragments.Count - 2];
                    var fragmentToPush = fragments.Last();

                    var navigationTransition = TransitionInflater.From(Context).InflateTransition(Resource.Transition.navigation_transition);
                    fragmentToPush.SharedElementEnterTransition  = navigationTransition;
                    fragmentToPop.SharedElementEnterTransition = navigationTransition;
                }
            }
        
            base.AddView(child);

        }

        protected override async Task<bool> OnPopToRootAsync(Page page, bool animated)
        {
            _transitionNameUpdated = false;

            return await base.OnPopToRootAsync(page, animated);
        }

        protected override async Task<bool> OnPopViewAsync(Page page, bool animated)
        {
            var result = await base.OnPopViewAsync(page, animated);

            if (_fragmentManager.Fragments.Count <= 2)
                _transitionNameUpdated = false;

            return result;
        }

        //Populate the ViewList used to set the shared transitions from the fragment we are leaving
        //Also we use a cache so we dont have to traverse multiple times the same fragments view tree when we have more than 2 fragments.
        //Btw the cache is useless when we have just two fragments to transition.
        protected void UpdateViewList(Fragment fragmentToLeave, bool isPush)
        {
            if (!_viewsToLeaveStack.ContainsKey(fragmentToLeave))
            {
                _viewsToLeaveWithTransitionName = fragmentToLeave.View.GetSubviewsWithTransitionName();

                if (isPush)
                    _viewsToLeaveStack.Add(fragmentToLeave, _viewsToLeaveWithTransitionName);
            }
            else
            {
                _viewsToLeaveWithTransitionName = _viewsToLeaveStack[fragmentToLeave];

                if (!isPush)
                    _viewsToLeaveStack.Remove(fragmentToLeave);
            }

        }

        //When using more than 2 pages, the same elements cant be used in shared transitions starting from the third page in stack.
        //This is because the base renderers hide and show fragments, thus maintaing the old views in the tree with their TransitionName. 
        //We need to change the TransitionName from older, hidden fragments and then restore it when navigating back
        protected void UpdateTransitionName(IList<Fragment> fragments, bool isPush)
        {
            //The new fragment to push is not inserted yet at this point.
            //When pushing, if we have at least 2 fragments, we are adding at least the third
            //When popping we execute this code only if we have already manipulated the TransitionName
            //This way we are sure to ginore completely this when we have just 2 pages in our NavigationPage
            if (fragments.Count > 1 && isPush || fragments.Count > 1 && _transitionNameUpdated)
            {
                var previousFragment = fragments[fragments.Count - 2];
                foreach (var transitionView in _viewsToLeaveStack[previousFragment])
                {
                    if (isPush)
                    {
                        transitionView.TransitionName += "|" + transitionView.Id;
                        _transitionNameUpdated = true;
                    }
                    else
                        transitionView.TransitionName = transitionView.TransitionName.Replace("|" + transitionView.Id, "");
                }
            }
        }

    }

}
