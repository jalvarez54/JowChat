/*
 * Author: José ALVAREZ
 * Date: 07/02/2015
 * Description: Helpers for this application.
 * File: cdf54.ja.signalrR.chat.helpers.js
 */
//#region reference path.
/// <reference path="~/Views/Chat/_ViewStart.cshtml" />
//#endregion

//#region CDF54.JA.SIGNALR.CHAT.HELPERS module.
CDF54.JA.SIGNALR.CHAT.HELPERS = (function () {
    'use strict';
    //
    // Private members
    //

    //
    // Public members
    //
    return {
        //
        // Public properties
        //

        //
        // Public methods
        //
        // get bootstrap version manual
        showBootstrapVersion: function (tag) {
            $('#' + tag).append("3.3.2");
        },
        // get bootstrap version need ~/Views/Chat/_ViewStart.cshtml
        showBootstrapVersion1: function (tag) {
            var appPath = AppPath();
            $.get(appPath + "/Scripts/bootstrap.js", function (data) {
                var version = data.match(/v[.\d]+[.\d]/);
                //alert("V= " + version);
                $('#' + tag).append(version);
            });
        },
        // get jquery version
        showJqueryVersion: function (tag) {
            $('#' + tag).append($.fn.jquery);
        },
        // get signalR version
        showSignalRVersion: function (tag) {
            $('#' + tag).append($.signalR.version);
        },
        // get
    };// Public members
})();//CDF54.JA.SIGNALR.CHAT.HELPERS.
//#endregion
