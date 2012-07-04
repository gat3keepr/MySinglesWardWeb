<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MSW.Model.MemberModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    View Member
    <%: Model.memberSurvey.prefName + " " + Model.user.LastName %>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentTitle" runat="server">
    <h1>
        <%: Model.memberSurvey.prefName + " " + Model.user.LastName %></h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <table class="MemberText">
        <tr>
            <td class="leftColumn">
                <img class="MemberPic" src="/Photo/<%: Model.photo.FileName %>" alt="Profile Picture" />
            </td>
            <td class="rightColumn">
                <% if (User.IsInRole("StakePres"))
                   { %>
                <div class="display-label">
                    <strong>Residence: </strong>
                    <%: Model.memberSurvey.residence%></div>
                <div class="display-label">
                    <strong>Birthday: </strong>
                    <%: String.Format("{0:d/M/yyyy}", Model.memberSurvey.birthday)%></div>
                <% } %>
                <div class="display-label">
                    <strong>Email: </strong><a href="mailto:<%: Model.user.Email%>">
                        <%: Model.user.Email%></a></div>
                <div class="display-label">
                    <strong>Cell Phone:</strong> <a href="tel:+1<%: Model.memberSurvey.cellPhone %>">
                        <%: Model.memberSurvey.cellPhone%></a>
                </div>
                <% if (User.IsInRole("StakePres"))
                   { %>
                <div class="display-label">
                    <strong>Priesthood Office: </strong>
                    <%: Model.memberSurvey.priesthood%></div>
                <div class="display-label">
                    <strong>Mission Information: </strong>
                    <%: Model.memberSurvey.missionInformation%></div>
            </td>
        </tr>
        <tr>
            <td class="leftColumn">
                <div class="display-label">
                    <strong>Home Phone: </strong><a href="tel:+1<%: Model.memberSurvey.homePhone%>">
                        <%: Model.memberSurvey.homePhone%></a></div>
                <div class="display-label">
                    <strong>Home Address: </strong>
                    <%: Model.memberSurvey.homeAddress%></div>
                <% if (User.IsInRole("StakePres"))
                   { %>
                <div class="display-label">
                    <strong>Home Stake: </strong>
                    <%: Model.memberSurvey.homeWardStake%></div>
                <div class="display-label">
                    <strong>Home Bishop: </strong>
                    <%: Model.memberSurvey.homeBishop%></div>
                <div class="display-label">
                    <strong>Previous Bishops: </strong>
                    <%: Model.memberSurvey.prevBishops%></div>
                <% } %>
                <br />
                <div class="display-label">
                    <strong>Patriarchal Blessing: </strong>
                    <%: Model.memberSurvey.patriarchalBlessing %></div>
                <div class="display-label">
                    <strong>Endowed: </strong>
                    <%: Model.memberSurvey.endowed %></div>
                <div class="display-label">
                    <strong>Current Temple Rec: </strong>
                    <%: Model.memberSurvey.templeRecommend %></div>
                <div class="display-label">
                    <strong>Temple Rec. Exp:</strong>
                    <%:  Model.memberSurvey.templeExpDate%></div>
                <div class="display-label">
                    <strong>Temple Worker: </strong>
                    <%: Model.memberSurvey.templeWorker %></div>
                <div class="display-label">
                    <strong>Current Calling(s): </strong>
                    <br />
                    <%foreach (var calling in Model.Callings)
                      { %>
                    <%: calling.organization.Title + " - " + calling.calling.Title%><br />
                    <% } %>
                </div>
                <div class="display-label">
                    <strong>Past Callings: </strong>
                    <%: Model.memberSurvey.pastCallings%></div>
                <br />
                <div class="display-label">
                    <strong>Suggested Ward Activities: </strong>
                    <%: Model.memberSurvey.activities%></div>
            </td>
            <td class="rightColumn">
                <div class="display-label">
                    <strong>Staying in Ward: </strong>
                    <%: Model.memberSurvey.timeInWard%></div>
                <div class="display-label">
                    <strong>School Info: </strong>
                    <%: Model.memberSurvey.schoolInfo%></div>
                <div class="display-label">
                    <strong>Religion Class: </strong>
                    <%: Model.memberSurvey.religionClass%></div>
                <div class="display-label">
                    <strong>Employed: </strong>
                    <%: Model.memberSurvey.employed%>.
                    <%: Model.memberSurvey.occupation%></div>
                <div class="display-label">
                    <strong>Music Skill:</strong>
                    <%: Model.memberSurvey.musicSkill%></div>
                <div class="display-label">
                    <strong>Music Ability:</strong>
                    <%: Model.memberSurvey.musicTalent%></div>
                <div class="display-label">
                    <strong>Teaching Skills: </strong>
                    <%: Model.memberSurvey.teachSkill%></div>
                <div class="display-label">
                    <strong>Teaching Desire: </strong>
                    <%: Model.memberSurvey.teachDesire%></div>
                <div class="display-label">
                    <strong>Calling Area:</strong>
                    <%: Model.memberSurvey.callingPref%></div>
                <br />
                <div class="display-label">
                    <strong>Emergency Contact: </strong>
                    <%: Model.memberSurvey.emergContact + " " + Model.memberSurvey.emergPhone%></div>
                <div class="display-label">
                    <strong>Self Description:</strong>
                    <%: Model.memberSurvey.description%></div>
                <div class="display-label">
                    <strong>Interests & Hobbies:</strong>
                    <%: Model.memberSurvey.interests%></div>
                <% } %>
            </td>
        </tr>
    </table>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MenuScript" runat="server">
    <script type="text/javascript">
        MenuSelect("Tools", "StakeDirectory");
    </script>
</asp:Content>
