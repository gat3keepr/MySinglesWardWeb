<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Resize
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="../../Scripts/jquery.Jcrop.css" rel="stylesheet" type="text/css" />
    <script src="../../Scripts/jquery.Jcrop.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            // a workaround for a flaw in the demo system (http://dev.jqueryui.com/ticket/4375), ignore!
            $("#dialog:ui-dialog").dialog("destroy");

            $("#dialog").dialog({
                autoOpen: true,
                height: 300,
                buttons: { "Ok": function () { $(this).dialog("close"); } }
            });


        });
	</script>
    <script type="text/javascript">


        // Remember to invoke within jQuery(window).load(...)
        // If you don't, Jcrop may not initialize properly
        jQuery(window).load(function () {

            jQuery('#cropbox').Jcrop({
                onChange: showPreview,
                onSelect: showPreview,
                aspectRatio: 1,
                onSelect: storeCoords
            });

        });

        // Our simple event handler, called from onChange and onSelect
        // event handlers, as per the Jcrop invocation above
        function showPreview(coords) {
            if (parseInt(coords.w) > 0) {
                var rx = 200 / coords.w;
                var ry = 200 / coords.h;

                jQuery('#preview').css({
                    width: Math.round(rx * <%: ViewData["Width"] %>) + 'px',
                    height: Math.round(ry * <%: ViewData["Height"] %>) + 'px',
                    marginLeft: '-' + Math.round(rx * coords.x) + 'px',
                    marginTop: '-' + Math.round(ry * coords.y) + 'px'
                });

            }
            

        }
        function storeCoords(c) {
            $('#cropButton').removeAttr('disabled');
            jQuery('#X').val(c.x);
            jQuery('#Y').val(c.y);
            jQuery('#W').val(c.w);
            jQuery('#H').val(c.h);
        };
 
    </script>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server"> 
   <table style="margin-left: 20px">
        <tr valign="top">
            <td>
        <div id="outer">
            <div class="jcExample">
                <div class="article">
                    <!-- This is the image we're attaching Jcrop to -->
                    <table cellpadding="0" cellspacing="0" border="0">
                        <tr>
                            <td>
                                <img src="/Photo/<%: ViewData["Image"] %>" id="cropbox" alt="" />
                            </td>
                            <td>
                                <div style="width: 200px; height: 200px; overflow: hidden; margin-left: 5px;">
                                    <img src="/Photo/<%: ViewData["Image"] %>" id="preview" alt="" />
                                </div>
                            </td>
                        </tr>
                    </table>
                    <% using (Html.BeginForm("StakeResize", "Photo", FormMethod.Post, new { enctype = "multipart/form-data" }))
                       {%>
                    <%: Html.ValidationSummary(true)%>
                    <%=Html.Hidden("X") %>
                    <%=Html.Hidden("Y") %>
                    <%=Html.Hidden("W") %>
                    <%=Html.Hidden("H") %>
                    <br />
                    <button id="cropButton" disabled="disabled" name="button" class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all">
                                <span class="ui-button-text">Crop Photo</span></button>
                    <% } %>
                </div>
            </div>
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
                                Use cursor to select a portion of the picture to use for your profile and the stake list.
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
    <div id="dialog" class="ui-dialog" title="Crop Your Photo">
	<p>Thanks for uploading your photo. Please use this crop tool to select a portion of the picture to use on the ward list.
    If you do not crop the picture, it will not appear on the ward list.</p>
    </div>
</asp:Content>
