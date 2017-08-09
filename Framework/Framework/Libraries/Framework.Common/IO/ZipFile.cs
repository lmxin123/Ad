using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace Framework.Common.IO
{
    /// <summary>
    /// 压缩文件管理类
    /// </summary>
    public class ZipFile
    {
        public static void CreateZipFile(string[] filenames, string zipFilePath, string zipName)
        {
            if (!Directory.Exists(zipFilePath))
                Directory.CreateDirectory(zipFilePath);

            FileStream fileStream = File.Open(zipFilePath + zipName, FileMode.OpenOrCreate);

            using (ZipOutputStream s = new ZipOutputStream(fileStream))
            {
                s.SetLevel(9); // 压缩级别 0-9
                //s.Password = "123"; //Zip压缩文件密码

                byte[] buffer = new byte[4096]; //缓冲区大小
                foreach (string file in filenames)
                {
                    ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                    entry.DateTime = DateTime.Now;
                    s.PutNextEntry(entry);
                    using (FileStream fs = File.OpenRead(file))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = fs.Read(buffer, 0, buffer.Length);
                            s.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }
                }
                s.Finish();
                s.Close();
            }
            fileStream.Dispose();
        }

        public static void UnZipFile(string zipFilePath)
        {
            if (!File.Exists(zipFilePath))
                throw new FileNotFoundException($"未找到文件 '{zipFilePath}'");

            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    Console.WriteLine(theEntry.Name);

                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(theEntry.Name))
                        {

                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
