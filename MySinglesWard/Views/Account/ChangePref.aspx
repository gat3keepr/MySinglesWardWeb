<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MSW.Models.ChangePasswordModel>" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Change Preferences
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>
        Change Preferences</h1>
</asp:Content>
<asp:Content ID="changePasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        New passwords are required to be a minimum of
        <%: ViewData["PasswordLength"] %>
        characters in length.
    </p>
    <p class="error">
        <%: ViewData["Error"]%></p>
    <p class="success">
        <%: ViewData["Success"]%></p>
    <br />
    <script type="text/javascript">
        $(function () {
            $("#accordion").accordion();
        });
    </script>
    <div id="accordion" class="prefList">
        <% if (!Page.User.IsInRole("StakePres") && !Page.User.IsInRole("Stake") && !bool.Parse(ViewData["IsBishopric"] as string))
           { %>
        <h3>
            <a href="#">Reminder Preferences</a></h3>
        <div>
            <% Html.RenderPartial("NotificationPreferences", ViewData["NotificationPref"], ViewData); %>
        </div>
        <% } %>
        <h3>
            <a href="#">Change Email Address</a></h3>
        <div>
            <% using (Html.BeginForm("ChangeEmail", "Account"))
               { %>
            <div class="editor-label">
                New Email Address
            </div>
            <div class="editor-field">
                <%: Html.TextBox("emailfield") %>
            </div>
            <br />
            <input class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all"
                type="submit" value="Change Email" />
            <% } %>
        </div>
        <h3>
            <a href="#">Change Password</a></h3>
        <div>
            <% using (Html.BeginForm())
               { %>
            <%: Html.ValidationSummary(true, "Password change was unsuccessful. Please correct the errors and try again.") %>
            <div>
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.OldPassword) %>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.OldPassword) %>
                    <%: Html.ValidationMessageFor(m => m.OldPassword) %>
                </div>
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.NewPassword) %>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.NewPassword) %>
                    <%: Html.ValidationMessageFor(m => m.NewPassword) %>
                </div>
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.ConfirmPassword) %>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.ConfirmPassword) %>
                    <%: Html.ValidationMessageFor(m => m.ConfirmPassword) %>
                </div>
                <br />
                <p>
                    <input class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all"
                        type="submit" value="Change Password" />
                </p>
                <% } %>
            </div>
        </div>
        <% if (Page.User.IsInRole("Bishopric"))
           {%>
        <h3>
            <a href="#">Ward Password Settings</a></h3>
        <div>
            <% using (Html.BeginForm("ChangeWardPassword", "Bishopric"))
               { %>
            <div class="editor-label">
                Current Ward Password
            </div>
            <div class="editor-field">
                <%: Html.Password("currentPassword") %>
            </div>
            <div class="editor-label">
                New Ward Password
            </div>
            <div class="editor-field">
                <%: Html.Password("newPassword") %>
            </div>
            <br />
            <p>
                <input class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all"
                    type="submit" value="Change Ward Password" />
            </p>
            <% } %>
            <% using (Html.BeginForm("RecoverWardPassword", "Bishopric"))
               { %>
            <br />
            <input class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all"
                type="submit" value="Recover Ward Password" />
            The password will be emailed to you.
            <% } %>
        </div>
        <% } %>
        <% if (Page.User.IsInRole("Stake"))
           {%>
        <h3>
            <a href="#">Stake Password Settings</a></h3>
        <div>
            <% using (Html.BeginForm("ChangeStakePassword", "Stake"))
               { %>
            <div class="editor-label">
                Current Ward Password
            </div>
            <div class="editor-field">
                <%: Html.Password("currentPassword") %>
            </div>
            <div class="editor-label">
                New Ward Password
            </div>
            <div class="editor-field">
                <%: Html.Password("newPassword") %>
            </div>
            <br />
            <p>
                <input class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all"
                    type="submit" value="Change Stake Password" />
            </p>
            <% } %>
            <% using (Html.BeginForm("RecoverStakePassword", "Stake"))
               { %>
            <br />
            <input class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all"
                type="submit" value="Recover Stake Password" />
            The password will be emailed to you.
            <% } %>
        </div>
        <% } %>
    </div>
</asp:Content>
