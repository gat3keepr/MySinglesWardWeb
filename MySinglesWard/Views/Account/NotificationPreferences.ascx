<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MSW.Models.dbo.NotificationPreference>" %>
<% using (Html.BeginForm("NotifcationPreferences", "Account"))
   { %>
<%: Html.Hidden("MemberID", ViewData["MemberID"])%>
<p>
    Specify the kind of notifications you would like to recieve from your ward:</p>
<% if (bool.Parse(ViewData["TakenSurvey"] as string))
   { %>
<div class="editor-label">
    Text Messages:
</div>
<div class="editor-field">
    <%: Html.CheckBoxFor(model => model.txt, ViewData["txt"])%>
    <% MSW.Models.DropDowns dropdowns = new MSW.Models.DropDowns();
       dropdowns.generateCarrierList(); %>
    Send Notifications
    <%: Html.DropDownListFor(model => model.carrier, dropdowns.getCarrierList())%>
</div>
<% } %>
<div class="editor-label">
    Email:
</div>
<div class="editor-field">
    <%: Html.CheckBoxFor(model => model.email, ViewData["EmailBox"])%>
    Send Notifications
</div>
<div class="editor-label">
    <i>Please select what groups you would like to recieve notifications from:</i> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <%: Html.ActionLink("Click Here to Join a Group", "Join", "Group", null, new { @target = "_blank", @class="link"})%>
</div>
<div class="editor-field">
    <table style="width: 300px">
        <tr>
            <td>
                <%: Html.CheckBoxFor(model => model.stake, ViewData["stake"])%>
                Stake
            </td>
            <td>
                <%: Html.CheckBoxFor(model => model.ward, ViewData["ward"])%>
                Ward
            </td>
        </tr>
        <tr>
            <td>
                <%: Html.CheckBoxFor(model => model.elders, ViewData["elders"])%>
                Elders Quorum
            </td>
            <td>
                <%: Html.CheckBoxFor(model => model.reliefsociety, ViewData["reliefsociety"])%>
                Relief Society
            </td>
        </tr>
        <tr>
            <td>
                <%: Html.CheckBoxFor(model => model.activities, ViewData["activities"])%>
                Activities
            </td>
            <td>
                <%: Html.CheckBoxFor(model => model.fhe, ViewData["fhe"])%>
                FHE
            </td>
        </tr>
    </table>
</div>
<br />
<input class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all"
    id="submit" type="submit" value="Save Preferences" />
<% } %>
<script type="text/javascript">
    $("#submit").click(function () {
        if ($('#txt').is(':checked')) {
            if ($('#carrier').val() == 'Cell Phone Provider') {
                alert("Please select a cell phone provider");
                return false;
            }
        }

    });
</script>
