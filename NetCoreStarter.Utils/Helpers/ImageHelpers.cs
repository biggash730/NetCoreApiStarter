using Newtonsoft.Json;
using RestSharp;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;

namespace NetCoreStarter.Utils
{
    public class ImageHelpers
    {
        private const string ImgUrClientId = "";
        public static string UploadImageToImgUr(string base64Data)
        {
            base64Data = base64Data.Replace("data:image/png;base64,", "");
            base64Data = base64Data.Replace("data:image/jpeg;base64,", "");
            base64Data = base64Data.Replace("data:image/jpg;base64,", "");
            base64Data = base64Data.Replace("data:image/gif;base64,", "");
            var auth = "Client-ID " + ImgUrClientId;

            var client = new RestClient("https://api.imgur.com/3/image");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", auth);
            request.AddHeader("Accept", "application/json");

            request.AddParameter("image", base64Data);
            request.AddParameter("type", "base64");

            var response = client.Execute(request);
            if (response.ResponseStatus != ResponseStatus.Completed) throw new Exception("Could not upload image to imgur. Try again!!");

            var res = JsonConvert.DeserializeObject<ImgUrResultObj>
                (response.Content);
            if (!res.Success) throw new Exception("Could not upload image to imgur. Try again!!");

            return res.Data.Link;
        }

        public static byte[] GetImageBytesFromDataUrl(string dataUrl)
        {
            var base64String = Regex.Match(dataUrl, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            return Convert.FromBase64String(base64String);
        }

        public static Image GetImageFromDataUrl(string dataUrl)
        {
            var base64Data = Regex.Match(dataUrl, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            return GetImageFromBase64(base64Data);
        }

        public static string CompressedImage(string picture)
        {
            var image = GetImageFromDataUrl(picture);
            var thumb = image.GetThumbnailImage(image.Width, image.Height, null, IntPtr.Zero);
            //todo: check the datatype
            //var dataType = picture.Substring(',');
            var dataType = picture.Split(',')[0];
            return $"{dataType},{ImageToBase64(thumb)}";
        }

        public static Image GetImageFromBase64(string base64String)
        {
            var imageBytes = Convert.FromBase64String(base64String);
            Image image;
            using (var ms = new MemoryStream(imageBytes))
            {
                image = Image.FromStream(ms);
            }
            return image;
        }

        public static string ImageToBase64(Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                var imageBytes = ms.ToArray();

                var base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
    }

    public class ImgUrResultObj
    {
        public ImgUrResultData Data { get; set; }
        public string Status { get; set; }
        public bool Success { get; set; }
    }

    public class ImgUrResultData
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Link { get; set; }
    }
}
