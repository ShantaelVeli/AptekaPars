using System.ComponentModel.DataAnnotations;
using mservis.Models.ProductModels;

namespace mservis.Models.ApiModels;

public class Variant
{
    [Required]
    public string Phrase { get; set; } = "";

    [Required]
    public List<ProductRet> Products { get; set; } = [];
}