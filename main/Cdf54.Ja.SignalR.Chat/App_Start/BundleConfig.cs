using System.Web.Optimization;

namespace Cdf54.Ja.SignalR.Chat
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jQuery.tmpl.min.js",
                        "~/Scripts/app/cdf54.ja.signalR.chatapp.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're 
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/chat").Include(
            "~/Scripts/jquery.cssemoticons.js",
            "~/Scripts/jquery.signalR-2.2.0.js",
           "~/Scripts/app/cdf54.ja.signalR.demotransports.js",
           "~/Scripts/app/cdf54.ja.signalR.chat.namespace.js",
            "~/Scripts/app/cdf54.ja.utils.js",
           "~/Scripts/app/cdf54.ja.signalR.chat.helpers.js",
           //"~/Scripts/app/cdf54.ja.signalR.chat.min.js"));
           "~/Scripts/app/cdf54.ja.signalR.chat.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/jquery.cssemoticons.css",
                      "~/Content/css/font-awesome.css",
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            // Code removed for clarity. http://www.asp.net/mvc/overview/performance/bundling-and-minification
            BundleTable.EnableOptimizations = false;
        }
    }
}
