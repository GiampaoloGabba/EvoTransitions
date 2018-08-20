using System;
using EvoTransitions.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using TagEffect = EvoTransitions.Droid.Effects.TagEffect;

[assembly: ResolutionGroupName(EffectBase.RESOLUTION_GROUP_NAME)]
[assembly: ExportEffect(typeof(TagEffect), EvoTransitions.Effects.TagEffect.EFFECT_NAME)]

namespace EvoTransitions.Droid.Effects
{
    public class TagEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            if (Control == null)
            {
                return;
            }

            Control.Tag = EvoTransitions.Effects.TagEffect.GetTag(Element as View);
            Control.TransitionName = "transition_" + Control.Tag;
        }

        protected override void OnDetached()
        {
            
        }
    }
}