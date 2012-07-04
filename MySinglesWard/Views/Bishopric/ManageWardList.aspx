<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<MSW.Model.MemberModel>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Manage Ward List
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
 <h1>Manage Ward List</h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   <div id="table-content">
                    <p style="color:Red"><%: TempData["Error"] %></p>
                        <!--  start message-blue -->
                        <div id="message-blue">
                            <table border="0" width="100%" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td class="blue-left">
                                        Select the checkbox next to a member to signify that their records have been requested from MLS.
                                    </td>
                                    <td class="blue-right">
                                        <a class="close-blue">
                                            <img src="../../Content/admin/images/table/icon_close_blue.gif" alt="" /></a>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <!--  end message-blue -->
                        <!--  start Member-table ..................................................................................... -->
                        <table border="0" width="100%" cellpadding="0" cellspacing="0" id="product-table">
                            <tr>
                                <th class="table-header-check table-header-repeat" >
                                   <a href="JavaScript:void()">MLS</a>
                                </th>
                                <th class="table-header-repeat line-left minwidth-1">
                                </th>
                                <th class="table-header-repeat line-left minwidth-1">
                                    <a href="JavaScript:void()">Name</a>
                                </th>
                                <th class="table-header-repeat line-left">
                                    <a href="JavaScript:void()">Apartment</a>
                                </th>
                                <th class="table-header-repeat line-left">
                                    <a href="JavaScript:void()">Current Calling</a>
                                </th>
                                <th class="table-header-options line-left">
                                    <a href="JavaScript:void()">Options</a>
                                </th>
                            </tr>
                            <%      var dropdown = new MSW.Models.DropDowns();
                                if(User.IsInRole("Clerk"))
                                {
                                    dropdown.generateClerkFriendlyRolesList();
                                }
                                else
                                    dropdown.generateRolesList();%>

                            <% foreach (var item in Model)
                               { %>
                            <tr id="<%: item.user.MemberID %>">
                                <td>
                                    <%: Html.CheckBox("RecordsRequested", item.user.RecordsRequested, new { @memberid = item.user.MemberID })%>
                                </td>
                                <td>
                                    <% ViewData["filename"] = item.photo.FileName; %>
                                    <img class="smallPic" src="/Photo/<%: ViewData["filename"] %>" alt="Profile Picture" />
                                </td>
                                <td>
                                    <%: Html.TextBox("prefName", item.memberSurvey.prefName, new { @prefName = item.user.MemberID, @style = "width:120px" })%>
                                    <%: Html.TextBox("lastName", item.user.LastName, new { @lastName = item.user.MemberID, @style = "width:120px" })%>
                                </td>
                                <td>
                                    <%: Html.TextBox("apartment", item.memberSurvey.residence, new { @apartment = item.user.MemberID })%>
                                    <%: Html.DropDownList("apartmentDD", ViewData["ApartmentList"] as IEnumerable<SelectListItem>, "Select a Residence", new { @memberid = item.user.MemberID })%>
                                </td>
                                <td>
									<% foreach (var calling in item.Callings)
									{ %>
                                   <%: calling.organization.Title%> - <%: calling.calling.Title%><br />       
								   <% } %>                          
                                </td>
                                <td class="options-width">
                                    <span name="update" title="Update" memberid="<%: item.user.MemberID %>" class="icon-5 info-tooltip"></span>&nbsp;
                                    <span name="upload" title="Upload Picture" memberid="<%: item.user.MemberID %>" class="icon-6 info-tooltip"></span>
                                    <span name="info" title="Member Info" memberid="<%: item.user.MemberID %>" class="icon-7 info-tooltip"></span>
                                    <% if (int.Parse(Session["MemberID"] as string) != item.user.MemberID)
                                       { %>
                                    <span name="remove" title="Remove From Ward" memberid="<%: item.user.MemberID %>" class="icon-2 info-tooltip"></span>
                                    <% } %>
                                     <%: Html.Hidden("cellphone", item.memberSurvey.cellPhone)%>
                                        <%: Html.Hidden("email", item.user.Email)%> 
										<%: Html.Hidden("birthday", String.Format("{0:d/M/yyyy}", item.memberSurvey.birthday))%> 
                                </td>
                            </tr>
                            <% } %>
                        </table>
                        </div>
                        <!--  end Member-table................................... -->
                    
    <div id="dialog" title="Upload Picture">
         <% using (Html.BeginForm("UploadPictures", "Photo",FormMethod.Post, new { @enctype = "multipart/form-data", @id = "UploadPicture" }))
            { %>
                    <table>
                        <tr>
                            <td>
                                Choose a picture file to upload for this member:&nbsp;
                            </td>
                            <td>
                                <input type="file" id="picture" name="picture" />                               
                                <%: Html.Hidden("uploadID")%>
                            </td>
                        </tr>
                    </table>
         <% } %>
    </div>

    <div id="memberInfo" title="Member Info">
        <table class="memberInfo">
            <tr>
                <td>
                    Name 
                </td>
                <td>
                    <p id="memberName"></p>
                </td>
            </tr>
            <tr>
                <td>
                    Cell Phone: 
                </td>
                <td>
                    <p id="memberCellPhone"></p>
                </td>
            </tr>
            <tr>
                <td>
                    Email:
                </td>
                <td>
                    <p id="memberEmail"></p>
                </td>
            </tr>
			<tr>
                <td>
                    Birthday:
                </td>
                <td>
                    <p id="memberBirthday"></p>
                </td>
            </tr>
        </table>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $("[type=checkbox]").change(function () {
                var memberID = $(this).attr("memberid");
                var records = $(this).attr("checked");
                var checkbox = $(this);

                $.ajax({
                    url: '/Bishopric/RequestRecord',
                    data: { id: memberID, records: records },
                    success: function (data) {
                        $('#' + memberID).animate({ backgroundColor: "#36B7F4" }, 'slow');
                        $('#' + memberID).animate({ backgroundColor: "#FFFFFF" }, 'slow');
                    },
                    error: function () {
                        alert("Please try again later");
                    }
                });

            });

            $("[name=update]").click(function () {
                var memberID = $(this).attr("memberid");
                var prefName = $("[prefName=" + memberID + "]").val();
                var lastName = $("[lastName=" + memberID + "]").val();
                var apartment = $("[apartment=" + memberID + "]").val();

                $.ajax({
                    url: '/Bishopric/UpdateMember',
                    data: { id: memberID, prefName: prefName, lastName: lastName, apartment: apartment },
                    success: function (data) {
                        $('#' + memberID).animate({ backgroundColor: "#36B7F4" }, 'slow');
                        $('#' + memberID).animate({ backgroundColor: "#FFFFFF" }, 'slow');
                    },
                    error: function () {
                        alert("Please try again later");
                    }
                });
            });

            $("[name=remove]").click(function () {
                var memberID = $(this).attr("memberid");
                var prefName = $("[prefName=" + memberID + "]").val();
                var lastName = $("[lastName=" + memberID + "]").val();
                
                if (confirm('Are you sure you want to remove ' + prefName + ' ' + lastName +' from the ward?')) {
                    $.ajax({
                        url: '/Bishopric/RemoveMember',
                        data: { id: memberID },
                        success: function (data) {
                            $('#' + memberID).fadeOut();
                        },
                        error: function () {
                            alert("Please try again later");
                        }
                    });
                }
            });

            $("[name=apartmentDD]").change(function () {
                var memberID = $(this).attr("memberid");
                $("[apartment=" + memberID + "]").val($(this).val());
            });


        });

        //Upload Pictures
        $("[name=upload]").click(function () {
            $('#dialog').dialog('open');
            $('#uploadID').val($(this).attr('memberid'));
            return false;
        });

        // a workaround for a flaw in the demo system (http://dev.jqueryui.com/ticket/4375), ignore!
        $("#dialog:ui-dialog").dialog("destroy");

        $("#dialog").dialog({
            autoOpen: false,
            height: 175,
            width: 700,
            modal: false,
            buttons: {
                "Upload": function () {
                    $('#UploadPicture').submit();
                },
                "Cancel": function () {
                    $(this).dialog("close");
                }
            }
        });

        //Change Authentications
        $("[name=NewRole]").change(function () {
            var id = $(this).siblings("[name=id]").val();
            var oldRole = $("[name=OldRole" + id + "]").val();
            var newRole = $(this).val();

            $.ajax({
                url: '/Bishopric/RequestAuthentication',
                data: { id: id, NewRole: newRole, OldRole: oldRole },
                success: function (data) {
                    $('#' + id).animate({ backgroundColor: "#36B7F4" }, 'slow');
                    $('#' + id).animate({ backgroundColor: "#FFFFFF" }, 'slow');
                    $(this).find("option:first-child").attr("disabled", "disabled");
                    $("[name=OldRole" + id + "]").val(newRole);
                },
                error: function (error) {
                    alert("Please try again later");
                }
            });
        });

        //Member Info
        $("#memberInfo").dialog({
            autoOpen: false,
            height: 175,
            width: 700,
            modal: false
        });

        $("[name=info]").click(function () {
            var memberID = $(this).attr("memberid");
            var prefName = $("[prefName=" + memberID + "]").val();
            var lastName = $("[lastName=" + memberID + "]").val();
            var cellphone = $(this).siblings("[name=cellphone]").val();
            var email = $(this).siblings("[name=email]").val();
            var birthday = $(this).siblings("[name=birthday]").val();

            $('#memberName').html(prefName + " " + lastName);
            $('#memberCellPhone').html('<a href=\"tel:+1' + cellphone + '\" class=\"link\">' + cellphone + '</a>');
            $('#memberEmail').html('<a href=\"mailto:' + email + '\" _target=\"_blank\" class=\"link\">' + email + '<\a>');
            $('#memberBirthday').html(birthday);
            $('#memberInfo').dialog('open');
            return false;
        });
        
    </script>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MenuScript" runat="server">
    <script type="text/javascript">
        MenuSelect("Ward", "ManageWardList");
    </script>
</asp:Content>