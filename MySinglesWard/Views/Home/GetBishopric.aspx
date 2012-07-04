<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MSW.Models.BishopricModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	View Bishopric Member
	<% if (Model.data.BishopricName != "")
	{ %>
		<%: Model.data.BishopricName %>
	<% }
	else
	{ %>
		<%: Model.user.LastName + ", " + Model.user.FirstName%>
	<% } %>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
	<% if (Model.data.BishopricName != "")
	{ %>
	<h1>
		<%: Model.data.BishopricName %></h1>
	<% }
	else
	{ %>
	<h1>
		<%: Model.user.LastName + ", " + Model.user.FirstName%></h1>
	<% } %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<table class="MemberText">
		<tr>
			<td class="leftColumn">
				<img class="MemberPic" src="/Photo/<%: Model.photo.FileName %>" alt="Profile Picture" />
			</td>
			<td class="rightColumn">
				<div class="display-label">
					<strong>Calling: </strong>
					<%: Model.data.BishopricCalling %></div>
				<div class="display-label">
					<strong>Email: </strong><a href="mailto:<%: Model.user.Email%>">
						<%: Model.user.Email%></a></div>
				<div class="display-label">
					<strong>Cell Phone:</strong> <a href="tel:+1<%: Model.data.BishopricPhone%>">
						<%: Model.data.BishopricPhone%></a>
				</div>
				<div class="display-label">
					<strong>Address:</strong>
					<%: Model.data.BishopricAddress %>
				</div>
				<div class="display-label">
					<strong>Wife's Name: </strong>
					<%: Model.data.WifeName%></div>
				<div class="display-label">
					<strong>Wife's Phone:</strong> <a href="tel:+1<%: Model.data.WifePhone %>">
						<%: Model.data.WifePhone%></a>
				</div>
			</td>
		</tr>
	</table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MenuScript" runat="server">
	<script type="text/javascript">
		MenuSelect("Tools", "WardList");
	</script>
</asp:Content>
