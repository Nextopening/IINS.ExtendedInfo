using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework;
using ColossalFramework.Globalization;
using IINS.UIExt;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace IINS.ExtendedInfo
{
    public class PanelTimer : ExtendedPanel
    {
        UISprite gameTimeSprite;
        public PanelTimer()
        {
            name = this.GetType().Name;
            relevantComponent = parentPanel.Find("PanelTime");
            gameTimeSprite = relevantComponent.Find<UISprite>("Sprite");
        }

        private UISprite RushHourSprite = null;
        private UILabel RushHourTimeLabel = null;
        private static UIPanel _savePanel;
        public override void Awake()
        {
            base.Awake();
            this.size = new Vector2(310, PANEL_HEIGHT);
            this.eventDoubleClick += DoDoubleClick;
            var pnl = UIView.library.Get<PauseMenu>("PauseMenu");
            if (pnl != null)
                _savePanel = pnl.Find<UIPanel>("Menu");
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
            UpdateLocalTimeVisible();
            btnDayNightSwitch.tooltip = "Left Click: " + Locale.Get("TIMECONTROL") + "\nRight Click: " + Locale.Get("OPTIONS_ENABLE_DAY_NIGHT");
            btnWeatherSwitch.tooltip = "Left Click: " + Locale.Get("THEME_WORLD_WEATHER") + "\nRight Click: " + Locale.Get("OPTIONS_ENABLE_WEATHER");

            if (MODUtil.IsChinaLanguage())
            {
                btnRadioIconContainer.tooltip = "电台 开/关";
                btnMuteAudioContainer.tooltip = "静音";
            }
            else
            {
                btnRadioIconContainer.tooltip = "Toggle Radio";
                btnMuteAudioContainer.tooltip = "Mute Audio";
            }

            btnRadioIconContainer.opacity = IsRadioToggle() ? 0.5f : 0.1f;


            // 兼容 Rush Hours 
            if (CityInfoDatas.RushHourUI != null && ExtendedInfoManager.infopanel != null)
            {
                UIPanel _panelTime = ExtendedInfoManager.infopanel.Find<UIPanel>("PanelTime");
                if (_panelTime != null)
                {
                    RushHourSprite = _panelTime.Find<UISprite>("NewSprite");
                    RushHourTimeLabel = _panelTime.Find<UILabel>("NewTime");
                }
            }
        }

        UIMultiStateButton btnPlay = null;
        UIMultiStateButton btnSpeed = null;
        UISprite barTime = null;
        MUILabel lblGameTime = null;
        UISprite lblDayTimeA = null;
        MUILabel lblDayTime = null;
        MUILabel lblLocalTime = null;
        UISprite lblLocalTimeA = null;
        UISprite lblThermometerA = null;
        MUILabel lblThermometer = null;
        UIButton btnDayNightSwitch = null;
        UIButton btnWeatherSwitch = null;
        UIButton btnChirperContainer = null;
        UIButton btnRadioIconContainer = null;
        UIButton btnMuteAudioContainer = null;

        int showTimeTag = 0;

        void InitControls()
        {
            LocaleManager.eventLocaleChanged += OnLocaleChanged;
            // 昼夜转换
            btnDayNightSwitch = this.AddUIComponent<UIButton>();
            btnDayNightSwitch.size = new Vector2(18, 9);
            btnDayNightSwitch.relativePosition = new Vector3(4, 6);
            btnDayNightSwitch.normalBgSprite = "IconSunMoon";
            btnDayNightSwitch.playAudioEvents = true;
            //btnDayNightSwitch.tooltipLocaleID = "TIMECONTROL";
            btnDayNightSwitch.eventClick += OnDayNightSwitchClicked;
            btnDayNightSwitch.eventMouseUp += OnDayNightSwitchMouseUp;
            btnDayNightSwitch.opacity = 0.5f;
            btnDayNightSwitch.isTooltipLocalized = true;
            btnDayNightSwitch.spritePadding = new RectOffset();
            // 天气转换
            btnWeatherSwitch = this.AddUIComponent<UIButton>();
            btnWeatherSwitch.size = new Vector2(13, 13);
            btnWeatherSwitch.relativePosition = new Vector3(25, 4);
            btnWeatherSwitch.normalBgSprite = "IconPolicyOnlyElectricity";
            btnWeatherSwitch.playAudioEvents = true;
            //btnWeatherSwitch.tooltipLocaleID = "THEME_WORLD_WEATHER";
            btnWeatherSwitch.eventClick += OnWeatherSwitchClicked;
            btnWeatherSwitch.eventMouseUp += OnWeatherSwitchMouseUp;
            btnWeatherSwitch.opacity = 0.5f;
            btnWeatherSwitch.isTooltipLocalized = true;
            btnWeatherSwitch.spritePadding = new RectOffset();
            // 啾啾显示
            btnChirperContainer = this.AddUIComponent<UIButton>();
            btnChirperContainer.size = new Vector2(13, 13);
            btnChirperContainer.relativePosition = new Vector3(42, 2);
            btnChirperContainer.normalBgSprite = "ChirperIcon";
            btnChirperContainer.playAudioEvents = true;
            btnChirperContainer.isTooltipLocalized = true;
            btnChirperContainer.spritePadding = new RectOffset();
            btnChirperContainer.eventClick += OnChirperTogglerClicked;
            btnChirperContainer.tooltipLocaleID = "CHIRPER_NAME";
            if (ChirpPanel.instance != null)
            {
                btnChirperContainer.isVisible = true;
                btnChirperContainer.opacity = ChirpPanel.instance.gameObject.activeSelf ? 0.5f : 0.1f;
            }
            else
                btnChirperContainer.isVisible = false;

            // 电台显示
            btnRadioIconContainer = this.AddUIComponent<UIButton>();
            btnRadioIconContainer.size = new Vector2(13, 13);
            btnRadioIconContainer.relativePosition = new Vector3(58, 2);
            btnRadioIconContainer.normalFgSprite = "ADIcon";
            btnRadioIconContainer.playAudioEvents = true;
            btnRadioIconContainer.isTooltipLocalized = true;
            btnRadioIconContainer.spritePadding = new RectOffset();
            btnRadioIconContainer.eventClick += OnRadioTogglerClicked;

            // 静音
            btnMuteAudioContainer = this.AddUIComponent<UIButton>();
            btnMuteAudioContainer.size = new Vector2(13, 13);
            btnMuteAudioContainer.relativePosition = new Vector3(74, 2);
            btnMuteAudioContainer.normalFgSprite = "IconPolicyNoLoudNoisesHovered";
            btnMuteAudioContainer.playAudioEvents = true;
            btnMuteAudioContainer.isTooltipLocalized = true;
            btnMuteAudioContainer.spritePadding = new RectOffset();
            btnMuteAudioContainer.eventClick += (UIComponent comp, UIMouseEventParameter p) =>
            {
                ExtendedInfoManager.AudioMuteAll = !ExtendedInfoManager.AudioMuteAll;
                Singleton<AudioManager>.instance.MuteAll = ExtendedInfoManager.AudioMuteAll;
                btnMuteAudioContainer.opacity = Singleton<AudioManager>.instance.MuteAll ? 0.1f : 0.5f;
            };
            btnMuteAudioContainer.opacity = Singleton<AudioManager>.instance.MuteAll ? 0.1f : 0.5f;

            // 暂停按钮
            btnPlay = this.AddUIComponent<UIMultiStateButton>();
            btnPlay.size = new Vector2(11, 12);
            btnPlay.relativePosition = new Vector3(206, LINE2 + 2);
            btnPlay.isTooltipLocalized = true;
            btnPlay.tooltipLocaleID = "MAIN_PLAYPAUSE";
            btnPlay.eventClick += OnPlayClicked;
            btnPlay.playAudioEvents = true;
            btnPlay.spritePadding = new RectOffset();

            UIMultiStateButton.SpriteSet btnPlaySpriteSet0 = btnPlay.foregroundSprites[0];
            btnPlaySpriteSet0.normal = "ButtonPause";
            btnPlaySpriteSet0.hovered = "ButtonPauseHovered";
            btnPlaySpriteSet0.pressed = "ButtonPausePressed";
            btnPlaySpriteSet0.focused = "ButtonPauseFocused";

            btnPlay.backgroundSprites.AddState();
            btnPlay.foregroundSprites.AddState();
            UIMultiStateButton.SpriteSet btnPlaySpriteSet1 = btnPlay.foregroundSprites[1];
            btnPlaySpriteSet1.normal = "ButtonPlay";
            btnPlaySpriteSet1.hovered = "ButtonPlayHovered";
            btnPlaySpriteSet1.pressed = "ButtonPlayPressed";
            btnPlaySpriteSet1.focused = "ButtonPlayFocused";

            // 速度按钮
            btnSpeed = this.AddUIComponent<UIMultiStateButton>();
            btnSpeed.size = new Vector2(28, 22);
            btnSpeed.relativePosition = new Vector3(280, LINE2 - 4);
            btnSpeed.isTooltipLocalized = true;
            btnSpeed.tooltipLocaleID = "MAIN_SPEED";
            btnSpeed.eventClick += OnSpeedClicked;
            btnSpeed.playAudioEvents = true;
            btnSpeed.spritePadding = new RectOffset();
            UIMultiStateButton.SpriteSet btnSpeedSpriteSet0 = btnSpeed.foregroundSprites[0];
            btnSpeedSpriteSet0.normal = "IconSpeed1Normal";
            btnSpeedSpriteSet0.hovered = "IconSpeed1Hover";
            btnSpeedSpriteSet0.pressed = "IconSpeed2Normal";

            btnSpeed.backgroundSprites.AddState();
            btnSpeed.foregroundSprites.AddState();
            UIMultiStateButton.SpriteSet btnSpeedSpriteSet1 = btnSpeed.foregroundSprites[1];
            btnSpeedSpriteSet1.normal = "IconSpeed2Normal";
            btnSpeedSpriteSet1.hovered = "IconSpeed2Hover";
            btnSpeedSpriteSet1.pressed = "IconSpeed3Normal";

            btnSpeed.backgroundSprites.AddState();
            btnSpeed.foregroundSprites.AddState();
            UIMultiStateButton.SpriteSet btnSpeedSpriteSet2 = btnSpeed.foregroundSprites[2];
            btnSpeedSpriteSet2.normal = "IconSpeed3Normal";
            btnSpeedSpriteSet2.hovered = "IconSpeed3Hover";
            btnSpeedSpriteSet2.pressed = "IconSpeed1Normal";

            // 时间条
            if (gameTimeSprite != null)
            {
                barTime = this.AddUIComponent<UISprite>();
                barTime.name = "NewSprite";
                barTime.size = new Vector2(140, 12);
                barTime.relativePosition = new Vector3(166, 4);
                barTime.atlas = gameTimeSprite.atlas;
                barTime.spriteName = gameTimeSprite.spriteName;
                barTime.fillAmount = 0.5f;
                barTime.fillDirection = UIFillDirection.Horizontal;
                barTime.color = gameTimeSprite.color;
                barTime.fillAmount = gameTimeSprite.fillAmount;
            }
            // 游戏日期标签
            lblGameTime = this.AddUIComponent<MUILabel>();
            lblGameTime.size = new Vector2(120, LINEW);
            lblGameTime.relativePosition = new Vector3(170, LINE2 - 3);
            lblGameTime.textColor = ExtendedPanel.COLOR_DARK_TEXT;
            lblGameTime.textAlignment = UIHorizontalAlignment.Center;
            //lblGameTime.fontStyle = FontStyle.Bold;
            lblGameTime.fontSize = (int)MUISkin.UIToScreen(11f);

            // 游戏时间标签
            lblDayTimeA = this.AddUIComponent<UISprite>();
            lblDayTimeA.size = new Vector2(14, 14);
            lblDayTimeA.relativePosition = new Vector3(107, 5);
            lblDayTimeA.spriteName = "InfoIconEntertainmentFocused";
            lblDayTimeA.opacity = 0.3f;
            lblDayTimeA.playAudioEvents = (CityInfoDatas.TimeWarpMod_sunManager == null);
            lblDayTimeA.eventClick += OnDayTimeClicked;

            lblDayTime = this.AddUIComponent<MUILabel>();
            lblDayTime.size = new Vector2(40, LINEW);
            lblDayTime.relativePosition = new Vector3(115, LINE1);
            lblDayTime.textColor = ExtendedPanel.COLOR_TEXT;
            lblDayTime.textAlignment = UIHorizontalAlignment.Right;
            lblDayTime.fontStyle = FontStyle.Bold;
            lblDayTime.fontSize = (int)MUISkin.UIToScreen(11f);
            lblDayTime.playAudioEvents = (CityInfoDatas.TimeWarpMod_sunManager == null);
            lblDayTime.eventClick += OnDayTimeClicked;

            // 本地时间
            lblLocalTimeA = this.AddUIComponent<UISprite>();
            lblLocalTimeA.size = new Vector2(7, 7);
            lblLocalTimeA.relativePosition = new Vector3(4, LINE2 + 5);
            lblLocalTimeA.spriteName = "OptionBaseFocused";
            lblLocalTimeA.opacity = 0.3f;

            lblLocalTime = this.AddUIComponent<MUILabel>();
            lblLocalTime.size = new Vector2(45, LINEW);
            lblLocalTime.relativePosition = new Vector3(14, LINE2 - 2);
            lblLocalTime.textColor = ExtendedPanel.COLOR_DARK_YELLOW;
            lblLocalTime.textAlignment = UIHorizontalAlignment.Left;
            lblLocalTime.fontStyle = FontStyle.Bold;
            lblLocalTime.fontSize = (int)MUISkin.UIToScreen(11f);
            lblLocalTime.eventClick += OnLocalTimeClick;
            lblLocalTime.playAudioEvents = true;

            // 温度
            lblThermometerA = this.AddUIComponent<UISprite>();
            lblThermometerA.size = new Vector2(16, 16);
            lblThermometerA.relativePosition = new Vector3(105, LINE2);
            lblThermometerA.spriteName = "ThermometerIcon";
            lblThermometerA.opacity = 0.3f;

            lblThermometer = this.AddUIComponent<MUILabel>();
            lblThermometer.size = new Vector2(40, LINEW);
            lblThermometer.relativePosition = new Vector3(115, LINE2);
            lblThermometer.textColor = ExtendedPanel.COLOR_DARK_TEXT;
            lblThermometer.textAlignment = UIHorizontalAlignment.Right;
            //lblThermometer.fontStyle = FontStyle.Bold;
            lblThermometer.fontSize = (int)MUISkin.UIToScreen(11f);
            lblThermometer.tooltipLocaleID = "MAIN_TEMPERATURE";

            setGameState();
        }

        void DoneControls()
        {
            LocaleManager.eventLocaleChanged -= OnLocaleChanged;
            Destroy(btnPlay); btnPlay = null;
            Destroy(btnSpeed); btnSpeed = null;
            Destroy(lblGameTime); lblGameTime = null;
            Destroy(lblDayTime); lblDayTime = null;
            Destroy(barTime); barTime = null;
            Destroy(lblLocalTime); lblLocalTime = null;
            Destroy(lblLocalTimeA); lblLocalTimeA = null;
            Destroy(lblThermometer); lblThermometer = null;
            Destroy(lblThermometerA); lblThermometerA = null;
            Destroy(btnDayNightSwitch); btnDayNightSwitch = null;
            Destroy(btnWeatherSwitch); btnWeatherSwitch = null;
            Destroy(btnChirperContainer); btnChirperContainer = null;
            Destroy(btnRadioIconContainer); btnRadioIconContainer = null;
        }

        void OnLocaleChanged()
        {
            if (btnDayNightSwitch != null)
            {
                btnDayNightSwitch.tooltip = "Left Click: " + Locale.Get("TIMECONTROL") + "\nRight Click: " + Locale.Get("OPTIONS_ENABLE_DAY_NIGHT");
                btnWeatherSwitch.tooltip = "Left Click: " + Locale.Get("THEME_WORLD_WEATHER") + "\nRight Click: " + Locale.Get("OPTIONS_ENABLE_WEATHER");
            }

            if (btnRadioIconContainer != null)
            {
                if (MODUtil.IsChinaLanguage())
                {
                    btnRadioIconContainer.tooltip = "电台 开/关";
                    btnMuteAudioContainer.tooltip = "静音";
                }
                else
                {
                    btnRadioIconContainer.tooltip = "Toggle Radio";
                    btnMuteAudioContainer.tooltip = "Mute Audio";
                }
            }
        }

        public override void OnScreenSizeChagned()
        {
            base.OnScreenSizeChagned();
            if (btnPlay != null)
            {
                btnPlay.size = new Vector2(11, 12);
                btnPlay.relativePosition = new Vector3(166, LINE2 + 2);
                btnSpeed.size = new Vector2(28, 22);
                btnSpeed.relativePosition = new Vector3(280, LINE2 - 4);
            }

            if (lblGameTime != null)
            {
                lblGameTime.size = new Vector2(120, LINEW);
                lblGameTime.relativePosition = new Vector3(170, LINE2 - 3);
                lblGameTime.fontSize = (int)MUISkin.UIToScreen(11f);

                lblDayTime.size = new Vector2(40, LINEW);
                lblDayTime.relativePosition = new Vector3(115, LINE1);
                lblLocalTimeA.size = new Vector2(7, 7);
                lblLocalTimeA.relativePosition = new Vector3(4, LINE2 + 5);
                lblLocalTime.size = new Vector2(45, LINEW);
                lblLocalTime.relativePosition = new Vector3(14, LINE2 - 2);
            }
            if (lblThermometer != null)
            {
                lblThermometer.size = new Vector2(40, LINEW);
                lblThermometer.relativePosition = new Vector3(115, LINE2);
                lblThermometerA.size = new Vector2(16, 16);
                lblThermometerA.relativePosition = new Vector3(105, LINE2 + 2);
            }
        }

        void UpdateLocalTimeVisible()
        {
            switch (showTimeTag)
            {
                case 0:
                    lblLocalTime.textColor = ExtendedPanel.COLOR_DARK_YELLOW;
                    break;
                case 1:
                    lblLocalTime.textColor = ExtendedPanel.COLOR_DARK_PURPLE;
                    break;
            }

            UpdateLocalTimeText();
        }

        void UpdateLocalTimeText()
        {
            switch (showTimeTag)
            {
                case 0:
                    lblLocalTime.text = DateTime.Now.ToString("HH:mm:ss", LocaleManager.cultureInfo);
                    break;
                case 1:
                    lblLocalTime.text = CityInfoDatas.PlayingTime.text;
                    break;
            }

        }

        //public override void OnGUI()
        //{
        //    base.OnGUI();
        //    GUI.Label(new Rect(100, 100, 1000, 20), "aspect = " + Camera.main.aspect + "/" + mainAspectRatio);
        //    //GUI.Label(new Rect(100, 130, 1000, 20), "sprites = " + (sprites));
        //}

        private float h;
        private void updateRushHourEventSprites(UISprite[] sprite)
        {
            sprites = 0;
            float per = barTime.width / RushHourSprite.width;
            for (int i = 0; i < sprite.Length; i++)
            {
                if (sprite[i].name.Equals("UISprite"))
                {
                    sprites += 1;
                    sprite[i].transform.parent = barTime.transform;
                    //sprite[i].width = sprite[i].width * per;
                    sprite[i].height = barTime.height - 4;

                    float startPercent = (float)(sprite[i].relativePosition.x / RushHourSprite.width);
                    if (sprites == 1)
                    {
                        h = sprite[i].relativePosition.x; // (int)Mathf.Round((float)(startPercent * 24D));
                    }
                    float endPosition = (float)(sprite[i].width + sprite[i].relativePosition.x);
                    float endPercent = (float)(endPosition / RushHourSprite.width);
                    float startPosition = (float)(barTime.width * startPercent);
                    endPosition = (float)(barTime.width * endPercent);
                    int endWidth = (int)Mathf.Round(endPosition - startPosition);

                    //float xpos = (float)((sprite[i].relativePosition.x / RushHourSprite.width) * barTime.width);
                    sprite[i].absolutePosition = barTime.absolutePosition;
                    sprite[i].relativePosition = new Vector3(85 + startPosition, 2);
                    sprite[i].width = endWidth;
                }
            }
        }

        private int sprites;
        public override void UpdateData()
        {
            setGameState();

            if (CityInfoDatas.RushHourUI != null)
            {
                if (RushHourTimeLabel != null/* && RushHourTimeLabel.isVisible*/)
                {
                    lblGameTime.text = RushHourTimeLabel.text;
                    lblGameTime.tooltip = RushHourSprite.tooltip;
                }
                else
                    lblGameTime.text = CityInfoDatas.GameTime.text;
            }
            else
                lblGameTime.text = CityInfoDatas.GameTime.text;

            lblDayTime.text = CityInfoDatas.GameTimeOfDay.text;
            lblThermometer.text = CityInfoDatas.Temperatur.text;

            //UpdateLocalTimeVisible();

            btnDayNightSwitch.opacity = Singleton<CityInfoDatas>.instance.enableDayNight ? 0.5f : 0.05f;
            btnDayNightSwitch.playAudioEvents = Singleton<CityInfoDatas>.instance.enableDayNight;
            btnWeatherSwitch.opacity = Singleton<CityInfoDatas>.instance.enableWeather ? 0.5f : 0.1f;
            btnWeatherSwitch.playAudioEvents = Singleton<CityInfoDatas>.instance.enableWeather;

            var timeControler = ExtendedInfoManager.timeControler;
            if (timeControler != null)
            {
                lblDayTime.textColor = (timeControler.speed.num == 0) ? MUIUtil.DarkenColor(ExtendedPanel.COLOR_TEXT, 0.5f) : ExtendedPanel.COLOR_TEXT;
                lblDayTimeA.opacity = (timeControler.speed.num == 0) ? 0.2f : 1f;
            }

            if (ChirpPanel.instance != null)
            {
                btnChirperContainer.opacity = ChirpPanel.instance.gameObject.activeSelf ? 0.5f : 0.1f;
            }
        }

        private float showtime = 0;
        private bool GamePauseState = false;
        private int GameSpeed = 0;

        public override void Update()
        {
            base.Update();
            if (gameTimeSprite != null && CityInfoDatas.RushHourUI != null)
            {
                if (RushHourSprite != null /*&& RushHourSprint.isVisible*/)
                {
                    barTime.color = RushHourSprite.color;
                    barTime.fillAmount = RushHourSprite.fillAmount;
                    var sprite = RushHourSprite.GetComponentsInChildren<UISprite>();
                    if (sprite.Length > 1)
                        updateRushHourEventSprites(sprite);
                }
                else
                {
                    barTime.color = gameTimeSprite.color;
                    barTime.fillAmount = gameTimeSprite.fillAmount;
                }
            }
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            if (GamePauseState != CityInfoDatas.isGamePaused())
            {
                GamePauseState = CityInfoDatas.isGamePaused();
                setGameState();
            }

            if (Singleton<SimulationManager>.exists)
            {
                if (GameSpeed != Singleton<SimulationManager>.instance.SelectedSimulationSpeed)
                {
                    GameSpeed = Singleton<SimulationManager>.instance.SelectedSimulationSpeed;
                    setGameState();
                }
            }

            showtime += Time.deltaTime;

            if (showtime >= 1f) // 每秒显示一次
            {
                showtime = 0f;
                UpdateLocalTimeText();
            }

            if (Singleton<AudioManager>.exists && _savePanel != null)
            {
                if (!_savePanel.isVisible)
                {
                    Singleton<AudioManager>.instance.MuteAll = ExtendedInfoManager.AudioMuteAll;
                    if (btnMuteAudioContainer != null)
                        btnMuteAudioContainer.opacity = Singleton<AudioManager>.instance.MuteAll ? 0.1f : 0.5f;
                }
            }

            if (gameTimeSprite != null)
            {
                if (CityInfoDatas.RushHourUI == null)
                {

                    barTime.color = gameTimeSprite.color;
                    barTime.fillAmount = gameTimeSprite.fillAmount;
                }
            }
        }

        public override void OnDrawPanel()
        {
            base.OnDrawPanel();
            if (CanOnGUIDraw())
            {
                float W = MUISkin.UIToScreen(160);
                float T = 4f;
                float H = MUISkin.UIToScreen(PANEL_HEIGHT) - T * 2;

                GUI.DrawTexture(new Rect(W, T, 1, H), lineTexture);
            }
        }

        private void DoDoubleClick(UIComponent component, UIMouseEventParameter eventParam)
        {
        }


        public void OnChirperTogglerClicked(UIComponent comp, UIMouseEventParameter p)
        {
            if (ChirpPanel.instance != null)
            {
                ChirpPanel.instance.gameObject.SetActive(!ChirpPanel.instance.gameObject.activeSelf);
                ExtendedInfoManager.ChirperVisible = ChirpPanel.instance.gameObject.activeSelf;
            }

            if (ChirpPanel.instance != null)
                btnChirperContainer.opacity = ChirpPanel.instance.gameObject.activeSelf ? 0.5f : 0.1f;
        }

        private static RadioChannelInfo savedRadioInfo;
        public static void ToggleRadio(bool enabled)
        {
            var radioPanel = GameObject.Find("RadioPanel");
            if (radioPanel != null)
            {
                RadioPanel rp = radioPanel.GetComponent(typeof(RadioPanel)) as RadioPanel;
                if (rp != null)
                {
                    var btn = rp.Find<UIButton>("RadioButton");
                    var pnl = rp.Find<UIPanel>("RadioPlayingPanel");
                    if (btn != null && pnl != null)
                    {
                        if (pnl.isVisible)
                        {
                            pnl.isVisible = enabled;
                        }
                        else
                        {
                            btn.isVisible = enabled;
                        }

                        bool isDisabled = !btn.isVisible && !pnl.isVisible;

                        AudioManager AM = Singleton<AudioManager>.instance;
                        if (isDisabled)
                        {
                            savedRadioInfo = AM.GetActiveRadioChannelInfo();
                            AM.SetActiveRadioChannel(0);
                        }
                        else
                        {
                            AM.SetActiveRadioChannelInfo(savedRadioInfo);
                            AM.PlayAudio(AM.CurrentListenerInfo);
                        }
                        AM.MuteRadio = isDisabled;
                    }
                }
            }

            ExtendedInfoManager.RadionVisible = PanelTimer.IsRadioToggle();
        }

        public static bool IsRadioToggle()
        {
            var radioPanel = GameObject.Find("RadioPanel");
            if (radioPanel != null)
            {
                RadioPanel rp = radioPanel.GetComponent(typeof(RadioPanel)) as RadioPanel;
                if (rp != null)
                {
                    var btn = rp.Find<UIButton>("RadioButton");
                    var pnl = rp.Find<UIPanel>("RadioPlayingPanel");
                    if (btn != null && pnl != null)
                    {
                        bool isDisabled = !btn.isVisible && !pnl.isVisible;
                        Singleton<AudioManager>.instance.MuteRadio = isDisabled;
                        return !isDisabled;
                    }
                }
            }

            return false;
        }

        public void OnRadioTogglerClicked(UIComponent comp, UIMouseEventParameter p)
        {
            ToggleRadio(!IsRadioToggle());
            btnRadioIconContainer.opacity = IsRadioToggle() ? 0.5f : 0.1f;
        }



        public void OnDayTimeClicked(UIComponent comp, UIMouseEventParameter p)
        {
            var timeControler = ExtendedInfoManager.timeControler;

            if (CityInfoDatas.TimeWarpMod_sunManager != null)
            {
                // 想控制 TimeWarpMod 的值，但是没有找到入口。
            }
            else if (timeControler != null)
            {
                if (timeControler.speed.num == 0)
                    timeControler.speedIndex = 1;
                else
                    timeControler.speedIndex = 0;

                timeControler.speed = TimeControler.TimeSpeeds[timeControler.speedIndex];
                UpdateData();
            }
        }

        public void OnPlayClicked(UIComponent comp, UIMouseEventParameter p)
        {
            SimulationPause();
        }

        [Serializable]
        public class Settings
        {
            public int speed;
            public uint dayOffsetFrames;
            public float longitude;
            public float lattitude;
            public float sunSize;
            public float sunIntensity;
        }

        public void OnDayNightSwitchClicked(UIComponent comp, UIMouseEventParameter p)
        {
            var time = Singleton<CityInfoDatas>.instance.WorldTimeOfDay;
            if (time >= 8f && time <= 19f)  // 8:00 AM -- 19:00 PM is Day
                Singleton<CityInfoDatas>.instance.WorldTimeOfDay = 0.0f;
            else
                Singleton<CityInfoDatas>.instance.WorldTimeOfDay = 12.001f;
        }

        public void OnDayNightSwitchMouseUp(UIComponent comp, UIMouseEventParameter p)
        {
            if (p.buttons == UIMouseButton.Right)
            {
                UIView.playSoundDelegate(this.GetUIView().defaultClickSound, 1f);
                Singleton<CityInfoDatas>.instance.enableDayNight = !Singleton<CityInfoDatas>.instance.enableDayNight;
                UpdateData();
            }
        }

        public void OnWeatherSwitchMouseUp(UIComponent comp, UIMouseEventParameter p)
        {
            if (p.buttons == UIMouseButton.Right)
            {
                UIView.playSoundDelegate(this.GetUIView().defaultClickSound, 1f);
                Singleton<CityInfoDatas>.instance.enableWeather = !Singleton<CityInfoDatas>.instance.enableWeather;
                UpdateData();
            }
        }

        public void OnWeatherSwitchClicked(UIComponent comp, UIMouseEventParameter p)
        {
            var WM = Singleton<WeatherManager>.instance;

            if (WM.m_currentRain <= 0.05f) // 晴
                Singleton<CityInfoDatas>.instance.WeatherRainIntensity = CityInfoDatas.RainSprinkle;
            else if (WM.m_currentRain < 0.2f) // 小雨
                Singleton<CityInfoDatas>.instance.WeatherRainIntensity = CityInfoDatas.RainMiddle;
            else if (WM.m_currentRain < 1.0f) //中雨
                Singleton<CityInfoDatas>.instance.WeatherRainIntensity = CityInfoDatas.RainHeavy;
            else if (WM.m_currentRain >= 1.5f) // 大雨
                Singleton<CityInfoDatas>.instance.WeatherRainIntensity = 0f;
        }


        public void OnSpeedClicked(UIComponent comp, UIMouseEventParameter p)
        {
            int speed = btnSpeed.activeStateIndex;
            speed += 1;
            if (speed > 3) speed = 1;
            SimulationSpeed(speed);
        }

        public void OnLocalTimeClick(UIComponent comp, UIMouseEventParameter p)
        {
            showTimeTag += 1;
            if (showTimeTag > 1)
                showTimeTag = 0;

            UpdateLocalTimeVisible();
        }


        public void setGameState()
        {
            bool pause = CityInfoDatas.isGamePaused();
            if (btnPlay != null)
            {
                btnPlay.activeStateIndex = (pause ? 0 : 1);
                int speed = Singleton<SimulationManager>.instance.SelectedSimulationSpeed;
                btnSpeed.activeStateIndex = speed - 1;
            }
        }

        protected void SimulationPause()
        {

            if (Singleton<SimulationManager>.exists)
            {
                Singleton<SimulationManager>.instance.ForcedSimulationPaused = !Singleton<SimulationManager>.instance.ForcedSimulationPaused;
            }
        }

        protected void SimulationSpeed(int speed)
        {
            if (Singleton<SimulationManager>.exists)
            {
                Singleton<SimulationManager>.instance.SelectedSimulationSpeed = speed;
            }
        }

    }
}
