using System.ComponentModel;
using EvoTransitions.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;


[assembly: ResolutionGroupName(TransitionEffect.ResolutionGroupName)]
[assembly: ExportEffect(typeof(EvoTransitions.iOS.Effects.TransitionMapEffect), TransitionEffect.EffectName)]

namespace EvoTransitions.iOS.Effects
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
                Control.Tag = TransitionEffect.RegisterTagInStack(element);
        }
    }
}
