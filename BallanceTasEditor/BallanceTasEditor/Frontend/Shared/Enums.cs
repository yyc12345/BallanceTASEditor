using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Frontend.Shared {

    public enum SequenceKind {
        Array,
        DoubleLinkedList
    }

    public static class SequenceKindHelper {
        public static Backend.ITasSequence CreateSequenceByKind(SequenceKind kind) {
            return kind switch {
                SequenceKind.Array => new Backend.ListTasSequence(),
                SequenceKind.DoubleLinkedList => new Backend.LegacyTasSequence(),
                _ => throw new UnreachableException(),
            };
        }

        public static bool TryParse(string s, out SequenceKind value) {
            switch (s) {
                case "array":
                    value = SequenceKind.Array;
                    return true;
                case "double_linked_list":
                    value = SequenceKind.DoubleLinkedList;
                    return true;
                default:
                    value = default;
                    return false;
            }
        }

        public static string ToString(SequenceKind kind) {
            return kind switch {
                SequenceKind.Array => "array",
                SequenceKind.DoubleLinkedList => "double_linked_list",
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

    public enum EditorPasteBehavior {
        Insert,
        Override
    }

    public static class EditorPasteBehaviorHelper {
        public static bool TryParse(string s, out EditorPasteBehavior value) {
            switch (s) {
                case "insert":
                    value = EditorPasteBehavior.Insert;
                    return true;
                case "override":
                    value = EditorPasteBehavior.Override;
                    return true;
                default:
                    value = default;
                    return false;
            }
        }

        public static string ToString(EditorPasteBehavior mode) {
            return mode switch {
                EditorPasteBehavior.Insert => "insert",
                EditorPasteBehavior.Override => "override",
                _ => throw new UnreachableException(),
            };
        }
    }


}
