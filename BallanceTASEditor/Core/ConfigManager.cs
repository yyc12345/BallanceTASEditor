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
            _defaultValue = defaultValue;
            
            Configuration = Read();
        }

        string _fileName;
        Dictionary<string, string> _defaultValue;
        public Dictionary<string, string> Configuration;

        public static readonly string CfgNode_Language = "Language";
        public static readonly string CfgNode_ItemCount = "ItemCount";
        public static readonly string CfgNode_IsHorizonLayout = "IsHorizonLayout";
        public static readonly string CfgNode_IsOverwrittenPaste = "IsOverwrittenPaste";

        Dictionary<string, string> Read() {
            if (!File.Exists(Path.Combine(Environment.CurrentDirectory, _fileName)))
                Init();

            Dictionary<string, string> data;
            using (StreamReader fs = new StreamReader(Path.Combine(Environment.CurrentDirectory, _fileName), Encoding.UTF8)) {
                data = JsonConvert.DeserializeObject<Dictionary<string, string>>(fs.ReadToEnd());
                fs.Close();
            }

            // check field to make sure each field is existed
            // because version update it might be changed
            foreach(var pair in _defaultValue) {
                if (!data.ContainsKey(pair.Key)) {
                    data.Add(pair.Key, pair.Value);
                }
            }

            return data;
        }

        void Init() {
            using (StreamWriter fs = new StreamWriter(Path.Combine(Environment.CurrentDirectory, _fileName), false, Encoding.UTF8)) {
                fs.Write(JsonConvert.SerializeObject(_defaultValue));
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
