using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Framework.Common.Xml
{
    public static class Serializer
    {
        /// <summary>
        /// 序列化 Object -> XML
        /// </summary>
        /// <param name="obj">Object 源</param>
        /// <param name="needXmlDeclaration">生成的 XML 中是否保留 xml 修饰(xml version="1.0" encoding="UTF-8" 之类) </param>
        /// <param name="needNamespace">生成的 XML 中是否保留 namespace</param>
        /// <param name="needFormatXML">注意：格式化后xml是不带BOM，不格式化会带BOM</param>
        /// <returns>目标 XML</returns>
        public static string Object2XML(object obj, bool needXmlDeclaration = false, bool needNamespace = false, bool needFormatXML = true)
        {
            if (obj == null)
                return string.Empty;

            XmlWriterSettings settings = new XmlWriterSettings();
            //去除xml声明
            settings.OmitXmlDeclaration = !needXmlDeclaration;
            settings.Encoding = new UTF8Encoding(false);// Encoding.UTF8;
            string result = null;

            using (MemoryStream mem = new MemoryStream())
            {
                result = Object2XML(obj, needNamespace, settings, mem);
            }


            if (needFormatXML)
            {
                string formattedResult = null;
                try
                {
                    using (var ms = new MemoryStream(settings.Encoding.GetBytes(result)))
                    {
                        formattedResult = SerializerHelper.FormatXML(ms);
                    }
                }
                catch (Exception e) { }

                if (!string.IsNullOrEmpty(formattedResult))
                    return formattedResult;
            }

            return result;
        }

        /// <summary>
        /// Serializes current  object into an XML document
        /// </summary>
        /// <returns>string XML value</returns>
        public static string Serialize<T>(T xml, bool isThrowException = false)
        {
            System.IO.StreamReader streamReader = null;
            System.IO.MemoryStream memoryStream = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                memoryStream = new System.IO.MemoryStream();
                serializer.Serialize(memoryStream, xml);
                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                streamReader = new System.IO.StreamReader(memoryStream);
                return streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                if (isThrowException) throw;
                else return null;
            }
            finally
            {
                if ((streamReader != null))
                {
                    streamReader.Dispose();
                }
                if ((memoryStream != null))
                {
                    memoryStream.Dispose();
                }
            }
        }

        private static string Object2XML(Object obj, bool needNamespace, XmlWriterSettings settings, MemoryStream mem)
        {
            using (XmlWriter writer = XmlWriter.Create(mem, settings))
            {
                if (needNamespace)
                {
                    XmlSerializer formatter = new XmlSerializer(obj.GetType());
                    formatter.Serialize(writer, obj);
                }
                else
                {
                    //去除默认命名空间xmlns:xsd和xmlns:xsi
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    XmlSerializer formatter = new XmlSerializer(obj.GetType());
                    formatter.Serialize(writer, obj, ns);
                }
            }
            return settings.Encoding.GetString(mem.ToArray());
        }


        /// <summary>
        /// Load xml to object
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="xml">xml string</param>
        /// <param name="needLog">need log or not</param>
        /// <returns>return the object</returns>
        public static T XML2Object<T>(string xml, bool isThrowException = false) where T : class
        {
            StringReader reader = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                reader = new StringReader(xml);
                return (T)serializer.Deserialize(reader);
            }
            catch (Exception e)
            {
                if (isThrowException) throw;
                else return null;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                }
            }
        }

        /// <summary>
        /// Load xml file to object
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="fileName">Full file name (Absolute path)</param>
        /// <param name="needLog">need log</param>
        /// <returns>return the object</returns>
        public static T XML2ObjectFromFile<T>(string fileName) where T : class
        {
            FileStream fs = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                return (T)serializer.Deserialize(fs);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// Load xml to object
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="stream">xml stream</param>
        /// <param name="needLog">need log</param>
        /// <returns>return the object</returns>
        public static T XML2Object<T>(Stream stream, bool needLog = false) where T : class
        {
            using (var sr = new StreamReader(stream))
            {
                return XML2Object<T>(sr.ReadToEnd(), needLog);
            }
        }
    }
}
