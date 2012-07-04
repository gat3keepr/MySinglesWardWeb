<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MSW.Models.dbo.MemberSurvey>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Update Survey
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
	<script src="../../Scripts/jquery.blockUI.js" type="text/javascript"></script>
	<script src="../../Scripts/SurveyScript.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>Update Member Survey</h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

            <a name="top"> </a>
       
                <% Html.RenderPartial("SurveyForm", Model); %>
            
</asp:Content>
