<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Introduction
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <center>
        
        <a class="pictureLink" href='<%: Url.Action("SelectWardStake","Home") %>'><img src="../../Content/images/page_intro.png" /></a>
         </center>
        <div style="clear: both;">
            &nbsp;</div>
   
</asp:Content>
