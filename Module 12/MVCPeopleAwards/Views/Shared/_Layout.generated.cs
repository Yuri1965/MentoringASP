﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using MVCPeopleAwards;
    
    #line 1 "..\..\Views\Shared\_Layout.cshtml"
    using MVCPeopleAwards.Models;
    
    #line default
    #line hidden
    using MvcSiteMapProvider.Web.Html;
    using MvcSiteMapProvider.Web.Html.Models;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/_Layout.cshtml")]
    public partial class _Views_Shared__Layout_cshtml : System.Web.Mvc.WebViewPage<dynamic>
    {
        public _Views_Shared__Layout_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <meta");

WriteLiteral(" charset=\"utf-8\"");

WriteLiteral(" />\r\n    <meta");

WriteLiteral(" name=\"viewport\"");

WriteLiteral(" content=\"width=device-width, initial-scale=1.0\"");

WriteLiteral(">\r\n    <title>");

            
            #line 7 "..\..\Views\Shared\_Layout.cshtml"
      Write(ViewBag.Title);

            
            #line default
            #line hidden
WriteLiteral("</title>\r\n");

WriteLiteral("    ");

            
            #line 8 "..\..\Views\Shared\_Layout.cshtml"
Write(Styles.Render("~/Content/css"));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("    ");

            
            #line 10 "..\..\Views\Shared\_Layout.cshtml"
Write(Scripts.Render("~/bundles/jquery"));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 11 "..\..\Views\Shared\_Layout.cshtml"
Write(Scripts.Render("~/bundles/jqueryunobtrusive"));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 12 "..\..\Views\Shared\_Layout.cshtml"
Write(Scripts.Render("~/bundles/jqueryui"));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 13 "..\..\Views\Shared\_Layout.cshtml"
Write(Scripts.Render("~/bundles/bootstrap"));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 14 "..\..\Views\Shared\_Layout.cshtml"
Write(Scripts.Render("~/bundles/modernizr"));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

            
            #line 16 "..\..\Views\Shared\_Layout.cshtml"
    
            
            #line default
            #line hidden
            
            #line 16 "..\..\Views\Shared\_Layout.cshtml"
     if (HttpContext.Current.User.IsInRole("CandidateAdmin") && Session["EventsUserCandidate"] != null)
    {


            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(" language=\"javascript\"");

WriteLiteral(@">

            var validNavigation = false;

            function wireUpEvents() {
                window.onbeforeunload = function () {
                    if (!validNavigation) {
                        return confirm(""У вас имеются НЕ сохраненные операции, вы действительно хотите закрыть вкладку? "");
                    }
                }

                validNavigation = false;
                // Attach the event keypress to exclude the F5 refresh
                $(document).bind('keypress', function (e) {
                    if (e.keyCode == 116) {
                        validNavigation = true;
                    }
                });

                // Attach the event click for all links in the page
                $(""a"").bind(""click"", function () {
                    validNavigation = true;
                });

                // Attach the event submit for all forms in the page
                $(""form"").bind(""submit"", function () {
                    validNavigation = true;
                });

                // Attach the event click for all inputs in the page
                $(""input[type=submit]"").bind(""click"", function () {
                    validNavigation = true;
                });

            }

            // Wire up the events as soon as the DOM tree is ready
            $(document).ready(function () {
                wireUpEvents();
            });
        </script>
");

            
            #line 60 "..\..\Views\Shared\_Layout.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n</head>\r\n<body>\r\n    <div");

WriteLiteral(" class=\"navbar navbar-inverse navbar-fixed-top\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"container\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"navbar-header\"");

WriteLiteral(">\r\n                <button");

WriteLiteral(" type=\"button\"");

WriteLiteral(" class=\"navbar-toggle\"");

WriteLiteral(" data-toggle=\"collapse\"");

WriteLiteral(" data-target=\".navbar-collapse\"");

WriteLiteral(">\r\n                    <span");

WriteLiteral(" class=\"icon-bar\"");

WriteLiteral("></span>\r\n                    <span");

WriteLiteral(" class=\"icon-bar\"");

WriteLiteral("></span>\r\n                </button>\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"navbar-collapse collapse\"");

WriteLiteral(">\r\n                <ul");

WriteLiteral(" class=\"nav navbar-nav\"");

WriteLiteral(">\r\n                    <li>");

            
            #line 75 "..\..\Views\Shared\_Layout.cshtml"
                   Write(Html.ActionLink("Список награжденных", "Index", "PeoplesAward"));

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n\r\n");

            
            #line 77 "..\..\Views\Shared\_Layout.cshtml"
                    
            
            #line default
            #line hidden
            
            #line 77 "..\..\Views\Shared\_Layout.cshtml"
                     if (Request.IsAuthenticated && (HttpContext.Current.User.IsInRole("Admin") || HttpContext.Current.User.IsInRole("CandidateAdmin")))
                    {

            
            #line default
            #line hidden
WriteLiteral("                        <li>");

            
            #line 79 "..\..\Views\Shared\_Layout.cshtml"
                       Write(Html.ActionLink("Справочник Награды", "Index", "Awards"));

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n");

WriteLiteral("                        <li>");

            
            #line 80 "..\..\Views\Shared\_Layout.cshtml"
                       Write(Html.ActionLink("Администрирование пользователей", "Index", "Admin"));

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n");

            
            #line 81 "..\..\Views\Shared\_Layout.cshtml"
                    }

            
            #line default
            #line hidden
WriteLiteral("                    ");

            
            #line 82 "..\..\Views\Shared\_Layout.cshtml"
                     if (HttpContext.Current.User.IsInRole("CandidateAdmin") && Session["EventsUserCandidate"] != null)
                    {
                        var events = Session["EventsUserCandidate"] as EventsUserViewModel;
                        if (events != null && events.ListEvents.Count > 0)
                        {

            
            #line default
            #line hidden
WriteLiteral("                            <li>");

            
            #line 87 "..\..\Views\Shared\_Layout.cshtml"
                           Write(Html.ActionLink("Список изменений(" + events.ListEvents.Count.ToString() + ")", "Index", "EventUser"));

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n");

            
            #line 88 "..\..\Views\Shared\_Layout.cshtml"
                        }
                        else
                        {

            
            #line default
            #line hidden
WriteLiteral("                            <li>");

            
            #line 91 "..\..\Views\Shared\_Layout.cshtml"
                           Write(Html.ActionLink("Список изменений", "Index", "EventUser"));

            
            #line default
            #line hidden
WriteLiteral("</li>\r\n");

            
            #line 92 "..\..\Views\Shared\_Layout.cshtml"
                        }
                    }

            
            #line default
            #line hidden
WriteLiteral("                </ul>\r\n");

WriteLiteral("                ");

            
            #line 95 "..\..\Views\Shared\_Layout.cshtml"
           Write(Html.Partial("_LoginPartial"));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n        </div>\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"container body-content\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 100 "..\..\Views\Shared\_Layout.cshtml"
   Write(Html.MvcSiteMap().SiteMapPath());

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 102 "..\..\Views\Shared\_Layout.cshtml"
   Write(RenderBody());

            
            #line default
            #line hidden
WriteLiteral("\r\n        <hr />\r\n        <footer>\r\n            <p>&copy; ");

            
            #line 105 "..\..\Views\Shared\_Layout.cshtml"
                 Write(DateTime.Now.Year);

            
            #line default
            #line hidden
WriteLiteral(" - EPAM Systems, ASP.NET MVC 5 Mentoring Program, г. Караганда</p>\r\n        </foo" +
"ter>\r\n    </div>\r\n\r\n");

WriteLiteral("    ");

            
            #line 109 "..\..\Views\Shared\_Layout.cshtml"
Write(RenderSection("scripts", required: false));

            
            #line default
            #line hidden
WriteLiteral("\r\n</body>\r\n</html>\r\n");

        }
    }
}
#pragma warning restore 1591
