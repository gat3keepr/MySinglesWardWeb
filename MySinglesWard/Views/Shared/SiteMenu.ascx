﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<!-- -36 / 69-->
<!--  start nav-outer-repeat................................................................................................. START -->
<div class="nav-outer-repeat">
    <!--  start nav-outer -->
    <div class="nav-outer">
        <!-- start nav-right -->
        <div id="nav-right">
        <% if (Page.User.Identity.IsAuthenticated)
           { %>
               <div class="nav-divider">
                &nbsp;</div>
            <div class="showhide-account">
                <img src="../../Content/admin/images/shared/nav/nav_myaccount.png" width="93" height="14"
                    alt="" /></div>
            <div class="nav-divider">
                &nbsp;</div>
            <a href="/Account/LogOff" id="logout">
                <img src="../../Content/admin/images/shared/nav/nav_logout.png" width="64" height="14"
                    alt="" /></a>
                 <%   }
           else
           {%>
               <div class="nav-divider">
                &nbsp;</div>
            <a href="/Account/Register" id="register">
                <img src="../../Content/admin/images/shared/nav/nav_register.png" width="93" height="14"
                    alt="" /></a>
            <div class="nav-divider">
                &nbsp;</div>
            <a href="/Account/Logon" id="login">
                <img src="../../Content/admin/images/shared/nav/nav_login.png" width="64" height="14"
                    alt="" /></a>
            <% } %>
            <div class="clear">
                &nbsp;</div>
            <!--  start account-content -->
            <div class="account-content">
                <div class="account-drop-inner">
                    <%: Html.ActionLink("Preferences", "ChangePref", "Account", null, new { @id="acc-settings" } )%>
                    <div class="clear">
                        &nbsp;</div>
                    <div class="acc-line">
                        &nbsp;</div>
                    <%: Html.ActionLink("Upload Picture", "UploadPicture", "Photo", null, new { @id="acc-details" } )%>
                    <div class="clear">
                        &nbsp;</div>
                    <% try
                       {
                           if (!bool.Parse(Session["IsBishopric"] as string))
                           { %>
                    <div class="acc-line">
                        &nbsp;</div>
                    <%: Html.ActionLink("Update Survey", "UpdateSurvey", "Home", null, new { @id = "acc-project" })%>
                    <div class="clear">
                        &nbsp;</div>
                    <div class="acc-line">
                        &nbsp;</div>
                    <%: Html.ActionLink("Change Ward", "ChangeWard", "Home", null, new { @id = "acc-settings" })%>
                    <% }
                           else
                           { %>
                    <div class="acc-line">
                        &nbsp;</div>
                    <%: Html.ActionLink("Change Ward", "SelectWard", "Bishopric", null, new { @id = "acc-settings" })%>
                    <%  }
                       }
                       catch { }%>
                </div>
            </div>
            <!--  end account-content -->
        </div>
        <!-- end nav-right -->
        <!--  start nav -->
        <div class="nav">
            <div class="table">
                <% if (Page.User.IsInRole("StakePres") || Page.User.IsInRole("Stake"))
                   {
                       Html.RenderPartial("StakeMenu");
                   } %>
                <% else
                    {
                        Html.RenderPartial("Menu");
                    } %>
                <div class="clear">
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
        <!--  start nav -->
    </div>
    <div class="clear">
    </div>
    <!--  start nav-outer -->
</div>
<!--  start nav-outer-repeat................................................... END -->
<div class="clear">
</div>
