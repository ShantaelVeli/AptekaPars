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
        public static async Task<Product[]> ParsProductGet(Category SubCategoryUrl, ParserSetting Aptekaru, List<ProxySetting> proxy, int indexProxey)
        {
            List<Product> result = new List<Product>();
            List<ParserSetting> nonCatalogProducts = new List<ParserSetting>();

            ParserSetting CardProductsSet = new ParserSetting
            {
                BaseUrls = SubCategoryUrl.UrlCategory,
                CssSelector = "div",
                ClassName = "catalog-card.card-flex"
            };

            IHtmlDocument? htmlContent = await proxyDowmloaderPage(CardProductsSet, indexProxey, proxy, 1);

            if (htmlContent != null)
            {
                var div = htmlContent.QuerySelectorAll("div.Paginator__page");
                if (div.Length != 0)
                {
                    var endPage = div[div.Length - 1].QuerySelector("a")?.GetAttribute("href");
                    if (endPage != null && endPage != "")
                    {
                        var match = Regex.Match(endPage, @"[?&]page=(\d+)");
                        if (match.Success)
                        {
                            CardProductsSet.EndPage = int.Parse(match.Groups[1].Value);
                        }
                    }
                }

                GetProductAttribute(SubCategoryUrl, result, Aptekaru, CardProductsSet, htmlContent, nonCatalogProducts);
            }

            for (int i = 2; i <= CardProductsSet.EndPage; i++)
            {
                htmlContent = null;
                htmlContent = await proxyDowmloaderPage(CardProductsSet, indexProxey, proxy, i);
                if (htmlContent != null)
                    GetProductAttribute(SubCategoryUrl, result, Aptekaru, CardProductsSet, htmlContent, nonCatalogProducts);
            }

            if (nonCatalogProducts.Count != 0)
            {
                foreach (var x in nonCatalogProducts)
                {
                    htmlContent = null;
                    htmlContent = await proxyDowmloaderPage(x, indexProxey, proxy, 1);
                    if (htmlContent != null)
                        GetAttributeNonCatalogProducts(SubCategoryUrl, result, Aptekaru, htmlContent, x);
                }

            }
            return result.ToArray();
        }

        private static async Task<IHtmlDocument?> proxyDowmloaderPage(ParserSetting CardProductsSet, int indexProxey, List<ProxySetting> proxy, int nPage)
        {
            Random rnd = new Random();
            await Task.Delay(rnd.Next(3, 6) * 1000);
            IHtmlDocument? htmlContent = null;

            if (proxy != null)
            {
                for (int i = indexProxey; i < proxy.Count; i++)
                {
                    indexProxey++;
                    if (indexProxey >= proxy.Count) indexProxey = 0;
                    if (proxy[i].Toggle == false) continue;
                    htmlContent = await HtmlLoader.HtmlDownlLoader(CardProductsSet, nPage, proxy[i]);
                    if (htmlContent != null) break;
                }
            }
            if (htmlContent == null)
                htmlContent = await HtmlLoader.HtmlDownlLoader(CardProductsSet, nPage, null);

            return htmlContent;
        }


        private static void GetProductAttribute(Category SubCategoryUrl, List<Product> result, ParserSetting Aptekaru, ParserSetting CardProductsSet, IHtmlDocument htmlContent, List<ParserSetting> nonCatalogProducts)
        {
            var div = htmlContent.QuerySelectorAll(CardProductsSet.CssSelector + "." + CardProductsSet.ClassName);
            if (div.Length != 0)
            {
                foreach (var x in div)
                {
                    Product prod = new Product();
                    prod.ProdUrl = Aptekaru.BaseUrls + x.QuerySelector("a.catalog-card__photos")?.GetAttribute("href");

                    var cardVar = x.QuerySelector("div.CardVariants__list")?.QuerySelectorAll("a");
                    if (cardVar != null && cardVar.Length != 0)
                    {
                        for (int i = 0; i < cardVar.Length; i++)
                        {
                            ParserSetting now = new ParserSetting();
                            now.BaseUrls = Aptekaru.BaseUrls + cardVar[i].GetAttribute("href");
                            if (now.BaseUrls != prod.ProdUrl)
                                nonCatalogProducts.Add(now);
                        }
                    }

                    prod.CategoryId = SubCategoryUrl.Id;
                    prod.ImageUrl = x.QuerySelector("div.CardPhotos")?.QuerySelector(".CardPhotos img")?.GetAttribute("src");
                    prod.Name = x.QuerySelector("span.catalog-card__name.emphasis")?.GetAttribute("title");

                    var price = x.QuerySelector("span.moneyprice__roubles")?.TextContent.Trim();
                    if (price != null && price != "")
                        prod.Price = double.Parse(Regex.Replace(price, @"\s+", ""), CultureInfo.InvariantCulture);

                    var cardExpect = x.QuerySelector("div.catalog-card__expect");
                    if (cardExpect != null)
                        prod.Availability = "нет в наличии";
                    else
                        prod.Availability = "есть в наличии";


                    result.Add(prod);
                }
            }
        }

        private static void GetAttributeNonCatalogProducts(Category SubCategoryUrl, List<Product> result, ParserSetting Aptekaru, IHtmlDocument htmlContent, ParserSetting nonCatalogProducts)
        {
            Product prod = new Product();
            prod.CategoryId = SubCategoryUrl.Id;

            var cardExpect = htmlContent.QuerySelector("div.ProductOffer__unavailable");
            if (cardExpect != null)
                prod.Availability = "нет в наличии";
            else
                prod.Availability = "есть в наличие";

            var price = htmlContent.QuerySelector("span.moneyprice__roubles")?.TextContent.Trim();
            if (price != null && price != "")
                prod.Price = double.Parse(Regex.Replace(price, @"\s+", ""), CultureInfo.InvariantCulture);

            prod.ImageUrl = htmlContent.QuerySelector("div.ProductPhotos-image")?.QuerySelector("picture.photopicture")?.QuerySelector("source")?.GetAttribute("srcset");
            prod.Name = htmlContent.QuerySelector("h1.ViewProductPage__title")?.TextContent;
            prod.ProdUrl = nonCatalogProducts.BaseUrls;

            result.Add(prod);
        }







        // public static async Task<Product[]> ParsProductGet2(List<Category> SubCategoryUrl, ParserSetting Aptekaru, List<ProxySetting> proxy)
        // {
        //     Random rnd = new Random();
        //     int indexProxy = 0;

        //     Product[] products = await ParsProductsUrl(SubCategoryUrl, Aptekaru, proxy);

        //     ParserSetting CardProductsSet = new ParserSetting
        //     {
        //         CssSelector = "div",
        //         ClassName = "catalog-card.card-flex"
        //     };

        //     foreach (var x in products)
        //     {
        //         CardProductsSet.BaseUrls = x.ProdUrl;
        //         Thread.Sleep(rnd.Next(3, 6) * 1000);
        //         IHtmlDocument? htmlContent = null;

        //         if (proxy != null)
        //         {
        //             for (int i = indexProxy; i < proxy.Count; i++)
        //             {
        //                 indexProxy++;
        //                 if (indexProxy >= proxy.Count) indexProxy = 0;
        //                 if (proxy[i].Toggle == false) continue;
        //                 htmlContent = await HtmlLoader.HtmlDownlLoader(CardProductsSet, 1, proxy[i]);
        //                 if (htmlContent != null) break;
        //             }
        //         }

        //         if (htmlContent == null)
        //             htmlContent = await HtmlLoader.HtmlDownlLoader(CardProductsSet, 1, null);



        //         if (htmlContent != null)
        //         {
        //             var cardExpect = htmlContent.QuerySelector("div.ProductOffer__unavailable")?.QuerySelector("span");
        //             if (cardExpect != null)
        //             {
        //                 x.Price = 0.0;
        //                 x.Availability = cardExpect.TextContent;
        //             }
        //             else
        //             {
        //                 x.Availability = "есть в наличие";
        //                 var prodOffer = htmlContent.QuerySelector("div.ProductOffer__price");
        //                 if (prodOffer != null)
        //                 {
        //                     string? prodPrice = prodOffer.QuerySelector("span.moneyprice__roubles")?.TextContent + prodOffer.QuerySelector("span.moneyprice__pennies")?.TextContent;
        //                     if (prodPrice != null && prodPrice != "")
        //                         x.Price = double.Parse(Regex.Replace(prodPrice, @"\s+", ""), CultureInfo.InvariantCulture);
        //                 }
        //             }

        //             var cardPhoto = htmlContent.QuerySelector("div.ProductPhotos-image");
        //             if (cardPhoto != null)
        //             {
        //                 x.ImageUrl = cardPhoto.QuerySelector("picture.photopicture")?.QuerySelector("source")?.GetAttribute("srcset");
        //             }
        //             x.Name = htmlContent.QuerySelector("h1.ViewProductPage__title")?.TextContent;
        //         }
        //     }
        //     return products;

        // }

        // private static async Task<Product[]> ParsProductsUrl(List<Category> url, ParserSetting Aptekaru, List<ProxySetting> proxy)
        // {
        //     var result = new List<Product>();
        //     Random rnd = new Random();
        //     int indexProxey = 0;

        //     ParserSetting CardProductsSet = new ParserSetting
        //     {
        //         CssSelector = "div",
        //         ClassName = "catalog-card.card-flex"
        //     };

        //     if (url.Count != 0)
        //     {
        //         Log.Information("Starting the products url parser");

        //         foreach (var x in url)//
        //         {
        //             if (x.Id < 39) continue;
        //             if (x.Id == 40) break;
        //             Thread.Sleep(rnd.Next(3, 6) * 1000);
        //             CardProductsSet.BaseUrls = x.UrlCategory;
        //             IHtmlDocument? htmlContent = null;

        //             if (proxy != null && proxy.Count != 0)
        //             {
        //                 for (int i = indexProxey; i < proxy.Count; i++)
        //                 {
        //                     indexProxey++;
        //                     if (indexProxey >= proxy.Count) indexProxey = 0;
        //                     if (proxy[i].Toggle == false) continue;
        //                     htmlContent = await HtmlLoader.HtmlDownlLoader(CardProductsSet, 1, proxy[i]);
        //                     if (htmlContent != null) break;
        //                 }
        //             }

        //             if (htmlContent == null)
        //                 htmlContent = await HtmlLoader.HtmlDownlLoader(CardProductsSet, 1, null);


        //             if (htmlContent != null)
        //             {
        //                 var div = htmlContent.QuerySelectorAll("div.Paginator__page");
        //                 if (div != null && div.Length != 0)
        //                 {
        //                     var endPage = div[div.Length - 1].QuerySelector("a")?.GetAttribute("href");
        //                     if (endPage != null)
        //                     {
        //                         var match = Regex.Match(endPage, @"[?&]page=(\d+)");
        //                         if (match.Success)
        //                         {
        //                             CardProductsSet.EndPage = int.Parse(match.Groups[1].Value);
        //                         }


        //                     }

        //                     var divCardFlex = htmlContent.QuerySelectorAll("div.catalog-card.card-flex");
        //                     if (divCardFlex != null)
        //                     {
        //                         ProdUrlGet(divCardFlex, result, Aptekaru, x.Id);
        //                     }
        //                     else
        //                     {
        //                         Log.Error("Not found selector: div.catalog-card.card-flex");
        //                     }
        //                 }

        //                 for (int i = 2; i <= CardProductsSet.EndPage; i++)
        //                 {
        //                     Thread.Sleep(rnd.Next(3, 6) * 1000);

        //                     htmlContent = null;

        //                     if (proxy != null)
        //                     {
        //                         for (int j = indexProxey; j < proxy.Count; j++)
        //                         {
        //                             indexProxey++;
        //                             if (indexProxey >= proxy.Count) indexProxey = 0;
        //                             if (proxy[j].Toggle == false) continue;
        //                             htmlContent = await HtmlLoader.HtmlDownlLoader(CardProductsSet, i, proxy[j]);
        //                             if (htmlContent != null) break;
        //                         }
        //                     }

        //                     if (htmlContent == null)
        //                         htmlContent = await HtmlLoader.HtmlDownlLoader(CardProductsSet, i, null);

        //                     if (htmlContent != null)
        //                     {
        //                         var divCardFlex = htmlContent.QuerySelectorAll("div.catalog-card.card-flex");
        //                         if (divCardFlex != null)
        //                         {
        //                             ProdUrlGet(divCardFlex, result, Aptekaru, x.Id);
        //                         }
        //                         else
        //                         {
        //                             Log.Error("Not found selector: div.catalog-card.card-flex");
        //                         }
        //                     }
        //                 }
        //             }//
        //             Log.Information("Finishing the products url parser");
        //         }
        //     }
        //     else
        //     {
        //         Log.Error("Sub categories are missing in the database");
        //     }

        //     return result.ToArray();
        // }

        // private static void ProdUrlGet(IHtmlCollection<IElement> divCard, List<Product> ProdUrl, ParserSetting Aptekaru, int CatId)
        // {
        //     foreach (var x in divCard)
        //     {
        //         var cardVar = x.QuerySelector("div.CardVariants__list")?.QuerySelectorAll("a");

        //         if (cardVar != null)
        //             foreach (var y in cardVar)
        //             {
        //                 Product prodv = new Product
        //                 {
        //                     CategoryId = CatId
        //                 };
        //                 string? var = y.GetAttribute("href");
        //                 if (var != null)
        //                 {
        //                     prodv.ProdUrl = Aptekaru.BaseUrls + var;
        //                     ProdUrl.Add(prodv);
        //                 }
        //             }
        //         else
        //         {
        //             Product prod = new Product
        //             {
        //                 CategoryId = CatId
        //             };
        //             var tegUrl = x.QuerySelector("a")?.GetAttribute("href");
        //             if (tegUrl != null)
        //                 prod.ProdUrl = Aptekaru.BaseUrls + tegUrl;
        //             ProdUrl.Add(prod);
        //         }
        //     }
        // }
    }

}