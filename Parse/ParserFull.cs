using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using DataBase.Contexts;
using Serilog;

namespace Parse
{
    public class ParseFull
    {
        private readonly ApplicationContext _db;

        public ParseFull(ApplicationContext db)
        {
            _db = db;
        }
        public async Task ParsingFullCategoryAsync()
        {
            ParserSetting AptekaRu = new ParserSetting();
            AptekaRu.GetParseSeting("https://apteka.ru", "div", "SidebarCategoriesList");
            Category[] mainCategory = await ParseCategory.ParseMainCategorysAsync(AptekaRu);
            await PushData.PushDataBase(mainCategory, _db);
            List<Category> mainCategoryUrl = await PullData.PullAllMainCategorys(_db);
            Category[] SubCategory = await ParseCategory.ParseSubCategorysAsync(mainCategoryUrl, AptekaRu);
            await PushData.PushDataBase(SubCategory, _db);
        }

        public async Task ParsingFullProductsAsync()
        {
            ParserSetting AptekaRu = new ParserSetting();
            AptekaRu.GetParseSeting("https://apteka.ru", "div", "SidebarCategoriesList");
            List<Category> subCategoryUrl = await PullData.PullAllSubCategorys(_db);
            Product[] products = await ParsProduct.ParsProductGet(subCategoryUrl, AptekaRu);
            await PushData.PushDataBase(products, _db);
        }
    }

}

