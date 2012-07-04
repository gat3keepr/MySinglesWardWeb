<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MSW.Models.StakeListModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Stake List
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>
        Stake List</h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="tabs">
        <ul>
            <!-- <li><a href="#tabs-1">Entire Stake</a></li>-->
            <% int position = 2;
               foreach (var ward in Model.StakeList)
               { %>
            <li><a href="GetWard?wardID=<%: ward.WardStakeID %>" title="GetWard?wardID=<%: ward.WardStakeID %>">
                <span>
                    <%: ward.ward + " Ward" %></span></a></li>
            <% position++;
               } %>
        </ul>
        <!--ENTIRE STAKE-->
        <!--<div id="tabs-1">
           
        </div>-->
        <!--EACH INDIVIUAL WARD-->
        <% position = 0;
           foreach (var ward in Model.StakeList)
           { %>
        <div id="GetWard?wardID=<%: ward.WardStakeID %>">
            <% if (position++ == 0)
               { %>
            <center name="loading">
                <img src="../../Content/images/loading.gif" style="width: 100px" /><br /></center>
            <% } %>
        </div>
        <%  } %>
    </div>
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
<asp:Content ID="Content5" ContentPlaceHolderID="MenuScript" runat="server">
    <script type="text/javascript">
        MenuSelect("Tools", "StakeDirectory");

        // Tabs
        $('#tabs').tabs({
            select: function (event, ui) {
                $('[name=loading]').show();

            },
            show: function (event, ui) {
                $('[name=loading]').show();

            },
            load: function (event, ui) { $('[name=loading]').hide(); },
            ajaxOptions: {
                error: function (xhr, status, index, anchor) {
                    if (xhr.status === 0 || xhr.readyState === 0) {
                        $('[name=loading]').show();
                        return;
                    }

                    $(anchor.hash).html(
						"Couldn't load this ward. Please try refreashing this page.");
                },
                cache: false
            },
            spinner: 'Retrieving Ward List...'
        });
    </script>
</asp:Content>
