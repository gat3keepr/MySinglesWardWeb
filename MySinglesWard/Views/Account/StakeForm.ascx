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
        <% using (Html.BeginForm("StakeRegister","Account"))
           { %>
        <%: Html.ValidationSummary(true, "Account creation was unsuccessful. Please correct the errors and try again.") %>
        <div>
                <div class="editor-label">
                    <%: Html.LabelFor(m => m.FirstName) %>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.FirstName, new { @user = "stake" })%>
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
                <div class="editor-label">
                    <%: Html.Label("Stake Creation Code") %>
                </div>
                <div class="editor-field">
                    <%: Html.Password("StakeCode") %>&nbsp;&nbsp;&nbsp; <a href="#" id="getStakeCode_link" class="ui-state-default ui-corner-all"><span>
                    </span>Get a Stake Code</a>                 
                </div>
                <div id="dynamic_recaptcha_stake"></div> 
                <div class="editor-label">
                    <p><%: Html.CheckBox("StakeAgree", false) %>&nbsp;
                    I have read and agree to <span name="privacy" class="link">MySinglesWard.com’s Privacy Statement</span>.</p>
                </div>
                <br />
                <p>
                    <input  class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all" id="StakeSubmit" type="submit" value="Register" disabled="disabled" />
                </p>
        </div>
        <% } %>
    <div id="getStakeCode" title="Get a Stake Code">
    <p>Send an email to <a class="link" href="mailto:RequestPassword@MySinglesWard.com">RequestPassword@MySinglesWard.com</a> or <br />call/text <a class="link" href="tel:+1(801) 997-0582">(801) 997-0582</a>. Give us your name, calling, and 
    phone number and you will be contacted with a password to 
  create a stake user.</p>
    </div>
    <script type="text/javascript">
        $(function () {
            // Dialog			
            $('#getStakeCode').dialog({
                autoOpen: false,
                position: top,
                width: 600,
                buttons: {
                    "Ok": function () {
                        $(this).dialog("close");
                    }
                }
            });

            // Dialog Link
            $('#getStakeCode_link').click(function () {
                $('#getStakeCode').dialog('open');
                return false;
            });
        });
    </script>

