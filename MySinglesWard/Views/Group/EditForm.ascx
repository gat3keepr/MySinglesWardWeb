<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MSW.Models.Group>" %>

<p class="error">
        <%: ViewData["Error"] %></p>
    <%: Html.Hidden("groupID", Model.GroupID) %>
    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true) %>
    <div class="editor-label">
        <b>Group Name:</b>
    </div>
    <div class="editor-field">
        <!-- Figures out if the person can edit the group -->
        <%  bool editable = false;
            foreach (string role in Model.access)
            {
                if (Page.User.IsInRole(role) && role.IndexOf("?") == -1)
                {
                    editable = true;
                }
            }%>
        <% if (editable)
           {%>
        <%: Html.TextBoxFor(model => model.Name) %>
        <% }
           else
           { %>
        <%: Html.Label(Model.Name)%>
        <% } %>
        <%: Html.ValidationMessageFor(model => model.Name) %>
    </div>
    <div class="editor-label">
        <b>Group Type:</b>
    </div>
    <div class="editor-field">
        <% if (editable)
           {%>
        <%: Html.DropDownListFor(model => model.TypeID, ViewData["GroupList"] as IEnumerable<SelectListItem>, "Select Type", null)%>
        <% }
           else
           { %>
        <%: Html.Label(Model.Type)%>
        <% } %>
        <%: Html.ValidationMessageFor(model => model.Type) %>
    </div>
    <div class="editor-label">
        <b>Group Leader:</b>
    </div>
    <div class="editor-field">
        <% if (editable)
           {%>
        <%: Html.DropDownListFor(model => model.LeaderID, ViewData["NameList"] as IEnumerable<SelectListItem>, "Select Leader", null) %>
        <% }
           else
           { %>
        <%: Html.Label(Model.Leader)%>
        <% } %>
        <%: Html.ValidationMessageFor(model => model.LeaderID) %>
    </div>
    <div class="editor-label">
        <b>Group Co-Leader:</b>
    </div>
    <div class="editor-field">
        <% if (editable)
           {%>
        <%: Html.DropDownListFor(model => model.CoLeaderID, ViewData["NameList"] as IEnumerable<SelectListItem>, "Select Co-Leader", null) %>
        <% }
           else
           { %>
        <%: Html.Label(Model.CoLeader)%>
        <% } %>
        <%: Html.ValidationMessageFor(model => model.CoLeaderID) %>
    </div>
    <% if (editable)
       {%>
    <div class="editor-label">
        <input class="ui-state-default ui-corner-all" type="submit" value="Save" />
    </div>
    <% } %>
    <% } %>
    <br />
    <br />
    <div class="editor-label">
        <h2>
            Group Members</h2>
        <table style="width: 325px">
            <tr>
                <td>
                    <strong>Name</strong>
                </td>
                <td>
                </td>
            </tr>
            <% foreach (var member in Model.MemberList)
               { %>
            <tr id="<%: member.user.MemberID %>X">
                <td style="padding-bottom: 4px">
                    <%: member.user.LastName + ", " + member.memberSurvey.prefName %>
                </td>
                <td style="padding-left: 10px; padding-bottom: 4px">
                    <button id="remove" memberid="<%: member.user.MemberID %>" name="button" class="ui-state-default ui-corner-all">
                        <span class="ui-button-text">Remove</span></button>
                </td>
            </tr>
            <% } %>
        </table>
        <br />
        <div>
            <%: Html.ActionLink("Back to List", "List") %>
        </div>
    </div>