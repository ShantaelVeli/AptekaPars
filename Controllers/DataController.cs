using Microsoft.AspNetCore.Mvc;
using Serilog;
using Microsoft.EntityFrameworkCore;
using mservis.Models.ApiModels;
using mservis.Models.ProductModels;
using mservis;
using System.Net.Http;

namespace DataBase.Contexts.Controller
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string? StackTrace { get; set; }
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
        [HttpPost("search")]
        public async Task<IActionResult> GetProductsAsync([FromBody] ProductCardRequest requestModel)
        {
            Log.Information("Start data export of Products from the database");
            if (!ModelState.IsValid)
            {
                Log.Error("BadRequest");
                return BadRequest(ModelState);
            }
            var res = new List<Variant>();
            

            foreach (var nameProduct in requestModel.SearchPhraseList)
            {
                var buffer = new List<Product>();
                var LowNameProduct = nameProduct.ToLower();

                buffer = await _dbContext.Products.FromSqlRaw("SELECT * FROM \"Products\" c WHERE to_tsvector('russian', \"Name\") @@ plainto_tsquery({0}) LIMIT ({1});", LowNameProduct, requestModel.MaxProductsCount).ToListAsync();

                if (buffer.Count == 0)
                    buffer = await _dbContext.Products.FromSqlRaw("SELECT * FROM \"Products\" c WHERE LOWER (c.\"Name\") LIKE LOWER ({0}) LIMIT ({1});", "%" + LowNameProduct + "%", requestModel.MaxProductsCount).ToListAsync();

                foreach (var x in buffer)
                {
                    var newVar = new Variant();
                    newVar.Phrase = nameProduct;
                    ProductRet newProd = new ProductRet();
                    if (x.Name != null)
                        newProd.Name = x.Name;

                    newProd.Price = Convert.ToDecimal(x.Price);

                    if (x.ProdUrl != null)
                        newProd.Link = x.ProdUrl;

                    if (x.Availability == "есть в наличии")
                        newProd.QuantityCurrent = 1;

                    newVar.Products.Add(newProd);

                    res.Add(newVar);
                }
            }
            return Ok(res);
        }

        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("details")]
        public async Task<IActionResult> GetDetailedProductsAsync([FromBody] DetailedProductCardRequest requestModel)
        {
            Log.Information("Start data export of Products from the database");
            if (!ModelState.IsValid)
            {
                Log.Error("BadRequest");
                return BadRequest(ModelState);
            }
            var res = new DetailedProductCardResponse();
            res.App = requestModel.App;

            foreach (var url in requestModel.ProductLinks)
            {

                var buffer = await _dbContext.Products.FromSqlRaw("SELECT * FROM \"Products\" c WHERE c.\"ProdUrl\" = {0}", url).ToListAsync();

                foreach (var x in buffer)
                {
                    var detail = new DetailedProduct();
                    var img = new Image();
                    if (x.Name != null)
                        detail.Name = x.Name;
                    detail.Price = Convert.ToDecimal(x.Price);

                    if (x.Availability == "есть в наличии")
                        detail.QuantityCurrent = 1;
                    if (x.ProdUrl != null)
                        detail.Link = x.ProdUrl;

                    HttpClient httpClient = new HttpClient();
                    var imgByte = await httpClient.GetByteArrayAsync(x.ImageUrl);
                    img.Format = "jpeg";
                    img.Base64Content = Convert.ToBase64String(imgByte);
                    detail.Images.Add(img);
                    res.ProductCards.Add(detail);
                }
            }

            return Ok(res);
        }







        //[Route("/pars/Apteka/SearchProductsByCategoryIdAsync/{CategoryId:int:required}")]
        //     public async Task<IActionResult> SearchProductsByCategoryIdAsync([FromBody] ProductCardRequest requestModel)
        //     {

        //         Log.Information("Start data export of Products from the database");
        //         var result = new List<ReturnData>();

        //         var buffer = await _dbContext.Categorys.FromSqlRaw("SELECT * FROM \"Categorys\" where \"Id\" = {0}", CategoryId).ToListAsync();

        //         if (buffer.Count != 0)
        //         {
        //             if (buffer[0].ParentCategoryId == null)
        //             {
        //                 var prod = await _dbContext.Products.FromSqlRaw("SELECT p.* from \"Categorys\" mc join \"Categorys\" sc on sc.\"ParentCategoryId\"  = mc.\"Id\" join \"Products\" p on p.\"CategoryId\" = sc.\"Id\" where mc.\"Id\" = {0};", CategoryId).ToListAsync();
        //                 if (prod.Count != 0)
        //                 {
        //                     foreach (var x in prod)
        //                     {
        //                         ReturnData now = new ReturnData();
        //                         now.Sinchron(x);
        //                         if (now.Name != null)
        //                         {
        //                             result.Add(now);
        //                         }
        //                     }
        //                     Log.Information("data export of Products success");
        //                     return Ok(result.ToArray());
        //                 }
        //                 else
        //                 {
        //                     Log.Information("Finish export result: not found products");
        //                     return NotFound("not found products");
        //                 }
        //             }
        //             else
        //             {
        //                 var prod = await _dbContext.Products.FromSqlRaw("SELECT p.* FROM \"Products\" p join \"Categorys\" c on c.\"Id\" = p.\"CategoryId\" where c.\"Id\" = {0};", CategoryId).ToListAsync();
        //                 if (prod.Count != 0)
        //                 {
        //                     foreach (var x in prod)
        //                     {
        //                         ReturnData now = new ReturnData();
        //                         now.Sinchron(x);
        //                         if (now.Name != null)
        //                         {
        //                             result.Add(now);
        //                         }
        //                     }
        //                     Log.Information("data export of Products success");
        //                     return Ok(result.ToArray());
        //                 }
        //                 else
        //                 {
        //                     Log.Information("Finish export result: not found products");
        //                     return NotFound("not found products");
        //                 }
        //             }
        //         }
        //         else
        //         {
        //             Log.Information("Finish export result: there are no categories in the database");
        //             return NotFound("there are no categories in the database");
        //         }



        //     }


        //     [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        //     [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        //     [HttpGet]
        //     [Route("/pars/Apteka/SearchProductsByCategoryNameAsync/{CategoryName}")]
        //     public async Task<IActionResult> SearchProductsByCategoryNameAsync([FromRoute] string CategoryName)
        //     {
        //         Log.Information("Start data export of Products from the database");
        //         var result = new List<ReturnData>();


        //         var buffer = await _dbContext.Categorys.FromSqlRaw("SELECT * FROM \"Categorys\" c WHERE LOWER (c.\"NameCategory\") LIKE LOWER ({0});", "%" + CategoryName + "%").ToListAsync();

        //         if (buffer.Count != 0)
        //         {
        //             foreach (var x in buffer)
        //             {
        //                 if (x.ParentCategoryId == null)
        //                 {
        //                     var prod = await _dbContext.Products.FromSqlRaw("SELECT p.* from \"Categorys\" mc join \"Categorys\" sc on sc.\"ParentCategoryId\"  = mc.\"Id\" join \"Products\" p on p.\"CategoryId\" = sc.\"Id\" where mc.\"Id\" = {0};", x.Id).ToListAsync();
        //                     foreach (var y in prod)
        //                     {
        //                         ReturnData now = new ReturnData();
        //                         now.Sinchron(y);
        //                         if (!result.Contains(now) && now.Name != null)
        //                         {
        //                             result.Add(now);
        //                         }
        //                     }
        //                 }
        //                 else
        //                 {
        //                     var prod = await _dbContext.Products.FromSqlRaw("SELECT p.* FROM \"Products\" p join \"Categorys\" c on c.\"Id\" = p.\"CategoryId\" where c.\"Id\" = {0};", x.Id).ToListAsync();
        //                     foreach (var y in prod)
        //                     {
        //                         ReturnData now = new ReturnData();
        //                         now.Sinchron(y);
        //                         if (!result.Contains(now) && now.Name != null)
        //                         {
        //                             result.Add(now);
        //                         }
        //                     }
        //                 }
        //             }
        //         }
        //         else
        //         {
        //             Log.Information("Finish export result: there are no categories in the database");
        //             return NotFound("there are no categories in the database");
        //         }

        //         if (result.Count != 0)
        //         {
        //             Log.Information("data export of Products success");
        //             return Ok(result.ToArray());
        //         }
        //         else
        //         {
        //             Log.Information("Finish export result: not found products");
        //             return NotFound("not found products");
        //         }
        //     }
        //     [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        //     [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        //     [HttpGet]
        //     [Route("/pars/Apteka/SearchProductsByNameProductAsync/{NameProduct}")]
        //     public async Task<IActionResult> SearchProductsByNameProductAsync([FromRoute] string NameProduct)
        //     {

        //         Log.Information("Start data export of Products from the database");
        //         var buffer = await _dbContext.Products.FromSqlRaw("SELECT * FROM \"Products\" c WHERE LOWER (c.\"Name\") LIKE LOWER ({0});", "%" + NameProduct + "%").ToListAsync();
        //         var result = new List<ReturnData>();

        //         foreach (var x in buffer)
        //         {
        //             var now = new ReturnData();
        //             now.Sinchron(x);
        //             if (now.Name != null)
        //                 result.Add(now);
        //         }


        //         if (result.Count != 0)
        //         {
        //             Log.Information("data export of Products success");
        //             return Ok(result.ToArray());
        //         }
        //         else
        //         {
        //             Log.Information("Finish export result: not found products");
        //             return NotFound("not found products");
        //         }

        //     }
        // }
    }
}