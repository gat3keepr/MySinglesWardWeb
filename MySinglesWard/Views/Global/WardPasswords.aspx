<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<MSW.Model.tSupportedWard>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Ward Passwords
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


    <table>
        <tr>
            <th>
                Location
            </th>
            <th>
                Stake
            </th>
            <th>
                Ward
            </th>
            
            <th>
                Password
            </th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <%: item.Location %>
            </td>
            <td>
                <%: item.Stake %>
            </td>
            <td>
                <%: item.Ward %>
            </td>
            
            <td>
                <%: MSW.Utilities.Cryptography.DecryptString(item.Password) %>
            </td>
        </tr>
    
    <% } %>

    </table>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">

    <h2>Ward Passwords</h2>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="MenuScript" runat="server">
</asp:Content>

