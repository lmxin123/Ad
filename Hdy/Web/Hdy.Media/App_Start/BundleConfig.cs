using System.Web;
using System.Web.Optimization;

namespace Hdy.Media
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/common").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/respond.js",
                        "~/Scripts/theme.js",
                        "~/Scripts/plugins/common.js"
                        ));
            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            //bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //          "~/Scripts/bootstrap.js",
            //          "~/Scripts/respond.js"));

            //bundles.Add(new StyleBundle("~/bundles/bootstrap").Include(
            //          "~/Content/bootstrap.min.css",
            //          "~/Content/bootstrap-overrides.css.css"
            //          ));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                   "~/Scripts/angular/angular.min.js",
                   "~/Scripts/angular-ui/ui-bootstrap-tpls.js"
                   ));

            bundles.Add(new StyleBundle("~/Content/jQuery-File-Upload").Include(
                   "~/Content/jQuery.FileUpload/css/jquery.fileupload.css",
                   "~/Content/jQuery.FileUpload/css/jquery.fileupload-ui.css",
                   "~/Content/blueimp-gallery2/css/blueimp-gallery.css",
                   "~/Content/blueimp-gallery2/css/blueimp-gallery-video.css",
                   "~/Content/blueimp-gallery2/css/blueimp-gallery-indicator.css"));

            bundles.Add(new ScriptBundle("~/bundles/jQuery-File-Upload").Include(
                    "~/Scripts/jQuery.FileUpload/vendor/jquery.ui.widget.js",
                    "~/Scripts/jQuery.FileUpload/tmpl.min.js",
                    "~/Scripts/jQuery.FileUpload/load-image.all.min.js",
                    "~/Scripts/jQuery.FileUpload/canvas-to-blob.min.js",
                    "~/Scripts/jQuery.FileUpload/jquery.iframe-transport.js",
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload.js",
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-process.js",
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-image.js",
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-audio.js",
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-video.js",
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-validate.js",
                    "~/Scripts/jQuery.FileUpload/jquery.fileupload-ui.js",
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery.js",
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery-video.js",
                    "~/Scripts/blueimp-gallery2/js/blueimp-gallery-indicator.js",
                    "~/Scripts/blueimp-gallery2/js/jquery.blueimp-gallery.js"));
        }
    }
}
