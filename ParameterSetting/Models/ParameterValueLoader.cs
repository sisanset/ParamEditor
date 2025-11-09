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
using System.Text.Json;
using Microsoft.Win32;

namespace ParamEditor.Models
{

    public static class ConfigFileHandler
    {
        public static Dictionary<object, object> Load(string path)
        {
            if (!File.Exists(path))
                return new Dictionary<object, object>();
            var text = File.ReadAllText(path);
            var ext = Path.GetExtension(path).ToLower();
            if (ext == ".json")
            {
                var jsonDict = JsonSerializer.Deserialize<Dictionary<string, object>>(text);
                return ConvertJsonObject(jsonDict);
            }
            else if (ext == ".yaml" || ext == ".yml")
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                return deserializer.Deserialize<Dictionary<object, object>>(text);
            }
            throw new NotSupportedException($"Unsupported file extension: {ext}");
        }
        public static void Save(Dictionary<string, object?> data, string filepath)
        {
            var ext = Path.GetExtension(filepath).ToLower();
            if (ext == ".json")
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                var json = JsonSerializer.Serialize(data, options);
                File.WriteAllText(filepath, json);
            }
            else if (ext == ".yaml" || ext == ".yml")
            {
                var serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                var yaml = serializer.Serialize(data);
                File.WriteAllText(filepath, yaml);
            }
            else
            {
                throw new NotSupportedException($"Unsupported file extension: {ext}");
            }
        }
        private static Dictionary<object, object> ConvertJsonObject(Dictionary<string, object>? jsonDict)
        {
            var result = new Dictionary<object, object>();
            if (jsonDict == null)
                return result;
            foreach (var kv in jsonDict)
            {
                if (kv.Value is JsonElement jsonElement)
                {
                    result[kv.Key] = ConvertJsonElement(jsonElement);
                }
                else
                {
                    result[kv.Key] = kv.Value!;
                }
            }
            return result;
        }
        private static object ConvertJsonElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    var dict = new Dictionary<object, object>();
                    foreach (var prop in element.EnumerateObject())
                    {
                        dict[prop.Name] = ConvertJsonElement(prop.Value);
                    }
                    return dict;
                case JsonValueKind.Array:
                    var list = new List<object>();
                    foreach (var item in element.EnumerateArray())
                    {
                        list.Add(ConvertJsonElement(item));
                    }
                    return list;
                case JsonValueKind.String:
                    return element.GetString() ?? "";
                case JsonValueKind.Number:
                    if (element.TryGetInt64(out long l))
                        return l;
                    if (element.TryGetDouble(out double d))
                        return element.GetDouble();
                    return element.GetRawText();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Null:
                    return null!;
                default:
                    return element.GetRawText();
            }
        }
    }
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
            var node = ConfigFileHandler.Load(path);
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
                {
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
            ConfigFileHandler.Save(dict, paramPath);
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
