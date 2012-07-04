<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MSW.Models.StakeWardModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Stake Member Data
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="tabs">
        <ul>
            <li><a href="#tabs-1">Entire Stake</a></li>
            <% int position = 2;
               foreach (var ward in Model.StakeList)
               { %>
            <li><a href="#tabs-<%: position %>">
                <%: Html.Label(ward.WardName) %></a></li>
            <% position++;
                   } %>
        </ul>
        <!--ENTIRE STAKE-->
        <div id="tabs-1">
            <div class="printTable">
                <h2>
                    Stake Data</h2>
                <table >
                    <tr>
                        <td>
                        </td>
                        <td>
                            CSV
                        </td>
                    </tr>
                    <tr>
                        <% using (Html.BeginForm("StakeCSV", "Print", FormMethod.Get))
                           {%>
                        <%: Html.Hidden("WardStakeID",-1) %>
                        <td>
                            Print off data for the entire stake:
                        </td>
                        <td>
                            <input type="image" src="../../Content/images/excelLogo.png" alt="Submit button" />
                        </td>
                        <% } %>
                    </tr>
                </table>
            </div>
        </div>
        <!--EACH INDIVIUAL WARD-->
        <% int tabPosition = 2;
           foreach (var ward in Model.StakeList)
           { %>
        <div id="tabs-<%: tabPosition %>">
            <div class="printTable">
            <h2>Ward Print-Out</h2>
                <table>
                    <tr>
                        <td>
                        </td>
                        <td>
                            CSV
                        </td>
                    </tr>
                    <tr>
                        <% using (Html.BeginForm("StakeCSV", "Print", FormMethod.Get))
                           {%>
                        <%: Html.Hidden("WardStakeID",ward.WardStakeID) %>
                        <td>
                            Print off ward data for the
                            <%: Html.Label(ward.WardName) %>:
                        </td>
                        <td>
                            <input type="image" src="../../Content/images/excelLogo.png" alt="Submit button" />
                        </td>
                        <% } %>
                    </tr>
                </table>
            </div>
        </div>
        <% tabPosition++;
               } %>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            // Tabs
            var $tabs = $('#tabs').tabs(); // first tab selected
        });        
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MenuScript" runat="server">
    <script type="text/javascript">
        MenuSelect("Tools", "GetStakeData");
    </script>
</asp:Content>