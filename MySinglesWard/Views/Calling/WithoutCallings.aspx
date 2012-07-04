<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<table>
    <% var members = ViewData["MembersList"] as IEnumerable<ListItem>;
       if(members.Count() > 0)
       {
           foreach (var member in members)
       { %>
    <tr>
        <td>
            <%: Html.ActionLink(member.Text, "GetMember", "Home", new { memberID = member.Value }, new { @target="_blank" }) %>
        </td>
    </tr>
    <% }
       } else
           { %>
           <tr>
        <td>
            Every member has a calling.
        </td>
    </tr>

    <%} %>
</table>
