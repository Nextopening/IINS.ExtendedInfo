using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;
using ColossalFramework.Globalization;
using IINS.UIExt;
using System;
using System.IO;

namespace IINS.ExtendedInfo
{
    public class PanelPopulation: ExtendedPanel
    {
        private static readonly string[] sHappinessLevels = new string[] { "VeryUnhappy", "Unhappy", "Happy", "VeryHappy", "ExtremelyHappy" };

        public PanelPopulation()
        {
            name = this.GetType().Name;
            relevantComponent = parentPanel.Find("PopulationPanel");
        }

        public override void Awake()
        {
            base.Awake();
            this.size = new Vector2(340, PANEL_HEIGHT);
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

            lblUnemployment.isTooltipLocalized = false;
            string s = "";
            s = Locale.Get("INFO_POPULATION_UNEMPLOYMENT");
            s = s.Substring(0, s.IndexOf("{"));
            lblUnemployment.tooltip = s.Substring(0, s.Length - 2);
        }

        MUILabel lblIcon = null;
        MUILabel lblPopulation = null;
        MUILabel lblChange = null;
        MUILabel lblUnemploymentA = null;
        MUILabel lblUnemployment = null;
        MUILabel lblHealthA = null;
        MUILabel lblHealth = null;
        MUILabel lblSeniorA = null;
        MUILabel lblSenior = null;
        MUILabel lblHappiness = null;
        MUILabel lblHappinessA = null;
        MUILabel lblEducationA = null;
        MUILabel lblEducation1 = null;
        MUILabel lblEducation2 = null;
        MUILabel lblEducation3 = null;
        MUILabel lblActivityA = null;
        MUILabel lblActivity = null;
        void InitControls()
        {
            LocaleManager.eventLocaleChanged += OnLocaleChanged;
            // 图标
            lblIcon = this.AddUIComponent<MUILabel>();
            lblIcon.size = new Vector2(PANEL_HEIGHT - 2, PANEL_HEIGHT - 2);
            lblIcon.relativePosition = new Vector3(0, 0);
            lblIcon.backgroundSprite = "InfoIconPopulationFocused";
            // 总人口
            lblPopulation = this.AddUIComponent<MUILabel>();
            lblPopulation.size = new Vector2(83, LINEW);
            lblPopulation.relativePosition = new Vector3(32, LINE1);
            lblPopulation.textColor = ExtendedPanel.COLOR_TEXT;
            lblPopulation.textAlignment = UIHorizontalAlignment.Right;
            lblPopulation.fontStyle = FontStyle.Bold;
            lblPopulation.fontSize = (int)MUISkin.UIToScreen(14f);
            lblPopulation.tooltipLocaleID = "MAIN_POPULATION";
            // 人口变化
            lblChange = this.AddUIComponent<MUILabel>();
            lblChange.size = new Vector2(73, LINEW);
            lblChange.relativePosition = new Vector3(42, LINE2);
            lblChange.textColor = COLOR_DARK_GREEN;
            lblChange.fontSize = (int)MUISkin.UIToScreen(12f);
            lblChange.textAlignment = UIHorizontalAlignment.Right;
            lblChange.fontStyle = FontStyle.Bold;
            lblChange.tooltipLocaleID = "MAIN_POPULATIONDELTA";
            // 失业率
            lblUnemploymentA = this.AddUIComponent<MUILabel>();
            lblUnemploymentA.size = new Vector2(16, 16);
            lblUnemploymentA.relativePosition = new Vector3(125, LINE1 + 2);
            lblUnemploymentA.backgroundSprite = "IconPolicyWeAreTheNormPressed";
            lblUnemploymentA.opacity = 0.1f;

            lblUnemployment = this.AddUIComponent<MUILabel>();
            lblUnemployment.size = new Vector2(40, LINEW);
            lblUnemployment.relativePosition = new Vector3(130, LINE2);
            lblUnemployment.textColor = COLOR_DARK_PURPLE;
            lblUnemployment.textAlignment = UIHorizontalAlignment.Right;

            // 健康
            lblHealthA = this.AddUIComponent<MUILabel>();
            lblHealthA.size = new Vector2(16, 16);
            lblHealthA.relativePosition = new Vector3(183, LINE1 + 2);
            lblHealthA.backgroundSprite = "InfoIconHealthPressed";
            lblHealthA.opacity = 0.3f;

            lblHealth = this.AddUIComponent<MUILabel>();
            lblHealth.size = new Vector2(50, LINEW);
            lblHealth.relativePosition = new Vector3(175, LINE1);
            lblHealth.textAlignment = UIHorizontalAlignment.Right;
            lblHealth.textColor = COLOR_DARK_GREEN;
            lblHealth.tooltipLocaleID = "INFO_HEALTH_TITLE";

            // 教育
            lblEducationA = this.AddUIComponent<MUILabel>();
            lblEducationA.size = new Vector2(16, 16);
            lblEducationA.relativePosition = new Vector3(235, LINE2 + 2);
            lblEducationA.backgroundSprite = "ToolbarIconEducationPressed";
            lblEducationA.opacity = 0.3f;

            lblEducation1 = this.AddUIComponent<MUILabel>();
            lblEducation1.size = new Vector2(20, LINEW);
            lblEducation1.relativePosition = new Vector3(255, LINE2);
            lblEducation1.textColor = COLOR_DARK_TEXT;
            lblEducation1.textAlignment = UIHorizontalAlignment.Right;
            lblEducation1.tooltipLocaleID = "ZONEDBUILDING_EDUCATED";

            lblEducation2 = this.AddUIComponent<MUILabel>();
            lblEducation2.size = new Vector2(20, LINEW);
            lblEducation2.relativePosition = new Vector3(285, LINE2);
            lblEducation2.textAlignment = UIHorizontalAlignment.Center;
            lblEducation2.textColor = COLOR_DARK_TEXT;
            lblEducation2.tooltipLocaleID = "ZONEDBUILDING_WELLEDUCATED";

            lblEducation3 = this.AddUIComponent<MUILabel>();
            lblEducation3.size = new Vector2(20, LINEW);
            lblEducation3.relativePosition = new Vector3(315, LINE2);
            lblEducation3.textAlignment = UIHorizontalAlignment.Right;
            lblEducation3.textColor = COLOR_DARK_YELLOW;
            lblEducation3.tooltipLocaleID = "ZONEDBUILDING_HIGHLYEDUCATED";

            lblEducation1.fontSize = (int)MUISkin.UIToScreen(10f);
            lblEducation2.fontSize = lblEducation1.fontSize;
            lblEducation3.fontSize = lblEducation1.fontSize;

            // 幸福度
            lblHappinessA = this.AddUIComponent<MUILabel>();
            lblHappinessA.size = new Vector2(14, 14);
            lblHappinessA.relativePosition = new Vector3(235, LINE1 + 2);
            string happinessString = GetHappinessString(Citizen.GetHappinessLevel(Singleton<DistrictManager>.instance.m_districts.m_buffer[0].m_finalHappiness));
            lblHappinessA.backgroundSprite = happinessString; // "InfoIconHappiness";
            lblHappinessA.opacity = 0.3f;

            lblHappiness = this.AddUIComponent<MUILabel>();
            lblHappiness.size = new Vector2(50, LINEW);
            lblHappiness.relativePosition = new Vector3(235, LINE1);
            lblHappiness.textColor = COLOR_DARK_TEXT;
            lblHappiness.textAlignment = UIHorizontalAlignment.Right;
            lblHappiness.tooltipLocaleID = "MAIN_HAPPINESS";

            // 老年人
            lblSeniorA = this.AddUIComponent<MUILabel>();
            lblSeniorA.size = new Vector2(16, 16); 
            lblSeniorA.relativePosition = new Vector3(295, LINE1 + 2);
            lblSeniorA.backgroundSprite = "InfoIconAgePressed";
            lblSeniorA.opacity = 0.3f;
            //lblSeniorA.textColor = COLOR_CAPTION;
            //lblSeniorA.text = Locale.Get("INFO_AGE_SENIOR") + ":";
            //lblSeniorA.fontSize = (int)MUISkin.UIToScreen(9f);

            lblSenior = this.AddUIComponent<MUILabel>();
            lblSenior.size = new Vector2(45, LINEW);
            lblSenior.relativePosition = new Vector3(290, LINE1);
            lblSenior.textAlignment = UIHorizontalAlignment.Right;
            lblSenior.textColor = COLOR_DARK_PURPLE; // MUIUtil.DarkenColor(Color.yellow, 0.50f);
            lblSenior.tooltipLocaleID = "CITY_SENIORAMOUNT";

            // 活动人口
            lblActivityA = this.AddUIComponent<MUILabel>();
            lblActivityA.size = new Vector2(70, LINEW);
            lblActivityA.relativePosition = new Vector3(125, LINE2);
            lblActivityA.textColor = COLOR_CAPTION;
            if (MODUtil.IsChinaLanguage())
                lblActivityA.text = "人口实例:";
            else
                lblActivityA.text = "instances:";
            lblActivityA.fontSize = (int)MUISkin.UIToScreen(10f);

            lblActivity = this.AddUIComponent<MUILabel>();
            lblActivity.size = new Vector2(95, LINEW);
            lblActivity.relativePosition = new Vector3(130, LINE2);
            lblActivity.textAlignment = UIHorizontalAlignment.Right;
            lblActivity.textColor = MUIUtil.DarkenColor(Color.yellow, 0.70f);

            lblIcon.eventClick += OnSetInfoModeClick;
            lblPopulation.eventClick += OnSetInfoModeClick;
            lblChange.eventClick += OnSetInfoModeClick;
            lblSenior.eventClick += OnSetInfoModeClick;
            lblUnemployment.eventClick += OnSetInfoModeClick;
            lblHealth.eventClick += OnSetInfoModeClick;
            lblEducation1.eventClick += OnSetInfoModeClick;
            lblEducation2.eventClick += OnSetInfoModeClick;
            lblEducation3.eventClick += OnSetInfoModeClick;
            lblHappiness.eventClick += OnSetInfoModeClick;
            lblIcon.playAudioEvents = true;
            lblPopulation.playAudioEvents = true;
            lblChange.playAudioEvents = true;
            lblSenior.playAudioEvents = true;
            lblUnemployment.playAudioEvents = true;
            lblHealth.playAudioEvents = true;
            lblEducation1.playAudioEvents = true;
            lblEducation2.playAudioEvents = true;
            lblEducation3.playAudioEvents = true;
            lblHappiness.playAudioEvents = true;
            lblHappinessA.playAudioEvents = true;
        }

        private void OnSetInfoModeClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (component == lblHappiness)
            {
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Happiness, InfoManager.SubInfoMode.Default);
            }
            else if(component == lblIcon || component == lblPopulation || component == lblChange || component == lblSenior || component == lblUnemployment)
            {
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Density, InfoManager.SubInfoMode.Default);
            }
            else if (component == lblHealth)
            {
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Health, InfoManager.SubInfoMode.Default);
            }
            else if (component == lblEducation1)
            {
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Education, InfoManager.SubInfoMode.ElementarySchool);
            }
            else if (component == lblEducation2)
            {
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Education, InfoManager.SubInfoMode.HighSchool);
            }
            else if (component == lblEducation3)
            {
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Education, InfoManager.SubInfoMode.University);
            }
        }

        void DoneControls()
        {
            LocaleManager.eventLocaleChanged -= OnLocaleChanged;
            Destroy(lblIcon); lblIcon = null;
            Destroy(lblPopulation); lblPopulation = null;
            Destroy(lblChange); lblChange = null;
            Destroy(lblSenior); lblSenior = null;
            Destroy(lblSeniorA); lblSeniorA = null;
            Destroy(lblUnemployment); lblUnemployment = null;
            Destroy(lblUnemploymentA); lblUnemploymentA = null;
            Destroy(lblHealth); lblHealth = null;
            Destroy(lblHealthA); lblHealthA = null;
            Destroy(lblEducationA); lblEducationA = null;
            Destroy(lblEducation1); lblEducation1 = null;
            Destroy(lblEducation2); lblEducation2 = null;
            Destroy(lblEducation3); lblEducation3 = null;
            Destroy(lblActivity); lblActivity = null;
            Destroy(lblActivityA); lblActivityA = null;
            Destroy(lblHappiness); lblHappiness = null;
            Destroy(lblHappinessA); lblHappinessA = null;
        }

        internal static string GetHappinessString(Citizen.Happiness happinessLevel)
        {
            return ("NotificationIcon" + sHappinessLevels[(int)happinessLevel]);
        }

        void OnLocaleChanged()
        {
            string s = "";
            if (lblUnemployment != null)
            {
                s = Locale.Get("INFO_POPULATION_UNEMPLOYMENT");

                s = s.Substring(0, s.IndexOf("{"));
                lblUnemployment.tooltip = s.Substring(0, s.Length - 2);
            }

            if (MODUtil.IsChinaLanguage())
                lblActivityA.text = "人口实例:";
            else
                lblActivityA.text = "instances:";
        }

        public override void OnScreenSizeChagned()
        {
            base.OnScreenSizeChagned();
            if (lblIcon != null)
            {
                lblIcon.size = new Vector2(PANEL_HEIGHT - 2, PANEL_HEIGHT - 2);
            }
            if (lblPopulation != null)
            {
                lblPopulation.fontSize = (int)MUISkin.UIToScreen(14f);
                lblPopulation.size = new Vector2(83, LINEW);
                lblPopulation.relativePosition = new Vector3(32, LINE1);
            }
            if (lblChange != null)
            {
                lblChange.fontSize = (int)MUISkin.UIToScreen(12f);
                lblChange.size = new Vector2(73, LINEW);
                lblChange.relativePosition = new Vector3(42, LINE2);
            }
            if (lblUnemployment != null)
            {
                lblUnemploymentA.relativePosition = new Vector3(125, LINE1 + 2);
                lblUnemployment.size = new Vector2(40, LINEW);
                lblUnemployment.relativePosition = new Vector3(130, LINE1);
            }
            if (lblHealth != null)
            {
                lblHealthA.relativePosition = new Vector3(183, LINE1 + 2);
                lblHealth.size = new Vector2(50, LINEW);
                lblHealth.relativePosition = new Vector3(175, LINE1);
            }
            if (lblSenior != null)
            {
                //lblSeniorA.fontSize = (int)MUISkin.UIToScreen(9f);
                //lblSeniorA.size = new Vector2(70, LINEW);
                lblSeniorA.relativePosition = new Vector3(295, LINE1 + 2);
                lblSenior.size = new Vector2(45, LINEW);
                lblSenior.relativePosition = new Vector3(290, LINE1);

                lblHappiness.fontSize = (int)MUISkin.UIToScreen(12f);
                lblHappinessA.relativePosition = new Vector3(235, LINE1 + 2);
                lblHappiness.size = new Vector2(50, LINEW);
                lblHappiness.relativePosition = new Vector3(235, LINE1);
            }
            if (lblEducationA != null)
            {
                lblEducationA.relativePosition = new Vector3(235, LINE2 + 2);
                lblEducation1.fontSize = (int)MUISkin.UIToScreen(10f);
                lblEducation2.fontSize = lblEducation1.fontSize;
                lblEducation3.fontSize = lblEducation1.fontSize;
                lblEducation1.size = new Vector2(20, LINEW);
                lblEducation1.relativePosition = new Vector3(255, LINE2);
                lblEducation2.size = new Vector2(20, LINEW);
                lblEducation2.relativePosition = new Vector3(285, LINE2);
                lblEducation3.size = new Vector2(20, LINEW);
                lblEducation3.relativePosition = new Vector3(315, LINE2);
            }

            if (lblActivity != null)
            {
                lblActivityA.size = new Vector2(70, LINEW);
                lblActivityA.relativePosition = new Vector3(125, LINE2);
                lblActivityA.fontSize = (int)MUISkin.UIToScreen(10f);
                lblActivity.size = new Vector2(95, LINEW);
                lblActivity.relativePosition = new Vector3(130, LINE2);
            }

        }

        public override void UpdateData()
        {
            lblPopulation.text = CityInfoDatas.Population.text;
            lblActivity.text = CityInfoDatas.CitizenInstanceCount.text;
            lblChange.text = CityInfoDatas.PopulationDelta.text;
            lblChange.textColor = CityInfoDatas.PopulationDelta.color;
            lblUnemployment.text = CityInfoDatas.Unemployment.text;
            lblUnemployment.textColor = CityInfoDatas.Unemployment.color;
            lblSenior.text = CityInfoDatas.Senior.text;
            lblHealth.text = CityInfoDatas.AvgHealth.text;
            lblEducation1.text = CityInfoDatas.EducatedLegend.text;
            lblEducation2.text = CityInfoDatas.WellEducatedLegend.text;
            lblEducation3.text = CityInfoDatas.HighlyEducatedLegend.text;
            string happinessString = GetHappinessString(Citizen.GetHappinessLevel(Singleton<DistrictManager>.instance.m_districts.m_buffer[0].m_finalHappiness));
            lblHappinessA.backgroundSprite = happinessString; 
            lblHappiness.text = CityInfoDatas.Happiness.text;
            lblHappiness.textColor = CityInfoDatas.Happiness.color;
        }

        public override void OnDrawPanel()
        {
            base.OnDrawPanel();
            if (CanOnGUIDraw())
            {
                float W = MUISkin.UIToScreen(120);
                float T = 4f;
                float H = MUISkin.UIToScreen(PANEL_HEIGHT) - T * 2;


                GUI.DrawTexture(new Rect(W, T, 1, H), lineTexture);
                W = MUISkin.UIToScreen(230);
                GUI.DrawTexture(new Rect(W, T, 1, H), lineTexture);
            }
        }

        public override void ResetPositionSize()
        {
            if (relevantComponent != null)
            {
                absolutePosition = relevantComponent.absolutePosition;
                if (mainAspectRatio > 0f && mainAspectRatio < 1.9f)
                    relativePosition = new Vector3(1245f, 4.0f);
                else
                    relativePosition = new Vector3(relativePosition.x, 4.0f);
            }
        }

        private void DoDoubleClick(UIComponent component, UIMouseEventParameter eventParam)
        {
        }

    }
}
