using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Support.Transitions;
using Android.Support.V7.Widget;
using EvoTransitions.Controls;
using EvoTransitions.Droid.Extensions;
using EvoTransitions.Droid.Renderers;
using EvoTransitions.Enums;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using View = Android.Views.View;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

[assembly: ExportRenderer(typeof(SharedTransitionNavigationPage), typeof(SharedTransitionNavigationPageRenderer))]

namespace EvoTransitions.Droid.Renderers
{
    public class SharedTransitionNavigationPageRenderer : NavigationPageRenderer
    {
        Page _current;

        readonly FragmentManager _fragmentManager;
        List<View> _viewsToLeaveWithTransitionName;
        bool _transitionNameUpdated;

        BackgroundTransition _backgroundTransition;
        long _sharedTransitionDuration;
        
        Page Current
        {
            get => _current;
            set
            {
                if (_current == value)
                    return;

                if (_current != null)
                    _current.PropertyChanged -= CurrentOnPropertyChanged;

                _current = value;

                if (_current != null)
                {
                    _current.PropertyChanged += CurrentOnPropertyChanged;
                    _backgroundTransition     = SharedTransitionNavigationPage.GetBackgroundTransition(_current);
                    TransitionDuration = (int) SharedTransitionNavigationPage.GetSharedTransitionDurationProperty(_current);
                }
            }
        }

        public SharedTransitionNavigationPageRenderer(Context context) : base(context)
        {
            _fragmentManager = ((FormsAppCompatActivity)Context).SupportFragmentManager;
        }

        protected override void SetupPageTransition(FragmentTransaction transaction, bool isPush)
        {

            var fragments      = _fragmentManager.Fragments;
            var fragmentToHide = fragments.Last();

            _viewsToLeaveWithTransitionName = fragmentToHide.View.GetSubviewsWithTransitionName();

            UpdateTransitionName(fragments, isPush);

            foreach (var transitionView in _viewsToLeaveWithTransitionName)
                transaction.AddSharedElement(transitionView, transitionView.TransitionName);

            //This is needed to make shared transitions works with hide & add fragments instead of .replace
            transaction.SetReorderingAllowed(true);
           
            if (_backgroundTransition == BackgroundTransition.Fade)
                transaction.SetCustomAnimations(Resource.Animation.fade_in, Resource.Animation.fade_out,
                                                Resource.Animation.fade_out, Resource.Animation.fade_in);
            else
                transaction.SetTransition((int)FragmentTransit.None);
        }

        public override void AddView(View child)
        {
            if (!(child is Toolbar))
            {
                var fragments = _fragmentManager.Fragments;

                //set transitions only when we have at least 2 fragments (push)
                if (fragments.Count > 1)
                {
                    var fragmentToHide = fragments[fragments.Count - 2];
                    var fragmentToShow = fragments.Last();

                    var navigationTransition = TransitionInflater.From(Context).InflateTransition(Resource.Transition.navigation_transition)
                        .SetDuration(_sharedTransitionDuration);

                    fragmentToShow.SharedElementEnterTransition  = navigationTransition;
                    fragmentToHide.SharedElementEnterTransition = navigationTransition;

                    //Switch the current here for all the page except the first.
                    //So we can read the properties for subsequent transitions from the page we leaving
                    Current = Element.CurrentPage;
                }
            }
        
            base.AddView(child);
        }

        protected override Task<bool> OnPushAsync(Page page, bool animated)
        {
            //At the very start of the navigationpage push occour inflating the first view
            //We save it immediately so we can access the Navigation options needed for the first transaction
            if (Element.Navigation.NavigationStack.Count == 1)
                Current = page;

            return base.OnPushAsync(page, animated);
        }

        protected override async Task<bool> OnPopViewAsync(Page page, bool animated)
        {
            Page pageToShow = ((INavigationPageController)Element).Peek(1);
            if (pageToShow == null)
                return await Task.FromResult(false);

            Current = pageToShow;

            var result = await base.OnPopViewAsync(page, animated);

            if (_fragmentManager.Fragments.Count <= 2)
                _transitionNameUpdated = false;

            //This is ugly but is needed!
            //If we press the back button very fast when we have more than 2 fragments in the stack,
            //unexpected behaviours can happen during pop (this is due to SetReorderingAllowed).
            //So we need to add a small delay for fast pop clicks starting the third fragment on stack.
            if (_fragmentManager.Fragments.Count > 2)
                await Task.Delay(100);

            return result;
        }

        protected override Task<bool> OnPopToRootAsync(Page page, bool animated)
        {
            _transitionNameUpdated = false;
            Current = page;

            return base.OnPopToRootAsync(page, animated);
        }

        protected override int TransitionDuration
        {
            //fix to prevent bad behaviours on pop (due to SetReorderingAllowed)
            //after the transition end, we need to wait a bit before telling the rendere that we are done
            get => (int)_sharedTransitionDuration + 100;
            set => _sharedTransitionDuration = value;
        }

        void CurrentOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == SharedTransitionNavigationPage.BackgroundTransitionProperty.PropertyName)
                _backgroundTransition = SharedTransitionNavigationPage.GetBackgroundTransition(Current);
            
            else if (e.PropertyName == SharedTransitionNavigationPage.SharedTransitionDurationProperty.PropertyName)
                TransitionDuration = (int) SharedTransitionNavigationPage.GetSharedTransitionDurationProperty(Current);
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
                foreach (var transitionView in previousFragment.View.GetSubviewsWithTransitionName())
                {
                    if (isPush)
                    {
                        transitionView.TransitionName += "|" + transitionView.Id;
                        _transitionNameUpdated = true;
                    }
                    else
                    {
                        transitionView.TransitionName = transitionView.TransitionName.Replace("|" + transitionView.Id, "");
                    } 
                }
            }
        }
    }
}
