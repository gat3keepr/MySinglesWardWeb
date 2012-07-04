$(document).ready(function () {

    /********************
    * AutoComplete(Individual Ward Lists)
    ********************/
    $("[type=searchWardList]").autocomplete({
        source: function (request, response) {
            var wardID = $(this).attr("wardID");
            alert(wardID);
            $.ajax({
                url: "/Stake/searchWardList",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: ({ name_startsWith: request.term, WardID: wardID }),
                success: function (members) {
                    $("[type=member" + wardID + "]").hide();

                    response($.map(members, function (item) {
                        $("[name=" + item.Value + wardID + "]").show();
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
            var wardID = $(this).attr("wardID");
            $("[type=member" + wardID + "]").hide();
            $("[name=" + ui.item.value + wardID + "]").show();
        }
    });
});