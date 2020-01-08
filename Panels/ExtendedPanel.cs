using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework;
using IINS.UIExt;
using System;
using System.IO;


namespace IINS.ExtendedInfo
{
    public class ExtendedPanel : MUIPanel
    {
        //public static Vector2 WordSize = Vector2.zero;
        public static Texture2D lineTexture = new Texture2D(1, 1);

        public static Color COLOR_TEXT = new Color32(185, 211, 254, 255);
        public static Color COLOR_DARK_TEXT = MUIUtil.DarkenColor(COLOR_TEXT, 0.3f);
        public static Color COLOR_CAPTION = MUIUtil.DarkenColor(COLOR_TEXT, 0.5f); // MUIUtil.BrightenColor(Color.grey, 0.35f);
        public static Color COLOR_PURPLE = new Color32(170, 58, 183, 255);
        public static Color COLOR_DARK_PURPLE = MUIUtil.DarkenColor(COLOR_PURPLE, 0.35f);
        public static Color COLOR_DARK_RED = MUIUtil.DarkenColor(Color.red, 0.54f);
        public static Color COLOR_DARK_GREEN = MUIUtil.DarkenColor(Color.green, 0.54f);
        public static Color COLOR_DARK_YELLOW = MUIUtil.DarkenColor(Color.yellow, 0.45f);

        public static float LINE1 = 0; // 第一行Y坐标
        public static float LINE2 = 0; // 第二行Y坐标
        public static float LINEW = 0; // 行高
        public static float PANEL_HEIGHT = 0; // 面板高度

        protected float mainAspectRatio = 0f;
        protected Texture2D Icon = null;
        protected UIComponent relevantComponent = null;

        public static UIComponent parentPanel
        {
            get
            {
                return ExtendedInfoManager.infopanel;
            }
        }

        public ExtendedPanel()
        {
            isVisible = false;
            color = MUIUtil.DarkenColor(Color.grey, 0.3f);
        }

        public override void Awake()
        {
            base.Awake();
            size = new Vector2(0, 0);
            lineTexture.SetPixel(1, 1, MUIUtil.DarkenColor(Color.grey, 0.10f));
            lineTexture.Apply();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (relevantComponent != null)
            {
                relevantComponent.isVisible = true;
            }

            relevantComponent = null;
            GameObject.Destroy(gameObject);
        }


        public override void Start()
        {
            base.Start();
        }


        protected override void OnVisibilityChanged()
        {
            base.OnVisibilityChanged();

            if (relevantComponent != null)
            {
                relevantComponent.isVisible = !this.isVisible;
            }

            ResetPositionSize();
        }

        public virtual void ResetPositionSize()
        {
            if (relevantComponent != null)
            {
                absolutePosition = relevantComponent.absolutePosition;
                relativePosition = new Vector3(relativePosition.x, 4.0f);
            }
        }

        public override void OnGUI()
        {
            base.OnGUI();
            if (isVisible && backgroundSprite==null) backgroundSprite = "ScrollbarTrack";
            if (mainAspectRatio != Camera.main.aspect)
            {
                mainAspectRatio = Camera.main.aspect;
                ResetPositionSize();
            }
        }

        public override void OnScreenSizeChagned()
        {
            InitParameters();
            base.OnScreenSizeChagned();
            this.size = new Vector2(this.size.x, PANEL_HEIGHT);
        }


        public static void InitParameters()
        {
            if (MUILabel.lableStyle == null)
            {
                MUILabel.lableStyle = new GUIStyle(GUI.skin.label);
                MUILabel.lableStyle.normal.textColor = Color.white;
                MUILabel.lableStyle.wordWrap = false;
            }

            if (MUILabel.lableStyle != null)
            {
                MUILabel.lableStyle.fontSize = (int)(MUILabel.defaultFontSize / UIView.GetAView().ratio);
                //if (UIView.GetAView().ratio < 1) lableStyle.fontSize -= 1;
                MUILabel.lableStyle.alignment = TextAnchor.MiddleLeft;
                MUILabel.lableStyle.normal.textColor = COLOR_DARK_TEXT;

                //WordSize = lableStyle.CalcSize(new GUIContent("H"));
            }

            if (parentPanel != null)
            {
                PANEL_HEIGHT = parentPanel.height - 8;
                LINE1 = 2; // MUISkin.UIToScreen(2);
                LINE2 = (int)((PANEL_HEIGHT) / 2) + 2; // MUISkin.UIToScreen((float)(int)((PANEL_HEIGHT) / 2));
                LINEW = LINE2 - LINE1 + 2;
                LINE1 += 0; LINE2 -= 2;
            }
        }

        public virtual void calcSizePosition()
        {
           
        }
        
        public virtual void UpdateData()
        {

        }

        float time = 0;
        public override void LateUpdate()
        {
            base.LateUpdate();

            if (time==0) UpdateData();
            time += Time.deltaTime;
            if (time >= 1f)
                time = 0f;
            
        }

    }
}
