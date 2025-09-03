namespace Parse
{
    public class ParserSetting
    {
        public string? BaseUrls;
        public string? CssSelector;
        public string? ClassName;

        public int EndPage = 1;

        public void GetParseSeting(string url, string cssSelector, string className)
        {
            BaseUrls = url;
            CssSelector = cssSelector;
            ClassName = className;
        }
    }

    public class ProxySetting
    {
        public string? Name { get; set; }
        public string? Server { get; set; }
        public string? Port { get; set; }
        public string? User { get; set; }
        public string? Password { get; set; }
        public bool Toggle { get; set; }
    }



}