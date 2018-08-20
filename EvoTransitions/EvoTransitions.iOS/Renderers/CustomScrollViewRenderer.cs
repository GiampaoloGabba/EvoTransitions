using System;
using System.Linq;
using EvoTransitions.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ScrollView), typeof(CustomScrollViewRenderer))]

namespace EvoTransitions.iOS.Renderers
{
    public class CustomScrollViewRenderer : ScrollViewRenderer
    {
        public CustomScrollViewRenderer() : base()
        {
            Scrolled += OnScrolled;
        }


        private void OnScrolled(object sender, EventArgs e)
        {

            var navController =
                (NavigationRenderer)Window.RootViewController.ChildViewControllers.FirstOrDefault(x =>
                    x is NavigationRenderer);


            if (navController != null)
            {
                //var scrollViewHeight        = Frame.Size.Height;
                //var scrollContentSizeHeight = ContentSize.Height;
                var scrollOffset            = ContentOffset.Y;

                if (scrollOffset < 0)
                {
                    //navController.HidesBarsOnSwipe = false;
                    navController.SetNavigationBarHidden(false, true);
                }
                else
                {
                    navController.HidesBarsOnSwipe = true;
                }
            }

            /*var scrollView = (UIScrollView) sender;

            var pan      = scrollView.PanGestureRecognizer;
            var velocity = pan.VelocityInView(scrollView).Y;
            var delta    = pan.TranslationInView(scrollView).Y;

            var navController =
                (NavigationRenderer) Window.RootViewController.ChildViewControllers.FirstOrDefault(x =>
                    x is NavigationRenderer);

            if (navController != null)
            {
                var navBar = navController.NavigationBar;
                var navBarHeight = navController.NavigationBar.Bounds.Height;

                if (velocity < -100 || delta < -navBarHeight)
                {
                    navController.NavigationBar.Frame = new CGRect();
                    //For slow scroll hide the navigation bar only after a minimal amount of delta (navbar height)
                    navController.SetNavigationBarHidden(true, true);
                }
                else if (velocity > 100 || delta > navBarHeight)
                {
                    //For slow scroll show the navigation bar only after a minimal amount of delta (navbar height)
                    navController.SetNavigationBarHidden(false, true);
                }
            }*/

        }


    }

}