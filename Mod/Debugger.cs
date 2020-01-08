using System;
using UnityEngine;
using ColossalFramework.Plugins;
using ColossalFramework.UI;

namespace IINS.ExtendedInfo
{
    public static class Debugger
    {
        public static bool loaded = false;
        public static string Prefix = ""; 

        private static bool initialized = false;
        private static string exceptions = "";

        private static string _lastMessage;
        private static int _duplicates = 0;

        private static bool _enabled = true;
        public static bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                if (value != _enabled)
                {
                    if (_enabled = value)
                    {
                        Initialize();
                    }
                    else
                    {
                        Deinitialize();
                    }
                }
            }
        }

        public static void Initialize()
        {
            Deinitialize();

            if (Enabled)
            {
                initialized = true;
            }
        }

        public static void Deinitialize()
        {
            if (initialized)
            {
                initialized = false;
                exceptions = "";
                loaded = false;
            }
        }

        public static void OnLevelLoaded()
        {
            loaded = true;
            ShowExceptions();
        }

        public static void OnLevelUnloading()
        {
            loaded = false;
        }

        public static void Log(string message)
        {
            if (initialized && Enabled)
            {
                if (message == _lastMessage)
                {
                    _duplicates++;
                }
                else if (_duplicates > 0)
                {
                    Debug.Log(Prefix + _lastMessage + "(x" + (_duplicates + 1) + ")");
                    Debug.Log(Prefix + message);
                    _duplicates = 0;
                }
                else
                {
                    Debug.Log(Prefix + message);
                }
                _lastMessage = message;
            }
        }

        public static void Message(string message)
        {
            Log(message);
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, Prefix + message);
        }

        public static void Warning(string message)
        {
            Debug.LogWarning(Prefix + message);
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, Prefix + message);
        }

        public static void Error(string message, Exception e)
        {
            exceptions += message;
            Debug.LogError(Prefix + message);
            DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, Prefix + message);
        }

        public static void Exception(Exception e)
        {
            string message = "ERROR: " + e.Message + "\n" + e.StackTrace + "\n";

            Debug.LogException(e);

            exceptions += message;

            if (loaded) ShowExceptions();
        }

        private static void ShowExceptions()
        {
            if (exceptions != "")
            {
                UIView.library.ShowModal<ExceptionPanel>("ExceptionPanel").SetMessage(
                    Prefix + " Error",
                    "Please report this error on the " + Prefix +" workshop page:\n" + exceptions,
                    true);

                exceptions = "";
            }
        }


    }
}
