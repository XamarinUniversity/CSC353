using System;
using System.Collections.Generic;

using Xamarin.Forms;
using CatsDinner.ViewModels;
using CatsDinner.Views;

namespace CatsDinner
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            MainViewModel vm = new MainViewModel();
            foreach (var cat in vm.Cats)
                Table.Children.Add(new CatView() { BindingContext = cat });

            BindingContext = vm;
        }
    }
}

