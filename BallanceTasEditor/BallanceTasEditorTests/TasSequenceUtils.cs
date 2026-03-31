using BallanceTasEditor.Backend;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditorTests {

    internal static class TasSequenceUtils {

        internal static IEnumerable<ITasSequence> EnumerateTasSequenceImplementation() {
            yield return new ListTasSequence();
            yield return new LegacyTasSequence();
            // TODO: Add GapBufferTasSequence once we finish it.
            //yield return new GapBufferTasSequence();
        }

    }

}
