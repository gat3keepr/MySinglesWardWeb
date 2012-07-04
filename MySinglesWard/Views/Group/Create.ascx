<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MSW.Model.tGroup>" %>

    <% using (Html.BeginForm("Create", "Group", FormMethod.Post, new { @id="CreateGroup" }))
       { %>
        <%: Html.ValidationSummary(true)%>
            <div class="editor-label">
                Creating a group will allow you and your group leaders to send email and txt notifcations to ward wembers. Once a member has been set as a group leader, they will be able to access
                the "Send Notification" feature. Members can set their notifications in the preferences menu.  
            </div><br />

            <div class="editor-label">
                Group Name:
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Name)%>
                <%: Html.ValidationMessageFor(model => model.Name)%>
            </div>
            
            <div class="editor-label">
                Group Type:
            </div>
            <div class="editor-field">
                <%: Html.DropDownListFor(model => model.Type, ViewData["GroupList"] as IEnumerable<SelectListItem>, "Select Type", null)%>
                <%: Html.ValidationMessageFor(model => model.Type)%>
            </div>            
            
            <div class="editor-label">
                Group Leader:
            </div>
            <div class="editor-field">
                <%: Html.DropDownListFor(model => model.LeaderID, ViewData["NameList"] as IEnumerable<SelectListItem>, "Select Leader", null)%>
                <%: Html.ValidationMessageFor(model => model.LeaderID)%>
            </div>
            
            <div class="editor-label">
                Group Co-Leader:
            </div>
            <div class="editor-field">
                <%: Html.DropDownListFor(model => model.CoLeaderID, ViewData["NameList"] as IEnumerable<SelectListItem>, "Select Co-Leader", null)%>
                <%: Html.ValidationMessageFor(model => model.CoLeaderID)%>
            </div>

    <% } %>
    <script type="text/javascript">
        $(function () {
            $("#CreateGroup").unbind("click")
                .click(function () {
                   
                });
        });
    </script>
