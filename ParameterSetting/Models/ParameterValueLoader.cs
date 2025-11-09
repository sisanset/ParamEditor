using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Diagnostics;
using ParamEditor.ViewModels;
using System.Printing.IndexedProperties;

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
        public static Dictionary<string, string?> LoadParameters(string path, SchemaRoot schema)
        {
            var yaml = new Dictionary<object, object>();
            if (!File.Exists(path))
                return new Dictionary<string, string?>();
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            using var reader = new StreamReader(path);

            var node = deserializer.Deserialize<Dictionary<object, object>>(reader);
            var dict = new Dictionary<string, string?>();
            foreach (var paramDef in schema.Parameters)
            {
                var value = GetNestedValue(node, paramDef.Name);
                dict[paramDef.Name] = value?.ToString();
            }
            return dict;
        }
        public static object? GetNestedValue(Dictionary<object, object> node, string path)
        {
            var parts = path.Split('/');
            object? current = node;
            foreach (var part in parts)
            {
                if (current is Dictionary<object, object> dict && dict.TryGetValue(part, out var next))
                //if (current is Dictionary<object, object> dict)
                {
                    //dict.Keys.ToList().ForEach(k => Debug.WriteLine($"Key: {k}"));
                    //Debug.WriteLine($"Looking for part: {part}");
                    //var b = dict.TryGetValue(part, out var next);
                    //Debug.WriteLine(b);
                    current = next;
                }
                else
                {
                    return null;
                }
            }
            return current;
        }
        public static void SaveParameters(string paramPath, Dictionary<string, string?> parameters)
        {
            var dict = new Dictionary<string, object?>();
            foreach (var param in parameters)
            {
                SetNestedValue(dict, param.Key, param.Value);
            }
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            var yaml = serializer.Serialize(dict);
            File.WriteAllText(paramPath, yaml);
        }
        public static void SetNestedValue(Dictionary<string, object?> node, string path, string? value)
        {
            var parts = path.Split('/');
            for (int i = 0; i < parts.Length; i++)
            {
                var key = parts[i];
                if (i == parts.Length - 1)
                {
                    node[key] = value;
                }
                else
                {
                    if (!node.ContainsKey(key) || node[key] is not Dictionary<string, object?>)
                    {
                        node[key] = new Dictionary<string, object?>();
                    }
                    node = (Dictionary<string, object?>)node[key]!;
                }
            }
        }
    }
}
