using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework;
using IINS.UIExt;
using System;
using System.IO;
using ColossalFramework.Packaging;
using ColossalFramework.PlatformServices;

namespace IINS.ExtendedInfo
{
    public class ExtendedInfoManager : Singleton<ExtendedInfoManager>
    {
        private static UIComponent _infopanel = null;
        public static UIComponent infopanel
        {
            get
            {
                if (_infopanel == null)
                {
                    _infopanel = UIView.GetAView().FindUIComponent("InfoPanel");
                }
                return _infopanel;
            }
        }

        public ExtendedInfoManager()
        {
            name = "IINS-ExtendedInfoManager";
        }

        public static void run()
        {
            Singleton<ExtendedInfoManager>.Ensure();
            Singleton<ExtendedInfoManager>.instance.Initialize();
            Singleton<ExtendedInfoManager>.instance.enabled = true;

            RadionVisible = PanelTimer.IsRadioToggle();
        }

        public static bool ChirperVisible = false;
        public static bool RadionVisible = false;
        public static bool AudioMuteAll = false;

        public static void stop()
        {
            if (Singleton<ExtendedInfoManager>.exists)
            {
                Singleton<ExtendedInfoManager>.instance.enabled = false;
                Singleton<ExtendedInfoManager>.instance.Deinitialize();
            }

            PanelTimer.ToggleRadio(false);
        }

        public static TimeControler timeControler = null;
        private void Initialize()
        {
            if (PlatformService.apiBackend != APIBackend.Steam) return;

            if (timeControler == null)
                timeControler = gameObject.AddComponent<TimeControler>();

            Singleton<CityInfoDatas>.Ensure();
            Singleton<CityInfoDatas>.instance.UpdateDataAll();
            Singleton<CityInfoDatas>.instance.enabled = true;

            if (PopulationPanel == null) UpdatePanelsVisitable(true);

            if (CityInfoDatas.TimeWarpMod_sunManager == null)
                HookZoomButtonControls();

            if (ChirpPanel.instance != null)
                ChirpPanel.instance.gameObject.SetActive(ChirperVisible);

            PanelTimer.ToggleRadio(RadionVisible);

            if (Singleton<AudioManager>.exists)
                AudioMuteAll = Singleton<AudioManager>.instance.MuteAll;
        }

        private void Deinitialize()
        {
            if (ChirpPanel.instance != null)
                ChirperVisible = ChirpPanel.instance.gameObject.activeSelf;

            if (CityInfoDatas.TimeWarpMod_sunManager == null) UnHookZoomButtonControls();

            UpdatePanelsVisitable(false);
            Singleton<CityInfoDatas>.instance.enabled = false;

            MUISkin.Done();
        }

        public T showPanel<T>() where T : ExtendedPanel
        {
            T panel = null;

            if (infopanel != null)
            {
                panel = UIView.GetAView().FindUIComponent<T>(typeof(T).Name);

                if (panel == null)
                {
                    panel = infopanel.AddUIComponent(typeof(T)) as T;
                    panel.name = typeof(T).Name;
                }

                if (panel != null)
                {
                    panel.calcSizePosition();
                    panel.Show();
                }
            }
            return panel;
        }

        public void closePanel<T>() where T : ExtendedPanel
        {
            T panel = UIView.GetAView().FindUIComponent<T>(typeof(T).Name);
            if (panel != null)
            {
                panel.parent.RemoveUIComponent(panel);
                if (infopanel != null)
                    infopanel.RemoveUIComponent(panel);
                Destroy(panel);
                panel = null;
            }
        }

        public static PanelPopulation PopulationPanel;
        public static PanelIncome IncomePanel;
        public static PanelTraffic TrafficPanel;
        public static PanelCity CityPanel;
        public static PanelTimer TimerPanel;
        public static PanelCapacities CapacitiesPanel;

        public void UpdatePanelsVisitable(bool show)
        {
            if (_infopanel == null)
            {
                if (UIView.GetAView() != null)
                    _infopanel = UIView.GetAView().FindUIComponent("InfoPanel");
            }

            if (show)
            {
                PopulationPanel = showPanel<PanelPopulation>();
                IncomePanel = showPanel<PanelIncome>();
                TrafficPanel = showPanel<PanelTraffic>();
                CityPanel = showPanel<PanelCity>();
                TimerPanel = showPanel<PanelTimer>();
                CapacitiesPanel = showPanel<PanelCapacities>();
            }
            else
            {
                closePanel<PanelPopulation>();
                closePanel<PanelIncome>();
                closePanel<PanelTraffic>();
                closePanel<PanelCity>();
                closePanel<PanelTimer>();
                closePanel<PanelCapacities>();
                Destroy(PopulationPanel); PopulationPanel = null;
                Destroy(IncomePanel); IncomePanel = null;
                Destroy(TrafficPanel); TrafficPanel = null;
                Destroy(CityPanel); CityPanel = null;
                Destroy(TimerPanel); TimerPanel = null;
                Destroy(CapacitiesPanel); CapacitiesPanel = null;
            }
        }


        private bool ZoomButtonHooked = false;
        private string zoomButtonbackgroundSprite1;
        private string zoomButtonbackgroundSprite2;
        public void HookZoomButtonControls()
        {
            if (!ZoomButtonHooked)
            {
                var obj = GameObject.Find("ZoomButton");
                if (obj != null)
                {
                    UIMultiStateButton zoomButton = obj.GetComponent<UIMultiStateButton>();

                    zoomButton.eventMouseMove += OnZoomButtonMouseMoved;
                    zoomButton.eventMouseUp += OnZoomButtonMouseMoved;

                    zoomButtonbackgroundSprite1 = zoomButton.backgroundSprites[0].hovered;
                    zoomButtonbackgroundSprite2 = zoomButton.backgroundSprites[1].hovered;
                    zoomButton.backgroundSprites[0].hovered = "";
                    zoomButton.backgroundSprites[1].hovered = "";
                    ZoomButtonHooked = true;
                }
            }
        }

        public void UnHookZoomButtonControls()
        {
            if (ZoomButtonHooked)
            {
                var obj = GameObject.Find("ZoomButton");
                if (obj != null)
                {
                    UIMultiStateButton zoomButton = obj.GetComponent<UIMultiStateButton>();
                    if (zoomButton != null)
                    {
                        zoomButton.eventMouseMove -= OnZoomButtonMouseMoved;
                        zoomButton.eventMouseUp -= OnZoomButtonMouseMoved;
                        zoomButton.backgroundSprites[0].hovered = zoomButtonbackgroundSprite1;
                        zoomButton.backgroundSprites[1].hovered = zoomButtonbackgroundSprite2;

                    }
                }
                ZoomButtonHooked = false;
            }
        }

        void OnZoomButtonMouseMoved(UIComponent component, UIMouseEventParameter eventParam)
        {
            Vector2 pos;
            component.GetHitPosition(eventParam.ray, out pos);

            Vector2 center = new Vector2(30, 30);

            float angle = Vector2.Angle(new Vector3(0, 30), pos - center);

            if (pos.x > center.x)
                angle = 360.0f - angle;

            float time = (angle * 12.0f) / 180.0f;

            if (eventParam.buttons == UIMouseButton.Right)
                CityInfoDatas.instance.WorldTimeOfDay = time;

        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (Singleton<InfoManager>.instance.CurrentMode != InfoManager.InfoMode.None)
                {
                    UIView.library.Hide("PauseMenu");
                    SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
                    UIView.playSoundDelegate(UIView.GetAView().defaultClickSound, 1f);
                }
            }
        }

        public static void SetCurrentMode(InfoManager.InfoMode Mode, InfoManager.SubInfoMode SubMode)
        {
            WorldInfoPanel.Hide<CityServiceWorldInfoPanel>();
            ToolsModifierControl.mainToolbar.CloseToolbar();
            var IM = Singleton<InfoManager>.instance;
            var mm = IM.CurrentMode;
            var sm = IM.CurrentSubMode;
            if (mm == Mode && sm == SubMode)
            {
                Singleton<InfoManager>.instance.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
            }
            else
            {
                Singleton<InfoManager>.instance.SetCurrentMode(Mode, SubMode);
            }
        }
    }
}