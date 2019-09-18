namespace Sitecore.Support.XA.Foundation.Presentation.Pipelines.GetRenderingCaching
{
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Mvc.Extensions;
    using Sitecore.Mvc.Presentation;
    using Sitecore.Support.Layouts;
    using Sitecore.XA.Foundation.Presentation;
    using Sitecore.XA.Foundation.Presentation.Pipelines.GetRenderingCaching;
    using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;

    public class GetSiteLevelCaching
    {
        //
        public static RenderingCaching Parse(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            RenderingCaching caching1 = new RenderingCaching();
            caching1.Cacheable = item["cacheable"] == "1";
            caching1.VaryByData = item["VaryByData"] == "1";
            caching1.VaryByDevice = item["VaryByDevice"] == "1";
            caching1.VaryByLogin = item["VaryByLogin"] == "1";
            caching1.VaryByParm = item["VaryByParm"] == "1";
            caching1.VaryByQueryString = item["VaryByQueryString"] == "1";
            caching1.VaryByUser = item["VaryByUser"] == "1";
            caching1.ClearOnIndexUpdate = item["ClearOnIndexUpdate"] == "1";

            // added VaryByUrlPath parameter to get it in ApplyVaryByUrlPath processor
            caching1.VaryByUrlPath = item["VaryByUrlPath"] == "1";
            return caching1;
        }

        public static void PatchedSetCachingOptions(Rendering rendering, RenderingCaching renderingCaching)
        {
            Assert.ArgumentNotNull(rendering, "rendering");
            Assert.ArgumentNotNull(renderingCaching, "renderingCaching");
            rendering["Cacheable"] = renderingCaching.Cacheable.ToBoolString();
            rendering["Cache_VaryByData"] = renderingCaching.VaryByData.ToBoolString();
            rendering["Cache_VaryByDevice"] = renderingCaching.VaryByDevice.ToBoolString();
            rendering["Cache_VaryByLogin"] = renderingCaching.VaryByLogin.ToBoolString();
            rendering["Cache_VaryByParameters"] = renderingCaching.VaryByParm.ToBoolString();
            rendering["Cache_VaryByQueryString"] = renderingCaching.VaryByQueryString.ToBoolString();
            rendering["Cache_VaryByUser"] = renderingCaching.VaryByUser.ToBoolString();

            // added VaryByUrlPath parameter to get it in ApplyVaryByUrlPath processor
            rendering["Cache_VaryByUrlPath"] = renderingCaching.VaryByUrlPath.ToBoolString();
        }

        
        private readonly IPresentationContext _presentationContext;
        
        public GetSiteLevelCaching(IPresentationContext presentationContext)
        {
            this._presentationContext = presentationContext;
        }

        protected virtual void PopulateCacheOptions(Rendering rendering, Item cacheSettingsItem)
        {
            if (cacheSettingsItem.InheritsFrom(Templates.Caching.ID))
            {
                RenderingCaching renderingCaching = Parse(cacheSettingsItem);
                PatchedSetCachingOptions(rendering, renderingCaching);
            }
        }

        public void Process(GetRenderingCachingArgs args)
        {
            Item item4;
            Item presentationItem = this._presentationContext.PresentationItem;
            if (presentationItem != null)
            {
                item4 = presentationItem.FirstChildInheritingFrom(Templates.CacheSettingsFolder.ID);
            }
            else
            {
                Item local1 = presentationItem;
                item4 = null;
            }
            Item item = item4;
            if (item != null)
            {
                string text1;
                RenderingItem renderingItem = args.Rendering.RenderingItem;
                if (renderingItem != null)
                {
                    text1 = renderingItem.ID.ToString();
                }
                else
                {
                    RenderingItem local2 = renderingItem;
                    text1 = null;
                }
                string local3 = text1;
                string str = local3 ?? args.Rendering.RenderingItemPath;
                foreach (Item item2 in item.Children)
                {
                    if (item2.InheritsFrom(Templates.CacheSettings.ID) && item2[Templates.CacheSettings.Fields.Renderings].Contains(str))
                    {
                        this.PopulateCacheOptions(args.Rendering, item2);
                        args.Rendering["InheritedCaching"] = "1";
                        break;
                    }
                }
            }
        }

    }
}