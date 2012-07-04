<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MSW.Models.GroupListModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Join a Group
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>
        Join a Group</h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table>
        <tr valign="top">
            <td>
                <% int type = 0; %>
                <% foreach (var GroupType in Model.groupsList)
                   { %>
                <% if (GroupType.Count > 0)
                   { %>
                <h2>
                        <%: Model.groupNames[type] %>
                        Groups</h2>
                    <table class="joinList">
                        <% foreach (var group in GroupType)
                           { %>
                        <tr>
                            <td class="button">
                                <button id="btnJoin" groupid="<%: group.GroupID %>" <% if(group.joined) { %> style="display: none;"
                                    <% } %> name="button" class="ui-state-green ui-corner-all">
                                    <span class="ui-button-text">Join</span></button>
                                <button id="btnLeave" groupid="<%: group.GroupID %>" <% if(!group.joined) { %> style="display: none;"
                                    <% } %>name="button" class="ui-state-red ui-corner-all">
                                    <span class="ui-button-text">Leave</span></button>
                                <img src="../../Content/images/loading.gif" style="display: none; width: 30px" id="<%: group.GroupID %>LOAD" />
                            </td>
                            <td class="nameColumn">
                                <%: Html.Label(group.Name)%>
                            </td>
                            <td class="column">
                                <b>Leader:</b>
                                <%: Html.Label(group.Leader)%>
                            </td>
                            <td class="column">
                                <b>Co-Leader:</b>
                                <%: Html.Label(group.CoLeader)%>
                            </td>
                        </tr>
                        <% } %>
                    </table>
                <% } 
                type++; 
                 } %>
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
                                    When you join a group, you can recieve notifications based on your
                                    <%: Html.ActionLink("preferences", "ChangePref", "Account",null,null) %>.
                                    <br />
                                    <br />
                                    Joining a Group lets you recieve notifications from the group leaders. This will
                                    help you stay in touch with your ward leadership and help you stay up to date on
                                    the stake and ward activities.
                                </p>
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
    <%: Html.Hidden("MemberID", Model.MemberID) %>
    <script type="text/javascript">
        $("[id=btnJoin]").click(function () {
            $(this).hide();
            var groupID = $(this).attr("groupID");
            var memberID = $("#MemberID").val();
            $("#" + groupID + "LOAD").show();
            SendGroupRequest('join', groupID, memberID);
        });
        $("[id=btnLeave]").click(function () {
            $(this).hide();
            var groupID = $(this).attr("groupID");
            var memberID = $("#MemberID").val();
            $("#" + groupID + "LOAD").show();
            SendGroupRequest('leave', groupID, memberID);
        });

        function SendGroupRequest(button, groupID, memberID) {
            $.ajax({
                url: '/Group/JoinRequest',
                data: { button: button, groupid: groupID, memberid: memberID },
                success: function (data) {
                    $("#" + groupID + "LOAD").hide();
                    if (button == "leave") {
                        $("[id=btnJoin][groupID=" + groupID + "]").show();
                    }
                    else {
                        $("[id=btnLeave][groupID=" + groupID + "]").show();
                    }
                },
                error: function (error) {
                    alert("An error has occurred: " + error);
                    location.reload();
                }
            });
        }

    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MenuScript" runat="server">
    <script type="text/javascript">
        MenuSelect("Ward", "Groups");
    </script>
</asp:Content>