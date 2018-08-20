using Xamarin.Forms;

namespace EvoTransitions.Effects
{
    public class TagEffect : EffectBase
    {
        public TagEffect() : base(FULL_NAME)
        {
        }

        public const string EFFECT_NAME = nameof(TagEffect);
        public const string FULL_NAME = RESOLUTION_GROUP_NAME + "." + EFFECT_NAME;

        public static readonly BindableProperty TagProperty = BindableProperty.CreateAttached("Tag", typeof(int), typeof(TagEffect), 0, propertyChanged: OnTagChanged);

        public static int GetTag(BindableObject bindable)
        {
            return (int)bindable.GetValue(TagProperty);
        }

        public static void SetTag(BindableObject bindable, int value)
        {
            bindable.SetValue(TagProperty, value);
        }

        static void OnTagChanged(BindableObject bindable, object oldValue, object newValue)
        {
            CheckAddEffect<View, TagEffect>(bindable as View, FULL_NAME);
        }
    }
}
