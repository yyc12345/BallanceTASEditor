using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BallanceTasEditor.Frontend.Models {
    public enum TasSequenceKind {
        Array,
        DoubleLinkedList
    }

    public static class  TasSequenceKindHelper {
        public static Backend.ITasSequence CreateSequenceByKind(TasSequenceKind kind) {
            return kind switch {
                TasSequenceKind.Array => new Backend.ListTasSequence(),
                TasSequenceKind.DoubleLinkedList => new Backend.LegacyTasSequence(),
                _ => throw new UnreachableException(),
            };
        }
    }

}
