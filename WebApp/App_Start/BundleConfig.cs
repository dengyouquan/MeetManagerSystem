using System.Web;
using System.Web.Optimization;

namespace WebApp
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/easyui").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.easyui.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.easyui.min.js"));

            

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/ajax").Include(
                "~/Scripts/jquery.unobtrusive-ajax.js",
                     "~/Scripts/MyAjaxForm.js"));

            bundles.Add(new ScriptBundle("~/bundles/date").Include(
                        "~/Scripts/calendar.js"));


            bundles.Add(new ScriptBundle("~/bundles/date1").Include(
                        "~/Scripts/calendar1.js"));

            bundles.Add(new StyleBundle("~/Content/date").Include(
                      "~/Content/css/cxcalendar.css"));

            bundles.Add(new StyleBundle("~/Content/KinderEditor").Include(
                      "~/KinderEditor/themes/default/default.css",
                      "~/KinderEditor/plugins/code/prettify.css"));

            bundles.Add(new ScriptBundle("~/bundles/KinderEditor").Include(
                "~/KinderEditor/kindeditor.js",
                     "~/KinderEditor/lang/zh_CN.js",
                     "~/KinderEditor/plugins/code/prettify.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

             bundles.Add(new StyleBundle("~/Content/table").Include(
                      "~/Content/pageBar.css",
                        "~/Content/tableStyle.css"));
            
                        

            bundles.Add(new StyleBundle("~/Content/easyui").Include(
                     "~/Content/demo.css",
                      "~/Content/themes/icon.css",
                       "~/Content/themes/default/easyui.css"));

        }
    }
}
