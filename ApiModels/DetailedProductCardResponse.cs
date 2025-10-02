using System.ComponentModel.DataAnnotations;
using mservis.Models.ProductModels;
using mservis.Models.Settings;

namespace mservis.Models.ApiModels;

public class DetailedProductCardResponse : ApiKeys
{
    [Required]
    public List<DetailedProduct> ProductCards { get; set; } = new();
}