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
}