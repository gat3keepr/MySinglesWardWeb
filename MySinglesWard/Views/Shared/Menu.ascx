﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%if (Page.User.Identity.IsAuthenticated)
  {  %>
<ul class="select">
	<li><a href="/home/profile" id="Home"><b>My Profile</b></a>
		<div class="select_sub">
			<ul class="sub">
				<li><a></a></li>
			</ul>
		</div>
	</li>
</ul>
<%}
  else
  { %>
<ul class="select">
	<li><a href="/home/index" id="A2"><b>Home</b></a>
		<div class="select_sub">
			<ul class="sub">
				<li><a></a></li>
			</ul>
		</div>
	</li>
</ul>
<div class="nav-divider">
	&nbsp;</div>
<ul class="select">
	<li><a href="/info/index" id="A1"><b>Information</b></a>
		<div class="select_sub">
			<ul class="sub">
				<li><a></a></li>
			</ul>
		</div>
	</li>
</ul>
<%}
  if (Page.User.Identity.IsAuthenticated)
  { %>
<div class="nav-divider">
	&nbsp;</div>
<ul class="select" id="Ward">
	<li><a href="#"><b>My Ward</b></a>
		<div class="select_sub">
			<ul class="sub">
				<li id="WardList">
					<%: Html.ActionLink("Ward List", "WardList", "Home")  %></li>
				<%if (Page.User.IsInRole("Bishopric") || Page.User.IsInRole("Clerk"))
	  {  %>
				<li id="ManageWardList">
					<%: Html.ActionLink("Manage Ward List", "ManageWardList", "Bishopric")  %></li>
				<li id="UpdateResidences">
					<%: Html.ActionLink("Update Residences", "UpdateResidence", "Bishopric")  %></li>
				<%} %>
				<li id="Groups">
					<%: Html.ActionLink("Groups & Notifications", "Index", "Group")  %></li>
				<%try
	  {
		  if (bool.Parse(Session["IsBishopric"] as string))
		  { %>
				<li id="JoinStake">
					<%: Html.ActionLink("Join a Stake", "JoinStake", "Bishopric")  %></li>
				<% }
	  }
	  catch { }  %>
			</ul>
		</div>
	</li>
</ul>
<%if (!Page.User.IsInRole("Member?"))
  {  %>
<div class="nav-divider">
	&nbsp;</div>
<ul class="select" id="Organization">
	<li><a href="#"><b>
		<% try
	 {
		 if (Page.User.IsInRole("Bishopric"))
		 { %>
		Organizations
		<% }
		 else if (bool.Parse(Session["isPriesthood"] as string))
		 { %>
		My Quorum
		<%}
		 else
		 { %>
		My Relief Society
		<% }
	 }
	 catch
	 { %>
		Organization
		<% } %></b></a>
		<div class="select_sub">
			<ul class="sub">
				<% try
	   {
		   if (!bool.Parse(Session["IsBishopric"] as string))
		   {
			   if (bool.Parse(Session["isPriesthood"] as string))
			   { %>
				<li id="orgTeaching">
					<%: Html.ActionLink("My Home Teaching", "Teaching", "Organization")%></li>
				<%}
			   else
			   { %>
				<li id="orgTeaching">
					<%: Html.ActionLink("My Visiting Teaching", "Teaching", "Organization")%></li>
				<% }
		   }
	   }
	   catch
	   { %>
				<% }  %>
				<%if (Page.User.IsInRole("Bishopric"))
	  {  %>
				<li id="Li7">
					<%: Html.ActionLink("Select Organization", "SelectOrganization", "Organization")  %></li>
				<% }
	  else
	  {%>
				<li id="orgProfile">
					<%: Html.ActionLink("Profile", "Index", "Organization") %>
				</li>
				<li id="orgMembership">
					<%: Html.ActionLink("Membership", "Membership", "Organization") %>
				</li>
				<% if (Page.User.IsInRole("Elders Quorum") || Page.User.IsInRole("Relief Society"))
	   { %>
				<li id="orgReport">
					<%: Html.ActionLink("Reports", "Reports", "Organization") %>
				</li>
				<% } %>
				<% } %>
			</ul>
		</div>
	</li>
</ul>
<% } %>
<%if ((!Page.User.IsInRole("Member") && !Page.User.IsInRole("Member?")) || Page.User.IsInRole("District Leader"))
  {  %>
<div class="nav-divider">
	&nbsp;</div>
<ul class="select" id="Calling">
	<li><a href="#"><b>My Calling</b></a>
		<div class="select_sub">
			<ul class="sub">
				<%if (Page.User.IsInRole("Bishopric"))
	  { %>
				<li id="Callings">
					<%: Html.ActionLink("Callings", "Index", "Calling")%></li>
				<li id="Reports">
					<%: Html.ActionLink("Reports", "Reports", "Calling")%></li>
				<% } %>
				<% if (Page.User.IsInRole("Elders Quorum") || Page.User.IsInRole("Relief Society") || Page.User.IsInRole("District Leader"))
	   { %>
				<li id="teachingReport">
					<%: Html.ActionLink("Report Teaching", "ReportTeaching", "Organization") %>
				</li>
				<% }
	   else if (!Page.User.IsInRole("Member") && !Page.User.IsInRole("Member?"))
	   {%>
				<li id="Stewardship">
					<%: Html.ActionLink("Stewardship Information - Print/Export", "GetData", "Print")  %></li>
				<% } %>
			</ul>
		</div>
	</li>
</ul>
<% } %>
<% if (Page.User.IsInRole("Global"))
   {   %>
<div class="nav-divider">
	&nbsp;</div>
<ul class="select" id="Global">
	<li><a href="#"><b>Global</b></a>
		<div class="select_sub">
			<ul class="sub">
				<li id="UnlockUser">
					<%: Html.ActionLink("Unlock User", "UnlockUser", "Global") %></li>
				<li id="proxy">
					<%: Html.ActionLink("Proxy", "Logon", "Global") %></li>
				<li id="Li1">
					<%: Html.ActionLink("Flush Cache", "FlushCache", "Global") %></li>
				<li id="Li3">
					<%: Html.ActionLink("Ward Passwords", "WardPasswords", "Global") %></li>
			</ul>
		</div>
	</li>
</ul>
<% }
  }
  else
  {  %>
<!--<div class="nav-divider">
    &nbsp;</div>
<ul class="select" id="Features">
    <li><a href="/info/index"><b>Features</b></a>
        <div class="select_sub">
            <ul class="sub">
                <li><a></a></li>
            </ul>
        </div>
    </li>
</ul>-->
<% }  %>