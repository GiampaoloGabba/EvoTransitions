using System;
using EvoTransitions.Controls;
using EvoTransitions.Enums;
using Xamarin.Forms;

namespace EvoTransitions
{
    public partial class FirstPage : ContentPage
    {
        public FirstPage()
        {
            InitializeComponent();
            SharedTransitionNavigationPage.SetBackgroundAnimation(this,BackgroundAnimation.Fade);
            SharedTransitionNavigationPage.SetSharedTransitionDuration(this, 500);
        }

        private async void ImageTapped(object sender, TappedEventArgs e)
        {
            SharedTransitionNavigationPage.SetSelectedTagGroup(this,1);
            await Navigation.PushAsync(new SecondPage());
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            DisplayAlert("ok", "cliccato", "si");
        }
    }
}
