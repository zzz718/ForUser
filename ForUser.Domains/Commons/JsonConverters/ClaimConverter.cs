using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ForUser.Domains.Commons.JsonConverters
{
    public class ClaimConverter:JsonConverter<Claim>
    {
        public override Claim Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }
            // 读取 JSON 对象的每个属性
            string type = default;
            string val = default;

            while (reader.Read())
            {
                // 处理属性名
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    type = reader.GetString();

                    reader.Read();
                    val = reader.GetString();
                }

                // 处理对象结束
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }
            }

            return new Claim(type, val);
        }

        public override void Write(Utf8JsonWriter writer, Claim value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(value.Type, value.Value);
            writer.WriteEndObject();
        }
    }
}
