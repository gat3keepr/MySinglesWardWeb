<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MSW.Models.LogOnModel>" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
	Log On
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentTitle" runat="server">
	<h1>
		Log On</h1>
</asp:Content>
<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
	<table>
		<tr>
			<td>
				<div id="login-holder" style="margin-left: 320px">
					<!--  start loginbox ................................................................................. -->
					<div id="loginbox">
						<!--  start login-inner -->
						<div id="login-inner">
							<% using (Html.BeginForm())
		                        { %>
							<table border="0" cellpadding="0" cellspacing="0">
								<tr>
									<th>
										Email
									</th>
									<td>
										<%: Html.TextBoxFor(m => m.UserName, new { @class="login-inp" })%>
									</td>
								</tr>
								<tr>
									<th>
										Password
									</th>
									<td>
										<%: Html.PasswordFor(m => m.Password, new { @class = "login-inp" })%>
									</td>
								</tr>
								<tr>
									<th>
									</th>
									<td valign="top">
										<%: Html.CheckBoxFor(m => m.RememberMe, new { @id="login-check", @class="checkbox-size" })%><label
											for="login-check">Remember me</label>
									</td>
								</tr>
								<tr>
									<th>
									</th>
									<td>
										<input type="submit" class="submit-login" />
									</td>
								</tr>
							</table>
							<% } %>
						</div>
						<!--  end login-inner -->
						<div class="clear">
						</div>
						<%: Html.ActionLink("Forgot Password?", "ForgotPassword", null, new { @class = "forgot-pwd" })%>
					</div>
					<!--  end loginbox -->
				</div>
				<!-- End: login-holder -->
			</td>
			<td>
				<%: Html.ValidationSummary(true, "", new { @class = "error", @style="list-style: none; padding-left: 10px" })%>
			</td>
		</tr>
	</table>
	<script type="text/javascript">
		$('#UserName').focus();
	</script>
</asp:Content>
