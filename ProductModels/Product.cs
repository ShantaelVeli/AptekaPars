namespace mservis.Models.ProductModels;

public class ProductRet
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public Decimal Price { get; set; } = 0;

    public string PriceCurrency { get; set; } = "RUB";

    public string Link { get; set; } = "";

    public Decimal QuantityCurrent { get; set; } = 0;
    
    public int QuantityInStock { get; set; } = 0;

    public string CatalogPath { get; set; } = "";
    
}
