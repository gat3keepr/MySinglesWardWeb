<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MSW.Models.RegisterModel>" %>
<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<script type="text/javascript">
    if ($.browser.msie) {
        alert("Registering with MySinglesWard.com sometimes has problems with Internet Explorer. If you have problems, please use a different browser.");
    }

</script>
    <script src="../../Scripts/js/jquery-1.4.4.min.js" type="text/javascript"></script>
    <script src="../../Scripts/js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="http://api.recaptcha.net/js/recaptcha_ajax.js"></script> 
    <script src="../../Scripts/RegisterScript.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            showRecaptcha('dynamic_recaptcha_bishopric', 'BishopricSubmit', 'white');
        });
    </script>

</asp:Content>
<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Register
</asp:Content>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
<div id="table-content">
        <table>
            <tr valign="top">
                <td>
                    <% Html.RenderPartial("BishopricForm"); %>
                </td>
                <td>
                    <% Html.RenderPartial("RegisterSidebar"); %>
                </td>
            </tr>
            <tr>
                <td>
                    <img src="../../Content/admin/images/shared/blank.gif" width="695" height="1" alt="blank" />
                </td>
                <td>
                </td>
            </tr>
        </table>
    </div>


        <% Html.RenderPartial("PrivacyStatement"); %>
    <script type="text/javascript">
        $("[id=MemberAgree]").change( function () {
            if ($('[id=MemberSubmit]').attr("disabled") == true) {
                $('[id=MemberSubmit]').attr("disabled", false);
            }
            else {
                $('[id=MemberSubmit]').attr("disabled", true);
            }
        });
        $("[id=BishopricAgree]").change(function () {
            if ($('[id=BishopricSubmit]').attr("disabled") == true) {
                $('[id=BishopricSubmit]').attr("disabled", false);
            }
            else {
                $('[id=BishopricSubmit]').attr("disabled", true);
            }
        });
        $("[id=StakeAgree]").change(function () {
            if ($('[id=StakeSubmit]').attr("disabled") == true) {
                $('[id=StakeSubmit]').attr("disabled", false);
            }
            else {
                $('[id=StakeSubmit]').attr("disabled", true);
            }
        });
	</script>

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>
        Create a New Account</h1>
</asp:Content>