using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Frontend.Models {

    public partial class EditorSetting : ObservableObject {

        public EditorSetting() {
            // YYC MARK:
            // Due to the shitty conflict between CommunityToolkit.Mvvm and Nullable aware,
            // I was forcely set this field to clear all warning.
            GamePath = "";

            // Initialize from singleton.
            FromSingleton();
        }

        [ObservableProperty]
        private Shared.SequenceKind sequenceKind;
        [ObservableProperty]
        private Shared.EditorLayoutKind editorLayout;
        [ObservableProperty]
        private Shared.EditorPasteBehavior pasteBehavior;
        [ObservableProperty]
        private int frameCount;
        [ObservableProperty]
        private string gamePath;

        [MemberNotNull(nameof(SequenceKind))]
        [MemberNotNull(nameof(EditorLayout))]
        [MemberNotNull(nameof(PasteBehavior))]
        [MemberNotNull(nameof(FrameCount))]
        [MemberNotNull(nameof(GamePath))]
        private void FromSingleton() {
            var singleton = Shared.EditorConfiguration.Instance;

            SequenceKind = singleton.SequenceKind;
            EditorLayout = singleton.EditorLayout;
            PasteBehavior = singleton.PasteBehavior;
            FrameCount = singleton.FrameCount;
            GamePath = singleton.GamePath;
        }

        private void ToSingleton() {
            var singleton = Shared.EditorConfiguration.Instance;

            singleton.SequenceKind = SequenceKind;
            singleton.EditorLayout = EditorLayout;
            singleton.PasteBehavior = PasteBehavior;
            singleton.FrameCount = FrameCount;
            singleton.GamePath = GamePath;
        }

        public void Save() {
            ToSingleton();
            Shared.EditorConfiguration.Instance.Save();
        }
    }

}
