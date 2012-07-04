<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MSW.Models.Group>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Group Details -
    <%: Model.Name %>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h2>
        <%: Html.Label(Model.Name) %></h2>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="editor-label">
        <b>Group Type:</b>
    </div>
    <div class="editor-field">
        <%: Html.Label(Model.Type) %>
    </div>
    <div class="editor-label">
        <b>Group Leader:</b>
    </div>
    <div class="editor-field">
        <%: Html.Label(Model.Leader) %>
    </div>
    <div class="editor-label">
        <b>Group Co-Leader:</b>
    </div>
    <div class="editor-field">
        <%: Html.Label(Model.CoLeader) %>
    </div>
    <br />
    <br />
    <div class="editor-field">
        <h2>
            Group Members</h2>
        <table style="width: 325px">
            <tr>
                <td>
                    <b>Name</b>
                </td>
            </tr>
            <% foreach (var member in Model.MemberList)
               { %>
            <tr id="<%: member.user.MemberID %>X">
                <td>
                    <%: Html.Label(member.user.LastName + ", " + member.memberSurvey.prefName) %>
                </td>
            </tr>
            <% } %>
        </table>
        <% 
            bool editable = false;
            foreach (string role in Model.access)
            {
                if (User.IsInRole(role))
                    editable = true;
            }
            if (editable)
            {  %>
        <br /> 
        <br />
            <%: Html.ActionLink("Edit Group", "Edit", new { id = Model.GroupID })%>
        <% } %>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MenuScript" runat="server">
    <script type="text/javascript">
        MenuSelect("Tools", "Groups");
    </script>
</asp:Content>