using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditorTests {
    internal static class AssertExtension {
        internal static T ThrowsDerivedException<T>(Action action) where T : Exception {
            try {
                action();
            } catch (T ex) {
                return ex;
            } catch (Exception ex) {
                if (ex is T derivedEx)
                    return derivedEx;

                throw new AssertFailedException(
                    $"Expected exception of type {typeof(T)} or derived type, but got {ex.GetType()}. " +
                    $"Message: {ex.Message}");
            }

            throw new AssertFailedException(
                $"Expected exception of type {typeof(T)} or derived type, but no exception was thrown.");
        }
    }
}
