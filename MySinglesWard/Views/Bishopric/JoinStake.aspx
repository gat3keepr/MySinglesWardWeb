<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../../Scripts/SelectStakeScript.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            // Dialog			
            $('#dialog').dialog({
                autoOpen: false,
                position: top,
                width: 600,
                buttons: {
                    "Cancel": function () {
                        $(this).dialog("close");
                    }
                }
            });

            // Dialog Link
            $('#dialog_link').click(function () {
                $('#dialog').dialog('open');
                return false;
            });
            //hover states on the static widgets
            $('#dialog_link, ul#icons li').hover(
					function () { $(this).addClass('ui-state-hover'); },
					function () { $(this).removeClass('ui-state-hover'); }
				);
        });
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Join a Stake
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>
        Join a Stake</h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table>
        <tr valign="top">
            <td>
                <p class="error">
                    <%: ViewData["Error"] %></p>
                <p style="padding: 5px">
                    Please select a stake to join or create a new stake.</p>
                <br />
                <% var DropDowns = new MSW.Models.DropDowns(); %>
                <% DropDowns.generateSupportedStakeList(); %>
                <% using (Html.BeginForm())
                   {%>
                <table class="SelectWardStake">
                    <tr>
                        <td>
                            Location
                        </td>
                        <td>
                            Stake
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <%: Html.DropDownList("ChosenArea", DropDowns.getSupportedStakesList(), "Select Supported Location")%>
                        </td>
                        <td>
                            <%: Html.DropDownList("ChosenStake", "Select Supported Stake")%>
                        </td>
                        <td>
                            <input class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all"
                                id="submit" type="submit" value="Choose Stake" disabled="disabled" />
                        </td>
                    </tr>
                </table>
                <% } %>
            </td>
            <td>
                <!--  start Information & Features -->
                <div id="related-activities">
                    <!--  start related-act-top -->
                    <div id="related-act-top">
                        <img src="../../Content/admin/images/forms/header_related_act.gif" width="271" height="43"
                            alt="" />
                    </div>
                    <!-- end related-act-top -->
                    <!--  start related-act-bottom -->
                    <div id="related-act-bottom">
                        <!--  start related-act-inner -->
                        <div id="related-act-inner">
                            <div class="left">
                                <img src="../../Content/admin/images/forms/icon_edit.gif" width="21" height="21"
                                    alt="" /></div>
                            <div class="right">
                                Joining a stake will give the stake presidency access to the information avaiable on your members. Stake
                                Presidencies get full access and other stake officials can view contact information on your members.
                                <br />
                                <br />
                                Stakes are created and secured with the same provisions a ward is. They require a password from MySinglesWard.com and 
                                each Stake President has a password that must be given to stake officials individually.
                            </div>
                            <div class="clear">
                            </div>
                        </div>
                        <!-- end related-act-inner -->
                        <div class="clear">
                        </div>
                    </div>
                    <!-- end related-act-bottom -->
                </div>
                <!-- end Information & Features -->
            </td>
        </tr>
        <tr>
            <td>
                <img src="../../Content/admin/images/shared/blank.gif" width="695" height="1" alt="blank" />
            </td>
            <td>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MenuScript" runat="server">
    <script type="text/javascript">
        MenuSelect("Ward", "JoinStake");
    </script>
</asp:Content>