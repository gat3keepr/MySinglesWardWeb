<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Forgot Password?
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
	<h1>
            Password Recovery</h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">    
        <p>
            Please enter your email to reset your password:
        </p>
         <% using (Html.BeginForm())
            { %>
        <%: Html.ValidationSummary(true, "Account creation was unsuccessful. Please correct the errors and try again.")%>

                <div class="editor-label">
                    <%: Html.Label("Email:") %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBox("email") %>
                    <%: Html.ValidationMessage("email") %>
                </div>
                <div class="editor-label">
                 <input class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all" id="Submit" type="submit" value="Reset Password" />
                 </div>
        <% } %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
