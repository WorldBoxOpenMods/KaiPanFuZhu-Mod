using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Diplomacy_Army
{
    public class DA_save
    {
        public static void SaveDictionaryToFile(Dictionary<string, bool> dictionary, string filePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            string json = JsonConvert.SerializeObject(dictionary, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static Dictionary<string, bool> LoadDictionaryFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<Dictionary<string, bool>>(json);
            }
            else
            {
                Debug.LogWarning("File not found: " + filePath);
                return new Dictionary<string, bool>();
            }
        }

        #region 对类进行处理的Json
        public static void SaveToFile(string filePath, Dictionary<string, NationalTraits> nationalTraits)
        {
            string json = JsonConvert.SerializeObject(nationalTraits, Formatting.Indented);
            File.WriteAllText(filePath, json);
            Console.WriteLine("Saved to file: " + filePath);
        }

        public static Dictionary<string, NationalTraits> LoadFromFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Dictionary<string, NationalTraits>>(json);
        }
        #endregion
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class DASet
    {
        public Guid Id { get; set; }
        [JsonProperty]
        public string Name = "NewSet";
        [JsonProperty]
        public bool Set = true;
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class DAStorage
    {
        public Guid Id { get; set; }
        [JsonProperty]
        public string Name = "NewStorage";
        public int nom = 1;
        public int num = 0;
        public int RS = 0;
    }
}