using Divergence.DataAccess.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Divergence.Models
{
    public static class UriExtensions
    {
        public static Uri PathwayUri(this UrlHelper url, Pathway pathway)
        {
            return new Uri(String.Format("{0}{1}", GetAuthority(url), url.Action("Index", "Pathway", new { arcKey = pathway.Arc.Key, key = pathway.Key })));
        }

        public static Uri ArcUri(this UrlHelper url, Arc arc)
        {
            return new Uri(String.Format("{0}{1}", GetAuthority(url), url.Action("Index", "Arc", new { key = arc.Key })));
        }

        public static Uri StoryUri(this UrlHelper url, Story story)
        {
            return new Uri(String.Format("{0}{1}", GetAuthority(url), url.Action("Index", "Story", new { key = story.Key })));
        }

        private static string GetAuthority(UrlHelper url)
        {
            return url.RequestContext.HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
        }
    }
}