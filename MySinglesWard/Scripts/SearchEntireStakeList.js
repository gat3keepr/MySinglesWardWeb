$(document).ready(function () {

    /********************
    * AutoComplete (Entire Stake List)
    ********************/
    $("[type=searchStakeList]").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: "/Stake/searchStakeList",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: ({ name_startsWith: request.term }),
                success: function (members) {
                    $("[type=entireStakeMember]").hide();

                    response($.map(members, function (item) {
                        $("[name=" + item.Value + "stake]").show();
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
            $("[type=entireStakeMember]").hide();
            $("[name=" + ui.item.value + "stake]").show();
            $("[type=searchStakeList]").val(ui.item.label);
            return false;
        }
    });

    /********************
    * AutoComplete(Individual Ward Lists)
    ********************/
    /*$("[type=searchWardList]").autocomplete({
    source: function (request, response) {
    var wardID = $(this).attr("name");
    $.ajax({
    url: "/Stake/searchWardList",
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    data: ({ name_startsWith: request.term, WardID: wardID }),
    success: function (members) {
    $("[type=entireStakeMember]").hide();

    response($.map(members, function (item) {
    $("[name=" + item.Value + "stake]").show();
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
    var wardID = $(this).attr("name");
    $("[type=member" + wardID + "]").hide();
    $("[name=" + ui.item.value + wardID + ]").show();
    }
    });*/
});