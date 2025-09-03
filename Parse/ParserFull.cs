using DataBase.Contexts;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;



namespace Parse
{
    public class ParseFull
    {
        private readonly ApplicationContext _db;
        private readonly IConfiguration _configuration;
        private List<ProxySetting> _proxy;

        public ParseFull(ApplicationContext db, IConfiguration configuration) //
        {
            _db = db;
            _configuration = configuration;
            _proxy = _configuration.GetSection("Proxies").Get<List<ProxySetting>>() ?? new List<ProxySetting>();

        }
        public async Task ParsingFullCategoryAsync()
        {
            ParserSetting AptekaRu = new ParserSetting();
            AptekaRu.GetParseSeting("https://apteka.ru", "div", "SidebarCategoriesList");
            Category[] mainCategory = await ParseCategory.ParseMainCategorysAsync(AptekaRu, _proxy);
            await PushData.PushDataBase(mainCategory, _db);
            List<Category> mainCategoryUrl = await PullData.PullAllMainCategorys(_db);
            Category[] SubCategory = await ParseCategory.ParseSubCategorysAsync(mainCategoryUrl, AptekaRu, _proxy);
            await PushData.PushDataBase(SubCategory, _db);
        }

        public async Task ParsingFullProductsAsync()
        {
            ParserSetting AptekaRu = new ParserSetting();
            AptekaRu.GetParseSeting("https://apteka.ru", "div", "SidebarCategoriesList");
            List<Category> subCategoryUrl = await PullData.PullAllSubCategorys(_db);
            int indexProxy = 0;

            for (int i = 1; i < 2; i++) //subCategoryUrl.Count
            {
                Product[] products = await ParsProduct.ParsProductGet(subCategoryUrl[i], AptekaRu, _proxy, indexProxy);
                await PushData.PushDataBase(products, _db);
            }
        }
    }

}

