namespace mservis.Models.ProductModels;

public class DetailedProduct
{
    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public Decimal Price { get; set; } = 0;

    public string PriceCurrency { get; set; } = "RUB";

    public decimal QuantityCurrent { get; set; } = 0;

    public decimal QuantityInStock { get; set; } = 0;

    public string Link { get; set; } = "";

    public string CatalogPath { get; set; } = "";

    public List<Property> Properties { get; set; } = [];

    public List<Image> Images { get; set; } = [];

    public List<Attachment> Attachments { get; set; } = [];
}

public class Attachment
{
    public string Name { get; set;} = "";
    
    public string Base64Content { get; set; } = "";
}

public class Image
{
    public string Format { get; set; } = "";

    public string Base64Content { get; set; } = "";
}

public class Property
{
    public string Name { get; set; } = "";
    
    public string Value { get; set; } = "";
}
