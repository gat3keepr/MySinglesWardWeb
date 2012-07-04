﻿<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<MSW.CallingReports.Organization>>" %>

<link href="../../Content/admin/css/callingReport.css" rel="stylesheet" type="text/css" />
<% if (Model.Count() > 0)
   {
       foreach (var org in Model)
       { %>
<div style="padding: 10px">
    <h2>
        <%: org.Title%></h2>
    <table class="reportTable" style="width:100%">
        <tr>
            <td style="width: 25%">
                <strong>Title</strong>
            </td>
            <td style="width: 20%">
                <strong>Member</strong>
            </td>
            <td style="width: 16%">
                <strong>Approved</strong>
            </td>
            <td style="width: 16%">
                <strong>Called</strong>
            </td>
            <td style="width: 16%">
                <strong>Sustained</strong>
            </td>
            <td style="width: 16%">
                <strong>Set Apart</strong>
            </td>
        </tr>
        <% foreach (var calling in org.Callings)
           { %>
        <tr>
            <td>
                <%: calling.Title%>
            </td>
            <td>
            <% if (calling.member != null)
               {%>
                <%: calling.member.user.LastName + ", " + calling.member.memberSurvey.prefName%>
                <% } %>
            </td>
            <td>
                <%:  String.Format("{0:M/d/yyyy}", calling.Approved)%>
            </td>
            <td>
                <%:  String.Format("{0:M/d/yyyy}", calling.Called)%>
            </td>
            <td>
                <%:  String.Format("{0:M/d/yyyy}", calling.Sustained)%>
            </td>
            <td>
                <%:  String.Format("{0:M/d/yyyy}", calling.SetApart)%>
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