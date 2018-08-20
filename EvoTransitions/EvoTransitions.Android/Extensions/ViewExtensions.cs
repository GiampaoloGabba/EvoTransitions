using System.Collections.Generic;
using Android.Views;

namespace EvoTransitions.Droid.Extensions
{
    public static class ViewExtensions
    {
        public static List<View> GetSubviewsWithTransitionName(this View startView)
        {
            var result = new List<View>();

            if (startView is ViewGroup startViewGroup)
                Traverse(startViewGroup);
            else if (!string.IsNullOrEmpty(startView.TransitionName))
                result.Add(startView);

            void Traverse(ViewGroup parentView)
            {
                for (int i = 0; i < parentView.ChildCount; i++)
                {
                    View vChild = parentView.GetChildAt(i);

                    if (vChild is ViewGroup vChildGroup)
                        Traverse(vChildGroup);
                    else if (!string.IsNullOrEmpty(vChild.TransitionName))
                        result.Add(vChild);
                }
            }

            return result;
        }

    }
}