using Blazored.LocalStorage;
using DotNetNews.Data;
using DotNetNews.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace DotNetNews.ViewModels
{
    public class ArticleViewModel : BaseViewModel
    {
        public ObservableCollection<Article> Articles { get; set; }
        public RelayCommand LoadNewCommand { get; set; }
        public RelayCommand LoadSavedCommand { get; set; }
        public RelayCommand PagingCommand { get; set; }
        public RelayCommand DeleteAllSavedArticlesCommand { get; set; }
        public RelayCommand<int> SaveArticleCommand => new RelayCommand<int>(async (x) => await ExecuteSaveArticle(x));
        public RelayCommand<int> UnsaveArticleCommand => new RelayCommand<int>(async (x) => await ExecuteUnsaveArticleCommand(x));
        public RelayCommand<int> DeleteArticleCommand => new RelayCommand<int>(async (x) => await ExecuteDeleteArticleCommand(x));
        public string LoadingCss => IsBusy ? string.Empty : "invisible";
        public string AlertDeletionCss = "d-none";
        public DataManager DataManager { get; set; }
        public ArticleViewModel(ILocalStorageService localStorage)
        {
            Articles = new ObservableCollection<Article>();
            LoadNewCommand = new RelayCommand(async () => await ExecuteLoadNewCommand());
            LoadSavedCommand = new RelayCommand(async () => await ExecuteLoadSavedCommand());
            PagingCommand = new RelayCommand(async () => await ExecutePaging());
            DeleteAllSavedArticlesCommand = new RelayCommand(async () => await ExecuteDeleteAllSavedArticlesCommand());
            DataManager = (localStorage == null) ? new DataManager(null) : new DataManager(localStorage);
        }

        public async Task ExecuteSaveArticle(int articleId)
        {
            Article article = Articles.FirstOrDefault(x => x.Id == articleId);
            await DataManager.SaveArticleAsync(article);
        }

        public async Task ExecuteUnsaveArticleCommand(int id)
        {
            Article article = Articles.FirstOrDefault(x => x.Id == id);
            await DataManager.UnsaveArticleAsync(article);
        }

        public async Task ExecuteDeleteArticleCommand(int id)
        {
            Article article = Articles.FirstOrDefault(x => x.Id == id);
            await DataManager.DeleteArticleAsync(Articles.ToList(), article);
            await ExecuteLoadSavedCommand();
        }

        public async Task ExecuteDeleteAllSavedArticlesCommand()
        {
            await DataManager.DeleteAllArticlesAsync();
            AlertDeletionCss = string.Empty;
        }

        public async Task ExecutePaging()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                List<Article> stories = await DataManager.PerformFeedPaging();
                foreach (var story in stories)
                {
                    Articles.Add(story);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ExecuteLoadSavedCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Articles.Clear();
                var stories = await DataManager.GetSavedArticles();
                foreach (var story in stories)
                {
                    Articles.Add(story);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ExecuteLoadNewCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Articles.Clear();
                var stories = await DataManager.GetTopStories();
                foreach (var story in stories)
                {
                    Articles.Add(story);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
