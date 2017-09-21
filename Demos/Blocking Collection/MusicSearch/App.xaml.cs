using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinUniversity.Interfaces;
using XamarinUniversity.Services;
using MusicSearch.ViewModels;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace MusicSearch
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var ds = XamUInfrastructure.Init();

            var navService = ds.Get<INavigationService>() as FormsNavigationPageService;
            navService.RegisterPage(AppPages.EditSearch, () => new EditSearchPage());
            navService.RegisterPage(AppPages.Results, () => new ResultsPage());

            // The root page of your application
            MainPage = new NavigationPage(new MainPage() { BindingContext = new MainViewModel(ds) });
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

