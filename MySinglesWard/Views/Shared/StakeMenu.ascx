﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<ul class="select">
    <% if (Page.User.Identity.IsAuthenticated)
   {%><a href="/home/profile"><b>Home</b></a>
   <% } else {%>
   <a href="/home/index"><b>Home</b></a>

   <% } %>
        <div class="select_sub">
            <ul class="sub">
                <li><a></a></li>
            </ul>
        </div>
    </li>
</ul>
<div class="nav-divider">
    &nbsp;</div>
<ul class="select" id="Tools">
    <li><a href="#"><b>Tools</b></a>
        <div class="select_sub">
            <ul class="sub">
                <li id="StakeDirectory">
                    <%: Html.ActionLink("Stake Directory", "StakeList", "Stake")%></li>
                <li id="GetStakeData">
                    <%: Html.ActionLink("Print/Export Info", "GetStakeData", "Print")%></li>
                <li id="Groups">
                    <%: Html.ActionLink("Groups & Notifications", "Index", "Group")%></li>
            </ul>
        </div>
    </li>
</ul>
