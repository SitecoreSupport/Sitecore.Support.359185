namespace Sitecore.Support.Layouts
{
    // extened existing class to add VaryByUrlPath property
    public class RenderingCaching : Sitecore.Layouts.RenderingCaching
    {
        public bool VaryByUrlPath { get; set; }
    }
}