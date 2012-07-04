<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Residences
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>
        Residences</h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table>
        <tr valign="top">
            <td>
                <table border="0" width="100%" cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <div style='padding-left: 25px'>
                                <table>
                                    <tr>
                                        <td>
                                            <h3>
                                                Residences in Ward</h3>
                                        </td>
                                        <td>
                                            <span name="dialog_link" style="cursor: pointer">
                                                <img src="../../Content/admin/images/forms/icon_plus.gif" width="21" style='padding: 0px 0px 8px 7px'
                                                    alt="" /></span>
                                        </td>
                                    </tr>
                                </table>
                                <p class="error">
                                    <%: ViewData["Error"] %></p>
                                <div style="width: 220px">
                                    <ul class="residenceList" id="sortable">
                                        <% foreach (var item in ViewData["ResidenceList"] as IEnumerable<MSW.Models.dbo.Residence>)
                                           { %>
                                        <li id="<%: item.id %>" class="ui-state-default" style="margin-bottom:3px">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <span class="ui-icon ui-icon-arrowthick-2-n-s" name="sort"></span>
                                                    </td>
                                                    <td class="residenceName">
                                                        <%: item.residence %>
                                                        <%: Html.Hidden(item.id + "ADDRESS", item.streetAddress) %>
                                                    </td>
                                                    <td>
                                                        <div style="margin: 0 0 0 13px; width: 100px">
                                                            <span name="address" title="Street Address" value="<%: item.id %>" class="icon-7 info-tooltip">
                                                    </span>
                                                            <span name="remove" title="Remove" value="<%: item.id %>" class="icon-2 info-tooltip">
                                                            </span>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </li>
                                        <% } %>
                                    </ul>
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
                <!-- end Residence-form  -->
            </td>
            <td>
                <!--  start related-activities -->
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
                                Adding a residence will add that residence to the survey options for your ward.
                                Members will then be able to choose from those options and sorting by residence
                                will be more effective.
                                <br />
                                <br />
                                The apartments will appear in the order they are in the list. You can sort residences by
                                selecting "Sort Residences" below and dragging the residences to the desired order. 
                                Save your changes by selecting the "Sorting Residences" button.
                            </div>
                            <div class="clear">
                            </div>
                            <div class="lines-dotted-short">
                            </div>
                            <div class="left">
                                <a href="" name="dialog_link">
                                    <img src="../../Content/admin/images/forms/icon_plus.gif" width="21" height="21"
                                        alt="" /></a></div>
                            <div class="right">
                                <h5 name="dialog_link" style="cursor: pointer">
                                    Add a Residence</h5>
                            </div>
                            <div class="clear">
                            </div>
                            <div class="lines-dotted-short">
                            </div>
                            <div class="left">
                                <span name="sortResidences" style="cursor: pointer">
                                    <img src="../../Content/admin/images/forms/icon_edit.gif" width="21" height="21"
                                        alt="" /></span></div>
                            <div class="right">
                                <span name="sortResidences" style="cursor: pointer">
                                    <h5 id="sortingTitle">
                                        Sort Residences</h5>
                                    <img src="../../Content/images/loading.gif" style="display: none; width: 30px; position: absolute;
                                        margin: -23px 0px 0px 125px" id="sorting" />
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
                <!-- end related-activities -->
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
    <div class="clear">
    </div>
    <div id="dialog" title="Add a Residence">
        <table>
            <tr>
                <td colspan="2">
                    Enter in a new residence:
                    <%: Html.TextBox("residence")%>
                </td>
            </tr>
        </table>
    </div>
    <div id="streetAddress" title="Street Address">
        <table>
            <tr>
                <td colspan="2">
                <div class="editor-label">
                    Enter the street address for this residence. This address will be used on the MLS Record Request Tool. 
                    <strong>Do not include City, State, or Zipcode.</strong> They will be indicated on the MLS Record Request tool.
                    </div>
                    <div class="editor-field">
                    Street Address:<br />
                    <%: Html.TextBox("streetAddressField", null, new { @style="width: 250px"})%>
                    <%: Html.Hidden("streetAddressID")%>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    <!-- Cloneable Residence Field -->
    <div style="display: none">
        <li id="residenceClone" class="ui-state-default" style="margin-bottom:3px">
            <table>
                <tr>
                    <td>
                        <span class="ui-icon ui-icon-arrowthick-2-n-s" name="sort"></span>
                    </td>
                    <td class="residenceName" name="newResidence">
                    </td>
                    <td>
                        <div style="margin: 0 0 0 13px; width: 100px">
                            <span name="newAddress" title="Street Address" value="" class="icon-7 info-tooltip">
                                                    </span>
                            <span name="newRemove" title="Remove" class="icon-2 info-tooltip"></span>
                        </div>
                    </td>
                </tr>
            </table>
        </li>
    </div>
    <script type="text/javascript">
        //Sort information
        var sorting = false;

        function AddResidence() {
            var newResidence = $("#residence").val();
            $.ajax({
                url: "/Bishopric/AddResidence",
                data: ({ residence: newResidence }),
                success: function (residenceID) {

                    var residence = $("#residenceClone");

                    //Sets the Name on the residence
                    $(residence).find('[name=newResidence]').html(newResidence + 
                        "<input type=\"hidden\" name=\"" + residenceID + "ADDRESS\" value=\"\">");

                    
                    //Sets the Remove Event
                    $('[name=newRemove]').attr('onClick', 'remove(' + residenceID + ')');
                    $('[name=newAddress]').attr('onClick', 'openStreetAddress(' + residenceID + ')');

                    residence = $("#residenceClone").clone();
                    $(residence).attr('id', residenceID);
                    $(residence).find('[name=newResidence]').attr('name', '');
                    $(residence).find('[name=newAddress]').attr('name', '');
                    $(residence).find('[name=newRemove]').attr('name', '');

                    $('#sortable').append(residence);
                    $('#sortable').sortable();
                    $('#sortable').sortable('disable');
                }
            });

            $("#residence").val('');
        }

        $("[name=remove]").click(function () {
            var residenceID = $(this).attr("value");
            remove(residenceID);
        });

        function remove(residenceID) {
            $.ajax({
                url: "/Bishopric/RemoveResidence",
                data: ({ id: residenceID }),
                success: function (residence) {
                    $("[id=" + residenceID + "]").hide();
                }
            });

        }

        //New Residence DIALOG
        $("#dialog").dialog({
            autoOpen: false,
            height: 175,
            width: 700,
            modal: false,
            buttons: {
                "Add Residence": function () {
                    AddResidence();
                    $(this).dialog("close");
                },
                "Cancel": function () {
                    $(this).dialog("close");
                }
            }
        });

        $('[name=dialog_link]').click(function () {
            if (sorting == true) {
                alert("Please finish sorting before adding a residence");
            }
            else {
                $('#dialog').dialog('open');
            }
            return false;
        });

        //Street Address DIALOG
        $("#streetAddress").dialog({
            autoOpen: false,
            height: 300,
            width: 700,
            modal: false,
            buttons: {
                "Save": function () {
                    var residenceID = $('#streetAddressID').val();
                    var address = $('#streetAddressField').val();
                    $('[name=' + residenceID + 'ADDRESS]').val(address);

                    if (address != "") {
                        $.ajax({
                            url: "/Bishopric/StreetAddress",
                            data: ({ residenceID: residenceID, address: address }),
                            error: function () {
                                alert("Please try again later");
                            }
                        });

                        $(this).dialog("close");
                    }
                    else {
                        alert("Please Type a Street Address");
                        $('#streetAddressField').focus();
                    }
                },
                "Cancel": function () {
                    $('#streetAddressID').val("");
                    $('#streetAddressField').val("");
                    $(this).dialog("close");
                }
            }
        });

        $('[name=address]').click(function () {
            var residenceID = $(this).attr('value');
            openStreetAddress(residenceID);
            
            return false;
        });

        function openStreetAddress(residenceID) {
            var streetAddress = $('[name=' + residenceID + 'ADDRESS]').val();
            $('#streetAddressField').val(streetAddress);
            $('#streetAddressID').val(residenceID);

            $('#streetAddress').dialog('open');
        }

        $('#sortable').sortable();
        $('#sortable').sortable('disable');
        $('[name=sortResidences]').click(function(){
            if(sorting == false)
            {
                sorting = true;

                //Show Loading Icon and change Text
                $('#sorting').show();
                $('#sortingTitle').attr('style', 'color:#36B7F4');
                $('#sortingTitle').html('Sorting Residences');

                $('#sortable').sortable('enable');
                $('#sortable').disableSelection();
            }
            else {
                sorting = false;
                //Reset Sorting selection
                $('#sorting').hide();
                $('#sortingTitle').attr('style', '');
                $('#sortingTitle').html('Sort Residences');

                var result = $('#sortable').sortable('toArray');

                $.ajax({
                    url: '/Bishopric/SortResidences',
                    dataType: 'json',
                    type: 'POST',
                    traditional: true,
                    data: { result: result }
                });

                $('#sortable').sortable('disable');
            }
        });
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MenuScript" runat="server">
    <script type="text/javascript">
        MenuSelect("Ward", "UpdateResidences");
    </script>
</asp:Content>
