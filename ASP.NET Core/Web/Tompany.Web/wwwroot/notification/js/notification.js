"use strict"

let notificationConnection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

notificationConnection.start().then(function () {
    notificationConnection.invoke("GetUserNotificationCount").catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});

notificationConnection.on("ReceiveNotification", function (count) {
    document.getElementById("notificationCount").innerText = count;
});

notificationConnection.on("VisualizeNotification", function (notification) {
    let div = document.getElementById("allUserNotifications");

    if (div) {
        let newNotification = createNotification(notification);
        if (div.children.length % 5 == 0 && div.children.length > 0) {
            let lastNotification = div.lastElementChild;
            div.removeChild(lastNotification);
            document.getElementById("loadMoreNotifications").disabled = false;
        }
        if (div.children.length == 0) {
            div.appendChild(newNotification);
        } else {
            div.insertBefore(newNotification, div.childNodes[0]);
        }
    }
});