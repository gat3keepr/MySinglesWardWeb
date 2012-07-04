<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<MSW.Models.Group>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Group List
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>
        Group List</h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table>
        <tr valign="top">
            <td>
                <p class="error">
                    <%: ViewData["Error"] %></p>
                <table style="width: 100%">
                    <tr>
                        <td style="padding-bottom: 25px">
                        </td>
                        <td>
                            <strong>Name</strong>
                        </td>
                        <td>
                            <strong>Type</strong>
                        </td>
                        <td>
                            <strong>Leader</strong>
                        </td>
                        <td>
                            <strong>Co-Leader</strong>
                        </td>
                        <td>
                        </td>
                    </tr>
                    <% foreach (var item in Model)
                       { %>
                    <tr id="<%: item.GroupID %>X">
                        <td>
                            <img id="<%: item.GroupID %>" style="visibility: hidden; width: 15px" src="../../Content/images/green_check.gif"
                                alt="" />
                        </td>
                        <td>
                            <%: Html.ActionLink(item.Name,"Edit","Group", new { @id=item.GroupID }, null)%>
                        </td>
                        <td>
                            <%: Html.Label(item.Type )%>
                        </td>
                        <td>
                            <%: Html.Label(item.Leader) %>
                        </td>
                        <td>
                            <%: Html.Label(item.CoLeader) %>
                        </td>
                        <td>
                            <%: Html.ActionLink("Edit", "Edit", "Group", new { @id = item.GroupID }, new {  })%>
                            | <a href="" type="Remove" groupid="<%: item.GroupID %>">Remove</a>
                        </td>
                    </tr>
                    <% } %>
                </table>
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
                                    You can use groups to send notifcations to members in the ward. Bishopric members
                                    can create any type of group and organization leaders can create groups for their
                                    organization.
                                    <br />
                                    <br />
                                    After you assign a member to be a group leader/co-leader, they will be able to send
                                    notifications to the people in the group. Members in the group need to set their
                                    preferences to specify how they want to recieve notifications.
                                </p>
                            </div>
                            <div class="clear">
                            </div>
                            <div class="lines-dotted-short">
                            </div>
                            <div class="left">
                                <span name="dialog_link" style="cursor: pointer">
                                    <img src="../../Content/admin/images/forms/icon_plus.gif" width="21" height="21"
                                        alt="" /></span></div>
                            <div class="right">
                                <span name="dialog_link" style="cursor: pointer">
                                    <h5>
                                        Create New Group</h5>
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
    <div id="dialog" title="Create New Ward">
        <% Html.RenderPartial("Create", ViewData["tGroup"]); %>
    </div>
    <script type="text/javascript">
        /********************
        * Remove Group from List
        ********************/
        $("[type=Remove]").click(function () {
            if (confirm("Are you sure you want to remove this group?") == true) {
                var groupID = $(this).attr("groupid");
                SendRemoveRequest(groupID);
            }
            return false;
        });

        function SendRemoveRequest(groupID) {
            $.ajax({
                url: '/Group/RemoveGroup',
                data: { id: groupID },
                success: function (data) {
                    $('#' + groupID + "X").hide();
                },
                error: function (data) {
                    alert("You are unauthorized to delete this group.");
                }
            });
        }        
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../../Scripts/jquery.blockUI.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            // Dialog			
            $('#dialog').dialog({
                autoOpen: false,
                position: top,
                width: 600,
                buttons: {
                    "Create New Group": function () {
                        $.blockUI();
                        var string = "";
                        if ($('#Name').val() == "") {
                            string += "- Please Enter Name\n";
                        }
                        if ($('#Type').val() == "") {
                            string += "- Please Select Type\n";
                        }

                        if (string != "") {
                            alert(string);
                            $.unblockUI();
                        }
                        else {
                            $('#dialog').dialog("close");
                        }
                        $('#CreateGroup').submit();
                    },
                    "Cancel": function () {
                        $(this).dialog("close");
                    }
                }
            });

            // Dialog Link
            $('[name=dialog_link]').click(function () {
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
<asp:Content ID="Content5" ContentPlaceHolderID="MenuScript" runat="server">
    <script type="text/javascript">
        MenuSelect("Ward", "Groups");
    </script>
</asp:Content>