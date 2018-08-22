using System;
using EvoTransitions.Controls;
using EvoTransitions.Enums;
using Xamarin.Forms;

namespace EvoTransitions
{
    public partial class SecondPage : ContentPage
    {
        public SecondPage()
        {
            InitializeComponent();

            SharedTransitionNavigationPage.SetBackgroundTransition(this, BackgroundTransition.None);
            SharedTransitionNavigationPage.SetSharedTransitionDurationProperty(this, 1000);
        }

        private void ImageTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ThirdPage());
        }
    }
}
