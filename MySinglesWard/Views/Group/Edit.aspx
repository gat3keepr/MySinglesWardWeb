<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MSW.Models.Group>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Edit Group -
    <%: Model.Name %>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>
        Edit Group</h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% Html.RenderPartial("EditForm", Model); %>
    <script type="text/javascript">
        /********************
        * Remove Member from Group
        ********************/
        $("[id=remove]").click(function () {
            var memberID = $(this).attr("memberid");
            var groupID = $("#groupID").val();
            SendRemoveRequest(memberID, groupID);
        });

        function SendRemoveRequest(memberID, groupID) {
            $.ajax({
                url: '/Group/RemoveMember',
                data: ({ groupid: groupID, memberid: memberID }),
                success: function (removed) {
                    if (removed == "True") {
                        $('#' + memberID + "X").hide();
                    }
                    else {
                        alert("You are not authorized to remove members from this group.");
                    }

                },
                error: function (removed) {
                    alert("You are not authorized to remove members");
                }
            });
        }
        
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MenuScript" runat="server">
    <script type="text/javascript">
        MenuSelect("Tools", "Groups");
    </script>
</asp:Content>