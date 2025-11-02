using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ParamEditor.Models
{
    internal class ParameterValueLoader
    {
        public static Dictionary<string, string?> LoadValues(string path)
        {
            if (!File.Exists(path))
                return new Dictionary<string, string?>();
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            using var reader = new StreamReader(path);
            var values = deserializer.Deserialize<Dictionary<string, object>>(reader);
            var dict = new Dictionary<string, string?>();
            foreach (var kvp in values)
                dict[kvp.Key] = kvp.Value?.ToString();
            return dict;
        }
    }
}
