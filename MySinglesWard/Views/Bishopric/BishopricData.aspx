<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MSW.Models.dbo.BishopricData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Bishopric Information
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
     <h1>Enter your information for the Ward List</h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

                <p class="error">
                    <%: ViewData["Error"] %></p>
                <% using (Html.BeginForm())
                   {%>
                <%: Html.ValidationSummary(true) %>
                <h2>Personal Information</h2>
                    <div class="editor-label">
                        <%: Html.Label("Enter the name you want to appear on the ward list:") %>
                    </div>
                    <div class="editor-field">
                        <%: Html.TextBoxFor(model => model.BishopricName) %>
                        <%: Html.ValidationMessageFor(model => model.BishopricName) %>
                    </div>

                    <div class="editor-label">
                        <%: Html.Label("Enter the calling you want to appear on the ward list:") %>
                    </div>
                    <div class="editor-field">
                         <%: Html.DropDownListFor(x => x.BishopricCalling, ViewData["DropDown"] as IEnumerable<SelectListItem>, 
                        "Choose a Calling:")%>
                        <%: Html.ValidationMessageFor(model => model.BishopricCalling) %>
                    </div>

                    <div class="editor-label">
                        <%: Html.Label("Enter your phone number:") %>
                    </div>
                    <div class="editor-field">
                        <%: Html.TextBoxFor(model => model.BishopricPhone) %>
                        <%: Html.ValidationMessageFor(model => model.BishopricPhone) %>
                    </div>

                    <div class="editor-label">
                        <%: Html.Label("Enter your address:") %>
                    </div>
                    <div class="editor-field">
                        <%: Html.TextAreaFor(model => model.BishopricAddress, new{ @class="text-area"}) %>
                        <%: Html.ValidationMessageFor(model => model.BishopricAddress) %>
                    </div>

                    <h2>Wife Information</h2>
                     
                    <div class="editor-label">
                        <%: Html.Label("Wife's Name:") %>
                    </div>
                    <div class="editor-field">
                        <%: Html.TextBoxFor(model => model.WifeName) %>
                        <%: Html.ValidationMessageFor(model => model.WifeName) %>
                    </div>

                    <div class="editor-label">
                        <%: Html.Label("Wife's Phone Number:") %>
                    </div>
                    <div class="editor-field">
                        <%: Html.TextBoxFor(model => model.WifePhone) %>
                        <%: Html.ValidationMessageFor(model => model.WifePhone) %>
                    </div>
                    <p>
                        <input class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all" type="submit" value="Save Information" />
                    </p>
                
                <% } %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
