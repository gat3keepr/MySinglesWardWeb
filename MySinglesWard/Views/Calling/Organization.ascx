<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MSW.Models.OrganizationModel>" %>

<div>
    <div class="organization">
        <!--  start organization-top -->
        <div class="organization-top">
            <span style="cursor: pointer" name="orgTop">
                <img name='minus' src="../../Content/admin/images/forms/header_org_minus.png" height="43"
                    alt="" />
                <img name='plus' src="../../Content/admin/images/forms/header_org_plus.png" height="43"
                    alt="" style="display: none" />
                <span class="OrgTitle">
                    <%: Model.data.Title %></span> </span>
        </div>
        <!-- end organization-top -->
        <!--  start organization-bottom -->
        <div class="organization-bottom" name="orgBottom">
            <!-- Wrapper for animation -->
                <div orgid="<%: Model.data.OrgID %>bg" class="noBackground">            
                <!--  organization-inner -->
                <div class="organization-inner">
                    <span name="RemoveOrganization" orgid="<%: Model.data.OrgID %>" class="addCallingLink">
                        <img src="../../Content/admin/images/forms/icon_minus_red.gif" width="13" alt="" />
                        Remove Organization</span> <span name="AddCalling" orgid="<%: Model.data.OrgID %>" class="addCallingLink">
                            <img src="../../Content/admin/images/forms/icon_plus.gif" width="13" alt="" />
                            Add Calling</span>
                    <p class="organization_header">
                        Organization Leader:
                        <%: Html.DropDownListFor(model => Model.LeaderCallingID, Model.CallingList, "Select a Calling", new { @style = "width: 161px", @orgLeader = Model.data.OrgID })%>
                        &nbsp; <span name="AddCoLeaderCalling" orgid="<%: Model.data.OrgID %>" class="addLeaderLink">
                            <img src="../../Content/admin/images/forms/icon_plus.gif" width="13" alt="" />
                            Add Leader</span> &nbsp; Report Access:
                        <%: Html.DropDownListFor(model => Model.ReportID, (ViewData["dropdown"] as MSW.Models.DropDowns).getReportList(), "Select a Report", new { @reportID = Model.data.OrgID })%>
                    </p>
                    <table class="callingHeaders">
                        <tr>
                            <td style="width: 465px">
                                Title
                            </td>
                            <td style="width: 306px">
                                Assignment
                            </td>
                            <td style="width: 129px">
                                Status
                            </td>
                            <td style="width: 167px">
                                ITS
                            </td>
                        </tr>
                    </table>
                    <ul class="callingList" id="<%: Model.data.OrgID %>ul" name="sortable" orgid = "<%: Model.data.OrgID %>">
                        <% foreach (var calling in Model.Callings)
                           {
                               Html.RenderPartial("Calling", calling, ViewData);
                           } %>
                    </ul>
                    <div class="clear">
                    </div>
                </div>
                <!-- end organization-inner -->
                <div class="clear">
                </div>
            </div>
        </div>
        <!-- end organization-bottom -->
    </div>
    <!-- end organization -->
</div>

