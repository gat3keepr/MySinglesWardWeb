<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <script src="../../Scripts/SelectWardScript.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            // Dialog			
            $('#dialog').dialog({
                autoOpen: false,
                position: top,
                width: 650,
                buttons: {
                    "Create Ward": function () {
                        $(this).dialog("close");
                        $.blockUI();
                        $('#NewWard').submit();
                    },
                    "Cancel": function () {
                        $(this).dialog("close");
                    }
                }
            });

            // Dialog Link
            $('[name=dialog_link]').click(function () {
                $('#dialog').dialog('open');
                return false;
            });
            //hover states on the static widgets
            $('#dialog_link, ul#icons li').hover(
					function () { $(this).addClass('ui-state-hover'); },
					function () { $(this).removeClass('ui-state-hover'); }
				);
        });
    </script>
    <script src="../../Scripts/jquery.blockUI.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Join/Create Ward
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>
        Join/Create A Ward</h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table>
        <tr valign="top">
            <td>
                <p class="error">
                    <%: ViewData["Error"] %></p>
                <p style="padding: 5px">
                    Please select a ward to join or create a new ward.</p>
                <br />
                <% var DropDowns = new MSW.Models.DropDowns(); %>
                <% DropDowns.generateSupportedWardList(); %>
                <% using (Html.BeginForm("SelectWard", "Bishopric", FormMethod.Post))
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
                            Ward
                        </td>
                        <td>
                            Ward Password
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <%: Html.DropDownList("ChosenArea", DropDowns.getSupportedWardsList(), "Select Supported Location")%>
                        </td>
                        <td>
                            <%: Html.DropDownList("ChosenStake", "Select Supported Stake")%>
                        </td>
                        <td>
                            <%: Html.DropDownList("ChosenWard", "Select Supported Ward")%>
                        </td>
                        <td>
                            <%: Html.Password("password")%>
                        </td>
                        <td>
                            <input class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all"
                                id="submit" type="submit" value="Continue" disabled="disabled" />
                        </td>
                    </tr>
                </table>
                <% } %>
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
                <div id="dialog" title="Create New Ward">
                    <% using (Html.BeginForm("CreateNewWard", "Bishopric", FormMethod.Post, new { @id="NewWard" }))
                       { %>
                    <div class="editor-label">
                        Please Enter the Area of your Singles Ward (e.g. <b><i>Provo</i></b> 14th Stake 203rd Ward)
                    </div>
                    <div class="editor-field">
                        <%: Html.TextBox("Area") %>
                    </div>
                    <div class="editor-label">
                        Please Enter the Stake of your Singles Ward (e.g. Provo <b><i>14th</i></b> Stake 203rd Ward)
                    </div>
                    <div class="editor-field">
                        <%: Html.TextBox("Stake") %>
                    </div>
                    <div class="editor-label">
                        Please Enter the name of your Singles Ward (e.g. Provo 14th Stake <b><i>203rd</i></b> Ward)
                    </div>
                    <div class="editor-field">
                        <%: Html.TextBox("Ward") %>
                    </div>
                    <div class="editor-label">
                        Please Enter a password for your Singles Ward
                    </div>
                    <div class="editor-field">
                        <%: Html.Password("Password") %>
                    </div>
                    <% } %>
                    </div>
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
                                Ward Passwords can be retrieved from a member of the bishopric. This password is
                                set upon the creation of the ward.
                            </div>
                            <div class="clear">
                            </div>
                            <div class="lines-dotted-short">
                            </div>
                            <div class="left">
                                <span name="dialog_link" style="cursor: pointer">
                                    <img src="../../Content/admin/images/forms/icon_plus.gif" width="21" height="21"
                                        alt="" /></span></div>
                            <div class="right">
                                <span name="dialog_link" style="cursor: pointer">
                                    <h5>
                                        Create New Ward</h5>
                                </span>
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
