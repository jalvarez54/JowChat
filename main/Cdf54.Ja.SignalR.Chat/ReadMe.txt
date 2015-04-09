===============================================
13/03/2015 ASP.NET MVC SignalR Chat application
=================================================================
/////////////////////////
// Internet Ref. projects
// "Render therefore unto Caesar the things which are Caesar's; and unto God the things that are God's."
////////////////////////////////////////////////////////////////////////////////////////////////////////
- http://www.codeproject.com/Articles/562023/Asp-Net-SignalR-Chat-Room (For chat)
- https://github.com/SignalR/SignalR/tree/master/samples/Microsoft.AspNet.SignalR.Samples/Hubs/Chat (For ContentProviders)
- https://github.com/paulduran/blaze/tree/master/Blaze/ContentProviders (For ContentProviders)

////////////////////////
// TODO
////////////////////////////////////////////////////////////////////////////////////////////////////////
- @Request.ServerVariables["SERVER_SOFTWARE"].Contains("IIS") equivalent in JS.
	===> impossible because server side.
- BUG: Chat dropdown pb.
	pb occure when using CDF54.JA.SIGNALR.CHAT.HELPERS.showBootstrapVersion1()
	which read bootstrap.min.js.
	===> Sol: ?
- Add # to Chat and ChatAdmin link ===> disable refresh
	===> Sol: Impossible to have an action name beginning with #
- Detect tag and specific code for Chat and ChatAdmin
	var p = window.location.pathname;
	var pathArray = window.location.pathname.split('/');
	alert(pathArray[pathArray.length - 1]); ==> Chat or ChatAdmin
	===> done
- Review append html code
	==> done
- Replace: .click() with .on
	http://chez-syl.fr/2012/02/les-evenements-sur-des-elements-charges-en-ajax/
	http://chez-syl.fr/2011/12/comment-traduire-gestionnaires-evenements-avec-le-nouveau-on/
	==>
- Replace Bootstrap carousel with fancybox carousel in Home/Index (responsive)
	==>
- PB: Why can I see _StartFail and not StartDown ?
	==> CDF54.JA.SIGNALR.CHAT.APP. cause StartDown in a return
- BUG: public override System.Threading.Tasks.Task OnReconnected() 
	When publish web.config usercount => 0 with same ConnectionID
	http://www.asp.net/signalr/overview/guide-to-the-api/handling-connection-lifetime-events
	==>
- Show multimedia content, image, video...
	Ref. : https://github.com/SignalR/SignalR/tree/master/samples/Microsoft.AspNet.SignalR.Samples/Hubs/Chat
	CollegeHumorContentProvider, ImageContentProvider only, YouTubeContentProvider fail ==> ADD: yield return "https://www.youtube.com"; ==> OK !.
	==> done
	ADD: audio
	==> done
	ADD: Code comments
	==> done
- Console client
	https://damienbod.wordpress.com/2013/11/01/signalr-messaging-with-console-server-and-client-web-client-wpf-client/
	https://github.com/damienbod/SignalRMessaging
	==> done
- Refactor ContentProvider
	==> done
- How to Migration with multiple web.config (multiple connectionString) ?
	==> done
- Use template to insert HTML code.
	==> done
- Review trace messages
	==>
- CHANGE: Design
	http://www.designbootstrap.com/bootstrap-chat-box-template-example/
		==>
- ADD: emoticons
	==> done
- ADD: user avatar
	==> done
- CORRECTION: Private message send on return
	==> done
- CHANGE: partial view usage for photos
	==> impossible
- ADD: user avatar in Chat
	==>
- BUG: crash if PhotoUrl = NULL
	==> done
- ADD: pseudo and IsnoPhotoChecked properties
	==> done
- BUG: nochange photo line 108 ManageController
	==> done
- ???: Can i suppress IsnoPhotoChecked from database ?
	yes i can, so add notmapped attribut
	==> done
- CHANGE: In chat, show pseuso instead of email
	==> done

////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////

////////////////////////
// Version 1.0.Alpha
////////////////////////////////////////////////////////////////////////////////////////////////////////
- Add Shared/Articles recipient folder
- Add _Specifications.cshtml file in Articles for Home/Index: @Html.Partial("Articles/_Specifications")
- Add XML comment with scripts reference for js intellisense 
	https://msdn.microsoft.com/fr-fr/library/bb385682.aspx#Script
- Add comments 
	https://msdn.microsoft.com/fr-fr/library/bb514138.aspx
- Add json namespace
- Add _references.js for intellisense
	http://madskristensen.net/post/the-story-behind-_referencesjs
- 2015-03-25 COMMIT: Codeplex jowchat.codeplex.com (107264) Version initiale
- Show multimedia content, image, video...
	From https://github.com/SignalR/SignalR/tree/master/samples/Microsoft.AspNet.SignalR.Samples/Hubs/Chat project.
		ADD ContentProviders
		ADD in ChatHub.cs
			message = message.Replace("<", "&lt;").Replace(">", "&gt;");
			HashSet<string> links;
			var messageText = Transform(message, out links);
			if (links.Any()){} in SendPrivateMessage and SendMessageToAll
			private string Transform(string message, out HashSet<string> extractedUrls)
			private Task<string> ExtractContent(string url)
			private string ExtractContent(HttpWebResponse response)
		In cdf54.ja.signalR.chat.js
			ADD: CDF54.JA.SIGNALR.CHAT.APP.ChatHubProxy.client.showMessageReceived in CDF54.JA.SIGNALR.CHAT.MESSAGE.PUBLIC.
			MODIF: CDF54.JA.SIGNALR.CHAT.APP.ChatHubProxy.client.onConnected
			INSERT: '<p id=' + message.Id + ' in CDF54.JA.SIGNALR.CHAT.MESSAGE.AddMessage
			ADD: CDF54.JA.SIGNALR.CHAT.MESSAGE.AddContentMessage: function(message){} in CDF54.JA.SIGNALR.CHAT.MESSAGE.
			ADD: yield return "https://www.youtube.com"; in YouTubeContentProvider
- 2015-03-26 COMMIT: Codeplex jowchat.codeplex.com (107270) Show multimedia content, image, video...
- ADD: DailyMotionContentProvider
- CHANGE: @"<object to @"<iframe in DailyMotionContentProvider, YouTubeContentProvider, CollegeHumorContentProvider.
- 2015-03-28 COMMIT : Codeplex jowchat.codeplex.com (107277) ADD: ContentProviders
- ADD: AudioContentProvider
- ADD: Perfs Tools.zip
- Console client
	https://damienbod.wordpress.com/2013/11/01/signalr-messaging-with-console-server-and-client-web-client-wpf-client/
	https://github.com/damienbod/SignalRMessaging
	http://www.codeproject.com/Articles/804770/Implementing-SignalR-in-Desktop-Applications
	https://damienbod.wordpress.com/2013/11/13/signalr-messaging-a-complete-client-with-a-console-application/
	ADD: project :Cdf54.Ja.SignalR.Console.Client.Chat
	INSTALL: nuget package Microsoft.AspNet.SignalR.Client
- 2015-04-01 COMMIT: Codeplex jowchat.codeplex.com (107301) First commit
- 2015-04-01 COMMIT: Codeplex jowchat.codeplex.com (107302) Docs
- 2015-04-01 COMMIT: Codeplex jowchat.codeplex.com (107305) Cdf54.Ja.SignalR.Console.Client.Chat
- 2015-04-01 COMMIT: Codeplex jowchat.codeplex.com (107307) Remove web.xxx.config files (security)
- 2015-04-02 COMMIT: Codeplex jowchat.codeplex.com (107312) Unmap Documentation
- ADD: .tfignore file
	https://msdn.microsoft.com/library/vstudio/ms245454%28v=vs.110%29.aspx#tfignore
- ADD jquery.tmpl.js see _ViewStart.cshtml
	http://www.borismoore.com/2010/09/introducing-jquery-templates-1-first.html
- 2015-04-04 COMMIT: Codeplex jowchat.codeplex.com (107323) ADD jquery.tmpl.js review code
- 2015-04-04 COMMIT: Codeplex jowchat.codeplex.com (107324) DELETE: package folder
- ADD: Emoticons
	http://os.alfajango.com/css-emoticons/
	https://github.com/JangoSteve/jQuery-CSSEmoticons
- 2015-04-06 COMMIT: Codeplex jowchat.codeplex.com (107328) ADD: Emoticons
- ADD: User avatar.
	http://typecastexception.com/post/2014/06/22/ASPNET-Identity-20-Customizing-Users-and-Roles.aspx
	EXEC: Enable-Migrations -EnableAutomaticMigrations
	ADD: PhotoUrl and Photo properties in IdentityModels.cs
	EXEC: Add-Migration Photo
	EXEC: Update-Database
	UPDATE: RegisterViewModel
	UPDATE: Register View
		http://www.w3schools.com/tags/att_form_enctype.asp
	ADD: cdf54.ja.signalR.chatapp.js for image preview
	ADD: "~/Scripts/app/cdf54.ja.signalR.chatapp.js" to bundle
	ADD: public static string SavePhotoFileToDisk method in UTILS
	ADD: MyCustomAttributes.cs in CustomFiltersAttributes folder
	UPDATE: Register Method on AccountController
	ADD: ChangePhotoViewModel in ManageViewModels
	ADD: public ActionResult ChangePhoto() GET and POST in ManageController
	ADD Manage/ChangePhoto View
	ADD: User avatar to _LoginPartial.cshtml
	==> done
- EXEC: Update-Database in staging
	Update-Database -Script -ConnectionString "data source=192.168.107.232;initial catalog=Cdf54.Ja.SignalR.Chat;Persist Security Info=True;User ID=cdf54projet;Password=p@ssword2014"providerName="System.Data.SqlClient"
	==> !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
- BUG: Private input not unloaded when client disconnect
	==> done
- ADD: pseudo and IsnoPhotoChecked properties
	ADD: ManageViewModels/ChangePhotoViewModel
	ADD: IsNoPhotoChecked and pseudo properties in IdentityModel
	ADD: IsNoPhotoChecked and pseudo properties in AccountViewModels/RegisterViewModel
	UPDATE:  ChangePhoto and Register views.
	ADD: condition && model.IsNoPhotoChecked  in ChangePhoto ManageController
	ADD: Pseudo = model.Pseudo in AccountController/Register
	ADD: ManageViewModels/ChangeProfileViewModel
	ADD: Manage/ChangeProfile View for Pseudo modification
	ADD: 	ADD: public ActionResult ChangeProfile() GET and POST in ManageController
	EXEC: Add-Migration IsNoPhotoCheckedAndPseudo
	EXEC: Update-Database
- IsnoPhotoChecked not necessary in database so remove it.
	ADD: [NotMapped] attribut
	EXEC: Update-Database -TargetMigration Photo
	EXEC: Add-Migration Pseudo
	EXEC: Update-Database
- 2015-04-08 COMMIT: Codeplex jowchat.codeplex.com (107340) ADD: photo and pseudo
- CHANGE: UserName from Email to Pseudo in Account sub-system
	- AccountController/Login: SignInManager.PasswordSignInAsync(model.Email, ==> SignInManager.PasswordSignInAsync(model.Pseudo,
	- AccountViewModels/LoginViewModel: Email property ==> Pseudo property
	- Login view: m => m.Email ==> m => m.Pseudo
	- ADAPT: Manage section
	- BUG: Exception when trying to change Pseudo/UserName
		in GET action public ActionResult ChangePseudoUserName(EditMessageID? message = null)
		retreive old pseudo from User.Identity.Name (?)
		(Not resolved)
	==> done
- 2015-04-09 COMMIT: Codeplex jowchat.codeplex.com (107344) CHANGE: UserName from Email to Pseudo in Account sub-system
- USE: UserManager instead of ApplicationDbContext() instance.
	ManageController/ChangePhoto, ChangeProfile, _LoginPartial
	http://www.ciiycode.com/0yNm6PjPWjPQ/aspnet-identity-extend-methods-to-access-user-properties
- 2015-04-09 COMMIT: Codeplex jowchat.codeplex.com (?) USE: UserManager instead of ApplicationDbContext() instance


////////////////////////
// Version 0.0.Alpha
////////////////////////////////////////////////////////////////////////////////////////////////////////
- Ref. : http://www.codeproject.com/Articles/562023/Asp-Net-SignalR-Chat-Room
- Template: Empty web project
- Install-Package Microsoft.AspNet.Identity.Samples -Pre
	- http://www.codeproject.com/Articles/762428/ASP-NET-MVC-and-Identity-Understanding-the-Basics
	- http://www.asp.net/identity/overview/features-api/account-confirmation-and-password-recovery-with-aspnet-identity
	- http://www.asp.net/identity/overview/features-api/two-factor-authentication-using-sms-and-email-with-aspnet-identity
		Please see http://go.microsoft.com/fwlink/?LinkID=301950 for more information on using ASP.NET Identity.
		About this sample
		------------------------------------------------------------
		This is a sample template which shows how you can do the most common scenarios in ASP.NET Identity such 
		as Local Logins, Social Logins, Account Confirmation, Password Reset, Two-Factor Authentication and more.
		For more information on how to configure this sample for these feature, please visit http://go.microsoft.com/fwlink/?LinkID=320973

		Running this sample in your ASP.NET application
		------------------------------------------------------------
		This is a sample template so please install this in an empty ASP.NET project only. Installing this in an 
		existing application will have some side effects as this sample configures OWIN middlewares in StartupAuth.cs 
		and creates a database for storing membership information.
- Install-Package Microsoft.AspNet.SignalR
		Please see http://go.microsoft.com/fwlink/?LinkId=272764 for more information on using SignalR.

		Upgrading from 1.x to 2.0
		-------------------------
		Please see http://go.microsoft.com/fwlink/?LinkId=320578 for more information on how to 
		upgrade your SignalR 1.x application to 2.0.

		Mapping the Hubs connection
		----------------------------
		To enable SignalR in your application, create a class called Startup with the following:

		using Microsoft.Owin;
		using Owin;
		using MyWebApplication;

		namespace MyWebApplication
		{
			public class Startup
			{
				public void Configuration(IAppBuilder app)
				{
					app.MapSignalR();
				}
			}
		} 

		Getting Started
		---------------
		See http://www.asp.net/signalr/overview/getting-started for more information on how to get started.

		Why does ~/signalr/hubs return 404 or Why do I get a JavaScript error: 'myhub is undefined'?
		--------------------------------------------------------------------------------------------
		This issue is generally due to a missing or invalid script reference to the auto-generated Hub JavaScript proxy at '~/signalr/hubs'.
		Please make sure that the Hub route is registered before any other routes in your application.
 
		In ASP.NET MVC 4 you can do the following:
 
			  <script src="~/signalr/hubs"></script>
 
		If you're writing an ASP.NET MVC 3 application, make sure that you are using Url.Content for your script references:
 
			<script src="@Url.Content("~/signalr/hubs")"></script>
 
		If you're writing a regular ASP.NET application use ResolveClientUrl for your script references or register them via the ScriptManager 
		using a app root relative path (starting with a '~/'):
 
			<script src='<%: ResolveClientUrl("~/signalr/hubs") %>'></script>
 
		If the above still doesn't work, you may have an issue with routing and extensionless URLs. To fix this, ensure you have the latest 
		patches installed for IIS and ASP.NET.

- My Utilities installation (see Utils project ReadMe.txt file)
- Add UserDetailsModel, MessageDetailModel, ChatAdminViewModel, ServerContextModel
- Add Hubs folder
- Add Hubs/ChatHub.cs
- Add Scripts/app/JA.UTILS-1.0.0.js
- Add Scripts/app/CDF54.JA.SIGNALR.CHAT.APP-1.0.0.js
- Add Scripts/app/cdf54.ja.demotransports.js
- Update JQUERY 2.1.3
- Add ~/bundles/chat
- Add ChatController + Actions: Chat and ChatAdmin
- Add Views Chat and ChatAdmin
- Add links in _Layout
- Add some appSettings, see <!-- Cdf54.Ja.SignalR.Chat -->
- Modification Startup.cs
- Add /Scripts/app/notify.ogg, mp3, wav
- Add trace capability and customerror in web.config <system.web>, see <!-- Cdf54.Ja.SignalR.Chat -->
- Add <system.diagnostics> in web.config , see <!-- Cdf54.Ja.SignalR.Chat -->
- Add Solution Configuration Staging from Home and Staging from School
- Add publish profiles: CDF54 VM from Home, CDF54 VM from School
- Add Home/Index page
- Add  carousel images in Content/app
- Add ~/bundles/chatindex for page Home/Index
- Add "Perfs Tools" folder with cranck outside project
- Change IdentitySample to Cdf54.Ja.SignalR.Chat
- Add _ViewStart.cshtml in Chat views to share Razor with js files instead of having this code in each view file Chat folder.

////////////////////////
// SignalR Performances
////////////////////////////////////////////////////////////////////////////////////////////////////////
- Perfs Tools Folder (crank, SignalR-tools, Tresi)
- Crank
- https://github.com/SignalR/SignalR/tree/dev/src/Microsoft.AspNet.SignalR.Crank
	- http://www.asp.net/signalr/overview/performance/signalr-performance#perfcounters
	- http://www.codeproject.com/Articles/8590/An-Introduction-To-Performance-Counters
	- SignalR.exe ipc
	- crank /Connections:100 /Url:http://localhost:5192/signalr
- Tresi
	http://www.wcfstorm.com/wcf/home.aspx

////////////////////////
// NB
////////////////////////////////////////////////////////////////////////////////////////////////////////
- Elements like application settings, IIS virtual path are only resolved on server side.
	It's not possible to retrive those values in JS on client side.
		===> Sol:
			Insert functions in _ViewStart.cshtml
				ex:
					<script type="text/javascript">
						// Get appsetting IsTraceEnable for js.
						function IsTraceEnable() {
							return @Utils.GetAppSetting("IsTraceEnable");
						}
						// Get appsetting IsConsoleLoggingEnable for js.
						function IsConsoleLoggingEnable() {
							return @Utils.GetAppSetting("IsConsoleLoggingEnable");
						}
						// Get appsetting IsDemo for js.
						function IsDemo() {
							return @Utils.GetAppSetting("IsDemo");
						}
						// Get IIS application virtual path for js.
						function AppPath() {
							var a = '@HttpContext.Current.Request.ApplicationPath';
							if (a == '/') {
								appPath = '';
							}
							else {
								appPath = a;
							}
							return appPath;
						}
					</script>
				The Razor code is executed on server side and the values are resolved before the JS
				is loaded on client side.
				ex: result
					<script type="text/javascript">
						// Get appsetting IsTraceEnable for js.
						function IsTraceEnable() {
							return true;
						}
						// Get appsetting IsConsoleLoggingEnable for js.
						function IsConsoleLoggingEnable() {
							return false;
						}
						// Get appsetting IsDemo for js.
						function IsDemo() {
							return false;
						}
						// Get IIS application virtual path for js.
						function AppPath() {
							var a = '/Cdf54Chat';
							if (a == '/') {
								appPath = '';
							}
							else {
								appPath = a;
							}
							return appPath;
						}
					</script>
				From JS we can use those function to retrieve the values
				ex: if (JSON.parse(String(IsTraceEnable()).toLowerCase()))
