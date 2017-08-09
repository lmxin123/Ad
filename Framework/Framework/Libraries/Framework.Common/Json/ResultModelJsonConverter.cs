using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Framework.Common.Json;

namespace Framework.Common.Json
{
    public class ResultModelJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(GeneralResponseModel);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            GeneralResponseModel target = new GeneralResponseModel();
            serializer.Populate(reader, target);
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            GeneralResponseModel result = (GeneralResponseModel)value;
            if (result.SimpleJson)
            {
                serializer.Converters.Add(new SimpleJsonConverter(result.JsonFields));
            }

            //miaoxin 2015-02-04
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式  
            timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm";
            serializer.Converters.Add(timeConverter);
            //end miaoxin

            writer.WriteStartObject();

            Type type = typeof(GeneralResponseModel);
            Type jsonIgnoreAttrType = typeof(JsonIgnoreAttribute);

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);
            foreach (PropertyDescriptor prop in properties)
            {
                JsonIgnoreAttribute jsonIgnore = (JsonIgnoreAttribute)prop.Attributes[jsonIgnoreAttrType]; ;

                if (jsonIgnore == null)
                {
                    object val = prop.GetValue(result);
                    if (val != null && (!(val is string) || (val is string && (string)val != "")))
                    {
                        writer.WritePropertyName(prop.Name);
                        serializer.Serialize(writer, val);
                    }
                }
            }

            writer.WriteEndObject();
        }
    }
}
