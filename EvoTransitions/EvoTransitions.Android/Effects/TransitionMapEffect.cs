using System.ComponentModel;
using EvoTransitions.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName(TransitionEffect.ResolutionGroupName)]
[assembly: ExportEffect(typeof(EvoTransitions.Droid.Effects.TransitionMapEffect), TransitionEffect.EffectName)]

namespace EvoTransitions.Droid.Effects
{
    public class TransitionMapEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            if (Control == null)
                return;

            UpdateTag();
        }

        protected override void OnDetached()
        {
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            if (args.PropertyName == TransitionEffect.TagProperty.PropertyName ||
                args.PropertyName == TransitionEffect.TagGroupProperty.PropertyName)
                UpdateTag();

            base.OnElementPropertyChanged(args);
        }

        void UpdateTag()
        {
            if (Element is View element)
            {
                var tag = TransitionEffect.RegisterTagInStack(element, Control.Id, out var pageId);
                
                //transitionName must be unique in every fragment
                //this is needed when we have more than 2 pages to transition,
                //because the navPage hides old fragments without removing it!
                Control.TransitionName = $"{pageId}_transition_{tag}";
            }
        }
    }
}