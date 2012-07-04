﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<% using (Html.BeginForm("MissionPrint", "Print", FormMethod.Post))
   {%>
<div class="printTable">
    <h2>Ward Mission</h2>
        <table class="printTable">
            <tr>
                <td>
                    Sort By:
                </td>
                <td>
                    PDF
                </td>
                <td>
                    CSV
                </td>
            </tr>
            <tr>
                <% using (Html.BeginForm("MissionPrint", "Print", FormMethod.Get))
                   {%>
                <td>
                    <%: Html.DropDownList("PrintSelect" ,ViewData["DropDown"] as IEnumerable<SelectListItem>, "Choose Print-Out Option:")%>
                </td>
                <td>
                    <input type="image" src="../../Content/images/pdf_logo.bmp" alt="Submit button" />
                </td>
                <% } %>
                <% using (Html.BeginForm("MissionCSV", "Print", FormMethod.Get))
                   {%>
                <td>
                    <input type="image" src="../../Content/images/excelLogo.png" alt="Submit button" />
                </td>
                <% } %>
            </tr>
        </table>
</div>
<% } %>