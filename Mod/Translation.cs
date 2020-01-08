using ColossalFramework.Globalization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace IINS.ExtendedInfo
{
    public class Translation
    {
        private static readonly string DEFAULT_LANG = "en";
        private const string LANG_FILENAME_PREFIX = "lang_";
        private const string LANG_FILENAME_EXTENSION = ".txt"; 
        private static Dictionary<string, string> translations;
        private static string loadedLanguage = null;

        public static string Language
        {
            get
            {
                return LocaleManager.instance.language;
            }
        }

        private static Translation _text;

        public static Translation Text { get
            {
                if (_text == null)
                    _text = new Translation();

                return _text;
            }
        }

        public static int Count
        {
            get
            {
                loadTranslations();

                if (translations != null)
                    return translations.Count;
                else
                    return 0;
            }
        }

        public string this[string key] {
            get
            {
                loadTranslations();

                string ret = null;
                if (translations != null) try
                    {
                        translations.TryGetValue(key, out ret);                        
                    }
                    catch (Exception e)
                    {
                        Debugger.Error("Error fetching the key {key} from the translation dictionary: {e.ToString()}", e);
                        return key;
                    }

                if (ret == null)
                    return key;
                return ret;
                }
        }

        public static string getLocaleTransFileName(string language)
        {
            string lang = language;
            switch (lang)
            {
                case "jaex":
                    lang = "ja";
                    break;
            }

            return LANG_FILENAME_PREFIX + lang + LANG_FILENAME_EXTENSION;
        }

        internal static void OnLevelUnloading()
        {
            translations = null;
        }

        private static string[] loadStringFromAssembly(Assembly assembly, string file)
        {
            string[] lines = null;
            if (assembly != null)
            {
                string filename = assembly.GetName().Name + ".Resources." + file;
                
                using (Stream st = assembly.GetManifestResourceStream(filename))
                {
                    if (st != null)
                        using (StreamReader sr = new StreamReader(st))
                        {
                            lines = sr.ReadToEnd().Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None);
                        }
                }
            }
            return lines;
        }

        private static string[] loadStringFromFile(string file)
        {
            string[] lines = null;
            string filename = Path.Combine(MODUtil.GetAssemblyPath(), file);
            if (File.Exists(filename))
            {
                using (StreamReader sr = File.OpenText(filename))
                {
                    if (sr != null)
                        lines = sr.ReadToEnd().Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None);
                }
            }
            return lines;
        }

        private static void doLoadTranslations(string lang)
        {
            try
            {
                string[] lines = null;
                string filename = getLocaleTransFileName(lang);
                
                lines = loadStringFromAssembly(Assembly.GetExecutingAssembly(), filename);

                if (lines == null || lines.Length == 0)
                    lines = loadStringFromFile(filename);

                if (lines != null && lines.Length > 0)
                {
                    translations = new Dictionary<string, string>();
                    foreach (string line in lines)
                    {
                        if (line == null || line.Trim().Length == 0)
                        {
                            continue;
                        }
                        int delimiterIndex = line.Trim().IndexOf(' ');
                        if (delimiterIndex > 0)
                        {
                            translations.Add(line.Substring(0, delimiterIndex), line.Substring(delimiterIndex + 1).Trim().Replace("\\n", "\n"));                            
                        }
                    }

                    loadedLanguage = lang;
                }
            }
            catch (Exception e)
            {
                Debugger.Error("Error while loading translations: {e.ToString()}", e);
            }
        }

        private static void loadTranslations()
        {
            if (translations == null || loadedLanguage == null || !loadedLanguage.Equals(Language))
            {
                doLoadTranslations(Language);

                if (translations == null || translations.Count==0)
                    doLoadTranslations(DEFAULT_LANG);
            }
        }
    }
}
