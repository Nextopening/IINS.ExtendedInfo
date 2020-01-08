using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using ICities;
using ColossalFramework.Plugins;
using ColossalFramework.Globalization;


namespace IINS.ExtendedInfo
{
    public class MODLoadingExtension : LoadingExtensionBase
    {
        public static LoadMode loadMode;
        public static bool ModLoaded = false;

        public override void OnLevelLoaded(LoadMode mode)
        {
            loadMode = mode;
            base.OnLevelLoaded(mode);

            ModLoaded = true;
            Debugger.OnLevelLoaded();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            ModLoaded = false;
            Debugger.OnLevelUnloading();
        }
    }


    public class XMLSingleton<T>
    {
        // 读取配置
        public static T Deserialize(string filename)
        {
            if (!File.Exists(filename)) 
            {
                //--  Debugger.Message("no config file!");
                return default(T);
            }

            var serializer = new XmlSerializer(typeof(T));
            try
            {
                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(filename))
                {
                    return (T)serializer.Deserialize(streamReader);
                }
            }
            catch (Exception e)
            {
                Debugger.Warning("Couldn't load configuration (XML malformed?)");
                Debugger.Exception(e);
                return default(T); //throw e;
            }
        }

        //保存配置
        public static void Serialize(string filename, T config)
        {
            XmlSerializerNamespaces emptyNamespace = new XmlSerializerNamespaces();
            emptyNamespace.Add("", "");

            var serializer = new XmlSerializer(typeof(T));
            try
            {
                using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(filename))
                {
                    serializer.Serialize(streamWriter, config, emptyNamespace);
                }
            }
            catch (Exception e)
            {
                Debugger.Warning("Couldn't create configuration file at \"" + Directory.GetCurrentDirectory() + "\"");
                throw e;
            }
        }
    }

    public static class MODUtil
    {
        public static bool IsChinaLanguage()
        {
            return (LocaleManager.instance.language == "zh-cn" || LocaleManager.instance.language == "zh-tw" || LocaleManager.instance.language == "zh");
        }

        public static string GetAssemblyPath()
        {
            string _CodeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

            _CodeBase = _CodeBase.Substring(8, _CodeBase.Length - 8);    // 8是file:// 的长度

            string[] arrSection = _CodeBase.Split(new char[] { '/' });

            string _FolderPath = "";
            for (int i = 0; i < arrSection.Length - 1; i++)
            {
                _FolderPath += arrSection[i] + "/";
            }

            return _FolderPath;
        }

        public static bool IsModActiveByName(string modName)
        {
            var plugins = PluginManager.instance.GetPluginsInfo();
            return (from plugin in plugins.Where(p => p.isEnabled)
                    select plugin.GetInstances<IUserMod>() into instances
                    where instances.Any()
                    select instances[0].Name into name
                    where name == modName
                    select name).Any();
        }

        public static bool IsModActiveById(string WorkshopId)
        {
            var pluginManager = PluginManager.instance;
            var plugins = MODUtil.GetPrivate<Dictionary<string, PluginManager.PluginInfo>>(pluginManager, "m_Plugins");

            foreach (var item in plugins)
            {
                if (item.Value.name != WorkshopId)
                {
                    continue;
                }

                return item.Value.isEnabled;
            }

            return false;
        }


        public static T GetFieldValue<T>(FieldInfo field, object o)
        {
            return (T)field.GetValue(o);
        }

        public static void SetFieldValue(FieldInfo field, object o, object value)
        {
            field.SetValue(o, value);
        }

        public static Q GetPrivate<Q>(object o, string fieldName)
        {
            var fields = o.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field = null;

            foreach (var f in fields)
            {
                if (f.Name == fieldName)
                {
                    field = f;
                    break;
                }
            }

            return (Q)field.GetValue(o);
        }

        public static void SetPrivate<Q>(object o, string fieldName, object value)
        {
            var fields = o.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field = null;

            foreach (var f in fields)
            {
                if (f.Name == fieldName)
                {
                    field = f;
                    break;
                }
            }

            field.SetValue(o, value);
        }

    }
}
