using System.Globalization;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using DataBase.Contexts;
using Serilog;

namespace Parse
{
    public class ParsProduct
    {
        public static async Task<Product[]> ParsProductGet(List<Category> SubCategoryUrl, ParserSetting Aptekaru)
        {
            Random rnd = new Random();

            Product[] products = await ParsProductsUrl(SubCategoryUrl, Aptekaru);

                ParserSetting CardProductsSet = new ParserSetting
                {
                    CssSelector = "div",
                    ClassName = "catalog-card.card-flex"
                };

                foreach (var x in products)
                {
                    CardProductsSet.BaseUrls = x.ProdUrl;
                    if (x.CategoryId == 40) break; // ограничение по итерациямж
                    Thread.Sleep(rnd.Next(3, 6) * 1000);
                    var html = new HtmlLoader();
                    IHtmlDocument? htmlContent = await HtmlLoader.HtmlDownlLoader(CardProductsSet, 1);

                    if (htmlContent != null)
                    {
                        var cardExpect = htmlContent.QuerySelector("div.ProductOffer__unavailable")?.QuerySelector("span");
                        if (cardExpect != null)
                        {
                            x.Price = 0.0;
                            x.Availability = cardExpect.TextContent;
                        }
                        else
                        {
                            x.Availability = "есть в наличие";
                            var prodOffer = htmlContent.QuerySelector("div.ProductOffer__price");
                            if (prodOffer != null)
                            {
                                string? prodPrice = prodOffer.QuerySelector("span.moneyprice__roubles")?.TextContent + prodOffer.QuerySelector("span.moneyprice__pennies")?.TextContent;
                                if (prodPrice != null && prodPrice != "")
                                    x.Price = double.Parse(Regex.Replace(prodPrice, @"\s+", ""), CultureInfo.InvariantCulture);
                            }
                        }

                        var cardPhoto = htmlContent.QuerySelector("div.ProductPhotos-image");
                        if (cardPhoto != null)
                        {
                            x.ImageUrl = cardPhoto.QuerySelector("picture.photopicture")?.QuerySelector("source")?.GetAttribute("srcset");
                        }
                        x.Name = htmlContent.QuerySelector("h1.ViewProductPage__title")?.TextContent;
                    }
                }
            return products;
            
        }

        private static async Task<Product[]> ParsProductsUrl(List<Category> url, ParserSetting Aptekaru)
        {
            var result = new List<Product>();
            Random rnd = new Random();

            ParserSetting CardProductsSet = new ParserSetting
            {
                CssSelector = "div",
                ClassName = "catalog-card.card-flex"
            };

            if (url.Count != 0)
            {
                Log.Information("Starting the products url parser");

                foreach (var x in url)
                {
                    Thread.Sleep(rnd.Next(3, 6) * 1000);
                    CardProductsSet.BaseUrls = x.UrlCategory;

                    if (x.Id == 39) break;// ограничение на количество итераций для проверки выгрузка одной подкатегории;
                    var html = new HtmlLoader();
                    IHtmlDocument? htmlContent = await HtmlLoader.HtmlDownlLoader(CardProductsSet, 1);
                    if (htmlContent != null)
                    {
                        var div = htmlContent.QuerySelector("div.Paginator__page:last-child");
                        if (div != null)
                        {
                            var endPage = div.QuerySelector("a")?.GetAttribute("href");
                            if (endPage != null)
                                CardProductsSet.EndPage = endPage[endPage.Length - 1] - '0';
                        }

                        var divCardFlex = htmlContent.QuerySelectorAll("div.catalog-card.card-flex");
                        if (divCardFlex != null)
                        {
                            ProdUrlGet(divCardFlex, result, Aptekaru, x.Id);
                        }
                        else
                        {
                            Log.Error("Not found selector: div.catalog-card.card-flex");
                        }
                    }

                    for (int i = 2; i <= CardProductsSet.EndPage; i++)
                    {
                        Thread.Sleep(rnd.Next(3, 6) * 1000);
                        htmlContent = await HtmlLoader.HtmlDownlLoader(CardProductsSet, i);
                        if (htmlContent != null)
                        {
                            var divCardFlex = htmlContent.QuerySelectorAll("div.catalog-card.card-flex");
                            if (divCardFlex != null)
                            {
                                ProdUrlGet(divCardFlex, result, Aptekaru, x.Id);
                            }
                            else
                            {
                                Log.Error("Not found selector: div.catalog-card.card-flex");
                            }
                        }
                    }
                }
                Log.Information("Finishing the products url parser");
            }
            else
            {
                Log.Error("Sub categories are missing in the database");
            }

            return result.ToArray();
        }

        private static void ProdUrlGet(IHtmlCollection<IElement>? divCard, List<Product> ProdUrl, ParserSetting Aptekaru, int CatId)
        {
            foreach (var x in divCard)
            {
                var cardVar = x.QuerySelector("div.CardVariants__list")?.QuerySelectorAll("a");

                if (cardVar != null)
                    foreach (var y in cardVar)
                    {
                        Product prodv = new Product
                        {
                            CategoryId = CatId
                        };
                        string? var = y.GetAttribute("href");
                        if (var != null)
                        {
                            prodv.ProdUrl = Aptekaru.BaseUrls + var;
                            ProdUrl.Add(prodv);
                        }
                    }
                else
                {
                    Product prod = new Product
                    {
                        CategoryId = CatId
                    };
                    var tegUrl = x.QuerySelector("a")?.GetAttribute("href");
                    if (tegUrl != null)
                        prod.ProdUrl = Aptekaru.BaseUrls + tegUrl;
                    ProdUrl.Add(prod);
                }
            }
        }
    }

}