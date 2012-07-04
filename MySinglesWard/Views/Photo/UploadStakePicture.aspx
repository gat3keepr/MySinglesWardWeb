<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Upload Picture
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table style="margin-left: 20px">
        <tr valign="top">
            <td>
                <% using (Html.BeginForm("UploadStakePicture", "Photo", FormMethod.Post, new { enctype = "multipart/form-data" }))
                   {%>
                <%: Html.ValidationSummary(true)%>
                <p class="error">
                    <%: ViewData["Error"]%></p>
                
                <p>
                    <input class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all"
                        type="file" id="picture" name="picture" />
                    <input class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all"
                        type="submit" name="btnAdd" value="Upload" />
                </p>
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
                                Choose a picture file from your computer. This picture will be on the stake list
                    so please use a photo that will help other stake leadership identify you. If you do
                    not want your picture on the stake list, do not upload a picture.
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
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>
        Upload Picture</h1>
</asp:Content>
