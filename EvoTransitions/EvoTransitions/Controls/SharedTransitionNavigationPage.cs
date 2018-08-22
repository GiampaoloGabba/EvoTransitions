using EvoTransitions.Enums;
using Xamarin.Forms;

namespace EvoTransitions.Controls
{
    public class SharedTransitionNavigationPage : NavigationPage
    {
        public static readonly BindableProperty BackgroundTransitionProperty =
            BindableProperty.CreateAttached("BackgroundTransition", typeof(BackgroundTransition), typeof(SharedTransitionNavigationPage), BackgroundTransition.Fade);

        public static readonly BindableProperty SharedTransitionDurationProperty =
            BindableProperty.CreateAttached("SharedTransitionDuration", typeof(long), typeof(SharedTransitionNavigationPage), (long)300);

        public SharedTransitionNavigationPage() : base()
        {
        }

        public SharedTransitionNavigationPage(Page root) : base(root)
        {
        }
        public static BackgroundTransition GetBackgroundTransition(Page page)
        {
            return (BackgroundTransition)page.GetValue(BackgroundTransitionProperty);
        }

        public static void SetBackgroundTransition(Page page, BackgroundTransition value)
        {
            page.SetValue(BackgroundTransitionProperty, value);
        }

        public static long GetSharedTransitionDurationProperty(Page page)
        {
            return (long)page.GetValue(SharedTransitionDurationProperty);
        }

        public static void SetSharedTransitionDurationProperty(Page page, long value)
        {
            page.SetValue(SharedTransitionDurationProperty, value);
        }




    }
}
