$(document).ready(function () {

    /********************
    * AutoComplete for Ward List
    ********************/
    $("#searchWardList").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Home/getMemberNames",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: ({ name_startsWith: request.term }),
                success: function (members) {
                    $("[type=member]").hide();

                    response($.map(members, function (item) {
                        $("[name=" + item.Value + "]").show();
                        return {
                            label: item.Text,
                            value: item.Value
                        }
                    }));
                }
            });

        },
        minLength: 0,
        select: function (event, ui) {
            $("[type=member]").hide();
            $("[name=" + ui.item.value + "]").show();
            $("#searchWardList").val(ui.item.label);
            return false;
        }
    });

    /********************
    * AutoComplete for Organization List
    ********************/
    $("#searchOrgList").autocomplete({
        source: function (request, response) {
            var orgID = $(this).attr("id");
            $.ajax({
                url: "/Organization/getMemberNames",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: ({ name_startsWith: request.term, orgID: orgID }),
                success: function (members) {
                    $("[type=member]").hide();

                    response($.map(members, function (item) {
                        $("[name=" + item.Value + "]").show();
                        return {
                            label: item.Text,
                            value: item.Value
                        }
                    }));
                }
            });

        },
        minLength: 0,
        select: function (event, ui) {
            $("[type=member]").hide();
            $("[name=" + ui.item.value + "]").show();
            $("#searchOrgList").val(ui.item.label);
            return false;
        }
    });


});