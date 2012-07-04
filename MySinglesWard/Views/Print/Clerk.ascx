<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div class="printTable">
    <h2>Clerk</h2>
        <table style="width: 500px">
            <tr>
                <td>
                    Sort Ward List By:
                </td>
                <td>
                    Ward List PDF
                </td>
                <td>
                    MLS Record Request CSV
                </td>
            </tr>
            <tr>
                <% using (Html.BeginForm("WardList", "Print", FormMethod.Get))
                   {%>
                <td>
                    <%: Html.DropDownList("PrintSelect" ,ViewData["DropDown"] as IEnumerable<SelectListItem>, "Choose Print-Out Option:")%>
                </td>
                <td>
                    <input type="image" src="../../Content/images/pdf_logo.bmp" alt="Submit button" />
                </td>
                <% } %>
                <td>
                    <input id="ClerkCSVlink" type="image" src="../../Content/images/excelLogo.png" alt="Submit button" />
                </td>
            </tr>
        </table>
</div>
<div id="dialog" title="Download MLS Record Request">
        <% using (Html.BeginForm("ClerkCSV", "Print", FormMethod.Post, new { @id = "ClerkCSVDownload", @target = "_blank" }))
           { %>
        <% Html.RenderPartial("WardInfo", ViewData["WardInfo"]); %>
        <% } %>
    </div>
<script type="text/javascript">
    $("#ClerkCSVlink").click(function () {
        $('#dialog').dialog('open');
        return false;
    });

    $("#dialog").dialog({
        autoOpen: false,
        height: 370,
        width: 685,
        modal: false,
        buttons: {
            "Download": function () {
                var check = true;
                var message = "";

                //Check Fields City, State, Zipcode
                if ($('#City').val() == "") {
                    check = false;
                    message += "Please Enter a City.\n";
                }
                if ($('#State').val() == "") {
                    check = false;
                    message += "Please Enter a State.\n";
                }
                if (!isValidZipCode($('#Zipcode').val())) {
                    check = false;
                    message += "Please Enter a valid Zipcode.";
                }

                if (check == true) {
                    $('#ClerkCSVDownload').submit();
                    $(this).dialog("close");
                }
                else
                    alert(message);
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    });

    function isValidZipCode(value) {
        var re = /^\d{5}([\-]\d{4})?$/;
        return (re.test(value));
    }
</script>