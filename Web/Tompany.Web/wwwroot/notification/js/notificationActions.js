function updateStatus(newStatus, id) {
    let colors = {
        "Read": "green",
        "Unread": "red",
        "Pinned": "blue"
    };

    $.ajax({
        type: "POST",
        url: `/Notifications/EditStatus`,
        data: {
            'newStatus': newStatus,
            'notificationId': id
        },
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        success: function (data) {
            if (data) {
                document.getElementById(`${id}orderStatus`).innerText = newStatus;
                document.getElementById(`${id}orderStatus`).style.color = colors[newStatus];
            }
        },
        error: function (msg) {
            console.error(msg);
        }
    });
}

function deleteNotification(id) {
    $.ajax({
        type: "POST",
        url: `/Notifications/DeleteNotification`,
        data: {
            'notificationId': id
        },
        headers: {
            RequestVerificationToken:
                $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        success: function (data) {
            if (data) {
                let notification = document.getElementById(id);
                document.getElementById(`${username}Notifications`).removeChild(notification);
            }
        },
        error: function (msg) {
            console.error(msg);
        }
    });
}