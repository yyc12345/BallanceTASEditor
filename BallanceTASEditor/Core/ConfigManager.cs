using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace BallanceTASEditor.Core {
    public class ConfigManager {

        public ConfigManager(string fileName, Dictionary<string, string> defaultValue) {
            _fileName = fileName;
            _defaultValue = JsonConvert.SerializeObject(defaultValue);
            
            Configuration = Read();
        }

        string _fileName;
        string _defaultValue;
        public Dictionary<string, string> Configuration;

        public static readonly string CfgNode_Language = "Language";
        public static readonly string CfgNode_ItemCount = "ItemCount";
        public static readonly string CfgNode_IsHorizonLayout = "IsHorizonLayout";

        Dictionary<string, string> Read() {
            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, _fileName)))
                Init();

            Dictionary<string, string> data;
            using (StreamReader fs = new StreamReader(Path.Combine(Environment.CurrentDirectory, _fileName), Encoding.UTF8)) {
                data = JsonConvert.DeserializeObject<Dictionary<string, string>>(fs.ReadToEnd());
                fs.Close();
            }

            return data;
        }

        void Init() {
            using (StreamWriter fs = new StreamWriter(Path.Combine(Environment.CurrentDirectory, _fileName), false, Encoding.UTF8)) {
                fs.Write(_defaultValue);
                fs.Close();
            }
        }

        public void Save() {
            using (StreamWriter fs = new StreamWriter(Path.Combine(Environment.CurrentDirectory, _fileName), false, Encoding.UTF8)) {
                fs.Write(JsonConvert.SerializeObject(this.Configuration));
                fs.Close();
            }
        }

    }

}
