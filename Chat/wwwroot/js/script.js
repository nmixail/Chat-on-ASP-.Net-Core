"use strict";

let messageButton = document.querySelector("button");
let textArea = document.querySelector("textarea");
let input = document.querySelector("input");

let socket = new WebSocket("wss://localhost:44371/ws/websocket/connect");

socket.onopen = function (e) {
    console.log("[open] Соединение установлено");
    /*console.log("Отправляем данные на сервер");*/
    /*socket.send("Меня зовут Джон");*/
};

socket.onmessage = function (event) {
    console.log(`[message] Данные получены с сервера: ${event.data}`);
    switch (event.data) {
        case "0": {
            console.log("[open] Соединение установлено от сервера");
            break;
        }
        case "1": {
            console.log("Готов к передаче данных");
            messageButton.disabled = "";
            break;
        }
        default :{
            textArea.value += event.data + "\n";
        }

    }
    /*textArea.value += event.data + "\n";*/
};

socket.onclose = function (event) {
    if (event.wasClean) {
        console.log(`[close] Соединение закрыто чисто, код=${event.code} причина=${event.reason}`);
    } else {
        // например, сервер убил процесс или сеть недоступна
        // обычно в этом случае event.code 1006
        console.log('[close] Соединение прервано');
    }
};

socket.onerror = function (error) {
    console.log(`[error] ${error.message}`);
};


messageButton.onclick = function () {
    socket.send(input.value);
    console.log(`Данные отправлены: ${input.value}`);
}