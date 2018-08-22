using EvoTransitions.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName(EffectBase.RESOLUTION_GROUP_NAME)]
[assembly: ExportEffect(typeof(EvoTransitions.iOS.Effects.TagEffect), TagEffect.EFFECT_NAME)]

namespace EvoTransitions.iOS.Effects
{
    public class TagEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            if (Control == null)
                return;
            
            Control.Tag = EvoTransitions.Effects.TagEffect.GetTag(Element as View);
        }

        protected override void OnDetached()
        {
        }
    }
}
