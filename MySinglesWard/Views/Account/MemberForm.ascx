<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MSW.Models.RegisterModel>" %>
        <p>
            Use the form below to create a new account.
        </p>
        <p>
            Passwords are required to be a minimum of
            <%: TempData["PasswordLength"] %>
            characters in length.
        </p>
        <p class="error">
            <%: TempData["ReCaptcha"] %></p>
        <p class="error">
            <%: TempData["Error"] %></p>
        <% using (Html.BeginForm("MemberRegister", "Account"))
           { %>
        <%: Html.ValidationSummary(true, "Account creation was unsuccessful. Please correct the errors and try again.") %>
        <div>
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.FirstName) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.FirstName, new { @user = "member" })%>
                    <%: Html.ValidationMessageFor(m => m.FirstName) %>
                </div>
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.LastName) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.LastName) %>
                    <%: Html.ValidationMessageFor(m => m.LastName) %>
                </div>
                <!--<div class="editor-label">
                    <%: Html.LabelFor(m => m.UserName) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.UserName) %>
                    <%: Html.ValidationMessageFor(m => m.UserName) %>
                </div>-->
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.Email) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.Email) %>
                    <%: Html.ValidationMessageFor(m => m.Email) %>
                </div>
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.Password) %>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.Password) %>
                    <%: Html.ValidationMessageFor(m => m.Password) %>
                </div>
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.ConfirmPassword) %>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.ConfirmPassword) %>
                    <%: Html.ValidationMessageFor(m => m.ConfirmPassword) %>
                </div>
                <div id="dynamic_recaptcha_member"></div> 
                <div class="editor-label">
                    <p><%: Html.CheckBox("MemberAgree", false) %>&nbsp;
                    I have read and agree to <span name="privacy" class="link">MySinglesWard.com’s Privacy Statement</span>.</p>
                </div>
                <br />
                <p>
                    <input class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all" id="MemberSubmit" type="submit" value="Register" disabled="disabled" />
                </p>
        </div>
        <% } %>
