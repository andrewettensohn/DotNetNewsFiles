using DotNetNews.ViewModels;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ProgrammerNews.Views
{
    [DesignTimeVisible(false)]
    public partial class AboutPage : ContentPage
    {
        public ArticleViewModel ViewModel;
        public AboutPage()
        {
            InitializeComponent();
            BindingContext = ViewModel = new ArticleViewModel(null);
        }

        private async Task ButtonOpenGitHub(object sender, EventArgs e)
        {
            await Browser.OpenAsync("https://github.com/andrewettensohn/ProgrammerNews");
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            bool deleteConfirmed = await DisplayAlert("Confirm", "Are you sure you want to delete all saved articles?", "Delete", "Cancel");

            if (deleteConfirmed)
            {
                await ViewModel.ExecuteDeleteAllSavedArticlesCommand();
            }
        }
    }
}