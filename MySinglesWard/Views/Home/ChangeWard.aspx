<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Change Stake & Ward
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>Change Stake & Ward</h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../../Scripts/SelectWardScript.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table>
        <tr valign="top">
            <td>
           <% Html.RenderPartial("SelectWardDropdowns"); %>
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
                                When you change wards, you will no longer have access to the ward list. You will be released from your calling and be removed from
                                any groups you may have joined.
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
