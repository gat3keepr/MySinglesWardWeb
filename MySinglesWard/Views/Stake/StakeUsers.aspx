<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<MSW.Models.StakeUserModel>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Stake Users
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
	<h1>
		<%: ViewData["StakeName"] %></h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="table-content">
		<p style="color: Red">
			<%: TempData["Error"] %></p>
		<!--  start Member-table ..................................................................................... -->
		<table border="0" width="100%" cellpadding="0" cellspacing="0" id="product-table">
			<tr>
				<th class="table-header-repeat line-left minwidth-1" style="width: 126px">
				</th>
				<th class="table-header-repeat line-left minwidth-1">
					<a href="JavaScript:void()">Name</a>
				</th>
				<th class="table-header-repeat line-left">
					<a href="JavaScript:void()">Calling</a>
				</th>
				<th class="table-header-repeat line-left">
					<a href="JavaScript:void()">Email</a>
				</th>
				<th class="table-header-options line-left">
					<a href="JavaScript:void()">Phone</a>
				</th>
				<% if (User.IsInRole("StakePres"))
	   { %>
				<th class="table-header-options line-left">
					<a href="JavaScript:void()">Options</a>
				</th>
				<% } %>
			</tr>
			<% foreach (var item in Model)
	  { %>
			<tr id="<%: item.data.MemberID %>X">
				<td>
					<img class="smallPic" src="/Photo/<%: item.photo.FileName %>" alt="Profile Picture" />
				</td>
				<td>
					<% if (item.data.StakeName != "")
		{ %>
					<%: item.data.StakeName%>
					<% }
		else
		{ %>
					<%: item.user.FirstName + " " + item.user.LastName %>
					<% } %>
				</td>
				<td>
					<%: item.calling %>
				</td>
				<td>
					<%: item.user.Email %>
				</td>
				<td>
					<%: item.data.StakePhone %>
				</td>
				<% if (User.IsInRole("StakePres"))
	   { %>
				<td style="padding-left: 10px;">
					<% if (int.Parse(Session["MemberID"] as string) != item.data.MemberID)
		{ %>
					<button id="remove" memberid="<%: item.data.MemberID %>" name="button" class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all">
						<span class="ui-button-text">Remove</span></button>
					<% } %>
				</td>
				<% } %>
			</tr>
			<% } %>
		</table>
		<script type="text/javascript">
			/********************
			* Remove Member from Ward/List
			********************/
			$("[id=remove]").click(function () {
				var memberID = $(this).attr("memberid");
				SendRemoveRequest(memberID);
			});

			function SendRemoveRequest(memberID) {
				$.ajax({
					url: '/Stake/RemoveStakeUser',
					data: { id: memberID },
					success: function (data) {
						$('#' + memberID + "X").hide();
					},
					error: function (data) {
						location.reload();
					}
				});
			}
		</script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
