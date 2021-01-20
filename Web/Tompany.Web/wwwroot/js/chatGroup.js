"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
var group = document.getElementById("groupName").innerHTML;

function updateScroll() {
    var element = document.getElementById("demo-chat-body");
    element.scrollTop = element.scrollHeight;
}

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    var user = document.getElementById("fromUser").textContent;

    connection.invoke("JoinGroup", user, group).catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});


connection.on("GroupReceiveNewMessage", function (user, image, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    let dateTime = new Date()
    let formattedDate =
        `${dateTime.getDate()}-${(dateTime.getMonth() + 1)}-${dateTime.getFullYear()} ${dateTime.getHours()}:${dateTime.getMinutes()}`;

    var li = document.createElement("li");

    li.classList.add("mar-btm");
    li.innerHTML = `<div class="media-left">
                                        <img src=${image} class="img-circle img-sm" alt="Profile Picture">
                                    </div>
                                    <div class="media-body pad-hor">
                                        <div class="speech">
                                            <a href="/Profile/${user}" class="media-heading">${user}</a>
                                            <p>${msg}</p>
                                            <p class="speech-time">
                                                <i class="fa fa-clock-o fa-fw"></i>${formattedDate}
                                            </p>
                                        </div>
                                    </div>`;
    document.getElementById("messagesList").appendChild(li);
    updateScroll();
});

connection.on("ReceiveMessageSender", function (user, image, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    let dateTime = new Date()
    let formattedDate =
        `${dateTime.getDate()}-${(dateTime.getMonth() + 1)}-${dateTime.getFullYear()} ${dateTime.getHours()}:${dateTime.getMinutes()}`;

    var li = document.createElement("li");

    li.classList.add("mar-btm");
    li.innerHTML = `<div class="media-right">
                                        <img src=${image} class="img-circle img-sm" alt="Profile Picture">
                                    </div>
                                    <div class="media-body pad-hor speech-right">
                                        <div class="speech">
                                            <a href="/Profile/${user}" class="media-heading">${user}</a>
                                            <p>${msg}</p>
                                            <p class="speech-time">
                                                <i class="fa fa-clock-o fa-fw"></i>${formattedDate}
                                            </p>
                                        </div>
                                    </div>`;
    document.getElementById("messagesList").appendChild(li);
    updateScroll();
});

connection.on("JoinedGroupMessage", function (user, image, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    let dateTime = new Date()
    let formattedDate =
        `${dateTime.getDate()}-${(dateTime.getMonth() + 1)}-${dateTime.getFullYear()} ${dateTime.getHours()}:${dateTime.getMinutes()}`;

    var li = document.createElement("li");

    li.classList.add("mar-btm");
    li.innerHTML = `<div class="media-left">
                                        <img src=${image} class="img-circle img-sm" alt="Profile Picture">
                                    </div>
                                    <div class="media-body pad-hor">
                                        <div class="speech" style="background-color:#c18cff">
                                            <a href="/Profile/${user}" class="media-heading">${user}</a>
                                            <p>${msg}</p>
                                            <p class="speech-time">
                                                <i class="fa fa-clock-o fa-fw"></i>${formattedDate}
                                            </p>
                                        </div>
                                    </div>`;
    document.getElementById("messagesList").appendChild(li);
    updateScroll();

});

connection.on("SendMessageToGroup", function (user, image, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    let dateTime = new Date()
    let formattedDate =
        `${dateTime.getDate()}-${(dateTime.getMonth() + 1)}-${dateTime.getFullYear()} ${dateTime.getHours()}:${dateTime.getMinutes()}`;

    var li = document.createElement("li");

    li.classList.add("mar-btm");
    li.innerHTML = `<div class="media-left">
                                        <img src=${image} class="img-circle img-sm" alt="Profile Picture">
                                    </div>
                                    <div class="media-body pad-hor">
                                        <div class="speech">
                                            <a href="/Profile/${user}" class="media-heading">${user}</a>
                                            <p>${msg}</p>
                                            <p class="speech-time">
                                                <i class="fa fa-clock-o fa-fw"></i>${formattedDate}
                                            </p>
                                        </div>
                                    </div>`;
    document.getElementById("messagesList").appendChild(li);
    updateScroll();
});


document.getElementById("sendButton").addEventListener("click", function (event) {
    var fromUser = document.getElementById("fromUser").innerHTML;
    var message = document.getElementById("messageInput").value;

    if (message) {

        connection.invoke("SendMessageToGroup", fromUser, message, group).catch(function (err) {
            return console.error(err.toString());
        });

        document.getElementById("messageInput").value = "";
    }
    event.preventDefault();
});


