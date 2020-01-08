using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework;
using ColossalFramework.Globalization;
using IINS.UIExt;
using System;
using System.IO;

namespace IINS.ExtendedInfo
{
    public class PanelCity : ExtendedPanel
    {
        private UIComponent CityInfoButton = null;
        private UIComponent DemandBack = null;

        public PanelCity()
        {
            name = this.GetType().Name;
            relevantComponent = parentPanel.Find("Name");
            CityInfoButton = parentPanel.Find("CityInfo");
            DemandBack = parentPanel.Find("DemandBack");
        }

        public override void Awake()
        {
            base.Awake();
            this.size = new Vector2(295, PANEL_HEIGHT);
            this.eventDoubleClick += DoDoubleClick;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            DoneControls();
        }

        public override void Start()
        {
            base.Start();
            InitControls();
        }

        public override void ResetPositionSize()
        {
            if (relevantComponent != null)
            {
                absolutePosition = CityInfoButton.absolutePosition;

                if (mainAspectRatio > 0f && mainAspectRatio < 1.9f)
                {
                    relativePosition = new Vector3(340f, 4.0f);
                    if (DemandBack != null)
                    {
                        DemandBack.relativePosition = new Vector3(638f, 2.5f);
                    }
                }
                else
                {
                    relativePosition = new Vector3(relativePosition.x, 4.0f);
                    if (DemandBack != null)
                    {
                        DemandBack.relativePosition = new Vector3(643.5f, 2.5f); 
                    }
                }

            }
        }

        UIButton lblIcon = null;
        MUILabel lblCityName = null;
        MUILabel lblMaptheme = null;
        MUILabel lblLandValue = null;
        MUILabel lblDistricts = null; 
        MUILabel lblDistrictsA = null;
        MUILabel lblBuildings = null;
        MUILabel lblBuildingsA = null;
        MUILabel lblTrees = null;
        MUILabel lblTreesA = null;
        MUILabel lblPark = null;
        MUILabel lblTouristsAverage = null;

        void InitControls()
        {
            LocaleManager.eventLocaleChanged += OnLocaleChanged;
            // 图标
            lblIcon = this.AddUIComponent<UIButton>();
            lblIcon.size = CityInfoButton.size;  //new Vector2(PANEL_HEIGHT - 2, PANEL_HEIGHT - 2);
            lblIcon.relativePosition = new Vector3(4, 4);
            lblIcon.normalBgSprite = "CityInfo";
            lblIcon.hoveredBgSprite = "CityInfoHovered";
            lblIcon.pressedBgSprite = "CityInfoPressed";
            lblIcon.isTooltipLocalized = true;
            lblIcon.tooltipLocaleID = "MAIN_CITYINFO";
            lblIcon.eventClick += OnCityInfoClicked;
            lblIcon.playAudioEvents = true;

            //lblIcon.backgroundSprite = "InfoIconTrafficCongestionFocused";
            // 城市名称
            lblCityName = this.AddUIComponent<MUILabel>();
            lblCityName.size = new Vector2(159, LINEW);
            lblCityName.relativePosition = new Vector3(6, LINE1);
            lblCityName.textColor = ExtendedPanel.COLOR_TEXT;
            lblCityName.textAlignment = UIHorizontalAlignment.Right;
            lblCityName.fontStyle = FontStyle.Bold;
            lblCityName.fontSize = (int)MUISkin.UIToScreen(14f);
            //lblCityName.text = Singleton<SimulationManager>.instance.m_metaData.m_CityName;

            // 地图主题
            lblMaptheme = this.AddUIComponent<MUILabel>();
            lblMaptheme.size = new Vector2(80, LINEW);
            lblMaptheme.relativePosition = new Vector3(36, LINE2);
            lblMaptheme.textColor = ExtendedPanel.COLOR_DARK_TEXT;
            lblMaptheme.textAlignment = UIHorizontalAlignment.Left;
            lblMaptheme.fontSize = (int)MUISkin.UIToScreen(10f);
            lblMaptheme.text = CityInfoDatas.Maptheme.text;
            lblMaptheme.tooltipLocaleID = "LOADPANEL_MAPTHEME";

            // 地价
            lblLandValue = this.AddUIComponent<MUILabel>();
            lblLandValue.size = new Vector2(79, LINEW);
            lblLandValue.relativePosition = new Vector3(86, LINE2);
            lblLandValue.textColor = ExtendedPanel.COLOR_DARK_GREEN;
            lblLandValue.textAlignment = UIHorizontalAlignment.Right;
            lblLandValue.fontStyle = FontStyle.Bold;
            lblLandValue.tooltipLocaleID = "INFO_LANDVALUE_TITLE";
            lblIcon.BringToFront();
            // 分区
            lblDistrictsA = this.AddUIComponent<MUILabel>();
            lblDistrictsA.size = new Vector2(16, 16);
            lblDistrictsA.relativePosition = new Vector3(175, LINE1 + 2);
            lblDistrictsA.backgroundSprite = "InfoIconDistricts";

            lblDistricts = this.AddUIComponent<MUILabel>();
            lblDistricts.size = new Vector2(45, LINEW);
            lblDistricts.relativePosition = new Vector3(180, LINE1);
            lblDistricts.textColor = COLOR_DARK_YELLOW;
            lblDistricts.textAlignment = UIHorizontalAlignment.Right;
            lblDistricts.tooltipLocaleID = "MAIN_AREAS"; 
            // 建筑
            lblBuildingsA = this.AddUIComponent<MUILabel>();
            lblBuildingsA.size = new Vector2(16, 16);
            lblBuildingsA.relativePosition = new Vector3(175, LINE2 + 2);
            lblBuildingsA.backgroundSprite = "ToolbarIconMonuments"; // "UnlockingBackground";
            lblBuildingsA.opacity = 0.4f;

            lblBuildings = this.AddUIComponent<MUILabel>();
            lblBuildings.size = new Vector2(45, LINEW);
            lblBuildings.relativePosition = new Vector3(180, LINE2);
            lblBuildings.textColor = COLOR_DARK_TEXT;
            lblBuildings.textAlignment = UIHorizontalAlignment.Right;
            lblBuildings.tooltipLocaleID = "ASSETTYPE_BUILDING";
            lblBuildings.fontSize = (int)MUISkin.UIToScreen(11f);
            // 树
            lblTreesA = this.AddUIComponent<MUILabel>();
            lblTreesA.size = new Vector2(16, 14);
            lblTreesA.relativePosition = new Vector3(235, LINE2 + 4);
            lblTreesA.backgroundSprite = "ToolbarIconBeautification";
            lblTreesA.opacity = 0.3f;

            lblTrees = this.AddUIComponent<MUILabel>();
            lblTrees.size = new Vector2(60, LINEW);
            lblTrees.relativePosition = new Vector3(230, LINE2);
            lblTrees.textColor = COLOR_DARK_TEXT;
            lblTrees.textAlignment = UIHorizontalAlignment.Right;
            lblTrees.tooltipLocaleID = "ASSETTYPE_TREE";
            lblTrees.fontSize = (int)MUISkin.UIToScreen(11f);
            // 休闲设置
            lblPark = this.AddUIComponent<MUILabel>();
            lblPark.size = new Vector2(12, 16);
            lblPark.relativePosition = new Vector3(235, LINE1 + 2);
            lblPark.backgroundSprite = "SubBarBeautificationExpansion1";
            lblPark.opacity = 0.3f;
            // 游客
            lblTouristsAverage = this.AddUIComponent<MUILabel>();
            lblTouristsAverage.size = new Vector2(60, LINEW);
            lblTouristsAverage.relativePosition = new Vector3(230, LINE1);
            lblTouristsAverage.textColor = COLOR_TEXT;
            lblTouristsAverage.textAlignment = UIHorizontalAlignment.Right;
            lblTouristsAverage.tooltipLocaleID = "INFO_PUBLICTRANSPORT_TOURISTS";
            lblTouristsAverage.fontSize = (int)MUISkin.UIToScreen(11f);

            lblBuildings.eventClick += OnSetInfoModeClick;
            lblLandValue.eventClick += OnSetInfoModeClick;
            lblLandValue.eventMouseUp += OnRightSetInfoModeClick;
            lblDistricts.eventClick += OnSetInfoModeClick;
            lblTrees.eventClick += OnSetInfoModeClick;
            lblCityName.eventClick += OnSetInfoModeClick;
            lblCityName.eventMouseUp += OnRightSetInfoModeClick;
            lblTouristsAverage.eventMouseUp += OnSetInfoModeClick;

            lblBuildings.playAudioEvents = true;
            lblLandValue.playAudioEvents = true;
            lblDistricts.playAudioEvents = true;
            lblTrees.playAudioEvents = true;
            lblCityName.playAudioEvents = true;
            lblTouristsAverage.playAudioEvents = true;
        }

        private void OnSetInfoModeClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (component == lblBuildings)
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.BuildingLevel, InfoManager.SubInfoMode.Default);
            else if (component == lblLandValue)
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.LandValue, InfoManager.SubInfoMode.Default);
            else if (component == lblDistricts)
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Districts, InfoManager.SubInfoMode.Default);
            else if (component == lblTrees)
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.NaturalResources, InfoManager.SubInfoMode.Default);
            else if (component == lblCityName)
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.NoisePollution, InfoManager.SubInfoMode.Default);
            else if (component == lblTouristsAverage)
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Connections, InfoManager.SubInfoMode.Tourism);
        }

        private void OnRightSetInfoModeClick(UIComponent component, UIMouseEventParameter P)
        {
            if (P.buttons == UIMouseButton.Right)
            {
                if (component == lblCityName)
                {
                    UIView.playSoundDelegate(this.GetUIView().defaultClickSound, 1f);
                    ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Pollution, InfoManager.SubInfoMode.Default);
                } else
                if (component == lblLandValue)
                {
                    UIView.playSoundDelegate(this.GetUIView().defaultClickSound, 1f);
                    ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Entertainment, InfoManager.SubInfoMode.Default);
                }
            }
        }

        void DoneControls()
        {
            LocaleManager.eventLocaleChanged -= OnLocaleChanged;
            
            Destroy(lblIcon); lblIcon = null;
            Destroy(lblCityName); lblCityName = null;
            Destroy(lblMaptheme); lblMaptheme = null; 
            Destroy(lblLandValue); lblLandValue = null;
            Destroy(lblDistricts); lblDistricts = null;
            Destroy(lblDistrictsA); lblDistrictsA = null;
            Destroy(lblTrees); lblTrees = null;
            Destroy(lblTreesA); lblTreesA = null;
        }

        void OnLocaleChanged()
        {
            CityInfoDatas.instance.UpdateDataAll();
            lblMaptheme.text = CityInfoDatas.Maptheme.text;
        }

        public override void OnScreenSizeChagned()
        {
            base.OnScreenSizeChagned();
            if (lblIcon != null)
            {
                //lblIcon.size = new Vector2(PANEL_HEIGHT - 2, PANEL_HEIGHT - 2);
                lblIcon.relativePosition = new Vector3(4, 4);
            }
            if (lblCityName != null)
            {
                lblCityName.fontSize = (int)MUISkin.UIToScreen(14f);
                lblCityName.size = new Vector2(159, LINEW);
                lblCityName.relativePosition = new Vector3(6, LINE1);
                lblLandValue.size = new Vector2(79, LINEW);
                lblLandValue.relativePosition = new Vector3(86, LINE2);
                lblMaptheme.fontSize = (int)MUISkin.UIToScreen(10f);
                lblMaptheme.size = new Vector2(80, LINEW);
                lblMaptheme.relativePosition = new Vector3(36, LINE2);
            }
            if (lblDistricts != null)
            {
                lblDistricts.size = new Vector2(45, LINEW);
                lblDistricts.relativePosition = new Vector3(180, LINE1);
                lblDistrictsA.size = new Vector2(16, 16);
                lblDistrictsA.relativePosition = new Vector3(175, LINE1 + 2);
            }
            if (lblBuildings != null)
            {
                lblBuildings.size = new Vector2(45, LINEW);
                lblBuildings.relativePosition = new Vector3(180, LINE2);
                lblBuildings.fontSize = (int)MUISkin.UIToScreen(11f);
                lblBuildingsA.size = new Vector2(16, 16);
                lblBuildingsA.relativePosition = new Vector3(175, LINE2 + 2);
            }
            if (lblTrees != null)
            {
                lblTrees.size = new Vector2(60, LINEW);
                lblTrees.relativePosition = new Vector3(230, LINE2);
                lblTrees.fontSize = (int)MUISkin.UIToScreen(11f);
                lblTreesA.size = new Vector2(16, 14);
                lblTreesA.relativePosition = new Vector3(235, LINE2 + 4);
            }
            if (lblPark != null)
            {
                lblPark.size = new Vector2(16, 12);
                lblPark.relativePosition = new Vector3(235, LINE1 + 4);
                lblTouristsAverage.size = new Vector2(60, LINEW);
                lblTouristsAverage.relativePosition = new Vector3(230, LINE1 - 2);
            }
        }

        public override void UpdateData()
        {
            lblCityName.text = CityInfoDatas.CityInfo.text;
            lblLandValue.text = CityInfoDatas.FormatMoney((long)CityInfoDatas.CityInfo.value, false);
            lblDistricts.text = CityInfoDatas.DistrictsCount.text;
            lblBuildings.text = CityInfoDatas.BuildingCount.text;
            lblTrees.text = CityInfoDatas.TreeCount.text;
            lblTouristsAverage.text = CityInfoDatas.TouristsAverage.text;
        }

        public override void OnDrawPanel()
        {
            base.OnDrawPanel();
            if (CanOnGUIDraw())
            {
                float W = MUISkin.UIToScreen(170);
                float T = 4f;
                float H = MUISkin.UIToScreen(PANEL_HEIGHT) - T * 2;

                GUI.DrawTexture(new Rect(W, T, 1, H), lineTexture);
                W = MUISkin.UIToScreen(230);
                GUI.DrawTexture(new Rect(W, T, 1, H), lineTexture);
            }
        }

        private void SetName()
        {
            if (Singleton<SimulationManager>.exists)
            {
                string cityName = Singleton<SimulationManager>.instance.m_metaData.m_CityName;
                if (cityName != null)
                {
                    CityInfoDatas.CityInfo.text = cityName;
                }
            }
        }

        public void OnCityInfoClicked(UIComponent comp, UIMouseEventParameter p)
        {
            CityInfoPanel panel = UIView.library.Get<CityInfoPanel>("CityInfoPanel");
            if (panel.isVisible)
            {
                UIView.library.Hide("CityInfoPanel");
            }
            else
            {
                UIView.library.Show<CityInfoPanel>("CityInfoPanel");
                panel.transform.position = p.source.transform.position; // new Vector3(p.source.transform.position.x + 10, p.source.transform.position.y, p.source.transform.position.z);
                panel.SetRenameCallback(new Action(this.SetName));
            }
        }

        private void DoDoubleClick(UIComponent component, UIMouseEventParameter eventParam)
        {
        }
    }
}
