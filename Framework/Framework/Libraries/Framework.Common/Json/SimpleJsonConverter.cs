using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Framework.Common.Json
{
    public class SimpleJsonConverter : JsonConverter
    {
        public string Fields { get; set; }

        public SimpleJsonConverter() { }

        public SimpleJsonConverter(string fields)
        {
            Fields = fields;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (typeof(JObject) == value.GetType())
            {
                JObjectWriteJson(writer, value, serializer);
            }
            else
            {
                EntityBaseWriteJson(writer, value, serializer);
            }
        }

        private void JObjectWriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            Type type = value.GetType();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);

            List<string> fields = null;
            if (!string.IsNullOrEmpty(this.Fields))
            {
                fields = new List<string>();
                fields.AddRange(Fields.Split(','));
            }


            JObject jobj = (JObject)value;

            foreach (JProperty prop in jobj.Properties())
            {
                bool canSerializer = fields == null || fields.Exists(F => F == prop.Name);

                if (canSerializer)
                {
                    writer.WritePropertyName(prop.Name);
                    serializer.Serialize(writer, prop.Value);
                }
            }

            writer.WriteEndObject();
        }

        private void EntityBaseWriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            Type type = value.GetType();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);

            List<string> fields = null;
            if (!string.IsNullOrEmpty(Fields))
            {
                fields = new List<string>();
                fields.AddRange(Fields.Split(','));
            }

            Type jsonIgnoreAttrType = typeof(JsonIgnoreAttribute);
            Type jsonConverterAttrType = typeof(JsonConverterAttribute);

            foreach (PropertyDescriptor prop in properties)
            {
                bool canSerializer = true;
                if (fields != null)
                {
                    canSerializer = fields.Exists(F => F == prop.Name);
                }
                else
                {
                    JsonIgnoreAttribute jsonIgnore = (JsonIgnoreAttribute)prop.Attributes[jsonIgnoreAttrType];
                    canSerializer = jsonIgnore == null;
                }
                if (canSerializer)
                {
                    JsonConverter converter = null;
                    JsonConverterAttribute converterAttr = (JsonConverterAttribute)prop.Attributes[jsonConverterAttrType];
                    if (converterAttr != null)
                    {
                        converter = (JsonConverter)Activator.CreateInstance(converterAttr.ConverterType);
                        serializer.Converters.Add(converter);
                    }

                    writer.WritePropertyName(prop.Name);
                    serializer.Serialize(writer, prop.GetValue(value));
                }
            }

            writer.WriteEndObject();
        }
    }
}
