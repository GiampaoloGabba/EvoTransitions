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

            SharedTransitionNavigationPage.SetBackgroundAnimation(this, BackgroundAnimation.None);
            SharedTransitionNavigationPage.SetSharedTransitionDuration(this, 500);
        }

        private void ImageTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ThirdPage());
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            DisplayAlert("ok", "cliccato", "si");
        }
    }
}
