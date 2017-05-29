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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Awards/AwardSinglePartial.cshtml")]
    public partial class _Views_Awards_AwardSinglePartial_cshtml : System.Web.Mvc.WebViewPage<MVCPeopleAwards.Models.AwardViewModel>
    {
        public _Views_Awards_AwardSinglePartial_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("<tr");

WriteLiteral(" data-award-id=\"");

            
            #line 3 "..\..\Views\Awards\AwardSinglePartial.cshtml"
              Write(Model.Id);

            
            #line default
            #line hidden
WriteLiteral("\"");

WriteLiteral(">\n    <td>\n");

            
            #line 5 "..\..\Views\Awards\AwardSinglePartial.cshtml"
        
            
            #line default
            #line hidden
            
            #line 5 "..\..\Views\Awards\AwardSinglePartial.cshtml"
         if (@Model.PhotoAward == null)
        {
            
            
            #line default
            #line hidden
            
            #line 7 "..\..\Views\Awards\AwardSinglePartial.cshtml"
       Write(Html.ActionLink("Нет фото", "GetAwardById", "Awards", new { id = @Model.Id }, new { @class = "awardItem" }));

            
            #line default
            #line hidden
            
            #line 7 "..\..\Views\Awards\AwardSinglePartial.cshtml"
                                                                                                                        
        }
        else
        {

            
            #line default
            #line hidden
WriteLiteral("            <a");

WriteAttribute("href", Tuple.Create(" href=\"", 304), Tuple.Create("\"", 372)
            
            #line 11 "..\..\Views\Awards\AwardSinglePartial.cshtml"
, Tuple.Create(Tuple.Create("", 311), Tuple.Create<System.Object, System.Int32>(Url.Action("GetAwardById", "Awards", new { id = @Model.Id })
            
            #line default
            #line hidden
, 311), false)
);

WriteLiteral(" class=\"awardItem\"");

WriteLiteral(" >\n                <img");

WriteLiteral(" width=\"75\"");

WriteLiteral(" height=\"75\"");

WriteAttribute("src", Tuple.Create(" src=\"", 437), Tuple.Create("\"", 500)
            
            #line 12 "..\..\Views\Awards\AwardSinglePartial.cshtml"
, Tuple.Create(Tuple.Create("", 443), Tuple.Create<System.Object, System.Int32>(Url.Action("GetPhotoAward", "Awards", new { @Model.Id })
            
            #line default
            #line hidden
, 443), false)
);

WriteAttribute("title", Tuple.Create(" title=", 501), Tuple.Create("", 524)
            
            #line 12 "..\..\Views\Awards\AwardSinglePartial.cshtml"
                                   , Tuple.Create(Tuple.Create("", 508), Tuple.Create<System.Object, System.Int32>(Model.NameAward
            
            #line default
            #line hidden
, 508), false)
);

WriteLiteral(" />\n            </a>\n");

            
            #line 14 "..\..\Views\Awards\AwardSinglePartial.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </td>\n\n    <td>\n");

WriteLiteral("        ");

            
            #line 18 "..\..\Views\Awards\AwardSinglePartial.cshtml"
   Write(Html.DisplayFor(modelItem => @Model.NameAward));

            
            #line default
            #line hidden
WriteLiteral("\n    </td>\n    <td>\n");

WriteLiteral("        ");

            
            #line 21 "..\..\Views\Awards\AwardSinglePartial.cshtml"
   Write(Html.DisplayFor(modelItem => @Model.DescriptionAward));

            
            #line default
            #line hidden
WriteLiteral("\n    </td>\n\n");

            
            #line 24 "..\..\Views\Awards\AwardSinglePartial.cshtml"
    
            
            #line default
            #line hidden
            
            #line 24 "..\..\Views\Awards\AwardSinglePartial.cshtml"
     if (Request.IsAuthenticated && (HttpContext.Current.User.IsInRole("Admin") || HttpContext.Current.User.IsInRole("CandidateAdmin")))
    {

            
            #line default
            #line hidden
WriteLiteral("        <td>\n");

WriteLiteral("            ");

            
            #line 27 "..\..\Views\Awards\AwardSinglePartial.cshtml"
       Write(Html.ActionLink("Изменить", "EditAward", new { id = @Model.Id }, new { @class = "btn btn-default" }));

            
            #line default
            #line hidden
WriteLiteral("        \n\n");

            
            #line 29 "..\..\Views\Awards\AwardSinglePartial.cshtml"
            
            
            #line default
            #line hidden
            
            #line 29 "..\..\Views\Awards\AwardSinglePartial.cshtml"
             using (Ajax.BeginForm("DeleteAward", "Awards", new { id = @Model.Id }, 
                new AjaxOptions() { Confirm = "Удалить награду " + @Model.NameAward + "?", HttpMethod = "POST", OnSuccess = "OnAwardRemove", OnFailure = "OnError" },
                new { @style = "display: inline-block" }))
            {
                
            
            #line default
            #line hidden
            
            #line 33 "..\..\Views\Awards\AwardSinglePartial.cshtml"
           Write(Html.AntiForgeryToken());

            
            #line default
            #line hidden
            
            #line 33 "..\..\Views\Awards\AwardSinglePartial.cshtml"
                                        

            
            #line default
            #line hidden
WriteLiteral("                <input");

WriteLiteral(" data-val=\"true\"");

WriteLiteral(" data-val-number=\"Это поле должно быть целым числом.\"");

WriteLiteral(" data-val-required=\"Требуется поле Id.\"");

WriteLiteral(" id=\"Id\"");

WriteLiteral(" name=\"Id\"");

WriteLiteral(" type=\"hidden\"");

WriteAttribute("value", Tuple.Create(" value=\"", 1530), Tuple.Create("\"", 1547)
            
            #line 34 "..\..\Views\Awards\AwardSinglePartial.cshtml"
                                                                                          , Tuple.Create(Tuple.Create("", 1538), Tuple.Create<System.Object, System.Int32>(Model.Id
            
            #line default
            #line hidden
, 1538), false)
);

WriteLiteral(" />\n");

WriteLiteral("                <input");

WriteLiteral(" type=\"submit\"");

WriteLiteral(" value=\"Удалить\"");

WriteLiteral(" class=\"btn btn-danger\"");

WriteLiteral(" />\n");

            
            #line 36 "..\..\Views\Awards\AwardSinglePartial.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("        </td>\n");

            
            #line 38 "..\..\Views\Awards\AwardSinglePartial.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("</tr>\n\n\n");

        }
    }
}
#pragma warning restore 1591
