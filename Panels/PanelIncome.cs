using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework;
using ColossalFramework.Globalization;
using IINS.UIExt;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IINS.ExtendedInfo
{
    public class PanelIncome : ExtendedPanel
    {
        public PanelIncome()
        {
            name = this.GetType().Name;
            relevantComponent = parentPanel.Find("IncomePanel");
        }
        public override void Awake()
        {
            base.Awake();
            this.size = new Vector2(260, PANEL_HEIGHT);
            this.eventDoubleClick += DoDoubleClick;

            if (Singleton<TransferManager>.exists && m_resourceColors == null)
            {
                m_resourceColors = new Color[]
                {
                    Singleton<TransferManager>.instance.m_properties.m_resourceColors[13],
                    Singleton<TransferManager>.instance.m_properties.m_resourceColors[14],
                    Singleton<TransferManager>.instance.m_properties.m_resourceColors[0x11],
                    Singleton<TransferManager>.instance.m_properties.m_resourceColors[15],
                    Singleton<TransferManager>.instance.m_properties.m_resourceColors[0x10]
                };
            }

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

        MUILabel lblIcon = null;
        MUILabel lblCashAmount = null;
        MUILabel lblCashDelta = null;
        UISprite impOilColor = null;
        UISprite impOreColor = null;
        UISprite impForestryColor = null;
        UISprite impGoodsColor = null;
        UISprite impAgricultureColor = null;
        MUILabel lblImportTotal = null;

        UISprite expOilColor = null;
        UISprite expOreColor = null;
        UISprite expForestryColor = null;
        UISprite expGoodsColor = null;
        UISprite expAgricultureColor = null;
        MUILabel lblExportTotal = null;

        const int CSW = 9; // ColorSprite Width
        public static Color[] m_resourceColors = null;

        void InitControls()
        {
            LocaleManager.eventLocaleChanged += OnLocaleChanged;
            // 图标
            lblIcon = this.AddUIComponent<MUILabel>();
            lblIcon.size = new Vector2(PANEL_HEIGHT - 2, PANEL_HEIGHT - 2);
            lblIcon.relativePosition = new Vector3(2, 2);
            lblIcon.backgroundSprite = "InfoPanelIconCurrency";
            lblIcon.eventClick += OnCashAmountClick;
            lblIcon.playAudioEvents = true;

            // 金额
            lblCashAmount = this.AddUIComponent<MUILabel>();
            lblCashAmount.size = new Vector2(115, LINEW);
            lblCashAmount.relativePosition = new Vector3(32, LINE1);
            lblCashAmount.textColor = ExtendedPanel.COLOR_TEXT;
            lblCashAmount.textAlignment = UIHorizontalAlignment.Right;
            lblCashAmount.fontStyle = FontStyle.Bold;
            lblCashAmount.fontSize = (int)MUISkin.UIToScreen(14f);
            lblCashAmount.tooltipLocaleID = "MAIN_MONEYINFO";
            lblCashAmount.eventClick += OnCashAmountClick;

            // 每周收入
            lblCashDelta = this.AddUIComponent<MUILabel>();
            lblCashDelta.size = new Vector2(105, LINEW);
            lblCashDelta.relativePosition = new Vector3(42, LINE2);
            lblCashDelta.fontSize = (int)MUISkin.UIToScreen(12f);
            lblCashDelta.textColor = COLOR_DARK_GREEN;
            lblCashDelta.fontStyle = FontStyle.Bold;
            lblCashDelta.textAlignment = UIHorizontalAlignment.Right;
            lblCashDelta.tooltipLocaleID = "MAIN_MONEYDELTA";
            lblCashDelta.eventClick += OnCashAmountClick;

            int LOF = 155; //left offset

            // 进口
            lblImportTotal = this.AddUIComponent<MUILabel>();
            lblImportTotal.fontSize = (int)MUISkin.UIToScreen(10f);
            lblImportTotal.size = new Vector2(50, LINEW);
            lblImportTotal.relativePosition = new Vector3(155, LINE1 - 3);
            lblImportTotal.textColor = COLOR_DARK_TEXT;
            lblImportTotal.textAlignment = UIHorizontalAlignment.Right;
            lblImportTotal.tooltipLocaleID = "INFO_CONNECTIONS_IMPORT";

            impOilColor = createColorSprite(LOF, m_resourceColors[0], "INFO_CONNECTIONS_OIL");
            impOreColor = createColorSprite(LOF + CSW, m_resourceColors[1], "INFO_CONNECTIONS_ORE");
            impGoodsColor = createColorSprite(LOF + CSW * 2, m_resourceColors[2], "INFO_CONNECTIONS_GOODS");
            impForestryColor = createColorSprite(LOF + CSW * 3, m_resourceColors[3], "INFO_CONNECTIONS_FORESTRY");
            impAgricultureColor = createColorSprite(LOF + CSW * 4, m_resourceColors[4], "INFO_CONNECTIONS_AGRICULTURE");

             
            LOF = 210;
            // 出口
            lblExportTotal = this.AddUIComponent<MUILabel>();
            lblExportTotal.fontSize = (int)MUISkin.UIToScreen(10f);
            lblExportTotal.size = new Vector2(50, LINEW);
            lblExportTotal.relativePosition = new Vector3(203, LINE1 - 3);
            lblExportTotal.textColor = COLOR_DARK_TEXT;
            lblExportTotal.textAlignment = UIHorizontalAlignment.Right;
            lblExportTotal.tooltipLocaleID = "INFO_CONNECTIONS_EXPORT";

            expOilColor = createColorSprite(LOF, m_resourceColors[0], "INFO_CONNECTIONS_OIL");
            expOreColor = createColorSprite(LOF + CSW, m_resourceColors[1], "INFO_CONNECTIONS_ORE");
            expGoodsColor = createColorSprite(LOF + CSW * 2, m_resourceColors[2], "INFO_CONNECTIONS_GOODS");
            expForestryColor = createColorSprite(LOF + CSW * 3, m_resourceColors[3], "INFO_CONNECTIONS_FORESTRY");
            expAgricultureColor = createColorSprite(LOF + CSW * 4, m_resourceColors[4], "INFO_CONNECTIONS_AGRICULTURE");


            impOilColor.eventClick += OnSetInfoModeClick;
            impOreColor.eventClick += OnSetInfoModeClick;
            impGoodsColor.eventClick += OnSetInfoModeClick;
            impForestryColor.eventClick += OnSetInfoModeClick;
            impAgricultureColor.eventClick += OnSetInfoModeClick;
            lblImportTotal.eventClick += OnSetInfoModeClick;
            impOilColor.playAudioEvents = true;
            impOreColor.playAudioEvents = true;
            impGoodsColor.playAudioEvents = true;
            impForestryColor.playAudioEvents = true;
            impAgricultureColor.playAudioEvents = true;
            lblImportTotal.playAudioEvents = true;

            expOilColor.eventClick += OnSetInfoModeClick;
            expOreColor.eventClick += OnSetInfoModeClick;
            expGoodsColor.eventClick += OnSetInfoModeClick;
            expForestryColor.eventClick += OnSetInfoModeClick;
            expAgricultureColor.eventClick += OnSetInfoModeClick;
            lblExportTotal.eventClick += OnSetInfoModeClick;
            expOilColor.playAudioEvents = true;
            expOreColor.playAudioEvents = true;
            expGoodsColor.playAudioEvents = true;
            expForestryColor.playAudioEvents = true;
            expAgricultureColor.playAudioEvents = true;
            lblExportTotal.playAudioEvents = true;
        }

        //public class ToolsModifierControl : UICustomControl
        //{
        //    private static EconomyPanel m_EconomyPanel;
        //    public static EconomyPanel economyPanel
        //    {
        //        get
        //        {
        //            if (m_EconomyPanel == null)
        //            {
        //                UIComponent component = UIView.Find("EconomyPanel");
        //                if (component != null)
        //                {
        //                    m_EconomyPanel = component.GetComponent<EconomyPanel>();
        //                }
        //            }
        //            return m_EconomyPanel;
        //        }
        //    }

        //    public static void Show()
        //    {
        //        if (economyPanel != null)
        //        {
        //            economyPanel.Hide();
        //        }
        //    }
        //}

        private void OnSetInfoModeClick(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (component == impOilColor || component == impOreColor || component == impGoodsColor ||
                component == impForestryColor || component == impAgricultureColor || component == lblImportTotal)
            {
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Connections, InfoManager.SubInfoMode.Import);
            }
            else if (component == expOilColor || component == expOreColor || component == expGoodsColor ||
               component == expForestryColor || component == expAgricultureColor || component == lblExportTotal)
            {
                ExtendedInfoManager.SetCurrentMode(InfoManager.InfoMode.Connections, InfoManager.SubInfoMode.Export);
            }
        }

        private void OnCashAmountClick(UIComponent com, UIMouseEventParameter p)
        {
            if (ToolsModifierControl.economyPanel != null)
            {

                if (ToolsModifierControl.economyPanel.isVisible)
                {
                    ToolsModifierControl.economyPanel.CloseToolbar(); // Hide;
                }
                else
                {
                    ToolsModifierControl.mainToolbar.ShowEconomyPanel(-1);
                    WorldInfoPanel.Hide<CityServiceWorldInfoPanel>();
                }
            }
        }

        public override void ResetPositionSize()
        {
            if (relevantComponent != null)
            {
                absolutePosition = relevantComponent.absolutePosition;
                if (mainAspectRatio > 0f && mainAspectRatio < 1.9f)
                    relativePosition = new Vector3(980f, 4.0f);
                else
                    relativePosition = new Vector3(relativePosition.x + 20, 4.0f);
            }
        }

        public UISprite createColorSprite(int left, Color32 C, string id)
        {
            UISprite result = this.AddUIComponent<UISprite>();
            result.size = new Vector2(CSW - 1, 34);
            result.relativePosition = new Vector3(left, 1);
            result.color = C;
            result.fillDirection = UIFillDirection.Vertical;
            result.flip = UISpriteFlip.FlipVertical;
            result.fillAmount = 0.16f;
            result.opacity = 0.4f;
            result.spriteName = "EmptySprite";
            result.tooltipLocaleID = id;
            result.isTooltipLocalized = true;

            return result;
        }

        void DoneControls()
        {
            LocaleManager.eventLocaleChanged -= OnLocaleChanged;
            Destroy(lblIcon); lblIcon = null;
            Destroy(lblCashAmount); lblCashAmount = null;
            Destroy(lblCashDelta); lblCashDelta = null;
            Destroy(impOilColor); impOilColor = null;
            Destroy(impOreColor); impOreColor = null;
            Destroy(impForestryColor); impForestryColor = null;
            Destroy(impGoodsColor); impGoodsColor = null;
            Destroy(impAgricultureColor); impAgricultureColor = null;
            Destroy(expOilColor); expOilColor = null;
            Destroy(expOreColor); expOreColor = null;
            Destroy(expForestryColor); expForestryColor = null;
            Destroy(expGoodsColor); expGoodsColor = null;
            Destroy(expAgricultureColor); expAgricultureColor = null;
        }

        void OnLocaleChanged()
        {
        }

        public override void OnScreenSizeChagned()
        {
            base.OnScreenSizeChagned();
            if (lblIcon != null)
            {
                lblIcon.size = new Vector2(PANEL_HEIGHT - 2, PANEL_HEIGHT - 2);
            }
            if (lblCashAmount != null)
            {
                lblCashAmount.fontSize = (int)MUISkin.UIToScreen(14f);
                lblCashAmount.size = new Vector2(115, LINEW);
                lblCashAmount.relativePosition = new Vector3(32, LINE1);
            }
            if (lblCashDelta != null)
            {
                lblCashDelta.fontSize = (int)MUISkin.UIToScreen(12f);
                lblCashDelta.size = new Vector2(105, LINEW);
                lblCashDelta.relativePosition = new Vector3(42, LINE2);
            }
            if (lblImportTotal != null)
            {
                lblImportTotal.fontSize = (int)MUISkin.UIToScreen(10f);
                lblImportTotal.size = new Vector2(50, LINEW);
                lblImportTotal.relativePosition = new Vector3(155, LINE1 - 3);
            }
            if (lblExportTotal != null)
            {
                lblExportTotal.fontSize = (int)MUISkin.UIToScreen(10f);
                lblExportTotal.size = new Vector2(50, LINEW);
                lblExportTotal.relativePosition = new Vector3(203, LINE1 - 3);
            }
        }

        float getFillAmount(float value, float total)
        {
            float v = CityInfoDatas.GetPercentValue((int)value, (int)total, true);
            return v / 100;
        }

        public override void UpdateData()
        {
            lblCashAmount.text = CityInfoDatas.CashAmount.text;
            lblCashDelta.text = CityInfoDatas.CashDelta.text;
            lblCashDelta.textColor = CityInfoDatas.CashDelta.color;

            if (lblImportTotal != null)
            {
                lblImportTotal.text = CityInfoDatas.ImportTotal.value.ToString();
                impOilColor.fillAmount = getFillAmount(CityInfoDatas.ImportOil.value, CityInfoDatas.ImportTotal.value);
                impOreColor.fillAmount = getFillAmount(CityInfoDatas.ImportOre.value, CityInfoDatas.ImportTotal.value);
                impGoodsColor.fillAmount = getFillAmount(CityInfoDatas.ImportGoods.value, CityInfoDatas.ImportTotal.value);
                impForestryColor.fillAmount = getFillAmount(CityInfoDatas.ImportForestry.value, CityInfoDatas.ImportTotal.value);
                impAgricultureColor.fillAmount = getFillAmount(CityInfoDatas.ImportAgricultural.value, CityInfoDatas.ImportTotal.value);

                lblExportTotal.text = CityInfoDatas.ExportTotal.value.ToString();
                expOilColor.fillAmount = getFillAmount(CityInfoDatas.ExportOil.value, CityInfoDatas.ExportTotal.value);
                expOreColor.fillAmount = getFillAmount(CityInfoDatas.ExportOre.value, CityInfoDatas.ExportTotal.value);
                expGoodsColor.fillAmount = getFillAmount(CityInfoDatas.ExportGoods.value, CityInfoDatas.ExportTotal.value);
                expForestryColor.fillAmount = getFillAmount(CityInfoDatas.ExportForestry.value, CityInfoDatas.ExportTotal.value);
                expAgricultureColor.fillAmount = getFillAmount(CityInfoDatas.ExportAgricultural.value, CityInfoDatas.ExportTotal.value);
            }
        }

        public override void OnDrawPanel()
        {
            base.OnDrawPanel(); 
            if (CanOnGUIDraw())
            {
                float W = MUISkin.UIToScreen(150);
                float T = 4f;
                float H = MUISkin.UIToScreen(PANEL_HEIGHT) - T * 2;

                GUI.DrawTexture(new Rect(W, T, 1, H), lineTexture);
                W = MUISkin.UIToScreen(210);
                GUI.DrawTexture(new Rect(W, T, 1, H), lineTexture);
            }
        }

        private void DoDoubleClick(UIComponent component, UIMouseEventParameter eventParam)
        {
        }

    }
}
