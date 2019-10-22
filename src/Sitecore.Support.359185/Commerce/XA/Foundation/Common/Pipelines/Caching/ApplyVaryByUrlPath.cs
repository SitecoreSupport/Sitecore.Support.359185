namespace Sitecore.Support.Commerce.XA.Foundation.Common.Pipelines.Caching
{
    using Sitecore.Commerce.XA.Foundation.Common.Context;
    using Sitecore.Commerce.XA.Foundation.Common.Utils;
    using Sitecore.Data.Items;
    using Sitecore.DependencyInjection;
    using Sitecore.Diagnostics;
    using Sitecore.Mvc.Pipelines.Response.RenderRendering;
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using System.Globalization;
    using System.Web;

    public class ApplyVaryByUrlPath : RenderRenderingProcessor
    {

        public ApplyVaryByUrlPath()
        {
            this.Context = ServiceLocatorHelper.GetService<IContext>();
        }

        protected IContext Context { get; }

        public override void Process(RenderRenderingArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            if (args.Rendered || HttpContext.Current == null || (!args.Cacheable || string.IsNullOrWhiteSpace(args.CacheKey)) || args.Rendering.RenderingItem == null)
                return;
            Item obj1 = args.PageContext.Database.GetItem(args.Rendering.RenderingItem.ID);            

            // added additional check for VaryByUrlPath setting form CacheSetting item to ovverid rendering item definition settings
            if ((args.Rendering["Cache_VaryByUrlPath"] == "1") || (args.Rendering["Cache_VaryByUrlPath"] == null && obj1["VaryByUrlPath"] == "1"))
            {
                object obj2 = ServiceLocator.ServiceProvider.GetService<ISiteContext>().Items[(object)"ContainerDatasourceId"];
                string str = string.Format((IFormatProvider)CultureInfo.InvariantCulture, "_#varyByUrlPath_{0}_{1}_{2}_{3}", (object)this.Context.Site.Name, (object)Sitecore.Context.Language.Name, (object)HttpContext.Current.Request.Url.AbsoluteUri, (object)args.Rendering.RenderingItem.ID.ToString());
                if (obj2 != null && !string.IsNullOrEmpty(obj2.ToString()))
                {
                    str += (string)obj2;
                }
                args.CacheKey += str;
            }
        }
    }
}