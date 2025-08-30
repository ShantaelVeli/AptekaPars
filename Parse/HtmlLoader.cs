using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Io.Network;
using Serilog;

namespace Parse
{
    public class HtmlLoader
    {
        public static async Task<IHtmlDocument?> HtmlDownlLoader(ParserSetting set, int page)
        {

            if (set.BaseUrls == null)
            {
                Log.Error("url is not specified");
                return null;
            }

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/138.0.0.0 Safari/537.36");
            httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("ru-RU,ru;q=0.9");
            httpClient.DefaultRequestHeaders.Referrer = new System.Uri(set.BaseUrls);


            var requester = new HttpClientRequester(httpClient);
            var config = Configuration.Default.With(requester).WithDefaultCookies().WithDefaultLoader();
            var context = BrowsingContext.New(config);

            IDocument doc;

            try
            {
                Log.Information($"HTML page code export: {set.BaseUrls}");
                doc = await context.OpenAsync(set.BaseUrls + "?page=" + page);
                var parser = new HtmlParser();
                string htmlContent = doc.DocumentElement.OuterHtml;
                string path = "./Logs/page.html";
                File.WriteAllText(path, htmlContent);
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
            catch (HttpRequestException ex)
            {
                Log.Fatal($"Request error: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                Log.Fatal($"Loading timeout: {ex.Message}");

            }
            catch (Exception ex)
            {
                Log.Fatal($"General error: {ex.Message}");
            }
            return null;
        }
    }
}