<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MSW.Models.WardModel>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../../Scripts/js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
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
    Bishopric Home
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>
        <%: Model.ward %></h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table style="margin-left: 20px">
        <tr valign="top">
            <td>
                <table class="ProfileText">
                    <tr>
                        <td>
                            <img class="WardPic" src="../../Content/images/church.jpg" alt="Profile Picture" />
                        </td>
                        <td>
                            <table style="width: 540px; margin: 17px">
                                <tr>
                                    <td>
                                        Members in Ward:
                                    </td>
                                    <td>
                                        <%: Model.totalMembers%>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Brothers:
                                    </td>
                                    <td>
                                        <%: Model.brothers %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Sisters:
                                    </td>
                                    <td>
                                        <%: Model.sisters %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        # of Apartments:
                                    </td>
                                    <td>
                                        <%: Model.residences %>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        Stake:
                                    </td>
                                    <td>
                                        <% if (Model.stake == null)
                                           { %>
                                        <%: Html.ActionLink("Join a Stake", "JoinStake") %>
                                        <% }
                                           else
                                           { %>
                                        <%: Model.stake %>
                                        <% } %>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <h2>
                    Ward Moderation</h2>
                <div id="tabs">
                    <ul>
                        <li><a href="#Member">Member Approval</a></li>
                        <li><a href="#Photo">Photo Approval</a></li>
                    </ul>
                    <div id="Member">
                        <% if (Model.MemberApprovals.Count == 0)
                           { %>
                        <p>
                            There are no members to approve.</p>
                        <% }
                           else
                           { %>
                        <p>
                            These members were just added to the ward and are waiting approval:</p>
                        <br />
                        <table class="ApprovalList">
                            <%foreach (var item in Model.MemberApprovals)
                              { %>
                            <tr id="<%: item.user.MemberID %>">
                                <td>
                                    <% ViewData["id"] = item.photo.FileName; %>
                                    <img class="smallPic" src="/Photo/<%: ViewData["id"] %>" alt="Profile Picture" />
                                </td>
                                <td class="largerText">
                                    <%: Html.Label(item.user.LastName +", " + item.memberSurvey.prefName)%>
                                </td>
                                <td>
                                    <button id="btnApprove" memberid="<%:item.user.MemberID %>" name="button" value="approve"
                                        class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all">
                                        <span class="ui-button-text">Approve</span></button>
                                    <button id="btnDeny" memberid="<%:item.user.MemberID %>" name="button" value="deny"
                                        class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all">
                                        <span class="ui-button-text">Deny</span>
                                    </button>
                                </td>
                            </tr>
                            <% } %>
                        </table>
                        <% } %>
                    </div>
                    <div id="Photo">
                    <% if (Model.PhotoApprovals.Count == 0)
                           { %>
                        <p>
                            There are no photos to approve.</p>
                        <% }
                           else
                           { %>
                        <p>
                            These photos were just uploaded and are waiting approval:</p>
                        <br />
                        <table class="ApprovalList">
                            <%foreach (var item in Model.PhotoApprovals)
                              { %>
                            <tr id="<%: item.user.MemberID %>PHOTO">
                                <td>
                                    <% ViewData["image"] = item.photo.NewPhotoFileName; %>
                                    <img class="smallPic" src="/Photo/<%: ViewData["image"] %>" alt="Profile Picture" />
                                </td>
                                <td class="largerText">
                                    <%: Html.Label(item.user.LastName +", " + item.memberSurvey.prefName)%>
                                </td>
                                <td>
                                    <button id="photoApprove" photoid="<%:item.user.MemberID %>" name="button" value="approve"
                                        class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all">
                                        <span class="ui-button-text">Approve</span></button>
                                    <button id="photoDeny" photoid="<%:item.user.MemberID %>" name="button" value="deny"
                                        class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all">
                                        <span class="ui-button-text">Deny</span>
                                    </button>
                                </td>
                            </tr>
                            <% } %>
                        </table>
                        <% } %>
                    </div>
                </div>
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
                            <% try
                               {
                                   if (bool.Parse(Session["IsBishopric"] as string))
                                   { %>
                            <div class="left">
                                <img src="../../Content/images/<%: ViewData["InfoDone"] %>" width="21" height="21"
                                    alt="" /></div>
                            <div class="right">
                                <h5>
                                    <%: Html.ActionLink("Update Personal Information", "BishopricData", "Bishopric")  %></h5>
                            </div>
                            <div class="clear">
                            </div>
                            <div class="lines-dotted-short">
                            </div>
                            <div class="left">
                                <img src="../../Content/images/<%: ViewData["PictureDone"] %>" width="21" height="21"
                                    alt="" /></div>
                            <div class="right">
                                <h5>
                                    <%: Html.ActionLink("Upload Picture","UploadPicture","Photo") %></h5>
                            </div>
                            <div class="clear">
                            </div>
                            <div class="lines-dotted-short">
                            </div>
                            <div class="left">
                                <img src="../../Content/admin/images/forms/icon_edit.gif" width="21" height="21"
                                    alt="" /></div>
                            <div class="right">
                                <h5>
                                    Personal Options</h5>
                                <ul class="greyarrow">
                                    <li>
                                        <%: Html.ActionLink("Update Personal Information", "BishopricData", "Bishopric")%></li>
                                    <li>
                                        <%: Html.ActionLink("Upload Picture", "UploadPicture", "Photo" ) %></li>
                                    <li>
                                        <%: Html.ActionLink("Change Ward", "SelectWard", "Bishopric")%></li>
                                </ul>
                            </div>
                            <div class="clear">
                            </div>
                            <div class="lines-dotted-short">
                            </div>
                            <% }
                               }
                               catch { }%>
                            <div class="left">
                                <img src="../../Content/admin/images/forms/icon_edit.gif" width="21" height="21"
                                    alt="" /></div>
                            <div class="right">
                                <h5>
                                    Tools</h5>
                                <ul class="greyarrow">
                                    <li>
                                        <%: Html.ActionLink("Ward List", "WardList", "Home")%></li>
                                    <li>
                                        <%: Html.ActionLink("Groups & Notifications", "Index", "Group")%></li>
                                    <li>
                                        <%: Html.ActionLink("Stewardship Information - Print/Export", "GetData", "Print")%></li>
                                </ul>
                            </div>
                            <div class="clear">
                            </div>
                            <div class="lines-dotted-short">
                            </div>
                            <div class="left">
                                <img src="../../Content/admin/images/forms/icon_edit.gif" width="21" height="21"
                                    alt="" /></div>
                            <div class="right">
                                <h5>
                                    Manage Ward</h5>
                                <ul class="greyarrow">
                                    <li>
                                        <%: Html.ActionLink("Manage Ward List", "ManageWardList", "Bishopric")%></li>
                                    <li>
                                        <%: Html.ActionLink("Update Residences", "UpdateResidence", "Bishopric")%></li>
                                    <% try
                                       {
                                           if (bool.Parse(Session["IsBishopric"] as string))
                                           { %>
                                    <li>
                                        <%: Html.ActionLink("Join a Stake", "JoinStake", "Bishopric")%></li>
                                    <% }
                                       }
                                       catch { }%>
                                </ul>
                            </div>
                            <div class="clear">
                            </div>
                            <div class="lines-dotted-short">
                            </div>
                            <div class="left">
                                <img src="../../Content/admin/images/forms/icon_edit.gif" width="21" height="21"
                                    alt="" /></div>
                            <div class="right">
                                <h5>
                                    Manage Callings</h5>
                                <ul class="greyarrow">
                                    <li id="Callings">
                                        <%: Html.ActionLink("Callings", "Index", "Calling")%></li>
                                    <li id="Reports">
                                        <%: Html.ActionLink("Reports", "Reports", "Calling")%></li>
                                </ul>
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
    <script type="text/javascript">
        //Member Approval
        $("[id=btnApprove]").click(function () {

            var memberID = $(this).attr("memberID");
            SendApprovalRequest('approve', memberID);
        });
        $("[id=btnDeny]").click(function () {

            var memberID = $(this).attr("memberID");
            SendApprovalRequest('deny', memberID);
        });

        function SendApprovalRequest(button, memberID) {
            $.ajax({
                url: '/Bishopric/ApproveNewMember',
                data: { button: button, Memberid: memberID },
                success: function (data) {
                    $('#' + memberID).hide();
                },
                error: function (error) {
                    alert("An error has occurred: " + error);
                }
            });
        }

        //Photo Approval
        $("[id=photoApprove]").click(function () {
            var memberID = $(this).attr("photoID");
            SendPhotoRequest(true, memberID);
        });

        $("[id=photoDeny]").click(function () {
            var memberID = $(this).attr("photoID");
            SendPhotoRequest(false, memberID);
        });

        function SendPhotoRequest(isApproved, memberID) {
            $.ajax({
                url: '/Photo/ModeratePhoto',
                data: { memberID: memberID, isApproved: isApproved },
                success: function (data) {
                    $('#' + memberID + 'PHOTO').hide();
                },
                error: function (error) {
                    alert("An error has occurred: " + error);
                }
            });
        }

        // Tabs
        var $tabs = $('#tabs').tabs(); // first tab selected

    </script>
</asp:Content>
