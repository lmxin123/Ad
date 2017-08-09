using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;

namespace Framework.Common.IO
{
    /// <summary>
    /// 文件操作类，所有的路径默认都是基于StorageRoot目录下进行操作
    /// 不支持远程存储
    /// </summary>
    public class FileHelper
    {
        string[] imgExtansions = new string[] { ".jpg", ".jpeg", ".gif", ".bmp", ".png" };
        string[] vedioExtansions = new string[] { ".mp4", ".mpeg", ".mpg", ".avi", ".wmv", ".flv" };

        /// <summary>
        /// 存储图片的路径
        /// </summary>
        public string StorageImagePath { get; set; }
        /// <summary>
        /// 存储视频的路径
        /// </summary>
        public string StorageVedioPath { get; set; }
        /// <summary>
        ///默认的文件存储路径
        /// </summary>
        public string StorageDefaultPath { get; set; }
        /// <summary>
        /// 缩略图片存储路径
        /// </summary>
        public static string StorageThumbPath
        {
            get
            {
                return "/Content/thumbnail/";
            }
        }
        /// <summary>
        /// 缩略图宽
        /// </summary>
        public int ThumbWidth { get; set; }
        /// <summary>
        /// 缩略图高
        /// </summary>
        public int ThumbHeight { get; set; }
        private string _urlBase;
        /// <summary>
        /// 根url地址
        /// </summary>
        public string UrlBase
        {
            get
            {
                return string.IsNullOrEmpty(_urlBase) ? string.Empty : _urlBase.Contains("http") ? _urlBase : "http://" + _urlBase;
            }
            set
            {
                _urlBase = value.Contains("http") ? value : "http://" + value; ;
            }
        }
        public FileHelper(string storageDefaultPath, string urlBase, int thumbWidth = 80, int thumbHeight = 80)
        {
            StorageDefaultPath = storageDefaultPath;
            _urlBase = urlBase;
            ThumbWidth = thumbWidth;
            ThumbHeight = thumbHeight;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageImgaePath"></param>
        /// <param name="urlBase"></param>
        /// <param name="thumbPath"></param>
        /// <param name="thumbWidth"></param>
        /// <param name="thumbHeight"></param>
        public FileHelper(string storageImgaePath, string storageVedioPath, string urlBase, string storageDefaultPath = "", int thumbWidth = 80, int thumbHeight = 80)
        {
            StorageImagePath = storageImgaePath;
            StorageVedioPath = storageVedioPath;
            StorageDefaultPath = storageDefaultPath;
            _urlBase = urlBase;
            ThumbWidth = thumbWidth;
            ThumbHeight = thumbHeight;
        }
        /// <summary>
        /// 删除文件夹下面的所有文件，以及子目录下的文件
        /// </summary>
        /// <param name="pathToDelete"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool DeleteFiles(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentNullException("fullPath 参数不能为空！");

            if (Directory.Exists(fullPath))
            {
                DirectoryInfo di = new DirectoryInfo(fullPath);
                foreach (FileInfo fi in di.GetFiles())
                {
                    DeleteFile(fi.FullName);
                }
                di.Delete(true);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 删除单个文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool DeleteFile(string fullPath)
        {
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                string[] paths = fullPath.Split('/');
                if (paths.Length == 1)
                {
                    paths = fullPath.Split('\\');
                }
                string fileName = paths[paths.Length - 1];
                string thumbFullName = HttpContext.Current.Server.MapPath(StorageThumbPath + fileName);
                if (File.Exists(thumbFullName))
                    File.Delete(thumbFullName);

                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取目录下的所有文件
        /// </summary>
        /// <returns></returns>
        public List<UploadFilesResult> GetFileList(string path)
        {
            var files = new List<UploadFilesResult>();

            if (Directory.Exists(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                foreach (FileInfo file in dir.GetFiles())
                {
                    files.Add(UploadResult(file.Name, file.Length, file.FullName));
                }
            }

            return files;
        }

        public List<UploadFilesResult> Upload(HttpContextBase ContentBase)
        {
            var resultList = new List<UploadFilesResult>();
            var httpRequest = ContentBase.Request;

            foreach (string inputTagName in httpRequest.Files)
            {
                var headers = httpRequest.Headers;

                var file = httpRequest.Files[inputTagName];

                if (string.IsNullOrEmpty(headers["X-File-Name"]))
                {
                    resultList.Add(UploadWholeFile(file));
                }
                else
                {
                    resultList.Add(UploadPartialFile(headers["X-File-Name"], ContentBase));
                }
            }
            return resultList;
        }

        /// <summary>
        /// 整个文件上传并保存
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="statuses"></param>
        private UploadFilesResult UploadWholeFile(HttpPostedFileBase file)
        {
            var result = new UploadFilesResult();
            string fileName = file.FileName.Length > 14 ? file.FileName.Substring(file.FileName.Length - 14) : file.FileName;
            string fullPath = GetStorageFullName(fileName);

            file.SaveAs(fullPath);
            //Create thumb
            if (imgExtansions.Contains(Utility.GetFileSuffix(fileName).ToLower()))
            {
                var thumbFile = GetStorageFullName(fileName, true);
                using (Image tempimg = Image.FromFile(fullPath))
                {
                    Bitmap originalImage = new Bitmap(tempimg, ThumbWidth, ThumbHeight);
                    originalImage.Save(thumbFile);
                }
            }
            result = UploadResult(fileName, file.ContentLength, fullPath);

            return result;
        }
        /// <summary>
        /// 单点续传
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        private UploadFilesResult UploadPartialFile(string fileName, HttpContextBase requestContext)
        {
            var result = new UploadFilesResult();
            var request = requestContext.Request;
            if (request.Files.Count != 1)
                throw new HttpRequestValidationException("试图上传分块包含每个请求的多个片段文件");

            var file = request.Files[0];
            var inputStream = file.InputStream;
            var fullName = GetStorageFullName(fileName);
            var thumbfullName = GetStorageFullName(fileName, true);

            ImageHelper imgHelper = new ImageHelper();

            var imageBit = ImageHelper.LoadImage(fullName);
            imgHelper.Save(imageBit, ThumbWidth, ThumbHeight, 10, thumbfullName);

            using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
            }

            result = UploadResult(file.FileName, file.ContentLength, file.FileName);
            return result;
        }
        private UploadFilesResult UploadResult(string name, long size, string path)
        {
            string fileType = MimeMapping.GetMimeMapping(name);
            var result = new UploadFilesResult()
            {
                name = name,
                size = size,
                type = fileType,
                path = path,
                //Url = urlBase + name,
                thumbnailUrl = GetThumbUrl(fileType, name),
            };
            return result;
        }
        /// <summary>
        /// 获取缩略图的类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetThumbUrl(string type, string name)
        {
            var splited = type.Split('/');
            if (splited.Length == 2)
            {
                string extansion = splited[1];
                string thumbs = string.Empty;
                switch (extansion)
                {
                    case "jpeg":
                    case "jpg":
                    case "png":
                    case "gif":
                        thumbs = string.Concat(StorageThumbPath ?? StorageDefaultPath, name);
                        break;
                    case "octet-stream":
                        thumbs = "/Content/Free-file-icons/48px/exe.png";
                        break;
                    case "zip":
                        thumbs = "/Content/Free-file-icons/48px/zip.png";
                        break;
                    default:
                        thumbs = "/Content/Free-file-icons/48px/" + extansion + ".png";
                        break;
                }
                return UrlBase + thumbs;
            }
            return string.Concat(UrlBase, StorageThumbPath ?? StorageDefaultPath, name);
        }
        /// <summary>
        /// 根据文件后续判断存储路径
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isThumb"></param>
        /// <returns></returns>
        public string GetStorageFullName(string name, bool isThumb = false)
        {
            string path = string.Empty,
                extansion = Utility.GetFileSuffix(name);
            if (isThumb)
            {
                path = HttpContext.Current.Server.MapPath(StorageThumbPath ?? StorageDefaultPath);
            }
            else if (imgExtansions.Contains(extansion))
            {
                path = StorageImagePath ?? StorageDefaultPath;
            }
            else if (vedioExtansions.Contains(extansion))
            {
                path = StorageVedioPath ?? StorageDefaultPath;
            }
            else
            {
                path = StorageDefaultPath;
            }
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path + name;
        }
    }
    public class UploadFilesResult
    {
        public string name { get; set; }
        public long size { get; set; }
        public string type { get; set; }
        public string path { get; set; }
        public string url { get; set; }
        public string redirectUrl { get; set; }
        public string deleteUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public string deleteType { get; set; } = "GET";
    }

    public class UploadJsonFiles
    {
        public UploadFilesResult[] files;
        public string TempFolder { get; set; }
        public UploadJsonFiles(List<UploadFilesResult> filesList)
        {
            files = new UploadFilesResult[filesList.Count];
            for (int i = 0; i < filesList.Count; i++)
            {
                files[i] = filesList.ElementAt(i);
            }
        }
    }
}

