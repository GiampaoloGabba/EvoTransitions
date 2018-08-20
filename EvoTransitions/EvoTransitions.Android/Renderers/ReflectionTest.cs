using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Transitions;
using Android.Views;
using EvoTransitions.Droid.Renderers;
using EvoTransitions.Droid.Transitions;
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
        private List<View> _viewList;


        public TransitionNavigationRenderer(Context context) : base(context)
        {
            _fragmentManager = ((FormsAppCompatActivity)Context).SupportFragmentManager;
        }

        protected override async Task<bool> OnPushAsync(Page view, bool animated)
        {
            //_viewList = new List<View>();
            
         /*   var navigationStack = Element.Navigation.NavigationStack;

            if (navigationStack.Count > 1)
            {
                return await SwitchContentAsync(view, animated);
            }*/

            return await base.OnPushAsync(view, animated);

        }




        protected override void SetupPageTransition(FragmentTransaction transaction, bool isPush)
        {
            _viewList = new List<View>();
            var navigationStack = Element.Navigation.NavigationStack;

            if (navigationStack.Count > 1)
            {
                var fromPageRenderer = (PageRenderer)Platform.GetRenderer(navigationStack[navigationStack.Count - 2]);

                //var fromPageRenderer = (PageRenderer)Platform.GetRenderer(Element.CurrentPage);
                IterateViewChildren(fromPageRenderer.ViewGroup);

                foreach (var transitionView in _viewList)
                {
                    transaction.AddSharedElement(transitionView, transitionView.TransitionName);
                }

                transaction.SetReorderingAllowed(true);

                
            }

            base.SetupPageTransition(transaction, isPush);
            

        }

        public override void AddView(View child)
        {
            
            //only for lollipop and for page renderer
            var fragments = _fragmentManager.Fragments;
            var navigationStack = Element.Navigation.NavigationStack;

            //set transitions only when we have at least 2 fragments (during push)
            if (navigationStack.Count > 1 && fragments.Count > 1 && Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
            //    _viewList = new List<View>();
                var fromPageRenderer = (PageRenderer)Platform.GetRenderer(navigationStack[navigationStack.Count - 2]);
                //var toPageRenderer = (PageRenderer)Platform.GetRenderer(Element.CurrentPage);
                
          //      IterateViewChildren(fromPageRenderer.ViewGroup);

                //check if we have view to animate set the destination fragment
               // if (_viewList.Any())
                //{
                    var fromPageFragment = fragments.First();
                    var toPageFragment = fragments.Last();
                    
                    toPageFragment.SharedElementEnterTransition  = new NavigationTransition();
                    toPageFragment.SharedElementReturnTransition = new NavigationTransition();

             /*       for (var j = 0; j < _fragmentManager.BackStackEntryCount; j++)
                    {
                        var a = _fragmentManager.GetBackStackEntryAt(j);
                    }

                    var ft = _fragmentManager.BeginTransaction()
                        .Replace(toPageFragment.Id, toPageFragment);
                        //.AddToBackStack(null);


                    foreach (var transitionView in _viewList)
                    {
                        ft.AddSharedElement(transitionView, transitionView.TransitionName);
                    }

                     ft.Commit();*/
                    //toPageFragment.StartPostponedEnterTransition();

              //  }
            }
        
            base.AddView(child);

        }



        private void IterateViewChildren(View view)
        {
            if (view is ViewGroup vGroup)
            {
                for (int i = 0; i < vGroup.ChildCount; i++)
                {
                    View vChild = vGroup.GetChildAt(i);

                    if (!(vChild is ViewGroup) && !string.IsNullOrEmpty(vChild.TransitionName))
                        _viewList.Add(vChild);

                    IterateViewChildren(vChild);
                }

            }
        }

        private void UpdateToolbarRefl()
        {
            var rendererType = typeof(NavigationPageRenderer);
            var updateToolbarMethod = rendererType.GetMethod("UpdateToolbar", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod);
            updateToolbarMethod.Invoke(this, new object[0]);
        }

        private void AnimateArrowInRefl()
        {
            var rendererType = typeof(NavigationPageRenderer);
            var animateArrowInMethod = rendererType.GetMethod("AnimateArrowIn", BindingFlags.NonPublic | BindingFlags.Instance);
            animateArrowInMethod.Invoke(this, new object[0]);
        }

        Fragment GetFragmentRefl(Page page, bool removed, bool popToRoot)
        {
            var rendererType = typeof(NavigationPageRenderer);
            var GetFragmentMethod = rendererType.GetMethod("GetFragment", BindingFlags.NonPublic | BindingFlags.Instance);
            var test =  (Fragment) GetFragmentMethod.Invoke(this, new object[] {page, removed, popToRoot});

            return test;

        }


        private void AddTransitionTimer(TaskCompletionSource<bool> tcs, Fragment fragment, FragmentManager fragmentManager, IReadOnlyCollection<Fragment> fragmentsToRemove, int duration, bool shouldUpdateToolbar)
        {
            var rendererType = typeof(NavigationPageRenderer);
            var addTransitionTimerMethod = rendererType.GetMethod("AddTransitionTimer", BindingFlags.NonPublic | BindingFlags.Instance);
            addTransitionTimerMethod.Invoke(this, new object[] {tcs, fragment, fragmentManager, fragmentsToRemove, duration, shouldUpdateToolbar });
        }

        Task<bool> SwitchContentAsync(Page page, bool animated, bool removed = false, bool popToRoot = false)
        {

            var tcs = new TaskCompletionSource<bool>();

            Fragment fragment = GetFragmentRefl(page, removed, popToRoot);

            var currentProp = GetType().BaseType.GetProperty("Current", BindingFlags.NonPublic | BindingFlags.Instance);
            var current     = (Page)currentProp.GetValue(this);


            ((IPageController)Element).SendDisappearing();
            currentProp.SetValue(this, page);


            var drawerToggleField = GetType().BaseType.GetField("_drawerToggle", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this);

            var fragmentStackField  = GetType().BaseType.GetField("_fragmentStack", BindingFlags.NonPublic | BindingFlags.Instance);
            var fragmentStack = (List<Fragment>)fragmentStackField.GetValue(this);

            NavAnimationInProgress = true;
            FragmentTransaction transaction = _fragmentManager.BeginTransaction();

            //var changeBoundsTransition = TransitionInflater.From(Context).InflateTransition(Resource.Transition.change_image_transform);

            fragment.SharedElementEnterTransition  = new NavigationTransition();
            fragment.SharedElementReturnTransition = new NavigationTransition();

            

            // push
            Fragment currentToHide = fragmentStack.Last();

            //transaction.SetCustomAnimations(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out,
            //   Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);
            
            transaction.Hide(currentToHide);

            //now addback the old fragment
            transaction.Add(Id, fragment);
            transaction.SetReorderingAllowed(true);
            transaction.AddToBackStack("transition");

            //for shared transitions, i need to replace the old fragment
            //transaction.Replace(Id, fragment);

            //transaction.Add(currentToHide.Id, currentToHide);
            //transaction.Hide(currentToHide);


            fragmentStack.Add(fragment);

            _viewList = new List<View>();

            IterateViewChildren(currentToHide.View);

            
            foreach (var transitionView in _viewList)
            {
                transaction.AddSharedElement(transitionView, transitionView.TransitionName);
            }


            // We don't currently support fragment restoration, so we don't need to worry about
            // whether the commit loses state
            transaction.CommitAllowingStateLoss();

            // The fragment transitions don't really SUPPORT telling you when they end
            // There are some hacks you can do, but they actually are worse than just doing this:

            if (animated)
            {
                if (!removed)
                {
                    UpdateToolbarRefl();
                    if (drawerToggleField != null && ((INavigationPageController)Element).StackDepth == 2)
                        AnimateArrowInRefl();
                }

                AddTransitionTimer(tcs, fragment, _fragmentManager, new List<Fragment>(), TransitionDuration, false);
            }

            Context.HideKeyboard(this);
            NavAnimationInProgress = false;

            return tcs.Task;
        }


        bool _navAnimationInProgress;
        bool NavAnimationInProgress
        {
            get { return _navAnimationInProgress; }
            set
            {
                if (_navAnimationInProgress == value)
                    return;
                _navAnimationInProgress = value;
                if (value)
                    MessagingCenter.Send(this, "Xamarin.CloseContextActions");
            }
        }


    }


}