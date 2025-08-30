using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DataBase.Contexts
{
    public class PushData
    {
        private readonly ApplicationContext _db;

        public PushData(ApplicationContext db)
        {
            _db = db;
        }
        public static async Task PushDataBase(Category[] data, ApplicationContext db)
        {

            Log.Information("input of categories into the database");
            foreach (var x in data)
            {
                string name = x.NameCategory;
                string? url = x.UrlCategory;
                int? parentId = x.ParentCategoryId;
                await db.Database.ExecuteSqlRawAsync("INSERT INTO \"Categorys\" (\"NameCategory\", \"UrlCategory\", \"ParentCategoryId\") VALUES ({0}, {1}, {2})" +
"ON CONFLICT (\"UrlCategory\") DO NOTHING", name, url, parentId);
            }
        }


        public static async Task PushDataBase(Product[] data, ApplicationContext db)
        {
            Log.Information("input of products into the database");
            
                foreach (var x in data)
                {

                    var exam = await db.Products.FromSqlRaw("SELECT * FROM \"Products\" WHERE \"ProdUrl\" = {0}", x.ProdUrl).FirstOrDefaultAsync();

                    if (exam != null)
                    {
                        if (exam.Availability != x.Availability)
                        {
                            await db.Database.ExecuteSqlRawAsync("UPDATE \"Products\" SET \"Availability\" = {0} WHERE \"ProdUrl\" = {1}", x.Availability, x.ProdUrl);
                        }

                        if (exam.Price != x.Price)
                        {
                            await db.Database.ExecuteSqlRawAsync("UPDATE \"Products\" SET \"Price\" = {0} WHERE \"ProdUrl\" = {1}", x.Price, x.ProdUrl);
                        }
                    }
                    else
                    {
                        string? name = x.Name;
                        string? url = x.ProdUrl;
                        int? CategoryId = x.CategoryId;
                        string? imgUrl = x.ImageUrl;
                        double price = x.Price;
                        string? expect = x.Availability;

                        await db.Database.ExecuteSqlRawAsync("INSERT INTO \"Products\" (\"Name\", \"Price\", \"ImageUrl\", \"ProdUrl\"," +
                        "\"CategoryId\", \"Availability\") VALUES ({0}, {1}, {2}, {3}, {4}, {5})" + "ON CONFLICT (\"ProdUrl\") DO NOTHING", name, price, imgUrl, url, CategoryId, expect);
                    }
                }
            
        }
    }

    public class PullData
    {
        private readonly ApplicationContext _db;

        public PullData(ApplicationContext db)
        {
            _db = db;
        }
        public static async Task<List<Category>> PullAllMainCategorys(ApplicationContext db)
        {
            List<Category> CategoryUrl;
            
                Log.Information("data export of URLs under maincategories from the database");
                CategoryUrl = await db.Categorys.FromSqlRaw("SELECT * FROM \"Categorys\" WHERE \"ParentCategoryId\" IS NULL").ToListAsync();
            
            return CategoryUrl;
        }

        public static async Task<List<Category>> PullAllSubCategorys(ApplicationContext db)
        {
            List<Category> CategoryUrl;
            
                Log.Information("data export of URLs under subcategories from the database");
                CategoryUrl = await db.Categorys.FromSqlRaw("SELECT * FROM \"Categorys\" WHERE \"ParentCategoryId\" IS NOT NULL").ToListAsync();
            
            return CategoryUrl;
        }
    }
}