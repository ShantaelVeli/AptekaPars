using System.Net;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Io;
using AngleSharp.Io.Network;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Parse
{
    public class HtmlLoader
    {
        public static async Task<IHtmlDocument?> HtmlDownlLoader(ParserSetting set, int page, ProxySetting? proxySet)
        {
                if (set.BaseUrls == null)
                {
                    Log.Error("url is not specified");
                    return null;
                }

                IBrowsingContext context;
                if (proxySet != null)
                {
                    Log.Information($"Use proxe: {proxySet.Name}");
                    var proxy = new WebProxy($"{proxySet.Server}:{proxySet.Port}")
                    {
                        Credentials = new NetworkCredential(proxySet.User, proxySet.Password)
                    };

                    var handler = new HttpClientHandler
                    {
                        Proxy = proxy,
                        PreAuthenticate = true,
                        UseDefaultCredentials = false
                    };

                    var httpClient = new HttpClient(handler);

                    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");
                    httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("ru-RU,ru;q=0.9");
                    httpClient.DefaultRequestHeaders.Referrer = new System.Uri(set.BaseUrls);

                    var requester = new HttpClientRequester(httpClient);
                    var config = Configuration.Default
                            .With(requester)
                            .WithDefaultLoader();
                    context = BrowsingContext.New(config);
                }
                else
                {
                    Log.Information($"Use defult ip");
                    var config = CreateDefultConfig(set);
                    context = BrowsingContext.New(config);
                }

                return await CreateDocHtml(set, page, context);
        }

        private static AngleSharp.IConfiguration? CreateDefultConfig(ParserSetting set)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");
            httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("ru-RU,ru;q=0.9");
            httpClient.DefaultRequestHeaders.Referrer = new System.Uri(set.BaseUrls);
            var requester = new HttpClientRequester(httpClient);
            var config = Configuration.Default.With(requester).WithDefaultCookies().WithDefaultLoader();

            return config;
        }

        private static async Task<IHtmlDocument?> CreateDocHtml(ParserSetting set, int page, IBrowsingContext context)
        {
            Log.Information($"HTML page code export: {set.BaseUrls}");
            var doc = await context.OpenAsync(set.BaseUrls + "?page=" + page);
            var parser = new HtmlParser();
            string htmlContent = doc.DocumentElement.OuterHtml;
            IHtmlDocument document = await parser.ParseDocumentAsync(htmlContent);
            var div = document.QuerySelector("div.SidebarCategoriesList");
            if (div != null)
            {
                Log.Information($"Page {set.BaseUrls} successfully exported,");
                return document;
            }
            else
            {
                Log.Error($"Page {set.BaseUrls} upload error");
                return null;
            }
        }

    }
}