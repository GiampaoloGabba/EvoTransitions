using EvoTransitions.Enums;
using Xamarin.Forms;

namespace EvoTransitions.Controls
{
    public class SharedTransitionNavigationPage : NavigationPage
    {
        internal static readonly BindablePropertyKey TagMapPropertyKey =
            BindableProperty.CreateReadOnly("TagMap", typeof(ITagMapper), typeof(SharedTransitionNavigationPage), default(ITagMapper));

        public static readonly BindableProperty TagMapProperty = TagMapPropertyKey.BindableProperty;

        public static readonly BindableProperty BackgroundAnimationProperty =
            BindableProperty.CreateAttached("BackgroundAnimation", typeof(BackgroundAnimation), typeof(SharedTransitionNavigationPage), BackgroundAnimation.Fade);

        public static readonly BindableProperty SharedTransitionDurationProperty =
            BindableProperty.CreateAttached("SharedTransitionDuration", typeof(long), typeof(SharedTransitionNavigationPage), (long)300);

        public static readonly BindableProperty SelectedTagGroupProperty =
            BindableProperty.CreateAttached("SelectedTagGroup", typeof(int), typeof(SharedTransitionNavigationPage), 0);

        public ITagMapper TagMap
        {
            get => (ITagMapper)GetValue(TagMapProperty);
            internal set => SetValue(TagMapPropertyKey, value);
        }

        public SharedTransitionNavigationPage(Page root) : base(root) => TagMap = new TagMapper();

        public static BackgroundAnimation GetBackgroundAnimation(Page page)
        {
            return (BackgroundAnimation)page.GetValue(BackgroundAnimationProperty);
        }

        public static long GetSharedTransitionDuration(Page page)
        {
            return (long)page.GetValue(SharedTransitionDurationProperty);
        }

        public static int GetSelectedTagGroup(Page page)
        {
            return (int)page.GetValue(SelectedTagGroupProperty);
        }

        public static void SetBackgroundAnimation(Page page, BackgroundAnimation value)
        {
            page.SetValue(BackgroundAnimationProperty, value);
        }

        public static void SetSharedTransitionDuration(Page page, long value)
        {
            page.SetValue(SharedTransitionDurationProperty, value);
        }

        public static void SetSelectedTagGroup(Page page, int value)
        {
            page.SetValue(SelectedTagGroupProperty, value);
        }

        protected override void OnChildRemoved(Element child)
        {
            TagMap.Remove((Page)child);
        }
    }
}
