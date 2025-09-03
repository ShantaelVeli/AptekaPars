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
                    Cron.Monthly
                );
                Log.Information("Finish parsing Apteka.ru");

                // var recurringJobManager = new RecurringJobManager();
                // recurringJobManager.TriggerJob("category-parser");
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
                    Cron.Weekly
                );
                var recurringJobManager = new RecurringJobManager();
                recurringJobManager.TriggerJob("product-parser");
                Log.Information("Finish parsing Apteka.ru");
            }
            catch
            {
                Log.Fatal("The parser was not started");
            }
        }
    }
}