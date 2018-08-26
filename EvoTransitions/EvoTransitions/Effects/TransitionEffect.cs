using System;
using System.Linq;
using EvoTransitions.Controls;
using Xamarin.Forms;

namespace EvoTransitions.Effects
{
    public static class TransitionEffect
    {
        public const string ResolutionGroupName = "EvoTransitions";
        public const string EffectName = nameof(TransitionEffect);
        public const string FullName = ResolutionGroupName + "." + EffectName;

        public static readonly BindableProperty TagProperty = BindableProperty.CreateAttached("Tag", typeof(int), typeof(TransitionEffect), 0, propertyChanged: OnPropertyChanged);
        public static readonly BindableProperty TagGroupProperty = BindableProperty.CreateAttached("TagGroup", typeof(int), typeof(TransitionEffect), 0);

        public static int GetTag(BindableObject bindable)
        {
            return (int)bindable.GetValue(TagProperty);
        }

        public static int GetTagGroup(BindableObject bindable)
        {
            return (int)bindable.GetValue(TagGroupProperty);
        }

        public static void SetTag(BindableObject obj, int value)
        {
            obj.SetValue(TagProperty, value);
        }

        public static void SetTagGroup(BindableObject obj, int value)
        {
            obj.SetValue(TagGroupProperty, value);
        }

        //Unique ID used for transitions.
        //For dynamic transition (with tag group defined) start from 100.
        //In case of hybrid transition (static + dynamic) we have the first 100 reserved to static transitions
        public static int GetUniqueTag(BindableObject bindable)
        {
            return GetUniqueTag(GetTag(bindable), GetTagGroup(bindable));
        }

        public static int GetUniqueTag(int tag, int group, bool reverse = false)
        {
            if (group > 0)
                return reverse
                    ? tag - group - 99
                    : 99 + group + tag;
            
            return tag;
        }
        public static int RegisterTagInStack(BindableObject bindable)
        {
            return RegisterTagInStack(bindable, 0, out _);
        }

        public static int RegisterTagInStack(BindableObject bindable, int viewId, out Guid pageId)
        {
            if (bindable is View element)
            {
                var tag   = GetUniqueTag(element);
                var group = GetTagGroup(element);
                if (!(element.Navigation?.NavigationStack.Count > 0) || tag <= 0) return 0; 

                var currentPage = element.Navigation.NavigationStack.Last();
                if (currentPage.Parent is SharedTransitionNavigationPage navPage)
                {
                    navPage.TagMap.Add(currentPage, tag, group, viewId);
                    pageId = currentPage.Id;
                    return tag;
                }
            }

            return 0;
        }

        static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable == null)
                return;

            var element = (View)bindable;
            var existing = element.Effects.FirstOrDefault(x => x is TransitionMapEffect);

            if (existing == null && newValue != null && (int) newValue > 0)
            {
                element.Effects.Add(new TransitionMapEffect());
            }
            else if (existing != null)
            {
                element.Effects.Remove(existing);
            }
        }


    }

    public class TransitionMapEffect : RoutingEffect
    {
        public TransitionMapEffect() : base(TransitionEffect.FullName)
        {
        }
    }



}
