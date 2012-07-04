<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Groups & Notifications
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table>
        <tr valign="top">
            <td>
                <!-- SEND A NOTIFICATION -->
                <% if (User.IsInRole("StakePres") || User.IsInRole("Stake") || User.IsInRole("Stake?") || User.IsInRole("Bishopric") || User.IsInRole("Bishopric?")
            || User.IsInRole("Elders Quorum") || User.IsInRole("Elders Quorum?") || User.IsInRole("Relief Society") ||
            User.IsInRole("Relief Society?") || User.IsInRole("Activities") || User.IsInRole("Activities?") || User.IsInRole("FHE")
            || User.IsInRole("FHE?"))
                   {%>
                <a class="pictureLink" href='<%: Url.Action("SendNotification","Group") %>'>
                    <img src="../../Content/images/group_send.png" alt="Send Notification" /></a>
                <% } %>
                <!-- JOIN A GROUP -->
                <% if (!User.IsInRole("StakePres") && !User.IsInRole("Stake") && !bool.Parse(ViewData["IsBishopric"] as string))
                   {%>
                <a class="pictureLink" href='<%: Url.Action("Join","Group") %>'>
                    <img src="../../Content/images/group_join.png" alt="Join a Group" /></a>
                <% } %>
                <!-- CREATE A GROUP -->
                <% if (User.IsInRole("StakePres") || User.IsInRole("Stake") || User.IsInRole("Bishopric")
                || User.IsInRole("Elders Quorum") || User.IsInRole("Relief Society") || User.IsInRole("Activities") || User.IsInRole("FHE"))
                   {%>
                <a class="pictureLink" href='<%: Url.Action("List","Group") %>'>
                    <img src="../../Content/images/group_create.png" alt="Create a Group" /></a>
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
                                <p>
                                    Use groups in your ward to send reminders to your different quorums, groups and
                                    committees.</p>
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
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MenuScript" runat="server">
    <script type="text/javascript">
        MenuSelect("Ward", "Groups");
    </script>
</asp:Content>