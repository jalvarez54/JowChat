using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace Cdf54.Ja.SignalR.Chat.CustomFiltersAttributes
{
    public class MyCustomAttributes
    {
        /// <summary>
        ///  Add extension
        /// Customized data annotation validator for uploading file
        /// http://www.dotnet-tricks.com/Tutorial/mvc/aX9D090113-File-upload-with-strongly-typed-view-and-model-validation.html
        /// </summary>
        public class ValidateFileAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                string uri = HttpContext.Current.Request.Url.ToString();
                int maxContentLength = 1024 * 1024 * 3; // photo color = 3 x 8 bits = 3 bytes
                int maxHeight = 200;
                int maxWidth = 200;
                string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png" };
                var file = value as HttpPostedFileBase;
                // user don't want photo is permitted
                if (file == null)
                    return true;
                // test photo type
                else if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                {
                    ErrorMessage = string.Format("Please upload Your Photo of type: {0}", string.Join(", ", AllowedFileExtensions));
                    return false;
                }
                // test photo size
                else if (file.ContentLength > maxContentLength)
                {
                    ErrorMessage = string.Format("Your Photo is too large, maximum allowed size is : {0} MB", (maxContentLength / 1024).ToString());
                    return false;
                }
                else
                {
                    // test photo dimensions
                    using (System.Drawing.Image myImage = System.Drawing.Image.FromStream(file.InputStream))
                    {
                        if (myImage.Height > maxHeight && myImage.Width > maxWidth)
                        {
                            ErrorMessage = string.Format("Your Photo is too large, maximum allowed size is : Width {0} x Height {1} pixels", maxWidth, maxHeight);
                            return false;
                        }
                    }
                }
                return true;
            }
        }

    }
}