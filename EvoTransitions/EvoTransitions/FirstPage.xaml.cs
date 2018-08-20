using System;
using Xamarin.Forms;

namespace EvoTransitions
{
    public partial class FirstPage : ContentPage
    {
        public FirstPage()
        {
            InitializeComponent();
            
        }

        private async void ImageTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new SecondPage());
        }
    }
}
