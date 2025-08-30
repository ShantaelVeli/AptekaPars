using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using DataBase.Contexts;
using Serilog;

namespace Parse
{
    public class ParseCategory
    {
        public static async Task<Category[]> ParseMainCategorysAsync(ParserSetting Catalogy)
        {
            IHtmlDocument? htmlContent = await HtmlLoader.HtmlDownlLoader(Catalogy, 1);
            var result = new List<Category>();

            if (htmlContent != null && Catalogy.ClassName != null && Catalogy.CssSelector != null)
            {
                Log.Information("Starting the main categories parser");
                try
                {
                    var div = htmlContent.QuerySelector(Catalogy.CssSelector + "." + Catalogy.ClassName);

                    if (div != null)
                    {
                        try
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
                        catch (DomException ex)
                        {
                            Log.Error($"Selector error: {ex.Message}");//вывод ошибки
                        }
                    }
                    else
                    {
                        Log.Error("Not found selector: div.SidebarCategoriesList");
                    }
                }
                catch (DomException dex)
                {
                    Log.Error($"Selector error:: {dex.Message}"); //вывод ошибки
                }
            }
            return result.ToArray();
        }

        public static async Task<Category[]> ParseSubCategorysAsync(List<Category> MainCategoryUrl, ParserSetting Aptekaru)
        {
            ParserSetting pars = new ParserSetting
            {
                BaseUrls = null,
                CssSelector = "div",
                ClassName = "SidebarCategoriesList"
            };

            var result = new List<Category>();
            Random rnd = new Random();

            if (MainCategoryUrl.Count != 0)
            {
                Log.Information("Starting the sub categories parser");
                foreach (var x in MainCategoryUrl)
                {
                    Thread.Sleep(rnd.Next(3, 6) * 1000);
                    pars.BaseUrls = x.UrlCategory;
                    int ParentId = x.Id;
                    IHtmlDocument? htmlContent = await HtmlLoader.HtmlDownlLoader(pars, 1);
                    if (htmlContent != null)
                    {
                        try
                        {
                            var div = htmlContent.QuerySelector(pars.CssSelector + "." + pars.ClassName);

                            if (div != null)
                            {
                                try
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
                                catch (DomException dex)
                                {
                                    Console.WriteLine($"Ошибка селектора: {dex.Message}");//вывод ошибки
                                }

                            }
                            else
                            {
                                Log.Error("Not found selector: div.SidebarCategoriesList");
                            }
                        }
                        catch (DomException dex)
                        {
                            Log.Error(dex.Message); //вывод ошибки
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