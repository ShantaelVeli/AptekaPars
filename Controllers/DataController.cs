using Microsoft.AspNetCore.Mvc;
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Contexts.Controller
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string? StackTrace { get; set; }
    }

    public class ReturnData
    {
        public string? Name { get; set; }
        public double Price { get; set; }
        public string? Img { get; set; }
        public string? Url { get; set; }
        public string? Availability { get; set; }

        public void Sinchron(Product prod)
        {
            Name = prod.Name;
            Price = prod.Price;
            Img = prod.ImageUrl;
            Url = prod.ProdUrl;
            Availability = prod.Availability;
        }
    }



    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {

        private readonly ApplicationContext _dbContext;

        public DataController(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }


        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("/pars/Apteka/SearchProductsByCategoryIdAsync/{CategoryId:int:required}")]
        public async Task<IActionResult> SearchProductsByCategoryIdAsync([FromRoute] int CategoryId)
        {

            Log.Information("Start data export of Products from the database");
            var result = new List<ReturnData>();

            var buffer = await _dbContext.Categorys.FromSqlRaw("SELECT * FROM \"Categorys\" where \"Id\" = {0}", CategoryId).ToListAsync();

            if (buffer.Count != 0)
            {
                if (buffer[0].ParentCategoryId == null)
                {
                    var prod = await _dbContext.Products.FromSqlRaw("SELECT p.* from \"Categorys\" mc join \"Categorys\" sc on sc.\"ParentCategoryId\"  = mc.\"Id\" join \"Products\" p on p.\"CategoryId\" = sc.\"Id\" where mc.\"Id\" = {0};", CategoryId).ToListAsync();
                    if (prod.Count != 0)
                    {
                        foreach (var x in prod)
                        {
                            ReturnData now = new ReturnData();
                            now.Sinchron(x);
                            if (now.Name != null)
                            {
                                result.Add(now);
                            }
                        }
                        Log.Information("data export of Products success");
                        return Ok(result.ToArray());
                    }
                    else
                    {
                        Log.Information("Finish export result: not found products");
                        return NotFound("not found products");
                    }
                }
                else
                {
                    var prod = await _dbContext.Products.FromSqlRaw("SELECT p.* FROM \"Products\" p join \"Categorys\" c on c.\"Id\" = p.\"CategoryId\" where c.\"Id\" = {0};", CategoryId).ToListAsync();
                    if (prod.Count != 0)
                    {
                        foreach (var x in prod)
                        {
                            ReturnData now = new ReturnData();
                            now.Sinchron(x);
                            if (now.Name != null)
                            {
                                result.Add(now);
                            }
                        }
                        Log.Information("data export of Products success");
                        return Ok(result.ToArray());
                    }
                    else
                    {
                        Log.Information("Finish export result: not found products");
                        return NotFound("not found products");
                    }
                }
            }
            else
            {
                Log.Information("Finish export result: there are no categories in the database");
                return NotFound("there are no categories in the database");
            }



        }


        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("/pars/Apteka/SearchProductsByCategoryNameAsync/{CategoryName}")]
        public async Task<IActionResult> SearchProductsByCategoryNameAsync([FromRoute] string CategoryName)
        {
            Log.Information("Start data export of Products from the database");
            var result = new List<ReturnData>();


            var buffer = await _dbContext.Categorys.FromSqlRaw("SELECT * FROM \"Categorys\" c WHERE LOWER (c.\"NameCategory\") LIKE LOWER ({0});", "%" + CategoryName + "%").ToListAsync();

            if (buffer.Count != 0)
            {
                foreach (var x in buffer)
                {
                    if (x.ParentCategoryId == null)
                    {
                        var prod = await _dbContext.Products.FromSqlRaw("SELECT p.* from \"Categorys\" mc join \"Categorys\" sc on sc.\"ParentCategoryId\"  = mc.\"Id\" join \"Products\" p on p.\"CategoryId\" = sc.\"Id\" where mc.\"Id\" = {0};", x.Id).ToListAsync();
                        foreach (var y in prod)
                        {
                            ReturnData now = new ReturnData();
                            now.Sinchron(y);
                            if (!result.Contains(now) && now.Name != null)
                            {
                                result.Add(now);
                            }
                        }
                    }
                    else
                    {
                        var prod = await _dbContext.Products.FromSqlRaw("SELECT p.* FROM \"Products\" p join \"Categorys\" c on c.\"Id\" = p.\"CategoryId\" where c.\"Id\" = {0};", x.Id).ToListAsync();
                        foreach (var y in prod)
                        {
                            ReturnData now = new ReturnData();
                            now.Sinchron(y);
                            if (!result.Contains(now) && now.Name != null)
                            {
                                result.Add(now);
                            }
                        }
                    }
                }
            }
            else
            {
                Log.Information("Finish export result: there are no categories in the database");
                return NotFound("there are no categories in the database");
            }

            if (result.Count != 0)
            {
                Log.Information("data export of Products success");
                return Ok(result.ToArray());
            }
            else
            {
                Log.Information("Finish export result: not found products");
                return NotFound("not found products");
            }
        }
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [HttpGet]
        [Route("/pars/Apteka/SearchProductsByNameProductAsync/{NameProduct}")]
        public async Task<IActionResult> SearchProductsByNameProductAsync([FromRoute] string NameProduct)
        {

            Log.Information("Start data export of Products from the database");
            var buffer = await _dbContext.Products.FromSqlRaw("SELECT * FROM \"Products\" c WHERE LOWER (c.\"Name\") LIKE LOWER ({0});", "%" + NameProduct + "%").ToListAsync();
            var result = new List<ReturnData>();

            foreach (var x in buffer)
            {
                var now = new ReturnData();
                now.Sinchron(x);
                if (now.Name != null)
                    result.Add(now);
            }


            if (result.Count != 0)
            {
                Log.Information("data export of Products success");
                return Ok(result.ToArray());
            }
            else
            {
                Log.Information("Finish export result: not found products");
                return NotFound("not found products");
            }

        }
    }
}