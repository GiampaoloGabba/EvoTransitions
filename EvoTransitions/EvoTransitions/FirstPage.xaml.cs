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
            SharedTransitionNavigationPage.SetBackgroundTransition(this,BackgroundTransition.Fade);
        }

        private async void ImageTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new SecondPage());
        }
    }
}
