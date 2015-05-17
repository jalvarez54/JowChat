
/// <reference path="~/Scripts/app/cdf54.ja.utils.js" />
/// <reference path="~/Scripts/webcam.js" />
/// <reference path="~/Scripts/jquery.webcam.js" />

/* Add extension */
function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#blah').attr('src', e.target.result);
            document.getElementById("preview").hidden = false;
            document.getElementById("blah").hidden = false;
            document.getElementById("change_photo_button").disabled = false;
            document.getElementById("capture_button").disabled = true;
            document.getElementById("upload_button").disabled = true;
            document.getElementById("JQcapture_button").disabled = true;
        }

        reader.readAsDataURL(input.files[0]);
    }
}
$("#filePhoto").change(function () {
    $('#subfile').val($(this).val());
    readURL(this);
});
/* \Add extension */
//
// Events registering for ChangePhoto and Register
//
/// Obsolete
function UseGravatarClick(email, size) {
    var $this = $('#UseGravatar');
    // $this will contain a reference to the checkbox   
    if ($this.is(':checked')) {
        // the checkbox was checked
        document.getElementById("filePhoto").disabled = true;

        var inpc = document.getElementById("IsNoPhotoChecked");
        if (inpc)
            document.getElementById("IsNoPhotoChecked").disabled = true;

        document.getElementById("preview").hidden = false;

        document.getElementById("blah").src = get_gravatar(email, size);

    } else {
        // the checkbox was unchecked
        document.getElementById("filePhoto").disabled = false;
        var inpc = document.getElementById("IsNoPhotoChecked");
        if (inpc)
            document.getElementById("IsNoPhotoChecked").disabled = false;

        document.getElementById("preview").hidden = true;
        document.getElementById("blah").src = '#';

    }
}
(function () {
    $('#UseSocialNetworkPicture').click(function () {
        var $this = $(this);
        // $this will contain a reference to the checkbox   
        if ($this.is(':checked')) {
            // the checkbox was checked
            document.getElementById("filePhoto").disabled = true;
            document.getElementById("submitbutton").disabled = true;
            document.getElementById("WebCam").disabled = true;
            document.getElementById("capture_button").disabled = true;
            document.getElementById("upload_button").disabled = true;
            document.getElementById("JQWebCam").disabled = true;
            document.getElementById("JQcapture_button").disabled = true;
            document.getElementById("change_photo_button").disabled = false;
            var inpc = document.getElementById("IsNoPhotoChecked");
            if (inpc)
                document.getElementById("IsNoPhotoChecked").disabled = true;
            var usnp = document.getElementById("UseGravatar");
            document.getElementById("UseGravatar").disabled = true;
            document.getElementById("preview").hidden = false;
            document.getElementById("blah").src = get_socialnetworkpicture($this.attr("data-ExternalProvider"), $this.attr("data-ParameterProvider"))

        } else {
            // the checkbox was unchecked
            document.getElementById("filePhoto").disabled = false;
            document.getElementById("submitbutton").disabled = false;
            document.getElementById("WebCam").disabled = false;
            document.getElementById("capture_button").disabled = false;
            document.getElementById("upload_button").disabled = false;
            document.getElementById("JQWebCam").disabled = false;
            document.getElementById("JQcapture_button").disabled = false;
            document.getElementById("change_photo_button").disabled = true;
            var inpc = document.getElementById("IsNoPhotoChecked");
            if (inpc)
                document.getElementById("IsNoPhotoChecked").disabled = false;
            document.getElementById("UseGravatar").disabled = false;
            document.getElementById("preview").hidden = true;
            document.getElementById("blah").src = '#';

        }
    });

    $('#UseGravatar').click(function () {
        var $this = $(this);
        // $this will contain a reference to the checkbox   
        if ($this.is(':checked')) {
            // the checkbox was checked submitbutton
            document.getElementById("filePhoto").disabled = true;
            document.getElementById("submitbutton").disabled = true;
            document.getElementById("WebCam").disabled = true;
            document.getElementById("capture_button").disabled = true;
            document.getElementById("upload_button").disabled = true;
            document.getElementById("JQWebCam").disabled = true;
            document.getElementById("JQcapture_button").disabled = true;
            document.getElementById("upload_button").disabled = true;
            document.getElementById("change_photo_button").disabled = false;
            var inpc = document.getElementById("IsNoPhotoChecked");
            if (inpc)
                document.getElementById("IsNoPhotoChecked").disabled = true;
            var usnp = document.getElementById("UseSocialNetworkPicture");
            if (usnp)
                document.getElementById("UseSocialNetworkPicture").disabled = true;
            document.getElementById("preview").hidden = false;
            document.getElementById("blah").src = get_gravatar($this.attr("data-email"), $this.attr("data-size"))

        } else {
            // the checkbox was unchecked
            document.getElementById("filePhoto").disabled = false;
            document.getElementById("submitbutton").disabled = false;
            document.getElementById("WebCam").disabled = false;
            document.getElementById("capture_button").disabled = false;
            document.getElementById("upload_button").disabled = false;
            document.getElementById("JQWebCam").disabled = false;
            document.getElementById("JQcapture_button").disabled = false;
            document.getElementById("change_photo_button").disabled = true;
            var inpc = document.getElementById("IsNoPhotoChecked");
            if (inpc)
                document.getElementById("IsNoPhotoChecked").disabled = false;
            var usnp = document.getElementById("UseSocialNetworkPicture");
            if (usnp)
                document.getElementById("UseSocialNetworkPicture").disabled = false;
            document.getElementById("preview").hidden = true;
            document.getElementById("blah").src = '#';

        }
    });
    $("#IsNoPhotoChecked").click(function () {
        var $this = $(this);
        // $this will contain a reference to the checkbox   
        if ($this.is(':checked')) {
            // the checkbox was checked 
            document.getElementById("filePhoto").disabled = true;
            document.getElementById("submitbutton").disabled = true;
            document.getElementById("UseGravatar").disabled = true;
            document.getElementById("WebCam").disabled = true;
            document.getElementById("capture_button").disabled = true;
            document.getElementById("upload_button").disabled = true;
            document.getElementById("JQWebCam").disabled = true;
            document.getElementById("JQcapture_button").disabled = true;
            document.getElementById("upload_button").disabled = true;
            document.getElementById("change_photo_button").disabled = false;
            var usnp = document.getElementById("UseSocialNetworkPicture");
            if (usnp)
                document.getElementById("UseSocialNetworkPicture").disabled = true;
        } else {
            // the checkbox was unchecked
            document.getElementById("filePhoto").disabled = false;
            document.getElementById("submitbutton").disabled = false;
            document.getElementById("UseGravatar").disabled = false;
            document.getElementById("WebCam").disabled = false;
            document.getElementById("capture_button").disabled = false;
            document.getElementById("upload_button").disabled = false;
            document.getElementById("JQWebCam").disabled = false;
            document.getElementById("JQcapture_button").disabled = false;
            document.getElementById("change_photo_button").disabled = true;
            var usnp = document.getElementById("UseSocialNetworkPicture");
            if (usnp)
                document.getElementById("UseSocialNetworkPicture").disabled = false;
        }
    });

    //[10031] ADD: webcam function for webcamjs
    $("#WebCam").click(function () {
        var $this = $(this);
        // $this will contain a reference to the checkbox   
        if ($this.is(':checked')) {
            // the checkbox was checked 
            document.getElementById("filePhoto").disabled = true;
            document.getElementById("submitbutton").disabled = true;
            document.getElementById("UseGravatar").disabled = true;
            document.getElementById("capture_button").disabled = false;
            document.getElementById("upload_button").disabled = false;
            document.getElementById("JQWebCam").disabled = true;
            document.getElementById("JQcapture_button").disabled = true;
            document.getElementById("change_photo_button").disabled = true;

            var usnp = document.getElementById("UseSocialNetworkPicture");
            if (usnp)
                document.getElementById("UseSocialNetworkPicture").disabled = true;
            var inpc = document.getElementById("IsNoPhotoChecked");
            if (inpc)
                document.getElementById("IsNoPhotoChecked").disabled = true;

            Webcam.set({
                width: 320,
                height: 240,
                image_format: 'jpeg',
                jpeg_quality: 90,
                dest_width: 320,
                dest_height: 240,
                swf_url: "/Scripts/webcam.swf",
            });
            Webcam.attach('#my_camera');

        } else {
            // the checkbox was unchecked
            document.getElementById("filePhoto").disabled = false;
            document.getElementById("submitbutton").disabled = false;
            document.getElementById("UseGravatar").disabled = false;
            document.getElementById("capture_button").disabled = true;
            document.getElementById("upload_button").disabled = true;
            document.getElementById("JQWebCam").disabled = false;
            document.getElementById("JQcapture_button").disabled = true;
            document.getElementById("change_photo_button").disabled = false;

            var usnp = document.getElementById("UseSocialNetworkPicture");
            if (usnp)
                document.getElementById("UseSocialNetworkPicture").disabled = false;
            var inpc = document.getElementById("IsNoPhotoChecked");
            if (inpc)
                document.getElementById("IsNoPhotoChecked").disabled = false;
            Webcam.reset();
        }
    });
    $("#capture_button").click(function () {
        Webcam.snap(function (data_uri) {
            // display results in page
            $('#blah').attr('src', data_uri);
            document.getElementById("preview").hidden = false;
            document.getElementById("blah").hidden = false;
        });
    });
    $("#upload_button").click(function () {
        var data_uri = document.getElementById("blah").getAttribute('src');
        Webcam.upload(data_uri, rootDir + "Manage/Upload", function (code, text) {
            // Upload complete!
            // 'code' will be the HTTP response code from the server, e.g. 200
            // 'text' will be the raw response content
            alert("Photo Capture successfully! " + "code = " + code + " text = " + text);
            //[10031] BUG: Photo dont change in real time.
            document.getElementById("login_photo").setAttribute('src', data_uri);
            //[10031]
        });
    });
    //[10031]


    //[10031] ADD: webcam function for jquery.webcam.js
    $("#JQWebCam").click(function () {
        var $this = $(this);
        // $this will contain a reference to the checkbox   
        if ($this.is(':checked')) {
            // the checkbox was checked 
            document.getElementById("filePhoto").disabled = true;
            document.getElementById("submitbutton").disabled = true;
            document.getElementById("UseGravatar").disabled = true;
            document.getElementById("WebCam").disabled = true;
            document.getElementById("capture_button").disabled = true;
            document.getElementById("upload_button").disabled = true;
            document.getElementById("JQcapture_button").disabled = false;
            document.getElementById("change_photo_button").disabled = true;

            var usnp = document.getElementById("UseSocialNetworkPicture");
            if (usnp)
                document.getElementById("UseSocialNetworkPicture").disabled = true;
            var inpc = document.getElementById("IsNoPhotoChecked");
            if (inpc)
                document.getElementById("IsNoPhotoChecked").disabled = true;
            
            console.debug(CDF54.JA.UTILS.getContextPath() + "Scripts/jscam.swf");
            console.debug(rootDir + "Scripts/jscam.swf");

            $("#my_camera").webcam({
                width: 320,
                height: 240,
                mode: "save",
                swffile: rootDir + "Scripts/jscam.swf",
                onTick: function () { },
                onSave: function () {
                    UploadPic();
                }, onCapture: function () {
                    webcam.save("Capture");
                },
            });
        }
        else {
            // the checkbox was unchecked
            document.getElementById("filePhoto").disabled = false;
            document.getElementById("submitbutton").disabled = false;
            document.getElementById("UseGravatar").disabled = false;
            document.getElementById("WebCam").disabled = false;
            document.getElementById("capture_button").disabled = true;
            document.getElementById("upload_button").disabled = true;
            document.getElementById("JQcapture_button").disabled = true;
            document.getElementById("change_photo_button").disabled = false;

            var usnp = document.getElementById("UseSocialNetworkPicture");
            if (usnp)
                document.getElementById("UseSocialNetworkPicture").disabled = false;
            var inpc = document.getElementById("IsNoPhotoChecked");
            if (inpc)
                document.getElementById("IsNoPhotoChecked").disabled = false;
            $("#XwebcamXobjectX").remove();
        }
    });
    $("#JQcapture_button").click(function () {
        webcam.capture();
    });
    //[10031]

    //
    // At load time
    //
    if ($('#UseGravatar').is(':checked')) {
        // the checkbox is checked at load time
        document.getElementById("filePhoto").disabled = true;
        document.getElementById("WebCam").disabled = true;
        document.getElementById("capture_button").disabled = true;
        document.getElementById("upload_button").disabled = true;
        document.getElementById("JQWebCam").disabled = true;
        document.getElementById("JQcapture_button").disabled = true;
        document.getElementById("change_photo_button").disabled = true;
        var usnp = document.getElementById("UseSocialNetworkPicture");
        if (usnp)
            document.getElementById("UseSocialNetworkPicture").disabled = true;
        var inpc = document.getElementById("IsNoPhotoChecked");
        if (inpc)
            document.getElementById("IsNoPhotoChecked").disabled = true;
    }

    if ($('#UseSocialNetworkPicture').is(':checked')) {
        // the checkbox is checked at load time
        document.getElementById("filePhoto").disabled = true;
        document.getElementById("WebCam").disabled = true;
        document.getElementById("capture_button").disabled = true;
        document.getElementById("upload_button").disabled = true;
        document.getElementById("JQWebCam").disabled = true;
        document.getElementById("JQcapture_button").disabled = true;
        document.getElementById("change_photo_button").disabled = true;
        document.getElementById("UseGravatar").disabled = true;
        var inpc = document.getElementById("IsNoPhotoChecked");
        if (inpc)
            document.getElementById("IsNoPhotoChecked").disabled = true;
    }

    document.getElementById("capture_button").disabled = true;
    document.getElementById("upload_button").disabled = true;
    document.getElementById("JQcapture_button").disabled = true;
    document.getElementById("change_photo_button").disabled = true;


    //if ($('#WebCam').is(':checked')) {
    //    // the checkbox is checked at load time
    //    document.getElementById("filePhoto").disabled = true;
    //    document.getElementById("UseGravatar").disabled = true;
    //    var inpc = document.getElementById("IsNoPhotoChecked");
    //    if (inpc)
    //        document.getElementById("IsNoPhotoChecked").disabled = true;
    //}

    //if ($('#JQWebCam').is(':checked')) {
    //    // the checkbox is checked at load time
    //    document.getElementById("filePhoto").disabled = true;
    //    document.getElementById("UseGravatar").disabled = true;
    //    var inpc = document.getElementById("IsNoPhotoChecked");
    //    if (inpc)
    //        document.getElementById("IsNoPhotoChecked").disabled = true;
    //}

})();

function get_gravatar(email, size) {

    // MD5 (Message-Digest Algorithm) by WebToolkit
    // 
    var MD5 = function (s) { function L(k, d) { return (k << d) | (k >>> (32 - d)) } function K(G, k) { var I, d, F, H, x; F = (G & 2147483648); H = (k & 2147483648); I = (G & 1073741824); d = (k & 1073741824); x = (G & 1073741823) + (k & 1073741823); if (I & d) { return (x ^ 2147483648 ^ F ^ H) } if (I | d) { if (x & 1073741824) { return (x ^ 3221225472 ^ F ^ H) } else { return (x ^ 1073741824 ^ F ^ H) } } else { return (x ^ F ^ H) } } function r(d, F, k) { return (d & F) | ((~d) & k) } function q(d, F, k) { return (d & k) | (F & (~k)) } function p(d, F, k) { return (d ^ F ^ k) } function n(d, F, k) { return (F ^ (d | (~k))) } function u(G, F, aa, Z, k, H, I) { G = K(G, K(K(r(F, aa, Z), k), I)); return K(L(G, H), F) } function f(G, F, aa, Z, k, H, I) { G = K(G, K(K(q(F, aa, Z), k), I)); return K(L(G, H), F) } function D(G, F, aa, Z, k, H, I) { G = K(G, K(K(p(F, aa, Z), k), I)); return K(L(G, H), F) } function t(G, F, aa, Z, k, H, I) { G = K(G, K(K(n(F, aa, Z), k), I)); return K(L(G, H), F) } function e(G) { var Z; var F = G.length; var x = F + 8; var k = (x - (x % 64)) / 64; var I = (k + 1) * 16; var aa = Array(I - 1); var d = 0; var H = 0; while (H < F) { Z = (H - (H % 4)) / 4; d = (H % 4) * 8; aa[Z] = (aa[Z] | (G.charCodeAt(H) << d)); H++ } Z = (H - (H % 4)) / 4; d = (H % 4) * 8; aa[Z] = aa[Z] | (128 << d); aa[I - 2] = F << 3; aa[I - 1] = F >>> 29; return aa } function B(x) { var k = "", F = "", G, d; for (d = 0; d <= 3; d++) { G = (x >>> (d * 8)) & 255; F = "0" + G.toString(16); k = k + F.substr(F.length - 2, 2) } return k } function J(k) { k = k.replace(/rn/g, "n"); var d = ""; for (var F = 0; F < k.length; F++) { var x = k.charCodeAt(F); if (x < 128) { d += String.fromCharCode(x) } else { if ((x > 127) && (x < 2048)) { d += String.fromCharCode((x >> 6) | 192); d += String.fromCharCode((x & 63) | 128) } else { d += String.fromCharCode((x >> 12) | 224); d += String.fromCharCode(((x >> 6) & 63) | 128); d += String.fromCharCode((x & 63) | 128) } } } return d } var C = Array(); var P, h, E, v, g, Y, X, W, V; var S = 7, Q = 12, N = 17, M = 22; var A = 5, z = 9, y = 14, w = 20; var o = 4, m = 11, l = 16, j = 23; var U = 6, T = 10, R = 15, O = 21; s = J(s); C = e(s); Y = 1732584193; X = 4023233417; W = 2562383102; V = 271733878; for (P = 0; P < C.length; P += 16) { h = Y; E = X; v = W; g = V; Y = u(Y, X, W, V, C[P + 0], S, 3614090360); V = u(V, Y, X, W, C[P + 1], Q, 3905402710); W = u(W, V, Y, X, C[P + 2], N, 606105819); X = u(X, W, V, Y, C[P + 3], M, 3250441966); Y = u(Y, X, W, V, C[P + 4], S, 4118548399); V = u(V, Y, X, W, C[P + 5], Q, 1200080426); W = u(W, V, Y, X, C[P + 6], N, 2821735955); X = u(X, W, V, Y, C[P + 7], M, 4249261313); Y = u(Y, X, W, V, C[P + 8], S, 1770035416); V = u(V, Y, X, W, C[P + 9], Q, 2336552879); W = u(W, V, Y, X, C[P + 10], N, 4294925233); X = u(X, W, V, Y, C[P + 11], M, 2304563134); Y = u(Y, X, W, V, C[P + 12], S, 1804603682); V = u(V, Y, X, W, C[P + 13], Q, 4254626195); W = u(W, V, Y, X, C[P + 14], N, 2792965006); X = u(X, W, V, Y, C[P + 15], M, 1236535329); Y = f(Y, X, W, V, C[P + 1], A, 4129170786); V = f(V, Y, X, W, C[P + 6], z, 3225465664); W = f(W, V, Y, X, C[P + 11], y, 643717713); X = f(X, W, V, Y, C[P + 0], w, 3921069994); Y = f(Y, X, W, V, C[P + 5], A, 3593408605); V = f(V, Y, X, W, C[P + 10], z, 38016083); W = f(W, V, Y, X, C[P + 15], y, 3634488961); X = f(X, W, V, Y, C[P + 4], w, 3889429448); Y = f(Y, X, W, V, C[P + 9], A, 568446438); V = f(V, Y, X, W, C[P + 14], z, 3275163606); W = f(W, V, Y, X, C[P + 3], y, 4107603335); X = f(X, W, V, Y, C[P + 8], w, 1163531501); Y = f(Y, X, W, V, C[P + 13], A, 2850285829); V = f(V, Y, X, W, C[P + 2], z, 4243563512); W = f(W, V, Y, X, C[P + 7], y, 1735328473); X = f(X, W, V, Y, C[P + 12], w, 2368359562); Y = D(Y, X, W, V, C[P + 5], o, 4294588738); V = D(V, Y, X, W, C[P + 8], m, 2272392833); W = D(W, V, Y, X, C[P + 11], l, 1839030562); X = D(X, W, V, Y, C[P + 14], j, 4259657740); Y = D(Y, X, W, V, C[P + 1], o, 2763975236); V = D(V, Y, X, W, C[P + 4], m, 1272893353); W = D(W, V, Y, X, C[P + 7], l, 4139469664); X = D(X, W, V, Y, C[P + 10], j, 3200236656); Y = D(Y, X, W, V, C[P + 13], o, 681279174); V = D(V, Y, X, W, C[P + 0], m, 3936430074); W = D(W, V, Y, X, C[P + 3], l, 3572445317); X = D(X, W, V, Y, C[P + 6], j, 76029189); Y = D(Y, X, W, V, C[P + 9], o, 3654602809); V = D(V, Y, X, W, C[P + 12], m, 3873151461); W = D(W, V, Y, X, C[P + 15], l, 530742520); X = D(X, W, V, Y, C[P + 2], j, 3299628645); Y = t(Y, X, W, V, C[P + 0], U, 4096336452); V = t(V, Y, X, W, C[P + 7], T, 1126891415); W = t(W, V, Y, X, C[P + 14], R, 2878612391); X = t(X, W, V, Y, C[P + 5], O, 4237533241); Y = t(Y, X, W, V, C[P + 12], U, 1700485571); V = t(V, Y, X, W, C[P + 3], T, 2399980690); W = t(W, V, Y, X, C[P + 10], R, 4293915773); X = t(X, W, V, Y, C[P + 1], O, 2240044497); Y = t(Y, X, W, V, C[P + 8], U, 1873313359); V = t(V, Y, X, W, C[P + 15], T, 4264355552); W = t(W, V, Y, X, C[P + 6], R, 2734768916); X = t(X, W, V, Y, C[P + 13], O, 1309151649); Y = t(Y, X, W, V, C[P + 4], U, 4149444226); V = t(V, Y, X, W, C[P + 11], T, 3174756917); W = t(W, V, Y, X, C[P + 2], R, 718787259); X = t(X, W, V, Y, C[P + 9], O, 3951481745); Y = K(Y, h); X = K(X, E); W = K(W, v); V = K(V, g) } var i = B(Y) + B(X) + B(W) + B(V); return i.toLowerCase() };
    var size = size || 80;
    return 'http://www.gravatar.com/avatar/' + MD5(email) + '?d=wavatar&s=' + size;
}

function get_socialnetworkpicture(externalProvider, parameterProvider) {

    var photoUrl;
    if (externalProvider != "") {
        if (externalProvider == "Google")
            photoUrl = parameterProvider;
        if (externalProvider == "Microsoft") {
            photoUrl = CDF54.JA.UTILS.StringFormat("https://apis.live.net/v5.0/{0}/picture", parameterProvider);
        }
        if (externalProvider == "Facebook") {
            photoUrl = CDF54.JA.UTILS.StringFormat("http://graph.facebook.com/{0}/picture", parameterProvider);
        }
        if (externalProvider == "Twitter") {
            photoUrl = CDF54.JA.UTILS.StringFormat("https://twitter.com/{0}/profile_image?size=original", parameterProvider);
        }
        if (externalProvider == "GitHub") {
            photoUrl = CDF54.JA.UTILS.StringFormat("https://avatars.githubusercontent.com/u/{0}?v=3", parameterProvider);
        }
    }
    else {
        photoUrl = rootDir + "Content/Avatars/" + "NoPhoto.png";
    }

    return photoUrl;
}
//[10031] ADD: webcam function for jquery.webcam.js
function capture() {
    // actually snap photo (from preview freeze) and display it
    Webcam.snap(function (data_uri) {
        // display results in page
        $('#blah').attr('src', data_uri);
        document.getElementById("preview").hidden = false;
        document.getElementById("blah").hidden = false;
    });
}
function UploadPic() {
    $.ajax({  
        type: 'POST',  
        url: "Rebind",  
        dataType: 'json',  
        success: function (data) {  
            //$("#show").attr("src", data);  
            alert("Photo Capture successfully!");  
    }  
});  
}  
//[10031]
