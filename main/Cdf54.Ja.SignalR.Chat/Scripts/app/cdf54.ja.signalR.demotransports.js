/*
 * Author: José ALVAREZ
 * Date: 07/02/2015
 * Description: FOR DEMO ONLY
    You can use the following values for specifying the transport method:
        "webSockets"
        "foreverFrame"
        "serverSentEvents"
        "longPolling"
    Example:
    $.connection.hub.start()
    $.connection.hub.start({ transport: ['webSockets', 'longPolling'] })
* File: cdf54.ja.signalR.demotransports.js
 */

//#region reference path.
//Directives de référence https://msdn.microsoft.com/fr-fr/library/bb385682.aspx#Script
//Une directive reference permet à Visual Studio d'établir une relation entre le script vous modifiez actuellement et d'autres scripts.
//La directive reference vous permet d'inclure un fichier de script dans le contexte de script du fichier de script actuel.
//Cela permet à IntelliSense de référencer des fonctions définies extérieurement, des types et des champs lors de l'écriture de code.
//
/// <reference path="~/Scripts/app/cdf54.ja.signalR.chat.js" />
//#endregion

//#region Demo signalr transports.
function DemoTransports() {
    if (navigator.userAgent.match(/Firefox/g)) {
        $.connection.hub.start({ transport: 'webSockets' })
                .done(function () {
                    CDF54.JA.SIGNALR.CHAT.APP.StartDone();
                })
                .fail(function () {
                    CDF54.JA.SIGNALR.CHAT.APP.StartFail();
                });
    }
    if (navigator.userAgent.match(/Trident/g)) {
        $.connection.hub.start({ transport: 'foreverFrame' })
                .done(function () {
                    CDF54.JA.SIGNALR.CHAT.APP.StartDone();
                })
                .fail(function () {
                    CDF54.JA.SIGNALR.CHAT.APP.StartFail();
                });
    }
    if (navigator.userAgent.match(/Opera/g)) {
        $.connection.hub.start({ transport: 'serverSentEvents' })
                .done(function () {
                    CDF54.JA.SIGNALR.CHAT.APP.StartDone();
                })
                .fail(function () {
                    CDF54.JA.SIGNALR.CHAT.APP.StartFail();
                });
    }
    if (navigator.userAgent.match(/Chrome/g)) {
        $.connection.hub.start({ transport: 'longPolling' })
                .done(function () {
                    CDF54.JA.SIGNALR.CHAT.APP.StartDone();
                })
                .fail(function () {
                    CDF54.JA.SIGNALR.CHAT.APP.StartFail();
                });
    }
}
//#endregion
