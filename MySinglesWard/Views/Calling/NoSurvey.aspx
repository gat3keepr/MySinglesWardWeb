<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<table>
    <% var members = ViewData["MembersList"] as IEnumerable<ListItem>;
       if(members.Count() > 0)
       {
           foreach (var member in members)
       { %>
    <tr>
        <td>
            <%: member.Text %>
        </td>
    </tr>
    <% }
       } else
           { %>
           <tr>
        <td>
            Every member has taken the survey.
        </td>
    </tr>

    <%} %>
</table>
