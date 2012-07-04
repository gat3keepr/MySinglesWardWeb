<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
	<script src="../../Scripts/SelectStakeScript.js" type="text/javascript"></script>
	<script type="text/javascript">
		$(function () {
			// Dialog			
			$('#dialog').dialog({
				autoOpen: false,
				position: top,
				width: 600,
				buttons: {
					"Create Stake": function () {
						$('#NewStake').submit();
					},
					"Cancel": function () {
						$(this).dialog("close");
					}
				}
			});

			// Dialog Link
			$('[name=newStake_link]').click(function () {
				$('#dialog').dialog('open');
				return false;
			});

		});
	</script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Join/Create Stake
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
	<h1>
		Join/Create A Stake</h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<table>
		<tr valign="top">
			<td>
				<p class="error">
					<%: ViewData["Error"] %></p>
				<p style="padding: 5px">
					Please select a stake to join or create a new stake.</p>
				<br />
				<% var DropDowns = new MSW.Models.DropDowns(); %>
				<% DropDowns.generateSupportedStakeList(); %>
				<% using (Html.BeginForm("SelectStake", "Stake", FormMethod.Post))
	   {%>
				<table class="SelectWardStake">
					<tr>
						<td>
							Location
						</td>
						<td>
							Stake
						</td>
						<td>
							Stake Password
						</td>
						<td>
						</td>
					</tr>
					<tr>
						<td>
							<%: Html.DropDownList("ChosenArea", DropDowns.getSupportedStakesList(), "Select Supported Location")%>
						</td>
						<td>
							<%: Html.DropDownList("ChosenStake", "Select Supported Stake")%>
						</td>
						<td>
							<%: Html.Password("password") %>
						</td>
						<td>
							<input class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all"
								id="submit" type="submit" value="Continue" disabled="disabled" />
						</td>
					</tr>
				</table>
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				<br />
				
				<% } %>
				<% if (User.IsInRole("StakePres"))
	   { %>
				<div id="dialog" title="Create New Ward">
					<% using (Html.BeginForm("CreateNewStake", "Stake", FormMethod.Post, new { @id = "NewStake" }))
		{ %>
					<div class="editor-label">
						Please Enter the Area of your Stake
					</div>
					<div class="editor-field">
						<%: Html.TextBox("Area")%>
					</div>
					<div class="editor-label">
						Please Enter the Name of the Stake
					</div>
					<div class="editor-field">
						<%: Html.TextBox("Stake")%>
					</div>
					<div class="editor-label">
						Please Enter the password for your Stake
					</div>
					<div class="editor-field">
						<%: Html.Password("Password")%>
					</div>
					<% } %>
				</div>
				<% } %>
			</td>
			<td>
				<!--  start Information & Features -->
				<div id="related-activities">
					<!--  start related-act-top -->
					<div id="related-act-top">
						<img src="../../Content/admin/images/forms/header_related_act.gif" width="271" height="43"
							alt="" />
					</div>
					<!-- end related-act-top -->
					<!--  start related-act-bottom -->
					<div id="related-act-bottom">
						<!--  start related-act-inner -->
						<div id="related-act-inner">
							<div class="left">
								<img src="../../Content/admin/images/forms/icon_edit.gif" width="21" height="21"
									alt="" /></div>
							<div class="right">
								Stake Passwords can be retrieved from a member of the Stake Presidency. This password
								is set upon the creation of the stake.
							</div>
							<div class="clear">
							</div>
							<% if (User.IsInRole("StakePres"))
		  { %>
							<div class="lines-dotted-short">
							</div>
							<div class="left">
								<span name="newStake_link" style="cursor: pointer">
									<img src="../../Content/admin/images/forms/icon_plus.gif" width="21" height="21"
										alt="" /></span></div>
							<div class="right">
								<span name="newStake_link" style="cursor: pointer">
									<h5>
										Create New Stake</h5>
								</span>
							</div>
							<div class="clear">
							</div>
							<% } %>
						</div>
						<!-- end related-act-inner -->
						<div class="clear">
						</div>
					</div>
					<!-- end related-act-bottom -->
				</div>
				<!-- end Information & Features -->
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
</asp:Content>
