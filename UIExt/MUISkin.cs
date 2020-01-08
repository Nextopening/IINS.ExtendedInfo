using UnityEngine;
using ColossalFramework.UI;
using System.IO;
using System.Reflection;

namespace IINS.UIExt
{
    public static class MUISkin
    {
        public class SkinConfig
        {
            public int fontSize = 14;
            public string fontName = ""; //--DEBUG "Courier New Bold";

            public Color textColor = Color.white;
            public Color backgroundColor = new Color(0.321f, 0.321f, 0.321f, 0.75f);
            public Color titlebarColor = Color.gray;
            public Color titleTextColor = new Color(0.85f, 0.85f, 0.85f, 1.0f);
            public Color titleTextHoverColor = Color.yellow;
            public int resizeButtonWidth = 10;

            public Vector2 resizeButtonOffset = Vector2.zero;
            public Vector2 titleOffset = Vector2.zero;
            public Vector2 closeButtonOffset = Vector2.zero;

            public static SkinConfig instance = null;
            public static SkinConfig Instance
            {
                get
                {
                    if (instance == null)
                        instance = new SkinConfig();

                    return instance;
                }
            }
        }

        public delegate void OnInitSkin();
        public static OnInitSkin onInitSkin = null;
        public static GUISkin skin = null;
        public static Camera mainCamera = null;
        public static CameraController comeraController = null;
        public static bool initialized = false;
        public static bool drawSkinFlag = false;

        public static SkinConfig config
        {
            get { return SkinConfig.Instance; }
        }

        static MUISkin()
        {
            uiview = UnityEngine.Object.FindObjectOfType<UIView>();
            comeraController = GameObject.FindObjectOfType<CameraController>();
            if (comeraController != null)
            {
                mainCamera = comeraController.GetComponent<Camera>();
            }
        }

        private static UIView uiview = null;
        public static UIView UIView
        {
            get
            {
                if (uiview == null)
                {
                    uiview = UnityEngine.Object.FindObjectOfType<UIView>();
                }

                return uiview;
            }
        }

        public static float ScreenToUI(float p)
        {
            return (p * UIView.ratio);
        }

        public static float UIToScreen(float p)
        {
            return (p / UIView.ratio);
        }

        // 计算字体高度
        public static float CalcTextHeight(int fontsize = 0)
        {
            if (fontsize <= 0)
                fontsize = config.fontSize;

            float h = 0;
            if (MUISkin.skin != null)
            {
                GUIStyle style = new GUIStyle(MUISkin.skin.label);
                style.fontSize = fontsize;
                h = style.CalcHeight(new GUIContent("H"), 1024);
            }

            return h;
        }

        // 计算文本宽度
        public static float GetSizeOfWord(string word, int size = 0, FontStyle style = 0)
        {
            float width = 0.0f;
            if (MUISkin.skin != null)
            {
                CharacterInfo charInfo;
                foreach (char c in word)
                {
                    if (size > 0)
                        MUISkin.skin.font.GetCharacterInfo(c, out charInfo, size, style);
                    else
                        MUISkin.skin.font.GetCharacterInfo(c, out charInfo);

                    width += charInfo.advance;

                }
            }
            return width;
        }


        public static void Update()
        {
            if (skin != null)
            {
                skin.window.normal.background = null;
                //skin.window.onNormal.background = bgTexture;

                skin.button.normal.textColor = config.textColor;
                skin.label.normal.textColor = config.textColor;
                skin.textArea.normal.textColor = config.textColor;
                skin.textField.normal.textColor = config.textColor;
                skin.toggle.normal.textColor = config.textColor;

                skin.button.hover.textColor = config.textColor;
                skin.label.hover.textColor = config.textColor;
                skin.textArea.hover.textColor = config.textColor;
                skin.textField.hover.textColor = config.textColor;
                skin.toggle.hover.textColor = config.textColor;
                skin.font = Font.CreateDynamicFontFromOSFont(config.fontName, config.fontSize);
            }
        }

        public static void Done()
        {
            if (skin != null)
            {
                SkinConfig.instance = null;
                Object.Destroy(skin);
            }
            initialized = false;
            drawSkinFlag = false;
        }

        public static bool isStyleExists(string name)
        {
            return ((skin == null || name == "") ? false : skin.FindStyle(name) != null);
        }

        public static Assembly rootAssembly = null;
        public static void Init()
        {
            if (skin == null)
            {                
                skin = ScriptableObject.CreateInstance<GUISkin>();

                skin.box = new GUIStyle(GUI.skin.box);
                skin.button = new GUIStyle(GUI.skin.button);
                skin.label = new GUIStyle(GUI.skin.label);
                skin.textArea = new GUIStyle(GUI.skin.textArea);
                skin.textField = new GUIStyle(GUI.skin.textField);
                skin.toggle = new GUIStyle(GUI.skin.toggle);
                skin.window = new GUIStyle(GUI.skin.window);

                skin.horizontalScrollbar = new GUIStyle(GUI.skin.horizontalScrollbar);
                skin.horizontalScrollbarLeftButton = new GUIStyle(GUI.skin.horizontalScrollbarLeftButton);
                skin.horizontalScrollbarRightButton = new GUIStyle(GUI.skin.horizontalScrollbarRightButton);
                skin.horizontalScrollbarThumb = new GUIStyle(GUI.skin.horizontalScrollbarThumb);
                skin.horizontalSlider = new GUIStyle(GUI.skin.horizontalSlider);
                skin.horizontalSliderThumb = new GUIStyle(GUI.skin.horizontalSliderThumb);
                skin.scrollView = new GUIStyle(GUI.skin.scrollView);
                skin.verticalScrollbar = new GUIStyle(GUI.skin.verticalScrollbar);
                skin.verticalScrollbarDownButton = new GUIStyle(GUI.skin.verticalScrollbarDownButton);
                skin.verticalScrollbarThumb = new GUIStyle(GUI.skin.verticalScrollbarThumb);
                skin.verticalScrollbarUpButton = new GUIStyle(GUI.skin.verticalScrollbarUpButton);
                skin.verticalSlider = new GUIStyle(GUI.skin.verticalSlider);
                skin.verticalSliderThumb = new GUIStyle(GUI.skin.verticalSliderThumb);

                skin.settings.cursorColor = GUI.skin.settings.cursorColor;
                skin.settings.cursorFlashSpeed = GUI.skin.settings.cursorFlashSpeed;
                skin.settings.doubleClickSelectsWord = GUI.skin.settings.doubleClickSelectsWord;
                skin.settings.selectionColor = GUI.skin.settings.selectionColor;
                skin.settings.tripleClickSelectsLine = GUI.skin.settings.tripleClickSelectsLine;

                Update();
                if (onInitSkin != null) onInitSkin();
                initialized = true;
            }
        }
    }
}
