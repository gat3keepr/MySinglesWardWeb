<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MSW.Models.CallingModel>" %>
<li class="ui-state-default" id="<%: Model.CallingID %>"><span class="ui-icon ui-icon-arrowthick-2-n-s">
</span>
    <table>
        <tr>
            <td>
                <%: Html.TextBox("title", Model.Title, new { @title = Model.CallingID, @class="callingName" })%>
                <%: Html.HiddenFor(model => model.Description, new { @title = Model.CallingID, @description = Model.CallingID })%>
            </td>
            <td>
            <% if (Model.MemberID != 0 && Model.CallingStatus >= (int)MSW.Models.dbo.Calling.Status.CALLED)
               {%>
                <%: Html.DropDownListFor(model => model.MemberID, ViewData["NameList"] as IEnumerable<SelectListItem>, "Select a Member", new { @style = "width: 161px", @assignment = Model.CallingID, @disabled="disabled" })%>
            <% }
               else
               { %>
               <%: Html.DropDownListFor(model => model.MemberID, ViewData["NameList"] as IEnumerable<SelectListItem>, "Select a Member", new { @style = "width: 161px", @assignment = Model.CallingID })%>
            <% } %>
            </td>
            <td>
                <%: Html.DropDownListFor(model => model.CallingStatus, (ViewData["dropdown"] as MSW.Models.DropDowns).getCallingStatusList(), new { @style = "width: 87px", @status = Model.CallingID })%>
            </td>
            <td>
                <span style="margin: -12px 0px 0px -440px"><span name="description" title="Description"
                    callingid="<%: Model.CallingID %>" callingorgid="<%: Model.OrgID %>" class="icon-9 info-tooltip">
                </span></span>
            </td>
            <td>
                <span style="margin: -8px 0px 0px -18px">
                    <%: Html.CheckBoxFor(model => model.ITStake, new {  @its = Model.CallingID })%></span>
            </td>
            <td>
                <span style="margin: -12px 0px 0px 36px"><span name="update" title="Update" callingid="<%: Model.CallingID %>"
                    callingorgid="<%: Model.OrgID %>" class="icon-5 info-tooltip"></span></span>
                <span style="margin: -12px 0px 0px 59px"><span name="release" title="Release Member"
                    callingid="<%: Model.CallingID %>" class="icon-8 info-tooltip"></span></span><span
                        style="margin: -12px 0px 0px 82px"><span name="remove" title="Remove From Organization"
                            callingid="<%: Model.CallingID %>" callingorgid="<%: Model.OrgID %>" class="icon-2 info-tooltip">
                        </span></span>
            </td>
        </tr>
    </table>
</li>
