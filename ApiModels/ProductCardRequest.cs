using System.ComponentModel.DataAnnotations;
using mservis.Models.Settings;

namespace mservis.Models.ApiModels;

public class ProductCardRequest : ApiKeys
{
    [Required]
    public string[] SearchPhraseList { get; set; } = [];

    public int WaitTimeout { get; set; } = 1000;
    
    public int MaxProductsCount { get; set; } = 25;
}