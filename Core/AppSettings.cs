using System;
using System.IO;

namespace RobloxExecutor.Core
{
    public static class AppSettings
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        public static bool AlwaysOnTop { get; set; } = false;
        public static bool AutoInject { get; set; } = false;
        public static bool AutoExecute { get; set; } = false;

        public static void Load()
        {
            if (!File.Exists(FilePath)) return;

            try
            {
                string content = File.ReadAllText(FilePath);
                AlwaysOnTop = GetValue(content, "AlwaysOnTop");
                AutoInject = GetValue(content, "AutoInject");
                AutoExecute = GetValue(content, "AutoExecute");
            }
            catch { }
        }

        public static void Save()
        {
            try
            {
                string content = "{\n" +
                    $"  \"AlwaysOnTop\": {AlwaysOnTop.ToString().ToLower()},\n" +
                    $"  \"AutoInject\": {AutoInject.ToString().ToLower()},\n" +
                    $"  \"AutoExecute\": {AutoExecute.ToString().ToLower()}\n" +
                    "}";
                File.WriteAllText(FilePath, content);
            }
            catch { }
        }

        private static bool GetValue(string json, string key)
        {
            try
            {
                int keyIndex = json.IndexOf($"\"{key}\"");
                if (keyIndex == -1) return false;

                int colonIndex = json.IndexOf(":", keyIndex);
                int commaIndex = json.IndexOf(",", colonIndex);
                if (commaIndex == -1) commaIndex = json.IndexOf("}", colonIndex);

                string valStr = json.Substring(colonIndex + 1, commaIndex - colonIndex - 1).Trim();
                return valStr.Equals("true", StringComparison.OrdinalIgnoreCase);
            }
            catch { return false; }
        }
    }
}
