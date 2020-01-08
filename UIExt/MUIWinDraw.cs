using System;
using UnityEngine;
using ColossalFramework.UI;

namespace IINS.UIExt
{
    public partial class MUIWinDraw : MonoBehaviour
    {
        public int winId = 0;
        public MUIWindow win = null;

        public bool closeCapture = false;

        public Rect rect = new Rect(0, 0, 200, 120);
        public Rect titleRect;
        public Rect resizeRect;
        public Rect closeRect;

        public GUIStyle winStyle = null;
        public GUIStyle titleStyle = null;
        public Texture2D winTexture = null;
        public Texture2D titleTextture = null;
        public Texture2D resizeTextture = null;
        public Texture2D closeTextture = null;

        private States _states = 0;
        public States states { get { return _states; }
            set
            {
                if (_states != value) UpdateStates(value);                
            }
        }
        
        public enum States
        {
            HoverInTitlebar = 0x01,
            HoverInCloseButton = 0x10,
            HoverInResize = 0x100,
        }

        void UpdateStates(States s)
        {
            var color = win.titleColor; // MUISkin.config.titlebarColor;
            if ((s & States.HoverInTitlebar) == States.HoverInTitlebar)
            {
                color = MUIUtil.BrightenColor(color, 0.2f, color[3]);
                titleTextture.SetPixel(0, 0, color);
                titleStyle.normal.textColor = MUISkin.config.titleTextHoverColor;
            }
            else
            {
                titleTextture.SetPixel(0, 0, color);
                titleStyle.normal.textColor = MUISkin.config.titleTextColor;
            }

            color = win.backgroundColor;
            if (color == Color.clear)
            {
                color = Color.gray;
                color[3] = 0.5f; // alpha
            }


            if ((s & States.HoverInResize) == States.HoverInResize)
            {
                color = MUIUtil.BrightenColor(color, 0.8f, 1.0f);
            }
            else
            {
                color = MUIUtil.BrightenColor(color, 0.5f, color[3]);
            }

            if (MUISkin.isStyleExists("button_resize"))
                resizeTextture = MUISkin.skin.FindStyle("button_resize").normal.background;
            else
                resizeTextture.SetPixel(0, 0, color);

            color = new Color(0.8f, 0.0f, 0.0f, MUISkin.config.titlebarColor[3]);
            if ((s & States.HoverInCloseButton) == States.HoverInCloseButton)
            {
                closeTextture.SetPixel(0, 0, MUIUtil.BrightenColor(color, 0.65f, 1.0f));
                if (MUISkin.isStyleExists("button_close"))
                    closeTextture = MUISkin.skin.FindStyle("button_close").hover.background;
            }
            else
            {
                closeTextture.SetPixel(0, 0, color);
                if (MUISkin.isStyleExists("button_close"))
                    closeTextture = MUISkin.skin.FindStyle("button_close").normal.background;
            }
            _states = s;
            titleTextture.Apply(); 
            closeTextture.Apply();
            resizeTextture.Apply();
        }

        public void Awake()
        {
            winId = UnityEngine.Random.Range(9066, int.MaxValue);
        }

        public void Start()
        {
            enabled = false;
        }

        private bool initialized = false;
        void InitUI()
        {
            if (initialized) return;
            initialized = true;
            MUISkin.Init();
            if (!MUISkin.drawSkinFlag && MUISkin.initialized) // for firt window 
            {
                MUISkin.drawSkinFlag = true;
                win.titleColor = MUISkin.config.titlebarColor;
                win.titleOffset = MUISkin.config.titleOffset;
                win.closeButtonOffset = MUISkin.config.closeButtonOffset;
                win.resizeButtonOffset = MUISkin.config.resizeButtonOffset;
            }

            if (titleTextture == null)
            {
                titleTextture = new Texture2D(1, 1);
            }

            if (resizeTextture == null)
            {
                resizeTextture = new Texture2D(1, 1);
            }

            if (closeTextture == null)
            {
                closeTextture = new Texture2D(1, 1);
            }

            if (winTexture == null)
            {
                winTexture = new Texture2D(1, 1);
                winTexture.SetPixel(0, 0, win.backgroundColor);
                winTexture.Apply();
            }

            if (titleStyle==null)
            {
                titleStyle = new GUIStyle(GUI.skin.label);
                titleStyle.alignment = TextAnchor.MiddleLeft;
                titleStyle.fontStyle = FontStyle.Bold;
            }

            UpdateStates(states);
        }


        public void Update()
        {
            if (initialized && (win == null))
            {
                Destroy(this);
            }           
        }

        public void OnGUI()
        {
            if (!win.isVisible) return;

            if (!initialized) InitUI();

            if (!MUISkin.initialized) return;

            var oldSkin = GUI.skin;
            if (MUISkin.skin != null)
            {
                GUI.skin = MUISkin.skin;
            }

            if (winStyle == null)
            {
                winStyle = new GUIStyle(GUI.skin.window);
            };

            #region set Window background color

            if (winTexture != null)
            {
                int bkCount = 0;

                if (win.backgroundSprite != "")
                {
                    var style = MUISkin.skin.FindStyle(win.backgroundSprite);

                    if (style != null)
                    {
                        bkCount += 1;
                        winTexture = style.normal.background;
                        winStyle.border = style.border;
                        winStyle.margin = style.margin;
                        winStyle.padding = style.padding;
                    } else
                    {
                        var sInfo = UIView.GetAView().defaultAtlas[win.backgroundSprite];

                        if (sInfo != null && sInfo.texture != null)
                        {
                            bkCount += 1;
                            winTexture = sInfo.texture;
                        }
                    }
                }

                if (bkCount==0 && GUI.skin.window.normal.background != null)
                {
                    bkCount += 1;
                    winTexture = GUI.skin.window.normal.background;
                    winStyle.border = GUI.skin.window.border;
                    winStyle.margin = GUI.skin.window.margin;
                    winStyle.padding = GUI.skin.window.padding;
                }

                if (bkCount == 0 )
                {
                    winTexture.SetPixel(0, 0, win.backgroundColor);
                    winTexture.Apply();
                }

                winStyle.normal.background = winTexture;
                winStyle.onNormal.background = winTexture;
            }
            //else
            //{
            //    winStyle.normal.background = MUISkin.bgTexture;
            //    winStyle.onNormal.background = MUISkin.bgTexture;
            //}
            #endregion

            GUI.Window(winId, rect, DrawWindow, "", winStyle);

            GUI.skin = oldSkin;
        }

        public void DrawWindow(int id)
        {
            float h = win.titleHeight;
            GUILayout.Space(h - 18.0f);
            try
            {
                win.DrawWindow();
            } catch (Exception e)
            {
                Debug.LogException(e);
                //Debugger.Warning("IINS.CSLMOD.UI: An error has happened during the UI creation.");
                //Debugger.Exception(e);
            }

            DoDrawWindow();            
        }

        const int resizeButton_size = 11;
        void DoDrawWindow()
        {
            var h = win.titleHeight;
            if ((win.options & MUIWindow.Options.TitleBar) == MUIWindow.Options.TitleBar)
                titleRect = new Rect(win.titleOffset.x, win.titleOffset.y, rect.width - win.titleOffset.x * 3, h);
            else
                titleRect = new Rect(0, 0, 0, 0);

            if ((win.options & MUIWindow.Options.CloseButton) == MUIWindow.Options.CloseButton)
            {
                float size = win.closeButtonSize;
                if (size == 0) size = 16.0f;
                var offset = win.closeButtonOffset;
                

                closeRect = new Rect(rect.width - size + offset.x - 0.0f, offset.y, size, titleRect.y + (size / 2) + 2.0f);
                if (MUISkin.isStyleExists("button_close"))
                {
                    size = win.closeButtonSize;
                    if (size == 0) size = h;
                    if (MUISkin.UIView.ratio>1.0)
                        size /= MUISkin.UIView.ratio;
                    closeRect = new Rect(rect.width - size + offset.x, (h- size)/ 2 + offset.y, size, size);
                }
            }
            else
                closeRect = new Rect(0, 0, 0, 0);

            if (!titleRect.IsEmpty())
            {
                GUI.DrawTexture(titleRect, titleTextture, ScaleMode.StretchToFill);
                GUI.DrawTexture(closeRect, closeTextture, ScaleMode.ScaleAndCrop);
                titleStyle.alignment = win.titleAlignment;
                GUI.Label(new Rect(titleRect.x + 3.0f, titleRect.y, rect.width - 3.0f, titleRect.height), win.Title, titleStyle);                
            }

            if (!resizeRect.IsEmpty())
            {
                if (MUISkin.isStyleExists("button_resize"))
                {
                    resizeRect = new Rect(rect.width - resizeButton_size + win.resizeButtonOffset.x - 2.0f, 
                        rect.height - resizeButton_size + win.resizeButtonOffset.y - 2.0f,
                        resizeButton_size, resizeButton_size);                    
                }
                GUI.DrawTexture(resizeRect, resizeTextture, ScaleMode.StretchToFill);
            }
        }

        public bool isHoverInClose()
        {
            return (states & States.HoverInCloseButton) == States.HoverInCloseButton;
        }
        public bool isHoverInTitle()
        {
            return (states & States.HoverInTitlebar) == States.HoverInTitlebar;
        }
    }
}
