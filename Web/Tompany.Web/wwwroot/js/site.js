$(function () {
    //moment.locale("en");
    $("time").each(function (i, e) {
        const dateTimeValue = $(e).attr("datetime");
        if (!dateTimeValue) {
            return;
        }

        const time = moment.utc(dateTimeValue).local();
        $(e).html(time.format("llll"));
        $(e).attr("title", $(e).attr("datetime"));
    });
});

function getSearchResult() {
    let fromDestination = $("#FromDestinationId").val();
    let toDestination = $("#ToDestinationId").val();
    let dateOfDeparture = $("#DateOfDeparture").val();

    $.ajax({
        type: "GET",
        url: `/Search?`,
        contentType: "text/html; charset=utf-8",
        data: {
            FromDestinationId: fromDestination,
            ToDestinationId: toDestination,
            DateOfDeparture: dateOfDeparture,
        },
        success: function (res) {
            $("#searchResult").remove();
            $('#searchContainer').html(res);
        },
        error: function (res) {
            console.log(res);
        }
    });
}

function filterTrips() {
    let fromDestination = $("#FromDestinationId").val();
    let toDestination = $("#ToDestinationId").val();
    let dateOfDeparture = $("#DateOfDeparture").val();

    $.ajax({
        type: "GET",
        url: `/Trips/Search?`,
        contentType: "text/html; charset=utf-8",
        data: {
            FromDestinationId: fromDestination,
            ToDestinationId: toDestination,
            DateOfDeparture: dateOfDeparture,
        },
        success: function (res) {
            $("#searchResult").remove();
            $('#searchContainer').html(res);
        },
        error: function (res) {
            console.log(res);
        }
    });
}

function postSearchResult() {
    $.ajax({
        type: "POST",
        url: "/Search?"
    })
}