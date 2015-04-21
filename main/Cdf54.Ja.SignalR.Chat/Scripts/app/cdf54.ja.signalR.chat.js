/*
 * Author: José ALVAREZ
 * Date: 07/02/2015
 * Description: Namespaces: APP, MISC, USER, MESSAGE, PUBLIC, PRIVATE
 * http://falola.developpez.com/tutoriels/javascript/namespace/
 * http://thomas.junghans.co.za/frontendengineering/javascript-module-pattern/downloads/Javascript-Module-Pattern.pdf
 * File: cdf54.ja.signalR.chat.js
 */

//#region reference path.
//Directives de référence https://msdn.microsoft.com/fr-fr/library/bb385682.aspx#Script
//Une directive reference permet à Visual Studio d'établir une relation entre le script vous modifiez actuellement et d'autres scripts.
//La directive reference vous permet d'inclure un fichier de script dans le contexte de script du fichier de script actuel.
//Cela permet à IntelliSense de référencer des fonctions définies extérieurement, des types et des champs lors de l'écriture de code.
//
/// <reference path="~/Scripts/app/cdf54.ja.signalR.demotransports.js" />
/// <reference path="~/Scripts/app/cdf54.ja.signalR.chat.namespace.js" />
/// <reference path="~/Scripts/app/cdf54.ja.utils.js" />
/// <reference path="~/Scripts/app/cdf54.ja.signalR.chat.helpers.js" />
/// <reference path="~/Scripts/jQuery.tmpl.js" />
/// <reference path="~/Scripts/jquery.cssemoticons.js" />
/// <reference path="~/Views/Chat/_ViewStart.cshtml" />
//#endregion

//#region $(document).ready(function () {};
$(function () {
    CDF54.JA.SIGNALR.CHAT.APP.Init();
    CDF54.JA.SIGNALR.CHAT.APP.Start();
});
//#endregion

//#region CDF54.JA.SIGNALR.CHAT.APP module.
CDF54.JA.SIGNALR.CHAT.APP = (function () {
    'use strict';
    //
    // Private members
    //
    function _ChatOrChatAdmin() {
        var p = window.location.pathname;
        var pathArray = window.location.pathname.split('/');
        return pathArray[pathArray.length - 1];
    };
    function _RegisterClientMethods() {
        CDF54.JA.SIGNALR.CHAT.MISC.registerClientMethods();
        CDF54.JA.SIGNALR.CHAT.USER.registerClientMethods();
        CDF54.JA.SIGNALR.CHAT.MESSAGE.registerClientMethods();
        aliasPRIVATE.registerClientMethods();
        aliasPUBLIC.registerClientMethods();
        CDF54.JA.SIGNALR.CHAT.ADMIN.registerClientMethods();
    };
    function _StartFail() {
        CDF54.JA.SIGNALR.CHAT.MISC.MyTrace('hub.start.fail: Could not Connect!');
        CDF54.JA.SIGNALR.CHAT.MISC.FillState('hub.start.fail: Could not Connect!');
    }
    //
    // Public members
    //
    return {
        //
        // Public properties
        //
        // Proxy property to reference the hub.
        ChatHubProxy: null,
        // Property to persist UserName.
        UserName: null,
        // Property to persist ConnectionId.
        ConnectionId: null,
        // Property to differentiate pages Chat and ChatAdmin
        ChatOrChatAdmin: _ChatOrChatAdmin(),
        //
        // Public methods
        //
        StartDone: function () {
            CDF54.JA.SIGNALR.CHAT.MISC.MyTrace(CDF54.JA.UTILS.StringFormat('hub.start.done: Connected, connection ID= {0}', $.connection.hub.id));
            CDF54.JA.SIGNALR.CHAT.APP.RegisterEvents();
            CDF54.JA.SIGNALR.CHAT.MISC.ShowHub();
            //CDF54.JA.SIGNALR.CHAT.MISC.FillState('Connected, connection ID=' + $.connection.hub.id + ' with ' + 'Transport = <strong>' + $.connection.hub.transport.name + '</strong>');
            CDF54.JA.SIGNALR.CHAT.MISC.FillState(CDF54.JA.UTILS.StringFormat("Connected, connection ID={0} with Transport = <strong>{1}</strong>",
                $.connection.hub.id,
                $.connection.hub.transport.name
                ));
        },
        RegisterEvents: function () {
            aliasPUBLIC.registerEvents();
            aliasPRIVATE.registerEvents();
        },
        Init: function () {
            CDF54.JA.SIGNALR.CHAT.MISC.MyTrace("Init");

            // Enable console logs if IsConsoleLoggingEnable is true in web.config
            $.connection.hub.logging = JSON.parse(String(IsConsoleLoggingEnable()).toLowerCase());

            // Proxy to reference the hub initialisation.
            this.ChatHubProxy = $.connection.chatHub;

            _RegisterClientMethods();

            // Load audio files.
            CDF54.JA.SIGNALR.CHAT.MISC.LoadHtml5NewUserAudioTag();
            CDF54.JA.SIGNALR.CHAT.MISC.LoadHtml5AudioTag();

            // For Chat page
            if (this.ChatOrChatAdmin == "Chat") {
                CDF54.JA.SIGNALR.CHAT.MISC.showAppPath("spanAppPath");
                CDF54.JA.SIGNALR.CHAT.MISC.showAppVersion("spanJsVersion");
            };

            // For Admin page
            if (this.ChatOrChatAdmin == "ChatAdmin") {
                $('#spanCompany').append(CDF54.company.name);
                $('#spanAddress').append(CDF54.company.address);
                $('#spanDepartment').append(CDF54.company.departement);

                $('#spanIsTraceEnable').append(IsTraceEnable());
                $('#spanIsDemo').append(IsDemo());
                $('#spanIsConsoleLoggingEnable').append(IsConsoleLoggingEnable());
                $('#spanAppPath').append(AppPath());

                $('#spanChat').append(CDF54.JA.UTILS.StringFormat("Author = {0} Version = {1}",
                    CDF54.JA.SIGNALR.CHAT.author,
                    CDF54.JA.SIGNALR.CHAT.version))
                $('#spanUtils').append(CDF54.JA.UTILS.StringFormat("Author = {0} Version = {1}",
                    CDF54.JA.UTILS.author,
                    CDF54.JA.UTILS.version))

                CDF54.JA.SIGNALR.CHAT.HELPERS.showJqueryVersion("spanJqueryVersion");
                CDF54.JA.SIGNALR.CHAT.HELPERS.showBootstrapVersion("spanBootStrapVersion");
                CDF54.JA.SIGNALR.CHAT.HELPERS.showSignalRVersion("spanSignalRVersion");
            };
        },
        Start: function () {
            CDF54.JA.SIGNALR.CHAT.MISC.MyTrace('Calling hub.start()');

            if (JSON.parse(String(IsDemo()).toLowerCase())) {
                DemoTransports();
            }
            else {
                // Async execution.
                $.connection.hub.start()
                    .done(function () {
                        CDF54.JA.SIGNALR.CHAT.APP.StartDone();
                    })
                    .fail(function () {
                        _StartFail();
                    });
            }
        },
    }
})();// CDF54.JA.SIGNALR.CHAT.APP module 
//#endregion
//#region CDF54.JA.SIGNALR.CHAT.MISC module.
CDF54.JA.SIGNALR.CHAT.MISC = (function () {
    'use strict';
    //
    // Public members
    //
    return {
        //
        // Client RPC
        //
        registerClientMethods: function () {
            this.MyTrace('Entring CDF54.JA.SIGNALR.CHAT.MISC function registerClientMethods');

            // Called to show server traces.
            CDF54.JA.SIGNALR.CHAT.APP.ChatHubProxy.client.showServerTrace = function (serverTraceMessage) {
                var data = {
                    text: Date(),
                    value: serverTraceMessage,
                };
                $('#new-divmessval-template').tmpl(data).appendTo($('#divServerTrace'))

                //$('#divServerTrace').append('<div class="message"><span>' + '[' + Date() + '] ' + '</span> : ' + serverTraceMessage + '</div>');
            };
            // Called to fill status bar
            CDF54.JA.SIGNALR.CHAT.APP.ChatHubProxy.client.fillState = CDF54.JA.SIGNALR.CHAT.MISC.FillState;
            // Called to fill server context panel
            CDF54.JA.SIGNALR.CHAT.APP.ChatHubProxy.client.showServerContext = function (serverContext) {
                if (JSON.parse(String(IsTraceEnable()).toLowerCase())) {
                    var datas = [
                        {
                            text: '[Context.ConnectionId]',
                            value: serverContext.ConnectionId,
                        },
                        {
                            text: '[Context.User.Identity.Name]',
                            value: serverContext.Name,
                        },
                        {
                            text: '[Context.QueryString["Transport"]',
                            value: serverContext.Transport,
                        },
                    ];
                    $('#new-divmessval-template').tmpl(datas).appendTo($('#divContext'))
                };
            };
        },// Client RPC
        //
        // Public methods
        //
        // get app name
        showAppPath: function (tag) {
            $('#' + tag).append(AppPath());
        },
        // get your app version 
        showAppVersion: function (tag) {
            $('#' + tag).append(CDF54.JA.SIGNALR.CHAT.version);
        },
        ToggleVisibilty: function () {
            $('#inputPrivateMessage').fadeToggle('slow');
        },
        ShowHub: function () {
            var hub = $.connection.hub;
            if (JSON.parse(String(IsTraceEnable()).toLowerCase())) {
                var datashub = [
                    {
                        text: '[hub.id]',
                        value: hub.id,
                    },
                    {
                        text: '[hub.transport.name]',
                        value: hub.transport.name,
                    },
                    {
                        text: '[hub.protocol]',
                        value: hub.protocol,
                    },
                    {
                        text: '[hub.host]',
                        value: hub.host,
                    },
                    {
                        text: '[hub.appRelativeUrl]',
                        value: hub.appRelativeUrl,
                    },
                ];
                $('#new-divmessval-template').tmpl(datashub).appendTo($('#divHub'))

                if (hub.transport.name == 'webSockets')
                    $('#new-divmessval-template').tmpl({ text: '[hub.socket.url] ', value: hub.socket.url, }).appendTo($('#divHub'))
                //$('#divHub').append('<div class="message"><span>' + '[socket.url] ' + '</span> : ' + hub.socket.url + '</div>');
                $('#new-divmessval-template').tmpl({ text: '=========================================================================================', value: '', }).appendTo($('#divHub'))

                //$('#divHub').append('<div class="message"><span>' + '========================================================================================= ' + '</span> : ' + '</div>');
                var datasnavigator = [
                    {
                        text: '[navigator.appName]',
                        value: navigator.appName,
                    },
                    {
                        text: '[navigator.product]',
                        value: navigator.product,
                    },
                    {
                        text: '[navigator.userAgent]',
                        value: navigator.userAgent,
                    },
                ];
                $('#new-divmessval-template').tmpl(datasnavigator).appendTo($('#divHub'))
            }
        },
        ShowClientTrace: function (clientTraceMessage) {
            $('#new-divmessval-template').tmpl({ text: Date(), value: clientTraceMessage, }).appendTo($('#divClientTrace'))
            //$('#divClientTrace').append('<div class="message"><span>' + '[' + Date() + '] ' + '</span> : ' + clientTraceMessage + '</div>');
        },
        // Local and RPC method.
        FillState: function (state) {
            $('#spanState').html("");
            $('#spanState').append(state);
        },
        MyTrace: function (message) {
            // If IsTraceEnable = true activate client side trace. 
            if (JSON.parse(String(IsTraceEnable()).toLowerCase())) {
                console.log(message);
                this.ShowClientTrace(message);
            }
        },
        // Html5 Audio tag 
        LoadHtml5NewUserAudioTag: function () {
            var appPath = AppPath();
            var dataaudio =
                {
                    id: "newUserAudio",
                    appPath: AppPath(),
                    filename: "Spittoon",
                };
            $('#new-audio-template').tmpl(dataaudio).appendTo($('body'))

        },
        // Html5 Audio tag 
        LoadHtml5AudioTag: function () {
            var appPath = AppPath();
            var dataaudio =
                {
                    id: "chatAudio",
                    appPath: AppPath(),
                    filename: "notify",
                };
            $('#new-audio-template').tmpl(dataaudio).appendTo($('body'))

        },// Public methods
    };
})();// CDF54.JA.SIGNALR.CHAT.MISC module.
//#endregion
//#region CDF54.JA.SIGNALR.CHAT.USER module.
CDF54.JA.SIGNALR.CHAT.USER = (function () {
    'use strict';
    //
    // Public members
    //
    return {
        //
        // Client RPC
        //
        registerClientMethods: function () {

            CDF54.JA.SIGNALR.CHAT.MISC.MyTrace('Entring CDF54.JA.SIGNALR.CHAT.USER function registerClientMethods');
            // Called when UsersCount change
            CDF54.JA.SIGNALR.CHAT.APP.ChatHubProxy.client.showOnlineUsers = function (count) {
                CDF54.JA.SIGNALR.CHAT.MISC.MyTrace(CDF54.JA.UTILS.StringFormat('Server calling showOnlineUsers: count={0} ',
                    count));
                $('#spanUsersCount').text(count);
            }
            // On User Connected
            CDF54.JA.SIGNALR.CHAT.APP.ChatHubProxy.client.onConnected = function (user, allUsers, messages, contentMessages) {
                CDF54.JA.SIGNALR.CHAT.MISC.MyTrace(CDF54.JA.UTILS.StringFormat('Server calling CHAT_APP.ChatHubProxy.client.onConnected: id={0} username={1}',
                    user.ConnectionId,
                    user.UserName));

                // Some CDF54.JA.SIGNALR.CHAT properties initialisation.
                CDF54.JA.SIGNALR.CHAT.ConnectionId = user.ConnectionId;
                CDF54.JA.SIGNALR.CHAT.UserName = user.UserName;

                // for Chat page
                if (CDF54.JA.SIGNALR.CHAT.APP.ChatOrChatAdmin == "Chat") {
                    if (document.getElementById("spanUser") != null)
                        $('#spanUser').html(user.UserName);
                    if (document.getElementById("spanDateConnection") != null)
                        $('#spanDateConnection').html(user.ConnectionDateTime);
                    if (document.getElementById("spanTransport") != null)
                        $("#spanTransport").html($.connection.hub.transport.name);
                };
                // for Admin page
                if (CDF54.JA.SIGNALR.CHAT.APP.ChatOrChatAdmin == "ChatAdmin") {
                    if (document.getElementById("spanUserName") != null)
                        $("#spanUserName").html(user.UserName);
                    if (document.getElementById("spanIdConnection") != null)
                        $("#spanIdConnection").html(user.ConnectionId);
                    if (document.getElementById("spanTransport") != null)
                        $("#spanTransport").html($.connection.hub.transport.name);
                    if (document.getElementById("spanProtocol") != null)
                        $("#spanProtocol").html($.connection.hub.protocol);
                }
                // Add All Users
                for (i = 0; i < allUsers.length; i++) {
                    CDF54.JA.SIGNALR.CHAT.USER.AddUser(allUsers[i]);
                }
                // Add Existing Messages
                var i = 0;
                for (i = 0; i < messages.length; i++) {
                    CDF54.JA.SIGNALR.CHAT.MESSAGE.AddMessage(messages[i], "PUBLIC", "");
                }
                // Add Existing ContentMessages
                var i = 0;
                for (i = 0; i < contentMessages.length; i++) {
                    CDF54.JA.SIGNALR.CHAT.MESSAGE.AddContentMessage(contentMessages[i]);
                }
            }
            // On new User Connected
            CDF54.JA.SIGNALR.CHAT.APP.ChatHubProxy.client.onNewUserConnected = function (newUser) {
                CDF54.JA.SIGNALR.CHAT.MISC.MyTrace('Server calling CHAT_APP.ChatHubProxy.client.onNewUserConnected');

                if (document.getElementById("newUserAudio") != null)
                    $('#newUserAudio')[0].play();
                CDF54.JA.SIGNALR.CHAT.USER.AddUser(newUser);
            }
            // On User Disconnected
            CDF54.JA.SIGNALR.CHAT.APP.ChatHubProxy.client.onUserDisconnected = function (disconnectedUser) {
                CDF54.JA.SIGNALR.CHAT.MISC.MyTrace(CDF54.JA.UTILS.StringFormat('Server calling CDF54.JA.SIGNALR.CHAT.ChatHubProxy.client.onUserDisconnected({0}, {1})',
                    disconnectedUser.ConnectionId,
                    disconnectedUser.UserName))

                if ($('#' + disconnectedUser.ConnectionId).length > 0) {
                    $('#' + disconnectedUser.ConnectionId).remove();
                    var disc = $('<div class="disconnect">"' + disconnectedUser.UserName + '" logged off.</div>');
                    $(disc).hide();
                    $('#divusers').prepend(disc);
                    $(disc).fadeIn(200).delay(2000).fadeOut(200);

                    if ($('#Private_Input_' + disconnectedUser.ConnectionId).length > 0) {
                        $('#Private_Input_' + disconnectedUser.ConnectionId).find('#btnClosePrivate_' + disconnectedUser.ConnectionId).click();
                    }
                }
            }
        },// Client RPC
        //
        // Private methods
        //
        AddUser: function (newUser) {
            // Retreive current user id
            var userId = CDF54.JA.SIGNALR.CHAT.ConnectionId;
            var code = "";
            var codeA = "";
            // json data for "code"
            var data = {
                connectionId:"",
                theClass: "alert alert-success",
                theStyle: "",
                theTitle: newUser.UserName,
                name: newUser.UserName,
                //imagePath: AppPath() + "/Content/Avatars/BlankPhoto.jpg",
                imagePath: newUser.PhotoUrl,
            }

            if (userId == newUser.ConnectionId) {
                //code = $('<div class="alert alert-success well-sm">' + newUser.UserName + "</div>");
                //code = $('<p class="bg-success">' + newUser.UserName + "</p>");
                //code = $('<p class="alert alert-success"><img src="http://www.gravatar.com/avatar/${hash}?s=16&d=mm" class="gravatar" /> ' + newUser.UserName + "</p>");
                code = $('#new-adduser-template').tmpl(data);
            }
            else {
                //code = $('<div id="' + newUser.ConnectionId + '" class="alert alert-info well-sm" style="cursor: pointer" title="Double click for private talk">' + newUser.UserName + '</div>');
                //code = $('<p id="' + newUser.ConnectionId + '" class="bg-info" style="cursor: pointer" title="Double click for private talk">' + newUser.UserName + '</p>');
                //code = $('<p id="' + newUser.ConnectionId + '" class="alert alert-info" style="cursor: pointer" title="Double click for private talk"><img src="http://www.gravatar.com/avatar/${hash}?s=16&d=mm" class="gravatar" /> ' + newUser.UserName + '</p>');
                data.connectionId = newUser.ConnectionId;
                data.theClass = "alert alert-info";
                // Not for ChatAdmin
                if (CDF54.JA.SIGNALR.CHAT.APP.ChatOrChatAdmin == "Chat") {
                    data.theStyle = "cursor: pointer";
                    data.theTitle = "Double click for private talk";
                    //data.imagePath = AppPath() + "/Content/Avatars/BlankPhoto.jpg";
                    data.imagePath = newUser.PhotoUrl;
                }
                code = $('#new-adduser-template').tmpl(data);
                $(code).dblclick(function () {
                    var id = $(this).attr('id');
                    if (userId != id) {
                        //alert('AddUser dblclick : ' + id + ' / ' + newUser.UserName);
                        CDF54.JA.SIGNALR.CHAT.MESSAGE.PRIVATE.OpenPrivateChatPanel(id, newUser.UserName);
                    }
                });
            }
            $("#divusers").append(code);

            // Using jQuery to scroll to the bottom of #panelUsers DIV.
            var height = $('#panelUsers')[0].scrollHeight;
            $('#panelUsers').scrollTop(height);
        },// Private methods
    };// Public members
})();// CDF54.JA.SIGNALR.CHAT.USER module.
//#endregion
//#region CDF54.JA.SIGNALR.CHAT.MESSAGE module.
CDF54.JA.SIGNALR.CHAT.MESSAGE = (function () {
    'use strict';
    //
    // Public members
    //
    return {
        registerClientMethods: function () {
            CDF54.JA.SIGNALR.CHAT.MISC.MyTrace('Entring CDF54.JA.SIGNALR.CHAT.MESSAGE function registerClientMethods');

            // Called by server to push content message for PUBLIC and PRIVATE messages.
            CDF54.JA.SIGNALR.CHAT.APP.ChatHubProxy.client.showContentMessageReceived = function (message) {
                CDF54.JA.SIGNALR.CHAT.MISC.MyTrace(CDF54.JA.UTILS.StringFormat('Server calling CDF54.JA.SIGNALR.CHAT.ChatHubProxy.client.showContentMessageReceived({0}, {1}, {2})',
                    message.UserName, message.MessageDateTime,
                    message.Message))
                CDF54.JA.SIGNALR.CHAT.MESSAGE.AddContentMessage(message);
            };
        },
        //
        // Common methods
        //
        // Add PUBLIC and PRIVATE content message.
        AddContentMessage: function (message) {
            var e = $('#' + message.Id).append(message.Message);
        },
        // Add PUBLIC and PRIVATE message.
        AddMessage: function (message, type, from) {
            //var encodedMessage = CDF54.JA.UTILS.EncodeString(message.Message);
            var encodedMessage = message.Message;
            var datasmessage =
                {
                    Id: message.Id,
                    MessageDateTime: message.MessageDateTime,
                    UserName: message.UserName,
                    encodedMessage: encodedMessage,
                    myClass: "",
                    imagePath: message.PhotoUrl,
                }
            switch (type) {
                case "PUBLIC":

                    if (document.getElementById("divChatMessages") != null) {
                        if (message.UserName == CDF54.JA.SIGNALR.CHAT.UserName) {
                            //$('#divChatMessages').append('<div class="alert alert-success well-sm"><span>' + '[' + message.MessageDateTime + '] ' + message.UserName + '</span> : ' + encodedMessage + '</div>');
                            //$('#divChatMessages').append('<p id=' + message.Id + ' class="text-success"><strong>' + '[' + message.MessageDateTime + '] ' + message.UserName + ' : </strong><i>' + encodedMessage + '</i></p>');
                            datasmessage.myClass = "alert alert-success";
                            $('#new-pmessage-template').tmpl(datasmessage).appendTo($('#divChatMessages'));
                            $('.alert').emoticonize();


                        }
                        else {
                            //$('#divChatMessages').append('<div class="alert alert-info well-sm"><span>' + '[' + message.MessageDateTime + '] ' + message.UserName + '</span> : ' + encodedMessage + '</div>');
                            //$('#divChatMessages').append('<p id=' + message.Id + ' class="text-info"><strong>' + '[' + message.MessageDateTime + '] ' + message.UserName + ' : </strong><i>' + encodedMessage + '</i></p>');
                            datasmessage.myClass = "alert alert-info";
                            $('#new-pmessage-template').tmpl(datasmessage).appendTo($('#divChatMessages'));
                            $('.alert').emoticonize();

                        }
                        // Using jQuery to scroll to the bottom of #panelChatMessages DIV.
                        var height = $('#divChatMessages')[0].scrollHeight;
                        $('#divChatMessages').scrollTop(height);
                        // beep
                        CDF54.JA.UTILS.beep();
                    }
                    break;
                case "PRIVATE":
                    if (!$('#Private_List_Panel_' + from).is(':visible'))
                        $('#Private_List_Panel_' + from).toggle();
                    //if (document.getElementById("divChatMessages") != null) {
                        if (message.UserName == CDF54.JA.SIGNALR.CHAT.UserName) {
                            //$('#Private_List_' + from).append('<div class="alert alert-success well-sm"><span>' + '[' + message.MessageDateTime + '] ' + message.UserName + '</span> : ' + encodedMessage + '</div>');
                            //$('#Private_List_' + from).append('<p id=' + message.Id + ' class="text-success"><strong>' + '[' + message.MessageDateTime + '] ' + message.UserName + ' : </strong><i>' + encodedMessage + '</i></p>');
                            datasmessage.myClass = "alert alert-success";
                            $('#new-pmessage-template').tmpl(datasmessage).appendTo($('#Private_List_' + from))
                            $('.alert').emoticonize();
                        }
                        else {
                            //$('#Private_List_' + from).append('<div class="alert alert-info well-sm"><span>' + '[' + message.MessageDateTime + '] ' + message.UserName + '</span> : ' + encodedMessage + '</div>');
                            //$('#Private_List_' + from).append('<p id=' + message.Id + ' class="text-info"><strong>' + '[' + message.MessageDateTime + '] ' + message.UserName + ' : </strong><i>' + encodedMessage + '</i></p>');
                            datasmessage.myClass = "alert alert-info";
                            $('#new-pmessage-template').tmpl(datasmessage).appendTo($('#Private_List_' + from))
                            $('.alert').emoticonize();
                        }
                        // Using jQuery to scroll to the bottom of #Private_List_Panel_ DIV.
                        var height = $('#Private_List_Panel_' + from)[0].scrollHeight;
                        $('#Private_List_Panel_' + from).scrollTop(height);
                        // beep
                        $('#chatAudio')[0].play();
                        break;
                    //};
            }
        },// Common methods
    };// Public members
})();// CDF54.JA.SIGNALR.CHAT.MESSAGE module.
//#endregion
//#region CDF54.JA.SIGNALR.CHAT.MESSAGE.PUBLIC module.
// CDF54.JA.SIGNALR.CHAT.MESSAGE.PUBLIC module.
CDF54.JA.SIGNALR.CHAT.MESSAGE.PUBLIC = (function () {
    'use strict';
    //
    // Public members
    //
    return {
        //
        // Events registering
        //
        registerEvents: function () {
            CDF54.JA.SIGNALR.CHAT.MISC.MyTrace('Entring CDF54.JA.SIGNALR.CHAT.MESSAGE.PUBLIC function registerEvents');

            $('#btnSendMsg').click(function () {
                CDF54.JA.SIGNALR.CHAT.MISC.MyTrace('btnSendMsg');

                var msg = $("#txtMessage").val();
                if (msg.length > 0) {
                    CDF54.JA.SIGNALR.CHAT.APP.ChatHubProxy.server.sendMessageToAll(msg);
                    CDF54.JA.SIGNALR.CHAT.MISC.MyTrace(CDF54.JA.UTILS.StringFormat('Client calling CDF54.JA.SIGNALR.CHAT.ChatHubProxy.server.sendMessageToAll({0})',
                        msg))
                    $("#txtMessage").val('');
                }
            });
            $("#txtMessage").keypress(function (e) {
                if (e.which == 13) {
                    $('#btnSendMsg').click();
                }
            });
        },// Events registering
        //
        // Client RPC
        //
        registerClientMethods: function () {
            CDF54.JA.SIGNALR.CHAT.MISC.MyTrace('Entring CDF54.JA.SIGNALR.CHAT.MESSAGE.PUBLIC function registerClientMethods');
            //
            // Called by server to push PUBLIC message.
            //
            CDF54.JA.SIGNALR.CHAT.APP.ChatHubProxy.client.showMessageReceived = function (message) {
                CDF54.JA.SIGNALR.CHAT.MISC.MyTrace(CDF54.JA.UTILS.StringFormat('Server calling CDF54.JA.SIGNALR.CHAT.ChatHubProxy.client.showMessageReceived({0}, {1}, {2})',
                    message.UserName,
                    message.MessageDateTime,
                    message.Message))
                CDF54.JA.SIGNALR.CHAT.MESSAGE.AddMessage(message, "PUBLIC", "");
            };
        },// Client RPC
    };// Public members
})();// CDF54.JA.SIGNALR.CHAT.MESSAGE.PUBLIC module.
//#endregion
//#region CDF54.JA.SIGNALR.CHAT.MESSAGE.PRIVATE module.
CDF54.JA.SIGNALR.CHAT.MESSAGE.PRIVATE = (function () {
    'use strict';
    //
    // Private members
    //
    //
    // CDF54.JA.SIGNALR.CHAT.MESSAGE.PRIVATE properties
    //
    var _privatePanelOpenNumber = 0;// CDF54.JA.SIGNALR.CHAT.MESSAGE.PRIVATE properties
    //
    // Private methods
    //
    var _CreatePrivateChatPanel = function (id, name) {
        if (_privatePanelOpenNumber == 0) {
            CDF54.JA.SIGNALR.CHAT.MISC.ToggleVisibilty();
        }
        _privatePanelOpenNumber++;
        //var div = '<div class="input-group" id=Private_Input_' + id + '>' +
        //                    '<span class="input-group-addon" id="basic-addon2"><span class="glyphicon glyphicon-pencil"></span>&nbsp;To&nbsp;<strong><span>' + name + '></span></strong></span>' +
        //                    '<input type="text" class="form-control input-sm" placeholder="Your message here..." id="txtPrivateMessage" />' +
        //                    '<div class="input-group-btn">' +
        //                        '<div class="btn-group btn-group-xs" role="group"><button class="btn btn-warning" type="button" id="btnSendMessage" value="Send">Send private</button></div>' +
        //                        '<div class="btn-group btn-group-xs"  role="group"><button class="btn btn-danger" type="button" id="btnClosePrivate" value="Send">Close private</button></div>' +
        //                    '</div>' +
        //            '</div>' +
        //            '<div class="panel panel-default panelScrollDown1 divHide" id=Private_List_Panel_' + id + '>' +
        //                    '<div id=Private_List_' + id + '>' +
        //                    '</div>' +
        //            '</div>'
        var div = $('#new-private-template').tmpl({ id: id, name: name, });
        var $div = $(div);
        $div.find("#btnSendMessage_" + id).click(function () {
            CDF54.JA.SIGNALR.CHAT.MISC.MyTrace('btnSendMessage');

            var $textBox = $div.find("#txtPrivateMessage_" + id);
            var msg = $textBox.val();
            if (msg.length > 0) {
                CDF54.JA.SIGNALR.CHAT.APP.ChatHubProxy.server.sendPrivateMessage(id, msg);
                CDF54.JA.SIGNALR.CHAT.MISC.MyTrace(CDF54.JA.UTILS.StringFormat("Client calling CDF54.JA.SIGNALR.CHAT.ChatHubProxy.server.sendPrivateMessage({0}, {1})",
                    id,
                    msg))
                $("#txtPrivateMessage_" + id).val('');
            }
        });

        $div.find("#btnClosePrivate_" + id).click(function () {
            CDF54.JA.SIGNALR.CHAT.MISC.MyTrace('btnClosePrivate');
            var inputctrId = 'Private_Input_' + id;
            var listctrId = 'Private_List_' + id;
            var listpanelctrId = 'Private_List_Panel_' + id;

            $('#' + inputctrId).remove();
            $('#' + listctrId).remove();
            $('#' + listpanelctrId).remove();
            _privatePanelOpenNumber--;
            if (_privatePanelOpenNumber == 0) {
                CDF54.JA.SIGNALR.CHAT.MISC.ToggleVisibilty();
            }
        });

        $div.find("#txtPrivateMessage_" + id).keypress(function (e) {
            if (e.which == 13) {
                $("#btnSendMessage_" + id).click();
            }
        });

        AddDivToContainer($div);
    };
    var AddDivToContainer = function ($div) {
        $('#inputPrivateMessage').append($div);
    };// Private methods // Private members
    //
    // Public members
    //
    return {
        //
        // Events registring
        //
        registerEvents: function () {
            CDF54.JA.SIGNALR.CHAT.MISC.MyTrace('Entring CDF54.JA.SIGNALR.CHAT.MESSAGE.PRIVATE function registerEvents');

        },// Events registring
        //
        // Client RPC
        //
        registerClientMethods: function () {

            CDF54.JA.SIGNALR.CHAT.MISC.MyTrace('Entring CDF54.JA.SIGNALR.CHAT.MESSAGE.PRIVATE function registerClientMethods');

            //
            // Called by server to push PRIVATE message.
            //
            CDF54.JA.SIGNALR.CHAT.APP.ChatHubProxy.client.showPrivateMessage = function (toOrfromUser, message) {
                CDF54.JA.SIGNALR.CHAT.MISC.MyTrace(CDF54.JA.UTILS.StringFormat('Server calling CDF54.JA.SIGNALR.CHAT.ChatHubProxy.client.showPrivateMessage({0}, {1}, {2})',
                    toOrfromUser.ConnectionId,
                    message.UserName, message.Message))

                if (!$('#Private_Input_' + toOrfromUser.ConnectionId).is(":visible")) {
                    _CreatePrivateChatPanel(toOrfromUser.ConnectionId, message.UserName);
                }
                CDF54.JA.SIGNALR.CHAT.MESSAGE.AddMessage(message, "PRIVATE", toOrfromUser.ConnectionId);
            }
        },// Client RPC
        //
        // Public methods
        //
        OpenPrivateChatPanel: function (id, userName) {
            var ctrId = 'Private_Input_' + id;
            if ($('#' + ctrId).length > 0) return;
            _CreatePrivateChatPanel(id, userName);
        },// Public methods
    };// Public members
})();// CDF54.JA.SIGNALR.CHAT.MESSAGE.PRIVATE module.
//#endregion
//#region CDF54.JA.SIGNALR.CHAT.ADMIN module.
CDF54.JA.SIGNALR.CHAT.ADMIN = (function () {
    'use strict';
    //
    // Public members
    //
    return {
        //
        // Client RPC
        //
        registerClientMethods: function () {
            CDF54.JA.SIGNALR.CHAT.MISC.MyTrace('Entring CDF54.JA.SIGNALR.CHAT.ADMIN function registerClientMethods');

            //
            // Called to fill Admin table.
            //
            CDF54.JA.SIGNALR.CHAT.APP.ChatHubProxy.client.showAdminTable = function (ca) {
                if (CDF54.JA.SIGNALR.CHAT.APP.ChatOrChatAdmin == "ChatAdmin") {
                    if (document.getElementById("adminTbodyTable") != null) {
                        $("#adminTbodyTable").html("");
                        for (var i = 0; i < ca.length; i++) {
                            //$('#adminTbodyTable').append('<tr>' + '<td>' + ca[i].ConnectionId + '</td>' + '<td>' + ca[i].UserName + '</td>' + '<td>' + ca[i].ConnectionDateTime + '</td>' + '<td>' + ca[i].Message + '</td>' + '<td>' + ca[i].MessageDateTime + '</td>' + '</tr>');
                            $('#new-admintable-template').tmpl({
                                id: ca[i].ConnectionId,
                                name: ca[i].UserName,
                                connectionDateTime: ca[i].ConnectionDateTime,
                                message: ca[i].Message,
                                messageDateTime: ca[i].MessageDateTime,
                            }).appendTo($('#adminTbodyTable'));
                            // beep
                            CDF54.JA.UTILS.beep();
                        }
                    }
                }
            };
        },// Client RPC
    };// Public members
})();//CDF54.JA.SIGNALR.CHAT.ADMIN module.
//#endregion
//#region Namespace aliases.
var aliasPUBLIC = CDF54.JA.SIGNALR.CHAT.MESSAGE.PUBLIC;
var aliasPRIVATE = CDF54.JA.SIGNALR.CHAT.MESSAGE.PRIVATE;// Namespace aliases
//#endregion
