﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.235
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Avanade.BootStrapper.Web.Container.Ext.Views.Container
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
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    
    #line 1 "..\..\Views\Container\Index.cshtml"
    using Avanade.BootStrapper.Web.Container.Ext.Models;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "1.2.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Container/Index.cshtml")]
    public class Index : System.Web.Mvc.WebViewPage<ComponentViewModel>
    {
        public Index()
        {
        }
        public override void Execute()
        {


WriteLiteral("\r\n");


            
            #line 4 "..\..\Views\Container\Index.cshtml"
  
    ViewBag.Title = "Container";


            
            #line default
            #line hidden
WriteLiteral("\r\n");


            
            #line 8 "..\..\Views\Container\Index.cshtml"
  
   var grid = new WebGrid(Model.Components, rowsPerPage: 10, 
   canPage: true, canSort: true, defaultSort: "ServiceName");
 

            
            #line default
            #line hidden
WriteLiteral("\r\n<h2>Container Registry</h2>\r\n\r\n<p>Total number of component services: ");


            
            #line 15 "..\..\Views\Container\Index.cshtml"
                                  Write(Model.Components.Count());

            
            #line default
            #line hidden
WriteLiteral("</p>\r\n\r\n<div id=\"grid\">\r\n    ");


            
            #line 18 "..\..\Views\Container\Index.cshtml"
Write(grid.GetHtml(
    tableStyle: "grid",
    headerStyle: "head",
    alternatingRowStyle: "alt",
    columns: grid.Columns(
    grid.Column("ServiceName", "Service Name"),
    grid.Column("Implementation", "Implementation")
   )));

            
            #line default
            #line hidden
WriteLiteral("\r\n</div>\r\n\r\n\r\n");


        }
    }
}
#pragma warning restore 1591
