<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MSW.CallingReports.Report>" %>
    
    <table>
        <tr>
            <td>
                <div id="members_w_calling">
                </div>
            </td>
            <td>
                <div id="calling_status">
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div id="callings_filled">
                </div>
            </td>
            <td>
                <div id="members_taken_survey">
                </div>
            </td>
        </tr>
    </table>
    
<script type="text/javascript" src="https://www.google.com/jsapi"></script>
<script type="text/javascript">
    google.load("visualization", "1", { packages: ["corechart"] });
    google.setOnLoadCallback(drawChart);
    function drawChart() {
        var data = new google.visualization.DataTable();
        data.addColumn('string', 'Member_Calling');
        data.addColumn('number', 'Percent');
        data.addRows(2);
        data.setValue(0, 0, 'With a Calling');
        data.setValue(0, 1, <%: Model.membersCalling %>);
        data.setValue(1, 0, 'Without a Calling');
        data.setValue(1, 1, <%: Model.membersWithoutCalling %>);

        var chart = new google.visualization.PieChart(document.getElementById('members_w_calling'));
        chart.draw(data, { width: 450, height: 300, title: 'Member Calling Status' });
    }
</script>
<script type="text/javascript">
    google.load("visualization", "1", { packages: ["corechart"] });
    google.setOnLoadCallback(drawChart);
    function drawChart() {
        var data = new google.visualization.DataTable();
        data.addColumn('string', 'Calling_Status');
        data.addColumn('number', 'Percent');
        data.addRows(5);
        data.setValue(0, 0, 'Recommended');
        data.setValue(0, 1, <%: Model.recommended %>);
        data.setValue(1, 0, 'Approved');
        data.setValue(1, 1, <%: Model.approved %>);
        data.setValue(2, 0, 'Called');
        data.setValue(2, 1, <%: Model.called %>);
        data.setValue(3, 0, 'Sustained');
        data.setValue(3, 1, <%: Model.sustained %>);
        data.setValue(4, 0, 'Set Apart');
        data.setValue(4, 1, <%: Model.setApart %>);


        var chart = new google.visualization.PieChart(document.getElementById('calling_status'));
        chart.draw(data, { width: 450, height: 300, title: 'Calling Status' });
    }
</script>
<script type="text/javascript">
    google.load("visualization", "1", { packages: ["corechart"] });
    google.setOnLoadCallback(drawChart);
    function drawChart() {
        var data = new google.visualization.DataTable();
        data.addColumn('string', 'callings_filled');
        data.addColumn('number', 'Percent');
        data.addRows(2);
        data.setValue(0, 0, 'Callings Filled');
        data.setValue(0, 1, <%: Model.callingsFilled %>);
        data.setValue(1, 0, 'Callings Empty');
        data.setValue(1, 1, <%: Model.callingsEmpty %>);

        var chart = new google.visualization.PieChart(document.getElementById('callings_filled'));
        chart.draw(data, { width: 450, height: 300, title: 'Callings Filled' });
    }
</script>
<script type="text/javascript">
    google.load("visualization", "1", { packages: ["corechart"] });
    google.setOnLoadCallback(drawChart);
    function drawChart() {
        var data = new google.visualization.DataTable();
        data.addColumn('string', 'members_taken_survey');
        data.addColumn('number', 'Percent');
        data.addRows(2);
        data.setValue(0, 0, 'Survey Complete');
        data.setValue(0, 1, <%: Model.surveyComplete %>);
        data.setValue(1, 0, 'Survey Incomplete');
        data.setValue(1, 1, <%: Model.surveyIncomplete %>);

        var chart = new google.visualization.PieChart(document.getElementById('members_taken_survey'));
        chart.draw(data, { width: 450, height: 300, title: 'Survey Status' });
    }
</script>