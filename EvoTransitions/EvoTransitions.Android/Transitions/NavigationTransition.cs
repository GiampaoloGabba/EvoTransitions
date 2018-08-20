using Android.Annotation;
using Android.Content;
using Android.Transitions;
using Android.Util;

namespace EvoTransitions.Droid.Transitions
{
    [TargetApi(Value = 21)]
    public class NavigationTransition : TransitionSet
    {
        public NavigationTransition()
        {
            Init();
        }

        //This constructor allows us to use this transition in XML
        public NavigationTransition(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init();
        }

        public void Init()
        {
            SetOrdering(TransitionOrdering.Together);

            AddTransition(new ChangeBounds()).
                AddTransition(new ChangeTransform()).
                AddTransition(new ChangeImageTransform());
        }
    }
}