using Xamarin.Forms;
using System;

namespace MusicSearch
{
    public partial class EditSearchPage : ContentPage
    {
        public EditSearchPage()
        {
            InitializeComponent();
        }

        async void OnBack(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}

