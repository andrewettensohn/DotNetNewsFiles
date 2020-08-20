using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using DotNetNews.Models;

namespace DotNetNews.Data
{
    public class RestService
    {
        HttpClient client;
        public List<int> ArticleIds { get; set; }

        public readonly int PageCount = 15;

        public RestService(HttpClient http)
        {
            client = http;
        }

        public async Task<List<Article>> PerformFeedPaging()
        {
            List<int> newPageIds = ArticleIds.GetRange(0, PageCount);
            ArticleIds.RemoveRange(0, PageCount);

            List<Article> newArticles = await GetArticlesFromIds(newPageIds);
            newArticles = RemoveNonArticles(newArticles);
            return newArticles;
        }

        private List<Article> RemoveNonArticles(List<Article> articles)
        {
            articles.RemoveAll(x => string.IsNullOrEmpty(x.Url));
            return articles;
        }

        private async Task<List<Article>> GetArticlesFromIds(List<int> ids)
        {
            Uri itemBaseUri = new Uri(string.Format(Constants.itemBaseUrl, string.Empty));
            List<Article> articles = new List<Article>();

            foreach (int articleId in ids)
            {
                try
                {
                    HttpResponseMessage articleResponse = await client.GetAsync($"{itemBaseUri}/{articleId}.json");

                    if (articleResponse.StatusCode == HttpStatusCode.OK)
                    {
                        string jsonContent = await articleResponse.Content.ReadAsStringAsync();
                        Article article = JsonConvert.DeserializeObject<Article>(jsonContent);
                        article.Id = articleId;
                        articles.Add(article);
                    }
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc);
                }
            }

            return articles;
        }

        public async Task<List<Article>> GetTopStories()
        {
            List<Article> topArticles = new List<Article>();
            Uri topStoryIdsUri = new Uri(string.Format(Constants.topStoriesUri, string.Empty));

            try
            {
                HttpResponseMessage idResponse = await client.GetAsync(topStoryIdsUri);

                if (idResponse.StatusCode == HttpStatusCode.OK)
                {
                    string idsJsonContent = await idResponse.Content.ReadAsStringAsync();
                    ArticleIds = JsonConvert.DeserializeObject<List<int>>(idsJsonContent);

                    List<int> firstPageIds = ArticleIds.GetRange(0, PageCount);
                    ArticleIds.RemoveRange(0, PageCount);

                    topArticles = await GetArticlesFromIds(firstPageIds);
                    topArticles = RemoveNonArticles(topArticles);
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc);
            }

            return topArticles;
        }
    }
}
