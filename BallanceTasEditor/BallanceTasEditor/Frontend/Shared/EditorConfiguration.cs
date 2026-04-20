using BallanceTasEditor.Frontend.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Frontend.Shared {
    public class EditorConfiguration {

        public static readonly EditorConfiguration Instance = new EditorConfiguration();

        private EditorConfiguration() {
            Init();
        }

        public SequenceKind SequenceKind { get; set; }
        public EditorLayoutKind EditorLayout { get; set; }
        public EditorPasteBehavior PasteBehavior { get; set; }
        public int FrameCount { get; set; }
        public string GamePath { get; set; }

        [MemberNotNull(nameof(SequenceKind))]
        [MemberNotNull(nameof(EditorLayout))]
        [MemberNotNull(nameof(PasteBehavior))]
        [MemberNotNull(nameof(FrameCount))]
        [MemberNotNull(nameof(GamePath))]
        private void FromRaw(RawEditorConfiguration raw) {
            SequenceKind = raw.SequenceKind;
            EditorLayout = raw.EditorLayout;
            PasteBehavior = raw.PasteBehavior;
            FrameCount = raw.FrameCount;
            GamePath = raw.GamePath;
        }

        private RawEditorConfiguration ToRaw() {
            return new RawEditorConfiguration {
                SequenceKind = SequenceKind,
                EditorLayout = EditorLayout,
                PasteBehavior = PasteBehavior,
                FrameCount = FrameCount,
                GamePath = GamePath
            };
        }

        private string GetSettingFilePath() {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string selfAppDataPath = Path.Combine(appDataPath, "BallanceTasEditor");
            return Path.Combine(selfAppDataPath, "BallanceTasEditor.cfg");
        }

        private bool Load() {
            try {
                using (var fs = new StreamReader(GetSettingFilePath(), Encoding.UTF8)) {
                    // Read semester object.
                    var raw = JsonConvert.DeserializeObject<RawEditorConfiguration>(fs.ReadToEnd()).Unwrap();
                    // Apply to self.
                    FromRaw(raw);
                }
                return true;
            } catch (Exception) {
                return false;
            }
        }

        [MemberNotNull(nameof(SequenceKind))]
        [MemberNotNull(nameof(EditorLayout))]
        [MemberNotNull(nameof(PasteBehavior))]
        [MemberNotNull(nameof(FrameCount))]
        [MemberNotNull(nameof(GamePath))]
        private void Init() {
            var defaultRaw = new RawEditorConfiguration();
            FromRaw(defaultRaw);
        }

        public void LoadOrInit() {
            if (!Load()) {
                Init();
            }
        }

        public void Save() {
            try {
                // Check file path directory.
                var settingFilePath = GetSettingFilePath();
                var settingFileDirectory = Path.GetDirectoryName(settingFilePath).Unwrap();
                if (!Directory.Exists(settingFileDirectory)) {
                    Directory.CreateDirectory(settingFileDirectory);
                }
                // Write it.
                using (var fs = new StreamWriter(GetSettingFilePath(), false, Encoding.UTF8)) {
                    fs.Write(JsonConvert.SerializeObject(ToRaw()));
                }
            } catch (Exception) {
                // Do nothing.
            }
        }

        private class RawEditorConfiguration {
            [JsonProperty("sequence_kind", Required = Required.Always)]
            [JsonConverter(typeof(TasSequenceKindConverter))]
            public SequenceKind SequenceKind { get; set; } = SequenceKind.Array;

            [JsonProperty("editor_layout", Required = Required.Always)]
            [JsonConverter(typeof(EditorLayoutKindConverter))]
            public EditorLayoutKind EditorLayout { get; set; } = EditorLayoutKind.Vertical;

            [JsonProperty("paste_behavior", Required = Required.Always)]
            [JsonConverter(typeof(EditorPasteBehaviorConverter))]
            public EditorPasteBehavior PasteBehavior { get; set; } = EditorPasteBehavior.Insert;

            [JsonProperty("frame_count", Required = Required.Always)]
            public int FrameCount { get; set; } = 20;

            [JsonProperty("game_path", Required = Required.Always)]
            public string GamePath { get; set; } = "";
        }

        #region Custom JSON Converter

        private class TasSequenceKindConverter : JsonConverter<SequenceKind> {
            public override SequenceKind ReadJson(JsonReader reader, Type objectType, SequenceKind existingValue, bool hasExistingValue, JsonSerializer serializer) {
                if (reader.TokenType == JsonToken.String) {
                    var value = (reader.Value as string).Unwrap();
                    if (SequenceKindHelper.TryParse(value, out var kind)) {
                        return kind;
                    } else {
                        throw new JsonSerializationException($"given string can not be parsed as TasSequenceKind: {value}");
                    }
                } else {
                    throw new JsonSerializationException($"expect a integer but got {reader.TokenType}");
                }
            }

            public override void WriteJson(JsonWriter writer, SequenceKind value, JsonSerializer serializer) {
                writer.WriteValue(SequenceKindHelper.ToString(value));
            }
        }

        private class EditorLayoutKindConverter : JsonConverter<EditorLayoutKind> {
            public override EditorLayoutKind ReadJson(JsonReader reader, Type objectType, EditorLayoutKind existingValue, bool hasExistingValue, JsonSerializer serializer) {
                if (reader.TokenType == JsonToken.String) {
                    var value = (reader.Value as string).Unwrap();
                    if (EditorLayoutKindHelper.TryParse(value, out var kind)) {
                        return kind;
                    } else {
                        throw new JsonSerializationException($"given string can not be parsed as EditorLayoutKind: {value}");
                    }
                } else {
                    throw new JsonSerializationException($"expect a integer but got {reader.TokenType}");
                }
            }

            public override void WriteJson(JsonWriter writer, EditorLayoutKind value, JsonSerializer serializer) {
                writer.WriteValue(EditorLayoutKindHelper.ToString(value));
            }
        }

        private class EditorPasteBehaviorConverter : JsonConverter<EditorPasteBehavior> {
            public override EditorPasteBehavior ReadJson(JsonReader reader, Type objectType, EditorPasteBehavior existingValue, bool hasExistingValue, JsonSerializer serializer) {
                if (reader.TokenType == JsonToken.String) {
                    var value = (reader.Value as string).Unwrap();
                    if (EditorPasteBehaviorHelper.TryParse(value, out var kind)) {
                        return kind;
                    } else {
                        throw new JsonSerializationException($"given string can not be parsed as EditorPasteBehavior: {value}");
                    }
                } else {
                    throw new JsonSerializationException($"expect a integer but got {reader.TokenType}");
                }
            }

            public override void WriteJson(JsonWriter writer, EditorPasteBehavior value, JsonSerializer serializer) {
                writer.WriteValue(EditorPasteBehaviorHelper.ToString(value));
            }
        }

        #endregion

    }
}
