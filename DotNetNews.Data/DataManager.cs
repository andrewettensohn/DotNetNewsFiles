using Blazored.LocalStorage;
using DotNetNews.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DotNetNews.Data
{
    public class DataManager
    {
        public RestService RestService = new RestService(new HttpClient());
        static ArticleDatabase database;
        public static ArticleDatabase Database { private get; set; }
        //public static ArticleDatabase Database
        //{
        //    get
        //    {
        //        if (database == null && LocalStorage == null)
        //        {
        //            database = new ArticleDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Notes.db3"));
        //        }
        //        return database;
        //    }
        //}
        ILocalStorageService LocalStorage { get; set; }
        public DataManager(ILocalStorageService localStorage)
        {
            if(localStorage == null)
            {
                if (database == null && LocalStorage == null)
                {
                    database = new ArticleDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Notes.db3"));
                }
                Database = database;
            }
            else
            {
                LocalStorage = localStorage;
            }
        }

        //Remove from storage, keep in list.
        public async Task<List<Article>> DeleteArticleAsync(List<Article> articles, Article article)
        {
            if (LocalStorage != null)
            {
                articles.Remove(article);
                await LocalStorage.SetItemAsync("savedArticles", articles);
                return await LocalStorage.GetItemAsync<List<Article>>("savedArticles");
            }
            else
            {
                await Database.DeleteArticleAsync(article);
                return await Database.GetArticlesAsync();
            }
        }

        //Remove from storage, keep in list
        public async Task UnsaveArticleAsync(Article article)
        {
            List<Article> savedArticles = await LocalStorage.GetItemAsync<List<Article>>("savedArticles");
            savedArticles.Remove(article);
            await LocalStorage.SetItemAsync("savedArticles", savedArticles);

            article.Saved = false;
        }

        public async Task SaveArticleAsync(Article article)
        {
            if(LocalStorage != null)
            {
                article.Saved = true;
                List<Article> savedArticles = await LocalStorage.GetItemAsync<List<Article>>("savedArticles");

                if (savedArticles != null)
                {
                    if (!savedArticles.Any(x => x.Id == article.Id))
                    {
                        savedArticles.Add(article);
                        await LocalStorage.SetItemAsync("savedArticles", savedArticles);
                    }
                }
                else
                {
                    savedArticles = new List<Article> { article };
                    await LocalStorage.SetItemAsync("savedArticles", savedArticles);
                }
            }
            else
            {
                await Database.SaveArticleAsync(article);
            }
        }

        public async Task DeleteAllArticlesAsync()
        {
            if (LocalStorage != null)
            {
                await LocalStorage.ClearAsync();
            }
            else
            {
                await Database.DeleteAllSavedArticlesAsync();
            }
        }

        public async Task<List<Article>> GetSavedArticles()
        {
            if(LocalStorage != null)
            {
                return await LocalStorage.GetItemAsync<List<Article>>("savedArticles");
            }
            else
            {
                return await Database.GetArticlesAsync();
            }
        }

        public async Task<List<Article>> GetTopStories()
        {
            return await RestService.GetTopStories();
        }

        public async Task<List<Article>> PerformFeedPaging()
        {
            return await RestService.PerformFeedPaging();
        }
    }
}
