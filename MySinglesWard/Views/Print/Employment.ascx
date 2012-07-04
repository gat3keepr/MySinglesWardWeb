<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<% using (Html.BeginForm("EmploymentPrint", "Print", FormMethod.Post))
   {%>
<div class="printTable">
    <h2>Employment</h2>
        <table class="printTable">
            <tr>
                <td>
                    Sort By:
                </td>
                <td>
                    PDF
                </td>
                <!--<td>CSV    
    </td>-->
            </tr>
            <tr>
                <% using (Html.BeginForm("EmploymentPrint", "Print", FormMethod.Get))
                   {%>
                <td>
                    <%: Html.DropDownList("PrintSelect" ,ViewData["DropDown"] as IEnumerable<SelectListItem>, "Choose Print-Out Option:")%>
                </td>
                <td>
                    <input type="image" src="../../Content/images/pdf_logo.bmp" alt="Submit button" />
                </td>
                <% } %>
                <!--    <% using (Html.BeginForm("EmploymentCSV", "Print", FormMethod.Get))
               {%>
            <td>
                <input type="image" src="../../Content/images/excelLogo.png" alt="Submit button" />
            </td>
            <% } %>-->
            </tr>
        </table>
</div>
<% } %>