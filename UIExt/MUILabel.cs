using System;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;

namespace IINS.UIExt
{
    public class MUILabel : UILabel
    {
        public delegate void OnDrawLabelHandler();
        public event OnDrawLabelHandler onDrawLabel = null;

        public const int defFontSize = 12;
        public static int defaultFontSize = defFontSize;
        public static GUIStyle lableStyle;
        private float lastwidth = 0f;  
        private float lastheight = 0f;

        private string _text = "";
        public new string text { 
            get { return _text; }
            set { _text = value; } } 

        private FontStyle _fontStyle = FontStyle.Normal;
        public FontStyle fontStyle {
            get { return _fontStyle; }
            set { _fontStyle = value; }
        }

        private int _fontSize = 0;
        public int fontSize
        {
            get { return _fontSize==0? defaultFontSize : _fontSize; }
            set { _fontSize = (value == defaultFontSize ? 0 : value); }
        }

        public override void Awake()
        {
            base.Awake();
            isTooltipLocalized = true;
        }

        public override void Start()
        {
            base.Start();
        }

        public virtual void OnGUI()
        {
            if (lastwidth != Screen.width || lastheight != Screen.height)
            {
                lastwidth = Screen.width;
                lastheight = Screen.height;
                defaultFontSize = (int)(defFontSize / UIView.GetAView().ratio);
                if (UIView.GetAView().ratio < 1f)
                    defaultFontSize -= 1;
            }

            if (lableStyle == null)
            {
                lableStyle = new GUIStyle(GUI.skin.label);
                lableStyle.normal.textColor = Color.white;                
            }

            lableStyle.fontSize = defaultFontSize;
        }

        public virtual void DrawLabel()
        {
            OnDrawLabel();
            var r = new Rect(MUISkin.UIToScreen(relativePosition.x), MUISkin.UIToScreen(relativePosition.y),
                MUISkin.UIToScreen(size.x), MUISkin.UIToScreen(size.y));

            var _fColor = lableStyle.normal.textColor;
            var _fStyle = lableStyle.fontStyle;
            lableStyle.normal.textColor = this.textColor;
            lableStyle.fontSize = this.fontSize;

            lableStyle.fontStyle = this.fontStyle;
            switch (this.textAlignment)
                {
                case UIHorizontalAlignment.Left:
                    lableStyle.alignment = TextAnchor.MiddleLeft;
                    break;
                case UIHorizontalAlignment.Right:
                    lableStyle.alignment = TextAnchor.MiddleRight;
                    break;
                default:
                    lableStyle.alignment = TextAnchor.MiddleCenter;
                    break;
            }

            //GUI.DrawTexture(r, ExtendedPanel.lineTexture);
            GUI.Label(r, text, lableStyle);

            lableStyle.normal.textColor = _fColor;
            lableStyle.fontStyle = _fStyle;
            lableStyle.alignment = TextAnchor.MiddleLeft;
            lableStyle.fontSize = defaultFontSize;
        }

        public virtual void OnDrawLabel()
        {
            if (onDrawLabel != null) onDrawLabel();
        }
    }
}
