using System.ComponentModel.DataAnnotations;
using mservis.Models.ApiModels;
using mservis.Models.Settings;

namespace mservis;

public class DetailedProductCardRequest : ApiKeys
{
    [Required]
    public bool CanLoadAttachments { get; set; } = false;
    
    [Required]
    public List<string> ProductLinks { get; set; } = [];
}
