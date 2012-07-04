<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Stewardship Information
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>
        Stewardship Information</h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table style="margin-left: 20px">
        <tr valign="top">
            <td>
                <% if (User.IsInRole("Bishopric"))
                   {%>
                <% Html.RenderPartial("Bishopric"); %>
                <% Html.RenderPartial("EldersQuorum"); %>
                <% Html.RenderPartial("ReliefSociety"); %>
                <% Html.RenderPartial("Activities"); %>
                <% Html.RenderPartial("Emergency"); %>
                <% Html.RenderPartial("Employment"); %>
                <% Html.RenderPartial("FHE"); %>
                <% Html.RenderPartial("FamHist"); %>
                <% Html.RenderPartial("Institute"); %>
                <% Html.RenderPartial("Mission"); %>
                <% Html.RenderPartial("Music"); %>
                <% Html.RenderPartial("Teaching"); %>
                <% Html.RenderPartial("Clerk"); %>
                <% }
                   else
                   {%>
                <% if (User.IsInRole("Activities"))
                   {%>
                <% Html.RenderPartial("Activities"); %>
                <% } %>
                <% if (User.IsInRole("Elders Quorum"))
                   {%>
                <% Html.RenderPartial("EldersQuorum"); %>
                <% } %>
                <% if (User.IsInRole("Emergency"))
                   {%>
                <% Html.RenderPartial("Emergency"); %>
                <% } %>
                <% if (User.IsInRole("Employment"))
                   {%>
                <% Html.RenderPartial("Employment"); %>
                <% } %>
                <% if (User.IsInRole("Temple/FamHist"))
                   {%>
                <% Html.RenderPartial("FamHist"); %>
                <% } %>
                <% if (User.IsInRole("FHE"))
                   {%>
                <% Html.RenderPartial("FHE"); %>
                <% } %>
                <% if (User.IsInRole("Institute"))
                   {%>
                <% Html.RenderPartial("Institute"); %>
                <% } %>
                <% if (User.IsInRole("Mission"))
                   {%>
                <% Html.RenderPartial("Mission"); %>
                <% } %>
                <% if (User.IsInRole("Music"))
                   {%>
                <% Html.RenderPartial("Music"); %>
                <% } %>
                <% if (User.IsInRole("Relief Society"))
                   {%>
                <% Html.RenderPartial("ReliefSociety"); %>
                <% } %>
                <% if (User.IsInRole("Teaching"))
                   {%>
                <% Html.RenderPartial("Teaching"); %>
                <% } %>
                <% if (User.IsInRole("Clerk"))
                   {%>
                <% Html.RenderPartial("Clerk"); %>
                <% }
                   }%>
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
                                <% if (User.IsInRole("Bishopric"))
                                   {%>
                                <p>
                                    As a member of the bishopric and executive staff, you will be able to get any of
                                    the reports available on this website.
                                </p>
                                <% } %>
                                <% if (User.IsInRole("Activities"))
                                   {%>
                                <p>
                                    Here you can get the ideas that each of the ward members had for activities.
                                </p>
                                <% } %>
                                <% if (User.IsInRole("Elders Quorum"))
                                   {%>
                                <p>
                                    Here you can get information on each of the Elders in your ward.
                                </p>
                                <% } %>
                                <% if (User.IsInRole("Emergency"))
                                   {%>
                                <p>
                                    Here you can get information on people with emergency prep info.
                                </p>
                                <% } %>
                                <% if (User.IsInRole("Employment"))
                                   {%>
                                <p>
                                    Here you can information on people in the ward without a job.
                                </p>
                                <% } %>
                                <% if (User.IsInRole("FamHist"))
                                   {%>
                                <p>
                                    Here you can get Temple and Family history information.
                                </p>
                                <% } %>
                                <% if (User.IsInRole("FHE"))
                                   {%>
                                <p>
                                    Here you can get info for FHE.
                                </p>
                                <% } %>
                                <% if (User.IsInRole("Institute"))
                                   {%>
                                <p>
                                    Here you can get information on people that should be attending Institute.
                                </p>
                                <% } %>
                                <% if (User.IsInRole("Mission"))
                                   {%>
                                <p>
                                    Here you can get information on people for the ward mission.
                                </p>
                                <% } %>
                                <% if (User.IsInRole("Music"))
                                   {%>
                                <p>
                                    Here you can get information on people and their musical abilitites.
                                </p>
                                <% } %>
                                <% if (User.IsInRole("Relief Society"))
                                   {%>
                                <p>
                                    Here you can get information on all the sisters in the ward.
                                </p>
                                <% } %>
                                <% if (User.IsInRole("Teaching"))
                                   {%>
                                <p>
                                    Here you can get information on people you teach.
                                </p>
                                <% } %>
                                <% if (User.IsInRole("Clerk"))
                                   {%>
                                <p>
                                    Here you will be able to get information for Ward Memberships & Ward Directories.
                                </p>
                                <% } %>
                                <p>
                                    <br />
                                    If there is a order of members that is currently not on the list, please send an
                                    email to <a href="mailto:support@mysinglesward.com">support@MySinglesWard.com</a>
                                    and make a new request.</p>
                            </div>
                            <div class="clear">
                            </div>
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
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MenuScript" runat="server">
    <script type="text/javascript">
        MenuSelect("Calling", "Stewardship");
    </script>
</asp:Content>
