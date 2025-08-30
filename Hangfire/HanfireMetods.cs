using Hangfire;
using Serilog;

namespace Parse.Core.Hangfire
{
    public static class ApplicationExten
    {
        public static void LaunchCategPars(this IApplicationBuilder ab)
        {
            try
            {
                Log.Information("Start parsing Apteka.ru");
                RecurringJob.AddOrUpdate<ParseFull>(
                    "category-parser",
                    x => x.ParsingFullCategoryAsync(),
                    Cron.Weekly()
                );
                Log.Information("Finish parsing Apteka.ru");
            }
            catch
            {
                Log.Fatal("The parser was not started");
            }
        }
        public static void LaunchProductsPars(this IApplicationBuilder ab)
        {
            try
            {
                Log.Information("Start parsing Apteka.ru");
                RecurringJob.AddOrUpdate<ParseFull>(
                    "product-parser",
                    x => x.ParsingFullProductsAsync(),
                    Cron.Daily()
                );
                Log.Information("Finish parsing Apteka.ru");
            }
            catch
            {
                Log.Fatal("The parser was not started");
            }
        }
    }
}