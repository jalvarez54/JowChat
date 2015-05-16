using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace JA.UTILS.Helpers
{
    public class Utils
    {
        public static string AppPath()
        {
            string appPath = "";
            var a = HttpRuntime.AppDomainAppVirtualPath;
            if (a == "/")
            {
                appPath = "";
            }
            else
            {
                appPath = a;
            }
            return appPath;
        }

        /// <summary>
        /// 2005/04/30
        /// </summary>
        public enum AddressType
        {
            localhost,
            Intranet,
            Internet
        }
        /// <summary>
        /// 2005/04/30
        /// http://stackoverflow.com/questions/8113546/how-to-determine-whether-an-ip-address-in-private
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <returns></returns>
        public static bool isIPLocal(IPAddress ipaddress)
        {
            String[] straryIPAddress = ipaddress.ToString().Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            int[] iaryIPAddress = new int[] { int.Parse(straryIPAddress[0]), int.Parse(straryIPAddress[1]), int.Parse(straryIPAddress[2]), int.Parse(straryIPAddress[3]) };
            if (iaryIPAddress[0] == 10 || (iaryIPAddress[0] == 192 && iaryIPAddress[1] == 168) || (iaryIPAddress[0] == 172 && (iaryIPAddress[1] >= 16 && iaryIPAddress[1] <= 31)))
            {
                return true;
            }
            else
            {
                // IP Address is "probably" public. This doesn't catch some VPN ranges like OpenVPN and Hamachi.
                return false;
            }
        }
        /// <summary>
        /// 2005/04/30
        /// https://msdn.microsoft.com/fr-fr/library/system.net.ipaddress.isloopback%28v=vs.110%29.aspx
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static AddressType GetIpAddressType(string ipAddress)
        {
            System.Net.IPAddress address = System.Net.IPAddress.Parse(ipAddress);
            // Perform semantic check by verifying that the address is a valid IPv4 
            // or IPv6 loopback address. 
            if (IPAddress.IsLoopback(address) && address.AddressFamily == AddressFamily.InterNetworkV6)
                return AddressType.localhost;
            else
                if (IPAddress.IsLoopback(address) && address.AddressFamily == AddressFamily.InterNetwork)
                    return AddressType.localhost;

            return isIPLocal(address) ? AddressType.Intranet : AddressType.Internet;
        }
        /// <summary>
        /// 2005/04/17
        /// Gets the currently active Gravatar image URL for the email address supplied to this method call
        /// Throws a <see cref="Gravatar.NET.GravatarEmailHashFailedException"/> if the provided email address is invalid
        /// </summary>
        /// <param name="address">The address to retireve the image for</param>
        /// /// <param name="pars">The available parameters passed by the request to Gravatar when retrieving the image</param>
        /// <returns>The Gravatar image URL</returns>
        public static string GetGravatarUrlForAddress(string address)
        {

            const string GRAVATAR_URL_BASE = "http://s.gravatar.com/avatar/";
            var sbResult = new StringBuilder(GRAVATAR_URL_BASE);
            sbResult.Append(HashString(address));
            sbResult.Append("?");
            // https://ja.gravatar.com/site/implement/images/
            string defaultGravatarImageType = GetAppSetting("DefaultGravatarImageType");
            sbResult.Append("d=");
            sbResult.Append(defaultGravatarImageType);
            sbResult.Append("&");
            sbResult.Append("s=100");


            return sbResult.ToString();
        }


        /// <summary>
        /// 2005/04/17
        /// Hashes a string with MD5.  Suitable for use with Gravatar profile
        /// image urls
        /// </summary>
        /// <param name="myString"></param>
        /// <returns></returns>
        public static string HashString(string myString)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.  
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.  
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(myString));

            // Create a new Stringbuilder to collect the bytes  
            // and create a string.  
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string.  
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();  // Return the hexadecimal string. 
        }
        /// <summary>
        /// This function is used to check specified file being used or not
        /// http://dotnet-assembly.blogspot.fr/2012/10/c-check-file-is-being-used-by-another.html
        /// </summary>
        /// <param name="file">FileInfo of required file</param>
        /// <returns>If that specified file is being processed 
        /// or not found is return true</returns>
        public static Boolean IsFileLocked(string file)
        {
            FileStream stream = null;
            try
            {
                //Don't change FileAccess to ReadWrite, 
                //because if a file is in readOnly, it fails.
                stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            //file is not locked
            return false;
        }
        /// <summary>
        /// 2005/05/15
        /// </summary>
        /// <param name="picture"></param>
        /// <param name="ole"></param>
        /// <returns></returns>
        public static String ByteToStringImage(byte[] picture, int ole)
        {
            // Image in data base ole = 78 (ole Header)
            // http://www.codeproject.com/Articles/90908/Saving-OLE-Object-as-Image-Datatype-in-SQL
            byte[] photo = picture;
            string imageSrc = string.Empty;
            if (photo != null)
            {
                MemoryStream ms = new MemoryStream();
                ms.Write(photo, ole, photo.Length - ole); // strip out 78 byte OLE header (don't need to do this for normal images)
                string imageBase64 = Convert.ToBase64String(ms.ToArray());
                imageSrc = string.Format("data:image/png;base64,{0}", imageBase64);
            }
            return imageSrc;
        }
        public static string GetAssemblyName()
        {
            Assembly assembly = HttpContext.Current.ApplicationInstance.GetType().BaseType.Assembly;
            return assembly.GetName().Name;
        }
        public static DateTime GetAssemblyDateTime()
        {
            Assembly assembly = HttpContext.Current.ApplicationInstance.GetType().BaseType.Assembly;
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(assembly.Location);
            DateTime lastModified = fileInfo.LastWriteTime;
            return lastModified;
        }
        public static string GetAssemblyInformationnalVersion()
        {
            Assembly assembly = HttpContext.Current.ApplicationInstance.GetType().BaseType.Assembly;
            return System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
        }
        public static string GetAssemblyVersion()
        {
            Assembly assembly = HttpContext.Current.ApplicationInstance.GetType().BaseType.Assembly;
            return assembly.GetName().Version.ToString();
        }
        public static string GetAssemblyProduct()
        {
            Assembly assembly = HttpContext.Current.ApplicationInstance.GetType().BaseType.Assembly;
            AssemblyProductAttribute assemblyProductAttribut = (AssemblyProductAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyProductAttribute));
            return assemblyProductAttribut.Product;
        }
        public static string GetAssemblyDescription()
        {
            Assembly assembly = HttpContext.Current.ApplicationInstance.GetType().BaseType.Assembly;
            AssemblyDescriptionAttribute assemblyDescriptionAttribute = (AssemblyDescriptionAttribute)(Attribute.GetCustomAttribute(assembly, typeof(AssemblyDescriptionAttribute), false));
            return assemblyDescriptionAttribute.Description;
        }
        public static string GetAssemblyCompany()
        {
            Assembly assembly = HttpContext.Current.ApplicationInstance.GetType().BaseType.Assembly;
            AssemblyCompanyAttribute assemblyCompanyAttribute = (AssemblyCompanyAttribute)(Attribute.GetCustomAttribute(assembly, typeof(AssemblyCompanyAttribute), false));
            return assemblyCompanyAttribute.Company;
        }
        public static string GetAssemblyCopyright()
        {
            Assembly assembly = HttpContext.Current.ApplicationInstance.GetType().BaseType.Assembly;
            AssemblyCopyrightAttribute assemblyCompanyAttribute = (AssemblyCopyrightAttribute)(Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute), false));
            return assemblyCompanyAttribute.Copyright;
        }
        public static string GetConfigCompanyName()
        {
            return ConfigurationManager.AppSettings["cdf54.CompanyName"].ToString();
        }
        public static string GetConfigCompanyAddress()
        {
            return ConfigurationManager.AppSettings["cdf54.CompanyAddress"].ToString();
        }
        public static string GetUserName()
        {
            string returned = HttpContext.Current.User.Identity.Name;
            returned = returned == "" ? "Guest" : returned;
            return returned;
        }
        public static string GetCulture()
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.ToString();
        }
        public static string GetUiCulture()
        {
            return System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
        }
        public static string GetCnxDefaultConnection()
        {
            string defaultConnection = "No ConnectionString";
            string cnx = string.Empty;
            ConnectionStringSettingsCollection connections = ConfigurationManager.ConnectionStrings;
            if ((cnx = connections["DefaultConnection"].ConnectionString) != string.Empty)
                defaultConnection = cnx;
            return defaultConnection;
        }
        public static bool IsAspNetTraceEnabled()
        {
            return HttpContext.Current.Trace.IsEnabled;
        }
        public static bool IsCustomErrorEnabled()
        {
            return HttpContext.Current.IsCustomErrorEnabled;
        }
        public static bool IsDebuggingEnabled()
        {
            return HttpContext.Current.IsDebuggingEnabled;
        }
        public static string GetCompilationMode()
        {
            string compilationMode = "Release";
            if (IsDebuggingEnabled())
            {
                compilationMode = "Debug";
            }
            return compilationMode;
        }
        public static string GetAppSetting(string name)
        {
            return ConfigurationManager.AppSettings[name].ToString();
        }
        public static string GetClrVersion()
        {
            return System.Environment.Version.ToString();
        }

        /// <summary>
        /// Save photo to disk, used by Edit and Register with two different models.
        /// </summary>
        /// <param name="photo">HttpPostedFileWrapper</param>
        /// <param name="controller">Controller calling</param>
        /// <returns>Path where photo is stored with it's calculated filename, or default photo "BlankPhoto.jpg" or null on error</returns>
        public static string SavePhotoFileToDisk(Object myphoto, System.Web.Mvc.Controller controller, string oldPhotoUrl, bool isNoPhotoChecked)
        {

            HttpPostedFileWrapper photo = (HttpPostedFileWrapper)myphoto;

            string photoPath = string.Empty;
            string fileName = string.Empty;

            // If photo is uploaded calculate his name
            if (photo != null)
            {
                fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
            }
            else
            {
                // if user want to remove his photo
                if (oldPhotoUrl != null && isNoPhotoChecked == true)
                {
                    if (!oldPhotoUrl.Contains("BlankPhoto.jpg"))
                    {
                        string fileToDelete = Path.GetFileName(oldPhotoUrl);
                        var path = Path.Combine(controller.Server.MapPath("~/Content/Avatars"), fileToDelete);
                        FileInfo fi = new FileInfo(path);
                        if (fi.Exists)
                            fi.Delete();
                    }
                }

                // If no previews photo it's a new user who don't provide photo
                if (oldPhotoUrl == null || isNoPhotoChecked == true)
                {
                    fileName = "BlankPhoto.jpg";
                }
                else
                {
                    // User don't want to change his photo
                    return oldPhotoUrl;
                }
            }
            // We save the new/first photo on disk
            try
            {
                string path;
                path = Path.Combine(controller.Server.MapPath("~/Content/Avatars"), fileName);
                photoPath = Path.Combine(HttpRuntime.AppDomainAppVirtualPath, "Content/Avatars", fileName);
                // We save the new/first photo or nothing because BlankPhoto is in the folder
                if (photo != null) photo.SaveAs(path);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                // Handled exception catch code
                //Helpers.Utils.SignalExceptionToElmahAndTrace(ex, controller);
                return null;
            }
            return photoPath;
        }
        /// <summary>
        /// 2005/05/15
        /// Save photo to disk.
        /// </summary>
        /// <param name="myphoto">string</param>
        /// <param name="controller">Controller calling</param>
        /// <returns>Path where photo is stored with it's calculated filename.Or null on error.</returns>
        public static string SaveBytesPhotoFileToDisk(Object myphoto, System.Web.Mvc.Controller controller, string oldPhotoUrl, bool isNoPhotoChecked)
        {

           string dump = (string)myphoto;

            string photoPath = string.Empty;
            string fileName = string.Empty;

            // If photo is uploaded calculate his name
            if (dump != null)
            {
                fileName = Guid.NewGuid().ToString() + ".jpg";
            }

            try
            {
                //Delete old file if exist.
                if (oldPhotoUrl != null)
                {
                    if (!oldPhotoUrl.Contains("BlankPhoto.jpg"))
                    {
                        string fileToDelete = Path.GetFileName(oldPhotoUrl);
                        var path = Path.Combine(controller.Server.MapPath("~/Content/Avatars"), fileToDelete);
                        photoPath = Path.Combine(HttpRuntime.AppDomainAppVirtualPath, "Content/Avatars", fileName);
                        FileInfo fi = new FileInfo(path);
                        if (fi.Exists)
                            fi.Delete();
                    }
                }
                // We save the photo on disk
                var p = Path.Combine(controller.Server.MapPath("~/Content/Avatars"), fileName);
                System.IO.File.WriteAllBytes(p, String_To_Bytes2(dump));
                photoPath = Path.Combine(HttpRuntime.AppDomainAppVirtualPath, "Content/Avatars", fileName);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                // Handled exception catch code
                //Helpers.Utils.SignalExceptionToElmahAndTrace(ex, controller);
                return null;
            }
            return photoPath;
        }
        /// <summary>
        /// 2005/05/15
        /// 
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public static byte[] String_To_Bytes2(string strInput)
        {
            int numBytes = (strInput.Length) / 2;
            byte[] bytes = new byte[numBytes];

            for (int x = 0; x < numBytes; ++x)
            {
                bytes[x] = System.Convert.ToByte(strInput.Substring(x * 2, 2), 16);
            }

            return bytes;
        }

    }


}