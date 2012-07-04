<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MSW.Models.WardListModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Ward List
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table>
        <tr valign="top">
            <td>
                <table border="0" width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <div style='padding-left: 25px'>
                                <h3>
                                    Bishopric</h3>
                                <table class="WardListWithCallings">
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                            <strong>Name</strong>
                                        </td>
                                        <td>
                                            <strong>Calling</strong>
                                        </td>
                                        <td>
                                            <strong>Cell Phone</strong>
                                        </td>
                                        <td>
                                            <strong>Email</strong>
                                        </td>
                                        <% if (User.IsInRole("Bishopric"))
                                           { %>
                                        <td>
                                        </td>
                                        <% } %>
                                    </tr>
                                    <% foreach (var item in Model.bishopric)
                                       { %>
                                    <tr id="<%: item.user.MemberID %>X">
                                        <td>
                                            <% ViewData["filename"] = item.photo.FileName; %>
                                            <img class="smallPic" src="/Photo/<%: ViewData["filename"] %>" alt="Profile Picture" />
                                        </td>
                                        <td>
                                            <% if (item.data.BishopricName != "")
                                               { %>
                                            <%: Html.ActionLink(item.data.BishopricName, "GetBishopric", new { memberID= item.user.MemberID })%>
                                            <% }
                                               else
                                               { %>
                                            <%: Html.ActionLink(item.user.LastName + ", " + item.user.FirstName, "GetBishopric", new { memberID = item.user.MemberID })%>
                                            <% } %>
                                        </td>
                                        <% if (!User.IsInRole("Member?"))
                                           { %>
                                        <td>
                                            <%: item.data.BishopricCalling %>
                                        </td>
                                        <td>
                                            <a href="tel:+1<%: item.data.BishopricPhone %>">
                                                <%: item.data.BishopricPhone %></a>
                                        </td>
                                        <td>
                                            <a href="mailto:<%: item.user.Email %>">
                                                <%: item.user.Email %></a>
                                        </td>
                                        <% } %>
                                        <% if (User.IsInRole("Bishopric"))
                                           { %>
                                        <td style="padding-left: 10px;">
                                            <!-- Bishops are getting removed from the ward without any reason. Disabling this to make sure 
                                                kids aren't removing their bishop to be funny until we get some logs in plcae to figure out what it is-->
                                            <% if (int.Parse(Session["MemberID"] as string) != item.user.MemberID)
                                               { %>
                                            <button id="remove" memberid="<%: item.user.MemberID %>" name="button" class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all">
                                                <span class="ui-button-text">Remove</span></button>
                                            <% } %>
                                        </td>
                                        <% } %>
                                    </tr>
                                    <% } %>
                                </table>
                                <br />
                                <!--Member Table-->
                                <div>
                                    <table style="width: 100%">
                                        <tr>
                                            <td>
                                                <h3>
                                                    Members</h3>
                                            </td>
                                            <td>
                                                <% using (Html.BeginForm())
                                                   { %>
                                                Sort Ward List:
                                                <%: Html.DropDownList("SortSelect", ViewData["DropDown"] as IEnumerable<SelectListItem>, 
                        "Choose a sort method:", new { onchange = @"this.form.submit();"})%><% } %>
                                            </td>
                                            <td>
                                                Search by Name:
                                                <%: Html.TextBox("searchWardList") %>
                                            </td>
                                        </tr>
                                    </table>
                                    <br />
                                </div>
                                <table class="WardListWithCallings">
                                    <tr>
                                        <td>
                                        </td>
                                        <td>
                                            <strong>Name</strong>
                                        </td>
                                        <td>
                                            <strong>Apartment</strong>
                                        </td>
                                        <td>
                                            <strong>Cell Phone</strong>
                                        </td>
                                        <td>
                                            <strong>Email</strong>
                                        </td>
                                        <td>
                                            <strong>Calling</strong>
                                        </td>
                                        <% if (User.IsInRole("Bishopric"))
                                           { %>
                                        <td>
                                        </td>
                                        <% } %>
                                    </tr>
                                    <% foreach (var item in Model.members)
                                       { %>
                                    <tr id="<%: item.user.MemberID %>X" name="<%: (item.memberSurvey.prefName.ToLower() + "_"+ item.user.LastName.ToLower()).Replace(" ", "_") %>"
                                        type="member">
                                        <td>
                                            <% ViewData["filename"] = item.photo.FileName; %>
                                            <img class="smallPic" src="/Photo/<%: ViewData["filename"] %>" alt="Profile Picture" />
                                        </td>
                                        <td>
                                            <%: Html.ActionLink(item.memberSurvey.prefName + " " + item.user.LastName, "GetMember", "Home", new { memberID = item.user.MemberID }, null)%>
                                        </td>
                                        <% if (!User.IsInRole("Member?"))
                                           { %>
                                        <td>
                                            <%: item.memberSurvey.residence%>
                                        </td>
                                        <td>
                                            <% if (item.memberSurvey.publishCell || User.IsInRole("Bishopric"))
                                               { %>
                                            <a href="tel:+1<%: item.memberSurvey.cellPhone%>">
                                                <%: item.memberSurvey.cellPhone%></a>
                                            <% } %>
                                        </td>
                                        <td>
                                            <% if (item.memberSurvey.publishEmail || User.IsInRole("Bishopric"))
                                               { %>
                                            <a href="mailto:<%: item.user.Email%>">
                                                <%: item.user.Email%></a>
                                            <% } %>
                                        </td>
                                        <% } %>
                                        <td>
                                            <% foreach (var calling in item.Callings)
                                               { %>
                                               <div style="padding-bottom:5px">
                                            <%: calling.organization.Title%>
                                            -
                                            <%: calling.calling.Title%>
                                            </div>
                                            <% } %>
                                        </td>
                                        <% if (User.IsInRole("Bishopric"))
                                           { %>
                                        <td style="padding-left: 10px;">
                                            <% if (int.Parse(Session["MemberID"] as string) != item.user.MemberID)
                                               { %>
                                            <button id="remove" memberid="<%: item.user.MemberID %>" name="button" class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all">
                                                <span class="ui-button-text">Remove</span></button>
                                            <% } %>
                                        </td>
                                        <% } %>
                                    </tr>
                                    <% } %>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
                <!-- end Residence-form  -->
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
    <div class="clear">
    </div>
    <div id="dialog" title="Print Ward List">
        <% using (Html.BeginForm("WardList", "Print", FormMethod.Post, new { @id = "WardListDownload", @target = "_blank" }))
           { %>
        <p>
            Please select how you would like your ward list sorted:
            <%: Html.DropDownList("PrintSelect", ViewData["DropDown"] as IEnumerable<SelectListItem>,
                        "Choose a sort method:")%></p>
        <% } %>
    </div>
    <script type="text/javascript">
        /********************
        * Remove Member from Ward/List
        ********************/
        $("[id=remove]").click(function () {
            var memberID = $(this).attr("memberid");
            var name = $("[href='/Home/GetMember?memberID=" + memberID + "']").text();
            if (confirm("Are you sure you want to remove " + name + " from the ward?"))
                SendRemoveRequest(memberID);
        });

        function SendRemoveRequest(memberID) {
            $.ajax({
                url: '/Bishopric/RemoveMember',
                data: { id: memberID },
                success: function (data) {
                    $('#' + memberID + "X").hide();
                },
                error: function (data) {
                    location.reload();
                }
            });
        }
        $("[name=printWardList]").click(function () {
            $('#dialog').dialog('open');
            return false;
        });

        // a workaround for a flaw in the demo system (http://dev.jqueryui.com/ticket/4375), ignore!
        $("#dialog:ui-dialog").dialog("destroy");

        $("#dialog").dialog({
            autoOpen: false,
            height: 175,
            width: 685,
            modal: false,
            buttons: {
                "Download": function () {
                    $('#WardListDownload').submit();
                    $(this).dialog("close");
                },
                "Cancel": function () {
                    $(this).dialog("close");
                }
            }
        });
    </script>
    <script src="../../Scripts/WardList.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>
        Ward List <a href="" name="printWardList">
            <img style="width: 22px" alt="Downlaod Ward List" src="../../Content/images/pdf_logo.bmp" /></a></h1>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MenuScript" runat="server">
    <script type="text/javascript">
        MenuSelect("Ward", "WardList");
    </script>
</asp:Content>
