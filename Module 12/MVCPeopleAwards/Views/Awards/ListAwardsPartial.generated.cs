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
    using MvcSiteMapProvider.Web.Html;
    using MvcSiteMapProvider.Web.Html.Models;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Awards/ListAwardsPartial.cshtml")]
    public partial class _Views_Awards_ListAwardsPartial_cshtml : System.Web.Mvc.WebViewPage<MVCPeopleAwards.Models.ListAwardsViewModel>
    {
        public _Views_Awards_ListAwardsPartial_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("<table");

WriteLiteral(" class=\"table\"");

WriteLiteral(">\n    <tr>\n        <th>\n");

WriteLiteral("            ");

            
            #line 6 "..\..\Views\Awards\ListAwardsPartial.cshtml"
       Write(Html.DisplayName("Фото награды"));

            
            #line default
            #line hidden
WriteLiteral("\n        </th>\n        <th>\n");

WriteLiteral("            ");

            
            #line 9 "..\..\Views\Awards\ListAwardsPartial.cshtml"
       Write(Html.DisplayName("Наименование награды"));

            
            #line default
            #line hidden
WriteLiteral("\n        </th>\n        <th>\n");

WriteLiteral("            ");

            
            #line 12 "..\..\Views\Awards\ListAwardsPartial.cshtml"
       Write(Html.DisplayName("Описание награды"));

            
            #line default
            #line hidden
WriteLiteral("\n        </th>\n        <th></th>\n    </tr>\n    \n    <tbody");

WriteLiteral(" id=\"awardsListBody\"");

WriteLiteral(">\n");

            
            #line 18 "..\..\Views\Awards\ListAwardsPartial.cshtml"
        
            
            #line default
            #line hidden
            
            #line 18 "..\..\Views\Awards\ListAwardsPartial.cshtml"
         foreach (var item in Model.ListAwards)
        {
            Html.RenderPartial("AwardSinglePartial", item);
        }

            
            #line default
            #line hidden
WriteLiteral("    </tbody>\n\n</table>");

        }
    }
}
#pragma warning restore 1591