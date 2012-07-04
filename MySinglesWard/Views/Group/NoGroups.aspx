<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	No Groups
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>
       No Groups</h1>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MainContent" runat="server">
    <p>There are no groups in your ward.</p>
     <% if (User.IsInRole("StakePres") || User.IsInRole("Stake") || User.IsInRole("Bishopric")
                || User.IsInRole("Elders Quorum") || User.IsInRole("Relief Society") || User.IsInRole("Activities") || User.IsInRole("FHE"))
           {%>
           <br />
    <p><%: Html.ActionLink("Create A Group", "List", "Group") %></p>
    <% } %>
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MenuScript" runat="server">
    <script type="text/javascript">
        MenuSelect("Ward", "Groups");
    </script>
</asp:Content>