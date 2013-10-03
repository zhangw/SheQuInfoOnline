using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;
using System.IO;

namespace Wedo.Mvc.Utility.Common
{
    public class FileHelpers
    {
        public static string GetRecivePath(params string[] relativePaths)
        {
            string path = null;
            if (relativePaths != null && relativePaths.Length > 0)
            {
                path = Path.Combine(GetBasePath(), Path.Combine(relativePaths));
            }
            else
            {
                throw new Exception("请输入文件路径");
            }
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            return path;
            
        }

        private static string  GetBasePath()
        {
            return System.Configuration.ConfigurationManager.AppSettings["BasePath"] ?? "C:\\BasePath";
        }

        public static string SaveFile(HttpPostedFileBase file, string relativePath="upload", string Prefix=null)
        {
            string fileType = (System.Configuration.ConfigurationManager.AppSettings["uploadFileType"]??"").ToLower();
            string nowFileType = System.IO.Path.GetExtension(file.FileName);
            if (fileType.IsNotNullOrWhiteSpace() && !fileType.Contains(nowFileType))
            {
                throw new Exception(string.Format("{1} 文件类型才能被上传,{0} 文件类型错误！", file.FileName, fileType));
            }

            var uploadFileSizeLimitMax = ConfigurationManager.AppSettings["uploadFileSizeLimitMax"].TryParseTo<int>();
            if (uploadFileSizeLimitMax.HasValue && file.ContentLength > uploadFileSizeLimitMax)
            {
                throw new Exception(string.Format("{1} 文件太大, 最大的文件容量是 {0}KB", uploadFileSizeLimitMax / 1024.0, file.FileName));
            }

            string filename = System.IO.Path.GetFileName(file.FileName);
            if (Prefix.IsNotNullOrWhiteSpace())
                filename = Prefix +"_"+ filename;
            string filePath =GetRecivePath(relativePath,filename);

            string dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            file.SaveAs(filePath);
            return filePath;
        }
    }
}
