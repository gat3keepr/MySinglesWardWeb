<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<% var DropDowns = new MSW.Models.DropDowns(); %>
<% DropDowns.generateSupportedWardList(); %>
<% using (Html.BeginForm("ChangeWard", "Home", FormMethod.Post))
   {%>
<table class="SelectWardStake">
    <tr>
        <td>
            <strong>Location</strong>
        </td>
        <td>
             <strong>Stake</strong>
        </td>
        <td>
             <strong>Ward</strong>
        </td>
        <td>
        </td>
    </tr>
    <tr>
        <td>
            <%: Html.DropDownList("ChosenArea", DropDowns.getSupportedWardsList(), "Select Supported Location")%>
        </td>
        <td>
            <%: Html.DropDownList("ChosenStake", "Select Supported Stake")%>
        </td>
        <td>
            <%: Html.DropDownList("ChosenWard", "Select Supported Ward")%>
        </td>
        <td>
            <input class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all"
                id="submit" type="submit" value="Continue" disabled="disabled" />
        </td>
    </tr>
</table>
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<% } %>