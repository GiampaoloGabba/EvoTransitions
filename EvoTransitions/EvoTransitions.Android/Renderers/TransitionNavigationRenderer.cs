using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Transitions;
using Android.Support.V7.Widget;
using EvoTransitions.Droid.Extensions;
using EvoTransitions.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using View = Android.Views.View;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(TransitionNavigationRenderer))]

namespace EvoTransitions.Droid.Renderers
{
    public class TransitionNavigationRenderer : NavigationPageRenderer
    {
        private readonly FragmentManager _fragmentManager;
        private List<View> _viewList;

        public TransitionNavigationRenderer(Context context) : base(context)
        {
            _fragmentManager = ((FormsAppCompatActivity)Context).SupportFragmentManager;
        }

        protected override void SetupPageTransition(FragmentTransaction transaction, bool isPush)
        {
            var fragments = _fragmentManager.Fragments;

            var fragmentToLeave = fragments.Last();

            _viewList = fragmentToLeave.View.GetSubviewsWithTransitionName();

            foreach (var transitionView in _viewList)
            {
                transaction.AddSharedElement(transitionView, transitionView.TransitionName);
            }

            transaction.SetAllowOptimization(true);

            //This is needed to make shared transitions works with hide & add fragments instead of .replace
            transaction.SetReorderingAllowed(true);
            
            //Change the default transition for non shared elements.
            transaction.SetTransition((int)FragmentTransit.FragmentFade);

        }

        public override void AddView(View child)
        {
            if (!(child is Toolbar))
            {
                var fragments = _fragmentManager.Fragments;

                //set transitions only when we have at least 2 fragments (during push or pop)
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

    }

}
