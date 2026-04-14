using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Frontend.Models {
    public partial class TasFile : ObservableObject {
        public TasFile() {
            FileBody = null;
        }

        #region File Load and Save

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsFileLoaded))]
        [NotifyPropertyChangedFor(nameof(IsFileNotLoaded))]
        private Backend.ITasSequence? fileBody;

        [MemberNotNullWhen(true, nameof(FileBody))]
        public bool IsFileLoaded {
            get => FileBody is not null;
        }

        [MemberNotNullWhen(false, nameof(FileBody))]
        public bool IsFileNotLoaded {
            get => FileBody is null;
        }

        public void NewFile(TasSequenceKind kind, int count, uint fps) {
            // Check status
            if (IsFileLoaded) {
                throw new InvalidOperationException();
            }

            // Initialize sequence
            var seq = TasSequenceKindHelper.CreateSequenceByKind(kind);
            // Initialize items
            Backend.TasStorage.Init(seq, count, fps);
            // Set members
            FileBody = seq;
        }

        public void LoadFile(TasSequenceKind kind, string path) {
            // Check status
            if (IsFileLoaded) {
                throw new InvalidOperationException();
            }

            // Initialize sequence
            var seq = TasSequenceKindHelper.CreateSequenceByKind(kind);
            // Load into sequence
            Backend.TasStorage.Load(path, seq);
            // Set members
            FileBody = seq;
        }

        public void SaveFile(string path) {
            // Check status
            if (IsFileNotLoaded) {
                throw new InvalidOperationException();
            }

            // Save sequence
            Backend.TasStorage.Save(path, FileBody);
        }

        public void CloseFile() {
            // Check status
            if (IsFileNotLoaded) {
                throw new InvalidOperationException();
            }

            // Set member
            FileBody = null;
        }

        #endregion

    }
}
