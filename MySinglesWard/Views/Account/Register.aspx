<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MSW.Models.RegisterModel>" %>

<%@ Register TagPrefix="recaptcha" Namespace="Recaptcha" Assembly="Recaptcha" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        if ($.browser.msie) {
            alert("Registering with MySinglesWard.com sometimes has problems with Internet Explorer. If you have problems, please use a different browser.");
        }

    </script>
    <script src="../../Scripts/js/jquery-ui-1.8.13.custom.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="http://api.recaptcha.net/js/recaptcha_ajax.js"></script>
    <script src="../../Scripts/RegisterScript.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Register
</asp:Content>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <div id="dialog" class="ui-dialog" title="Select User Type">
        <p>
            Thanks for choosing MySinglesWard.com. Whether you are a <strong>Member</strong>
            user, <strong>Bishopric</strong> user, or <strong>Stake</strong> user, MySinglesWard.com
            will help you minister better in your respective calling.
        </p>
        <br />
        <p>
            All information stored on this site is <i>safe</i> and <i>secure.</i></p>
        <br />
        <p>
            Please select user type:</p>
            <br />
        <center>
            <ul class="reglist">
                <li>
                    <button id="member" name="button" class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all">
                        <span class="ui-button-text">Member User</span></button></li>
                <li>
                    <button id="bishopric" name="button" class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all">
                        <span class="ui-button-text">Bishopric User</span></button></li>
                <li>
                    <button id="stake" name="button" class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all">
                        <span class="ui-button-text">Stake User</span></button></li>
            </ul>
        </center>
    </div>
    <div id="table-content">
        <table>
            <tr valign="top">
                <td>
                    <div id="memberSection">
                        <% Html.RenderPartial("MemberForm"); %>
                    </div>
                    <div id="bishopricSection">
                        <% Html.RenderPartial("BishopricForm"); %>
                    </div>
                    <div id="stakeSection">
                        <% Html.RenderPartial("StakeForm"); %>
                    </div>
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

        $("[id=MemberAgree]").change(function () {
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
