<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MSW.Models.StakeModel>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        $(function () {
            // Dialog			
            $('#dialog').dialog({
                autoOpen: false,
                position: top,
                width: 600,
                buttons: {
                    "Approve": function () {
                        $(this).dialog("close");
                    },
                    "Deny": function () {
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
    Stake Home
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>
        <%: Html.Label(Model.stake) %></h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table style="margin-left: 20px">
        <tr valign="top">
            <td>
                <table class="ProfileText">
                    <tr>
                        <td>
                            <img class="WardPic" src="../../Content/images/stake.jpg" alt="Profile Picture" />
                        </td>
                        <td>
                            <table style="width: 300px">
                                <tr>
                                    <td>
                                        Number of Wards:
                                    </td>
                                    <td>
                                        <%: Html.Label(Model.wards.ToString())%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Number of Members:
                                    </td>
                                    <td>
                                        <%: Html.Label(Model.totalMembers.ToString()) %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <%: Html.ActionLink("Number of Stake Users:", "StakeUsers", "Stake") %>
                                    </td>
                                    <td>
                                        <%: Html.ActionLink(Model.StakeUsers.ToString(), "StakeUsers", "Stake")%>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <div style="clear: both;">
                    &nbsp;</div>
                <% if (User.IsInRole("StakePres"))
                   { %>
                <h2>
                    Approval List</h2>
                <% if (Model.NeedApprovals.Count == 0)
                   { %>
                <p>
                    There are no wards to approve.</p>
                <% }
                   else
                   { %>
                <p>
                    These wards just were added to the stake and are waiting approval:</p>
                <table class="StakeList">
                    <%foreach (var item in Model.NeedApprovals)
                      { %>
                    <tr id="<%: item.WardStakeID %>">
                        <td>
                            <img class="smallPic" src="../../Content/images/church.jpg" alt="Profile Picture" />
                        </td>
                        <td class="largerText">
                            <%: Html.Label(item.Location + " " + item.Stake + " " + item.ward)%>
                        </td>
                        <td>
                            <button id="btnApprove" wardid="<%:item.WardStakeID %>" name="button" value="approve"
                                class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all">
                                <span class="ui-button-text">Approve</span></button>
                            <button id="btnDeny" wardid="<%:item.WardStakeID %>" name="button" value="deny" class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all">
                                <span class="ui-button-text">Deny</span>
                            </button>
                        </td>
                    </tr>
                    <% } %>
                </table>
                <% } %>
                <div style="clear: both;">
                    &nbsp;</div>
                <% } %>
                <div style="clear: both;">
                    &nbsp;</div>
                <!-- end #content -->
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
                                <img src="../../Content/images/<%: ViewData["InfoDone"] %>" width="21" height="21"
                                    alt="" /></div>
                            <div class="right">
                                <h5>
                                        <%: Html.ActionLink("Update Personal Information","StakeUserData","Stake") %></h5>
                            </div>
                            <div class="clear">
                            </div>
                            <div class="lines-dotted-short">
                            </div>
                            <div class="left">
                                <img src="../../Content/images/<%: ViewData["PictureDone"] %>" width="21" height="21"
                                    alt="" /></div>
                            <div class="right">
                                <h5><%: Html.ActionLink("Upload Picture","UploadStakePicture","Photo") %></h5>
                            </div>
                            <div class="clear">
                            </div>
                            <div class="lines-dotted-short">
                            </div>
                            <div class="left">
                                <span name="newStake_link" style="cursor: pointer">
                                    <img src="../../Content/admin/images/forms/icon_edit.gif" width="21" height="21"
                                        alt="" /></span></div>
                            <div class="right">
                                <span name="newStake_link" style="cursor: pointer">
                                    <h5>
                                        Personal Options</h5>
                                        <ul class="greyarrow">
                                        <li><%: Html.ActionLink("Update Personal Information", "StakeUserData", "Stake")%></li>
                                        <li> <%: Html.ActionLink("Upload Picture", "UploadStakePicture", "Photo" ) %></li>
                                        <li><%: Html.ActionLink("Change Stake", "SelectStake", "Stake")%></li>
                                        </ul>
                                </span>
                            </div>
                            <div class="clear">
                            </div>
                            <div class="lines-dotted-short">
                            </div>
                            <div class="left">
                                <span name="newStake_link" style="cursor: pointer">
                                    <img src="../../Content/admin/images/forms/icon_edit.gif" width="21" height="21"
                                        alt="" /></span></div>
                            <div class="right">
                                <span name="newStake_link" style="cursor: pointer">
                                    <h5>
                                        Stake Options</h5>
                                        <ul class="greyarrow">
                                        <li><%: Html.ActionLink("Stake Directory", "StakeList", "Stake")%></li>
                                        <li><%: Html.ActionLink("Print/Export Info", "GetStakeData", "Print")%></li>
                                        <li><%: Html.ActionLink("Groups & Notifications", "Index", "Group")%></li>
                                        </ul>
                                </span>
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
    
    <div style="clear: both;">
        &nbsp;</div>
    <script type="text/javascript">
        $("[id=btnApprove]").click(function () {

            var WardStakeID = $(this).attr("wardID");
            SendApprovalRequest('approve', WardStakeID);
        });
        $("[id=btnDeny]").click(function () {

            var WardStakeID = $(this).attr("wardID");
            SendApprovalRequest('deny', WardStakeID);
        });

        function SendApprovalRequest(button, WardStakeID) {
            $.ajax({
                url: '/Stake/ApproveNewWard',
                data: { button: button, WardStakeID: WardStakeID },
                success: function (data) {
                    $('#' + WardStakeID).hide();
                },
                error: function (error) {
                    alert("An error has occurred: " + error);
                }
            });
        }

    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MenuScript" runat="server">
    <script type="text/javascript">
        MenuSelect("Home", null);
    </script>
</asp:Content>