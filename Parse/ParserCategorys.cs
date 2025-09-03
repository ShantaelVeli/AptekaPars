using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using DataBase.Contexts;
using Serilog;

namespace Parse
{
    public class ParseCategory
    {
        public static async Task<Category[]> ParseMainCategorysAsync(ParserSetting Catalogy, List<ProxySetting>? proxy)
        {
            IHtmlDocument? htmlContent = null;
            var result = new List<Category>();


            if (proxy != null)
            {
                foreach (var x in proxy)
                {
                    if (x.Toggle == true)
                        htmlContent = await HtmlLoader.HtmlDownlLoader(Catalogy, 1, x);
                    else continue;
                    if (htmlContent != null) break;
                }
            }

            if (htmlContent == null)
                htmlContent = await HtmlLoader.HtmlDownlLoader(Catalogy, 1, null);


            if (htmlContent != null && Catalogy.ClassName != null && Catalogy.CssSelector != null)
            {
                Log.Information("Starting the main categories parser");

                var div = htmlContent.QuerySelector(Catalogy.CssSelector + "." + Catalogy.ClassName);

                if (div != null)
                {
                    var li = div.QuerySelectorAll("a");
                    if (li != null)
                    {
                        foreach (var x in li)
                        {
                            var buffer = new Category
                            {
                                NameCategory = x.TextContent
                            };
                            if (x.GetAttribute("href")?[0] == '/')
                            {
                                buffer.UrlCategory = Catalogy.BaseUrls + x.GetAttribute("href");

                            }
                            else
                            {
                                buffer.UrlCategory = x.GetAttribute("href");
                            }
                            result.Add(buffer);
                        }
                    }
                    else
                    {
                        Log.Error("Not found selector:'a' from div.SidebarCategoriesList");
                    }
                }
                else
                {
                    Log.Error("Not found selector: div.SidebarCategoriesList");
                }
            }
            return result.ToArray();
        }

        public static async Task<Category[]> ParseSubCategorysAsync(List<Category> MainCategoryUrl, ParserSetting Aptekaru, List<ProxySetting>? proxy)
        {
            ParserSetting pars = new ParserSetting
            {
                BaseUrls = null,
                CssSelector = "div",
                ClassName = "SidebarCategoriesList"
            };

            var result = new List<Category>();
            Random rnd = new Random();
            int indexProxy = 0;

            if (MainCategoryUrl.Count != 0)
            {
                Log.Information("Starting the sub categories parser");
                foreach (var x in MainCategoryUrl)
                {
                    await Task.Delay(rnd.Next(3, 6) * 1000);
                    pars.BaseUrls = x.UrlCategory;
                    int ParentId = x.Id;
                    IHtmlDocument? htmlContent = null;

                    if (proxy != null)
                    {
                        for (int i = indexProxy; i < proxy.Count; i++)
                        {
                            indexProxy++;
                            if (indexProxy >= proxy.Count) indexProxy = 0;
                            if (proxy[i].Toggle == false) continue;
                            htmlContent = await HtmlLoader.HtmlDownlLoader(pars, 1, proxy[i]);

                            if (htmlContent != null) break;
                        }
                    }

                    if (htmlContent == null)
                        htmlContent = await HtmlLoader.HtmlDownlLoader(pars, 1, null);

                    if (htmlContent != null)
                    {
                        var div = htmlContent.QuerySelector(pars.CssSelector + "." + pars.ClassName);

                        if (div != null)
                        {
                            var li = div.QuerySelectorAll("a");
                            if (li != null)
                            {
                                foreach (var y in li)
                                {
                                    var buffer = new Category
                                    {
                                        NameCategory = y.TextContent,
                                        ParentCategoryId = ParentId
                                    };
                                    if (y.GetAttribute("href")?[0] == '/')
                                    {
                                        buffer.UrlCategory = Aptekaru.BaseUrls + y.GetAttribute("href");
                                    }
                                    else
                                    {
                                        buffer.UrlCategory = y.GetAttribute("href");
                                    }

                                    result.Add(buffer);
                                }
                            }
                            else
                            {
                                Log.Error("Not found selector:'a' from div.SidebarCategoriesList");
                            }


                        }
                        else
                        {
                            Log.Error("Not found selector: div.SidebarCategoriesList");
                        }
                    }
                }
                Log.Information("Finishing the sub categories parser");
            }
            else
            {
                Log.Error("Main categories are missing in the database");
            }
            return result.ToArray();
        }
    }
}