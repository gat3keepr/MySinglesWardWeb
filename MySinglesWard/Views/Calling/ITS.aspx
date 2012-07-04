<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<MSW.CallingReports.Organization>>" %>
<link href="../../Content/admin/css/callingReport.css" rel="stylesheet" type="text/css" />
<% if (Model.Count() > 0)
   {
       foreach (var org in Model)
       { %>
<div style="padding: 10px">
    <h2>
        <%: org.Title%></h2>
    <table class="reportTable">
        <% foreach (var calling in org.Callings)
           { %>
        <tr>
            <td>
                <%: calling.Title%>
            </td>
            <td>
                <% if (calling.member != null)
               { %>
                <%: calling.member.user.LastName + ", " + calling.member.memberSurvey.prefName%>
                <% } %>
            </td>
        </tr>
        <% } %>
    </table>
</div>
<% }
   }
   else
   {%>
<div style="padding: 10px">
    <table class="reportTable">
        <tr>
            <td>
                No callings to report.
            </td>
        </tr>
    </table>
</div>
<% } %>