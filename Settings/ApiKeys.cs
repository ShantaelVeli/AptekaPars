using System.ComponentModel.DataAnnotations;
using mservis.Models.ApiModels;

namespace mservis.Models.Settings;

public class ApiKeys
{
    [Required]
    public App App { get; set; } = new App();
}

public static class ApiKeysComparator
{
    public static bool IsEqual(ApiKeys a, ApiKeys b)
    {
        if (a == null || b == null)
        {
            return false;
        }
        
        return a.App.AppId == b.App.AppId 
            && a.App.AppSecret == b.App.AppSecret;
    }
}