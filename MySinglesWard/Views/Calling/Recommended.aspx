<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<MSW.CallingReports.Organization>>" %>
<% Html.RenderPartial("ReportTable", Model); %>