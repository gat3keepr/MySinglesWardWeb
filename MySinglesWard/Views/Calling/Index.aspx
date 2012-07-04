<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MSW.Models.CallingsModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Callings
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="table-content">
        <table>
            <tr valign="top">
                <td>
                    <% var dropdowns = new MSW.Models.DropDowns();
                       dropdowns.generateRolesList();
                       ViewData["dropdown"] = dropdowns;%>
                    <ul id="sortableOrg" style="list-style-type: none">
                        <% foreach (var org in Model.organizations)
                           { %>
                        <li id="<%: org.data.OrgID %>">
                            <% Html.RenderPartial("Organization", org, ViewData); %>
                        </li>
                        <% } %>
                    </ul>
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
                                    This page is used to create &amp; organize callings and organizations. Each calling
                                    belongs to an organization. Each organization has a calling assigned as an organization
                                    leader and any member called to that calling will have access to the chosen stewardship
                                    report.
                                    <br />
                                    <br />
                                    After any changes are made to a calling, click the green check mark to save the
                                    changes.
                                    <br />
                                    <br />
                                    Selecting the red 'x' will permanently remove the calling from your ward.
                                    <br />
                                    <br />
                                    Dragging and dropping an organization or a calling will change how it is sorted
                                    on this page and other pages.
                                    <br />
                                    <br />
                                    <strong>ITS</strong> - Important To Stake - Select to give the stake access to who
                                    is serving in this capacity.
                                </div>
                                <div class="clear">
                                </div>
                                <div class="lines-dotted-short">
                                </div>
                                <div class="left">
                                    <span name="addOrg" style="cursor: pointer">
                                        <img src="../../Content/admin/images/forms/icon_plus.gif" width="21" height="21"
                                            alt="" /></span></div>
                                <div class="right">
                                    <span name="addOrg" style="cursor: pointer">
                                        <h5>
                                            Add an Organization</h5>
                                    </span>
                                </div>
                                <div class="clear">
                                </div>
                                <div class="lines-dotted-short">
                                </div>
                                <div class="left">
                                    <span name="sortOrg" style="cursor: pointer">
                                        <img src="../../Content/admin/images/forms/icon_edit.gif" width="21" height="21"
                                            alt="" /></span></div>
                                <div class="right">
                                    <span name="sortOrg" style="cursor: pointer">
                                        <h5 id="sortingTitle">
                                            Sort Organizations</h5>
                                        <img src="../../Content/images/loading.gif" style="display: none; width: 30px; position: absolute;
                                            margin: -23px 0px 0px 125px" id="sorting" />
                                    </span>
                                </div>
                                <div class="clear">
                                </div>
                                <div class="lines-dotted-short">
                                </div>
                                <div class="left">
                                    <span name="collapseAll" style="cursor: pointer">
                                        <img src="../../Content/admin/images/forms/icon_edit.gif" width="21" height="21"
                                            alt="" /></span></div>
                                <div class="right">
                                    <span name="collapseAll" style="cursor: pointer">
                                        <h5>
                                            Collapse All</h5>
                                    </span>
                                </div>
                                <div class="clear">
                                </div>
                                <div class="lines-dotted-short">
                                </div>
                                <div class="left">
                                    <span name="expandAll" style="cursor: pointer">
                                        <img src="../../Content/admin/images/forms/icon_edit.gif" width="21" height="21"
                                            alt="" /></span></div>
                                <div class="right">
                                    <span name="expandAll" style="cursor: pointer">
                                        <h5>
                                            Expand All</h5>
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
    </div>
    <div id="descriptionEdit" title="Edit Description">
        <p>
            This description will appear to members after they have been sustained in their
            calling.</p>
        <br />
        <p>
            Calling Description:</p>
        <%: Html.TextArea("CallingDescriptionText", new { @style="width:723px; height:176px" })%>
        <%: Html.Hidden("descriptionCallingID") %>
    </div>
    <div id="AddOrganization" title="Add Organization">
        <p>
            <strong>Adding an organization will refresh this page.</strong> Please save all work before adding
            an organization.</p>
        <br />
        <table class="AddCoLeader">
            <tr>
                <td>
                    Organization Name:
                </td>
                <td>
                    <%: Html.TextBox("NewOrgName")%>
                </td>
            </tr>
            <tr>
                <td>
                    Organization Preset:
                </td>
                <td>
                    <%: Html.DropDownList("orgPreset" , dropdowns.getOrgPresets(), new { }) %>
                </td>
            </tr>
        </table>
    </div>
    <% foreach (var org in Model.organizations)
       { %>
    <!-- CoLeader Dialog Box -->
    <div name="coleaderDialog" title="Organization Co-Leaders" orgid="<%: org.data.OrgID %>"
        id="<%: org.data.OrgID %>dialog">
        Add New Co-Leader:
        <%: Html.DropDownList("newCoLeader" + org.data.OrgID, org.CallingList, new { }) %>
        <br />
        <br />
        Co-Leader Callings:
        <table class="AddCoLeader" id="<%: org.data.OrgID %>table">
            <% foreach (var calling in org.CoLeaders)
               { %>
            <tr id="<%: calling.Value %>coleader">
                <td>
                    <%: calling.Text %>
                </td>
                <td>
                    <span name="removeCoLeaderCalling" title="Remove Co-Leader Calling" callingid="<%: calling.Value %>"
                        callingorgid="<%: calling.Value %>" class="icon-2 info-tooltip"></span>
                </td>
            </tr>
            <% } %>
        </table>
    </div>
    <% } %>
    <!--Empty Calling to clone -->
    <div style="display: none">
        <li class="ui-state-default" id="newCalling" style="display: none"><span class="ui-icon ui-icon-arrowthick-2-n-s">
        </span>
            <table>
                <tr>
                    <td>
                        <%: Html.TextBox("title", null, new { @title = "", @class="callingName" })%>
                        <%: Html.Hidden("Description", null, new { @title = "", @description = "" })%>
                    </td>
                    <td>
                        <%: Html.DropDownList("MemberID", ViewData["NameList"] as IEnumerable<SelectListItem>, "Select a Member", new { @style = "width: 161px", @assignment = "" })%>
                    </td>
                    <td>
                        <%: Html.DropDownList("CallingStatus", (ViewData["dropdown"] as MSW.Models.DropDowns).getCallingStatusList(), new { @style = "width: 87px", @status = "" })%>
                    </td>
                    <td>
                        <span style="margin: -12px 0px 0px -440px"><span name="description" title="Description"
                            callingid="" callingorgid="" class="icon-9 info-tooltip"></span></span>
                    </td>
                    <td>
                        <span style="margin: -8px 0px 0px -18px">
                            <%: Html.CheckBox("ITStake", false, new {  @its = "" })%></span>
                    </td>
                    <td>
                        <span style="margin: -12px 0px 0px 36px"><span name="update" title="Update" callingid=""
                            callingorgid="" class="icon-5 info-tooltip"></span></span>
                            <span style="margin: -12px 0px 0px 59px"><span name="release" title="Release Member"
                    callingid="" class="icon-8 info-tooltip"></span></span>
                    <span style="margin: -12px 0px 0px 82px">
                                <span name="remove" title="Remove From Organization" callingid="" callingorgid=""
                                    class="icon-2 info-tooltip"></span></span>
                    </td>
                </tr>
            </table>
        </li>
    </div>
    <script src="../../Scripts/Callings.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
<script src="../../Scripts/jquery.blockUI.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>
        Organizations &amp; Callings</h1>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MenuScript" runat="server">
    <script type="text/javascript">
        MenuSelect("Calling", "Callings");
    </script>
</asp:Content>
