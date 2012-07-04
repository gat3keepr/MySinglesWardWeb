<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MSW.Models.dbo.StakeData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Stake User Information
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>
        Enter your information for the Stake List</h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table>
        <tr>
            <td>
                <p class="error">
                    <%: ViewData["Error"] %></p>
                <% using (Html.BeginForm())
                   {%>
                <%: Html.ValidationSummary(true) %>
                <div class="editor-label">
                    <%: Html.Label("Enter the name you want to appear on the stake list:") %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(model => model.StakeName) %>
                    <%: Html.ValidationMessageFor(model => model.StakeName) %>
                </div>
                <div class="editor-label">
                    <%: Html.Label("Enter the calling you want to appear on the stake list:") %>
                </div>
                <div class="editor-field">
                    <%: Html.DropDownListFor(x => x.StakeCalling, ViewData["DropDown"] as IEnumerable<SelectListItem>, 
                        "Choose a Calling:")%>
                    <%: Html.ValidationMessageFor(model => model.StakeCalling) %>
                </div>
                <div class="editor-label">
                    <%: Html.Label("Enter your phone number:") %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(model => model.StakePhone) %>
                    <%: Html.ValidationMessageFor(model => model.StakePhone) %>
                </div>
                <p>
                    <input class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all"
                        type="submit" value="Save Information" />
                </p>
                <% } %>
                <br />
                <br />
                <br />
                <br />
                <br />
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
