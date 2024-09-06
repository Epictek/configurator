using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace OpenIPC_Configurator.Helpers;

public class YamlReader
{
    public static Dictionary<string, string> ReadYamlToDictionary(Stream stream)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance) // Optional: handle underscores in YAML keys
            .Build();
            using var reader = new StreamReader(stream);

            var yamlObject = deserializer.Deserialize(reader);

            if (yamlObject == null)
                throw new InvalidOperationException("Failed to deserialize YAML.");

            var result = new Dictionary<string, string>();

            FlattenYamlObject(result, yamlObject, "");

            return result;
    }

    private static void FlattenYamlObject(Dictionary<string, string> result, object yamlObject, string parentKey)
    {
        if (yamlObject is Dictionary<object, object> dict)
        {
            foreach (var entry in dict)
            {
                var key = parentKey + "." + entry.Key.ToString()!.ToLowerInvariant();
                if (entry.Value is Dictionary<object, object>)
                {
                    FlattenYamlObject(result, entry.Value, key);
                }
                else
                {
                    result[key.TrimStart('.')] = entry.Value.ToString()!;
                }
            }
        }
        else
        {
            throw new ArgumentException("YAML structure is not valid.");
        }
    }
}
