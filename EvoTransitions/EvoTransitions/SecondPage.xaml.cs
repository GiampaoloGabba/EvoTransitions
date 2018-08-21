using System;
using Xamarin.Forms;

namespace EvoTransitions
{
    public partial class SecondPage : ContentPage
    {
        public SecondPage()
        {
            InitializeComponent();
        }

        private void ImageTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ThirdPage());
        }
    }
}
