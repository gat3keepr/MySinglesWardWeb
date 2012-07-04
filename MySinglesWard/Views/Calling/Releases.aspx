<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<MSW.CallingReports.Organization>>" %>

<div id="Releases">
    <link href="../../Content/admin/css/callingReport.css" rel="stylesheet" type="text/css" />
    <% if (Model.Count() > 0)
       { %>
    <input class="ui-button ui-button-text-only ui-widget ui-state-active ui-corner-all"
        id="RestoreAll" value="Restore All" /><input class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all"
            id="ReleaseAll" value="Release All" />
    <% foreach (var org in Model)
       { %>
    <div style="padding: 10px">
        <h2>
            <%: org.Title %></h2>
        <table class="releaseTable">
            <% foreach (var calling in org.Releases)
               { %>
            <tr id="<%: calling.calling.CallingID %>">
                <td style="width: 230px">
                    <%: calling.calling.Title %>
                </td>
                <td style="width: 140px">
                    <% if (calling.member != null)
               { %>
                <%: calling.member.user.LastName + ", " + calling.member.memberSurvey.prefName%>
                <% } %>
                </td>
                <td style="width: 240px">
                    <input class="ui-button ui-button-text-only ui-widget ui-state-default ui-corner-all"
                        name="release" callingid="<%: calling.calling.CallingID %>" value="Release" />
                    <input class="ui-button ui-button-text-only ui-widget ui-state-active ui-corner-all"
                        name="restore" callingid="<%: calling.calling.CallingID %>" value="Restore" />
                    <img src="../../Content/images/loading.gif" name="pendingLoading" style="display: none;
                        width: 30px" />
                </td>
            </tr>
            <% } %>
        </table>
    </div>
    <% } %>
    <% }
       else
       {%>
    <div style="padding: 10px">
        <table class="reportTable">
            <tr>
                <td>
                    No Releases to report.
                </td>
            </tr>
        </table>
    </div>
    <% } %>
</div>
<script src="../../Scripts/jquery.blockUI.js" type="text/javascript"></script>
<script type="text/javascript">
    $('[name=release]').click(function () {
        var callingID = $(this).attr('callingid');

        release(callingID);

    });

    $('[name=restore]').click(function () {
        var callingID = $(this).attr('callingid');

        restore(callingID);
    });

    $('#ReleaseAll').click(function () {
        $('[name=restore]').hide();
        $('[name=release]').hide();
        $('[name=pendingLoading]').show();
        $('[name=release]').each(function (index) {
            var callingID = $(this).attr('callingid');
            release(callingID);
        });
        $('#Releases').html('<div style="padding: 10px"><table class="reportTable"><tr><td>No Releases to report.</td></tr></table></div>');
    });

    $('#RestoreAll').click(function () {
        $('[name=restore]').hide();
        $('[name=release]').hide();
        $('[name=pendingLoading]').show();
        $('[name=restore]').each(function (index) {
            var callingID = $(this).attr('callingid');
            restore(callingID);
        });
        $('#Releases').html('<div style="padding: 10px"><table class="reportTable"><tr><td>No Releases to report.</td></tr></table></div>');
    });

    function release(callingID) {
        $.ajax({
            url: '/Calling/ReleaseFinal',
            data: { callingID: callingID },
            async: false,
            success: function (data) {
                success(callingID);
            },
            error: function () {
                alert("Please try again later");
            }
        });
    }

    function restore(callingID) {
        $.ajax({
            url: '/Calling/ReleaseRestore',
            data: { callingID: callingID },
            async: false,
            success: function (data) {
                if (data == "True") {
                    success(callingID);
                }
                else {
                    alert("A new member has already been sustained in that calling.");
                }
            },
            error: function () {
                alert("Please try again later");
            }
        });
    }

    function success(callingID) {
        $('#' + callingID).hide();
    }

</script>
