using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Frontend.Shared {

    public enum TasSequenceKind {
        Array,
        DoubleLinkedList
    }

    public static class TasSequenceKindHelper {
        public static Backend.ITasSequence CreateSequenceByKind(TasSequenceKind kind) {
            return kind switch {
                TasSequenceKind.Array => new Backend.ListTasSequence(),
                TasSequenceKind.DoubleLinkedList => new Backend.LegacyTasSequence(),
                _ => throw new UnreachableException(),
            };
        }

        public static bool TryParse(string s, out TasSequenceKind value) {
            switch (s) {
                case "array":
                    value = TasSequenceKind.Array;
                    return true;
                case "double_linked_list":
                    value = TasSequenceKind.DoubleLinkedList;
                    return true;
                default:
                    value = default;
                    return false;
            }
        }

        public static string ToString(TasSequenceKind kind) {
            return kind switch {
                TasSequenceKind.Array => "array",
                TasSequenceKind.DoubleLinkedList => "double_linked_list",
                _ => throw new UnreachableException(),
            };
        }
    }

    public enum EditorLayoutKind {
        Vertical,
        Horizontal
    }

    public static class EditorLayoutKindHelper {
        public static bool TryParse(string s, out EditorLayoutKind value) {
            switch (s) {
                case "vertical":
                    value = EditorLayoutKind.Vertical;
                    return true;
                case "horizontal":
                    value = EditorLayoutKind.Horizontal;
                    return true;
                default:
                    value = default;
                    return false;
            }
        }
        public static string ToString(EditorLayoutKind kind) {
            return kind switch {
                EditorLayoutKind.Vertical => "vertical",
                EditorLayoutKind.Horizontal => "horizontal",
                _ => throw new UnreachableException(),
            };
        }
    }

    public enum EditorPasteMode {
        Insert,
        Override
    }

    public static class EditorPasteModeHelper {
        public static bool TryParse(string s, out EditorPasteMode value) {
            switch (s) {
                case "insert":
                    value = EditorPasteMode.Insert;
                    return true;
                case "override":
                    value = EditorPasteMode.Override;
                    return true;
                default:
                    value = default;
                    return false;
            }
        }

        public static string ToString(EditorPasteMode mode) {
            return mode switch {
                EditorPasteMode.Insert => "insert",
                EditorPasteMode.Override => "override",
                _ => throw new UnreachableException(),
            };
        }
    }


}
