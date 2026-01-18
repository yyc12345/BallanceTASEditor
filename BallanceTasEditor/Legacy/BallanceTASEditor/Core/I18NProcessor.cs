using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BallanceTASEditor.Core {
    public static class I18NProcessor {

        public static string GetI18N(string key, params string[] parameters) {
            try {
                var cache = (string)(App.Current.Resources[key]);
                return string.Format(cache, parameters);
            } catch (Exception) {
                return "";
            }
        }

        public static void ChangeLanguage(string target) {
            ResourceDictionary langRd = null;
            try {
                langRd =
                    Application.LoadComponent(
                             new Uri(@"Language/" + target + ".xaml", UriKind.Relative))
                    as ResourceDictionary;
            } catch {
                ;
            }

            if (langRd != null) {
                if (App.Current.Resources.MergedDictionaries.Count > 0) {
                    App.Current.Resources.MergedDictionaries.Clear();
                }
                App.Current.Resources.MergedDictionaries.Add(langRd);
            }

        }

    }
}
