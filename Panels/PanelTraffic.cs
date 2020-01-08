using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework;
using ColossalFramework.Globalization;
using IINS.UIExt;
using System;
using System.IO;

namespace IINS.ExtendedInfo
{
    public class PanelTraffic : ExtendedPanel
    {
        public PanelTraffic()
        {
            name = this.GetType().Name;
            relevantComponent = parentPanel.Find("Heat'o'meter");
        }

        public override void Awake()
        {
            base.Awake();
            this.size = new Vector2(108, PANEL_HEIGHT);
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
            string s = "";
            s = Locale.Get("INFO_PUBLICTRANSPORT_COUNT");
            s = s.Substring(0, s.IndexOf("{"));
            if (s.Trim()=="")
            {
                s = Locale.Get("INFO_PUBLICTRANSPORT_COUNT");
                s = s.Substring(s.IndexOf("}") + 1);
            }
            lblPublicTransportAverage.tooltip = Locale.Get("INFO_PUBLICTRANSPORT_TITLE") + " : " + Locale.Get("INFO_PUBLICTRANSPORT_CITIZENS") + s;
        }

        public override void ResetPositionSize()
        {

            if (relevantComponent != null)
            {
                absolutePosition = relevantComponent.absolutePosition;
                if (mainAspectRatio > 0f && mainAspectRatio < 1.9f)
                    relativePosition = new Vector3(868f, 4.0f);
                else
                    relativePosition = new Vector3(relativePosition.x + 90, 4.0f);
            }
        }

        MUILabel lblIcon = null;
        MUILabel lblPublicTransportAverage = null;
        MUILabel lblVehicleCount = null;
        //MUILabel lblVehicleParked = null;

        void InitControls()
        {
            LocaleManager.eventLocaleChanged += OnLocaleChanged;
            // 图标
            lblIcon = this.AddUIComponent<MUILabel>();
            lblIcon.size = new Vector2(PANEL_HEIGHT - 2, PANEL_HEIGHT - 2);
            lblIcon.relativePosition = new Vector3(2, 2);
            lblIcon.backgroundSprite = "InfoIconTrafficCongestionFocused";
            // 每周公共运输量
            lblPublicTransportAverage = this.AddUIComponent<MUILabel>();
            lblPublicTransportAverage.size = new Vector2(65, LINEW);
            lblPublicTransportAverage.relativePosition = new Vector3(36, LINE1);
            lblPublicTransportAverage.textColor = ExtendedPanel.COLOR_TEXT;
            lblPublicTransportAverage.textAlignment = UIHorizontalAlignment.Right;
            lblPublicTransportAverage.fontStyle = FontStyle.Bold;
            //lblPublicTransportAverage.fontSize = (int)MUISkin.UIToScreen(12f);
            // 车辆
            lblVehicleCount = this.AddUIComponent<MUILabel>();
            lblVehicleCount.size = new Vector2(55, LINEW);
            //lblVehicleCount.relativePosition = new Vector3(55, LINE2);
            lblVehicleCount.relativePosition = new Vector3(48, LINE2);
            lblVehicleCount.textColor = ExtendedPanel.COLOR_CAPTION;
            lblVehicleCount.textAlignment = UIHorizontalAlignment.Right;
            lblVehicleCount.tooltipLocaleID = "ASSETTYPE_VEHICLE";
            //lblVehicleParked = this.AddUIComponent<MUILabel>();
            //lblVehicleParked.size = new Vector2(55, LINEW);
            //lblVehicleParked.relativePosition = new Vector3(78, LINE2);
            //lblVehicleParked.textColor = ExtendedPanel.COLOR_CAPTION;
            //lblVehicleParked.textAlignment = UIHorizontalAlignment.Right;

            lblIcon.eventClick += OnSetInfoModeClick;
            lblPublicTransportAverage.eventClick += OnSetInfoModeClick;
            lblVehicleCount.eventClick += OnSetInfoModeClick;

            lblIcon.playAudioEvents = true;
            lblPublicTransportAverage.playAudioEvents = true;
            lblVehicleCount.playAudioEvents = true;
        }

        void DoneControls()
        {
            LocaleManager.eventLocaleChanged -= OnLocaleChanged;
            Destroy(lblIcon); lblIcon = null;
            Destroy(lblPublicTransportAverage); lblPublicTransportAverage = null;
        }

        void OnLocaleChanged()
        {
            string s = "";
            s = Locale.Get("INFO_PUBLICTRANSPORT_COUNT");
            s = s.Substring(0, s.IndexOf("{"));
            if (s.Trim() == "")
            {
                s = Locale.Get("INFO_PUBLICTRANSPORT_COUNT");
                s = s.Substring(s.IndexOf("}") + 1);
            }
            lblPublicTransportAverage.tooltip = Locale.Get("INFO_PUBLICTRANSPORT_TITLE") + " : " + Locale.Get("INFO_PUBLICTRANSPORT_CITIZENS") + s;
        }

        public override void OnScreenSizeChagned()
        {
            base.OnScreenSizeChagned();
            if (lblIcon != null)
            {
                lblIcon.size = new Vector2(PANEL_HEIGHT - 2, PANEL_HEIGHT - 2);
            }
            if (lblPublicTransportAverage != null)
            {
                //lblPublicTransportAverage.fontSize = (int)MUISkin.UIToScreen(12f);
                lblPublicTransportAverage.size = new Vector2(66, LINEW);
                lblPublicTransportAverage.relativePosition = new Vector3(36, LINE1);
            }

            if (lblVehicleCount != null)
            {
                lblVehicleCount.size = new Vector2(55, LINEW);
                lblVehicleCount.relativePosition = new Vector3(48, LINE2);
                //lblVehicleParked.relativePosition = new Vector3(55, LINE2);
                //lblVehicleParked.size = new Vector2(55, LINEW);
            }
        }

        public override void UpdateData()
        {
            lblPublicTransportAverage.text = CityInfoDatas.PublicTransportAverage.text;
            lblVehicleCount.text = CityInfoDatas.VehicleCount.text;
            //lblVehicleParked.text = CityInfoDatas.VehicleParked.text;
        }

        public override void OnDrawPanel()
        {
            base.OnDrawPanel();
        }

        private void DoDoubleClick(UIComponent component, UIMouseEventParameter eventParam)
        {
        }

        private void OnSetInfoModeClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (component == lblPublicTransportAverage)
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Transport, InfoManager.SubInfoMode.Default);
            else if (component == lblVehicleCount || component == lblIcon)
            {
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Traffic, InfoManager.SubInfoMode.Default);
                UIView.library.Hide("TrafficInfoViewPanel");
            }
        }

    }
}