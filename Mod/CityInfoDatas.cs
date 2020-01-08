using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework;
using IINS.UIExt;
using ColossalFramework.Globalization;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace IINS.ExtendedInfo
{
    public struct InfoDataCell
    {
        public string text;
        public long value;
        public long subvalue;
        public float extvalue;
        public Color color {
            get { return getColor==null? ExtendedPanel.COLOR_TEXT : getColor(); }
        }
        public CityInfoDatas.getInfoDataCellColor getColor;
    }

    public class MapThemeLoader : LoadSavePanelBase<MapMetaData>
    {
        public static string GetThemeString(string environment)
        {
            return LoadSavePanelBase<SaveGameMetaData>.GetThemeString(null, environment, null);
        }
    }

    public class CityInfoDatas : Singleton<CityInfoDatas>
    {
        private UIDateTimeWrapper m_GameTime = new UIDateTimeWrapper(DateTime.MaxValue);
        private UITemperaturWrapper m_Temperature = new UITemperaturWrapper(float.MaxValue);

        private SavedBool m_EnableDayNight = new SavedBool(Settings.enableDayNight, Settings.gameSettingsFile, DefaultSettings.enableDayNight, true);
        private SavedBool m_EnableWeather = new SavedBool(Settings.enableWeather, Settings.gameSettingsFile, DefaultSettings.enableWeather, true);
        public delegate Color getInfoDataCellColor();

        public float Time_1 = 0;
        public float Time_3 = 0;
        public float Time_5 = 0;
        public float Time_10 = 0;

        public static float RainSprinkle = 0.15f;
        public static float RainMiddle = 0.4f;
        public static float RainHeavy = 2.0f;

        public static InfoDataCell CityInfo;
        public static InfoDataCell Maptheme;
        public static InfoDataCell DistrictsCount;
        public static InfoDataCell BuildingCount;
        public static InfoDataCell TreeCount;
        public static InfoDataCell GameTime;
        public static InfoDataCell GameTimeOfDay;
        public static InfoDataCell PlayingTime;
        public static InfoDataCell Temperatur;

        public static InfoDataCell Population;
        public static InfoDataCell PopulationDelta;
        public static InfoDataCell CitizenInstanceCount;
        public static InfoDataCell Senior;
        public static InfoDataCell Unemployment;
        public static InfoDataCell AvgHealth;
        public static InfoDataCell EducatedLegend;
        public static InfoDataCell WellEducatedLegend;
        public static InfoDataCell HighlyEducatedLegend;
        public static InfoDataCell Happiness;

        public static InfoDataCell CashAmount;
        public static InfoDataCell CashDelta;

        public static InfoDataCell ImportOil;
        public static InfoDataCell ImportOre;
        public static InfoDataCell ImportGoods;
        public static InfoDataCell ImportForestry;
        public static InfoDataCell ImportAgricultural;
        public static InfoDataCell ImportTotal;

        public static InfoDataCell ExportOil;
        public static InfoDataCell ExportOre;
        public static InfoDataCell ExportGoods;
        public static InfoDataCell ExportForestry;
        public static InfoDataCell ExportAgricultural;
        public static InfoDataCell ExportTotal;

        public static InfoDataCell BusCitizens;
        public static InfoDataCell MetroCitizens;
        public static InfoDataCell TramCitizens;
        public static InfoDataCell TaxiCitizens;
        public static InfoDataCell TrainCitizens;
        public static InfoDataCell ShipCitizens;
        public static InfoDataCell PlaneCitizens;
        public static InfoDataCell PublicTransportAverage;

        public static InfoDataCell VehicleCount;
        public static InfoDataCell VehicleParked;
        public static InfoDataCell TouristsAverage;

        public static InfoDataCell Electricity;
        public static InfoDataCell Water;
        public static InfoDataCell Sewage;

        public static InfoDataCell ElementarySchool;
        public static InfoDataCell HighSchool;
        public static InfoDataCell University;

        public CityInfoDatas()
        {
            name = "IISN-CityInfoDatas";
        }

        public void Awake()
        {
            PopulationDelta.getColor += getPopulationDeltaColor;
            Unemployment.getColor += getUnemploymentColor;
            Happiness.getColor += getHappinessColor;

            CashDelta.getColor += getCashDeltaColor;
            PlayingTime.extvalue = 0;
        }

        public static Component TimeWarpMod_sunManager = null;
        public static GameObject RushHourUI = null;
        public void Start()
        {
            var gameObjects = FindObjectsOfType<GameObject>();
            foreach (var go in gameObjects)
            {
                var tmp = go.GetComponent("SunManager");
                if (tmp != null)
                {
                    TimeWarpMod_sunManager = tmp;
                    break;
                }
            }

            RushHourUI = GameObject.Find("RushHourUI");
            UpdateDataAll();
        }

        public void OnDestroy()
        {
            //Singleton<WeatherManager>.instance.m_targetRain = 0;
            GameObject.Destroy(gameObject);
        }

        public bool enableDayNight
        {
            get
            {
                return (bool)this.m_EnableDayNight;
            }
            set
            {
                this.m_EnableDayNight.value = value;
                if (Singleton<SimulationManager>.exists)
                {
                    Singleton<SimulationManager>.instance.m_enableDayNight = value;
                    if (!value)
                        WorldTimeOfDay = 12.0001f;
                }
            }
        }

        public float WorldTimeOfDay
        {
            get { return Singleton<SimulationManager>.instance.m_currentDayTimeHour; }
            set
            {
                var SM = Singleton<SimulationManager>.instance;
                
                // suport time-warpMod
                if (TimeWarpMod_sunManager != null)
                {
                    var TimeOfDay = TimeWarpMod_sunManager.GetType().GetProperty("TimeOfDay");
                    if (TimeOfDay != null)
                    {
                        TimeOfDay.SetValue(TimeWarpMod_sunManager, value, null);
                    }
                }
                else // 没有 Time-WarpMod 时，直接改时间
                {
                    if (SM.m_enableDayNight)
                    {
                        int offset = (int)((value - SM.m_currentDayTimeHour) / SimulationManager.DAYTIME_FRAME_TO_HOUR);
                        uint dayOffsetFrames = SM.m_dayTimeOffsetFrames;
                        dayOffsetFrames = (uint)(((long)dayOffsetFrames + offset) % SimulationManager.DAYTIME_FRAMES);
                        SM.m_dayTimeOffsetFrames = dayOffsetFrames;
                        SM.m_currentDayTimeHour = value;
                    }
                }

                UpdateDate_GameTime();
            }
        }

        public bool enableWeather
        {
            get
            {
                return (bool)this.m_EnableWeather;
            }
            set
            {
                this.m_EnableWeather.value = value;
                if (Singleton<WeatherManager>.exists)
                {
                    Singleton<WeatherManager>.instance.m_enableWeather = value;
                }
            }
        }

        private float m_WeatherRainIntensity = 0;
        public float WeatherRainIntensity
        {
            get
            {
                return m_WeatherRainIntensity;
            }

            set
            {
                m_WeatherRainIntensity = value;
                if (Singleton<WeatherManager>.exists)
                {
                    Singleton<WeatherManager>.instance.m_currentRain = value;
                    Singleton<WeatherManager>.instance.m_targetRain = value;
                }
            }
        }

        public void UpdateDataAll()
        {
            UpdateDate_1();
            UpdateDate_3();
            UpdateDate_5();
            UpdateDate_10();
            if (Singleton<SimulationManager>.exists)
                Maptheme.text = MapThemeLoader.GetThemeString(Singleton<SimulationManager>.instance.m_metaData.m_environment);
        }

        public void UpdateDate_GameTime()
        {
            DateTime d = new DateTime();
            d = d.AddHours(Singleton<SimulationManager>.instance.m_currentDayTimeHour);
            //GameTimeOfDay.text = SM.m_currentDayTimeHour.ToString(); //
            GameTimeOfDay.extvalue = WorldTimeOfDay;
            GameTimeOfDay.text = (new DateTime()).AddHours(WorldTimeOfDay).ToString("HH:mm", LocaleManager.cultureInfo);
        }

        public void UpdateCapacities()
        {
            if (Singleton<DistrictManager>.exists)
            {
                var info = Singleton<DistrictManager>.instance.m_districts.m_buffer[0];
                // 电力用量
                Electricity.value = info.GetElectricityCapacity();
                Electricity.subvalue = info.GetElectricityConsumption();
                Electricity.extvalue = this.GetAvailabilityPercentage((int)Electricity.value, (int)Electricity.subvalue);

                // 水用量
                Water.value = info.GetWaterCapacity();
                Water.subvalue = info.GetWaterConsumption();
                Water.extvalue = this.GetAvailabilityPercentage((int)Water.value, (int)Water.subvalue);

                // 排污量
                Sewage.value = info.GetSewageCapacity();
                Sewage.subvalue = info.GetSewageAccumulation();
                Sewage.extvalue = this.GetAvailabilityPercentage((int)Sewage.value, (int)Sewage.subvalue);

                // 学校容量
                ElementarySchool.value = info.GetEducation1Capacity();
                ElementarySchool.subvalue = info.GetEducation1Need();
                ElementarySchool.extvalue = this.GetAvailabilityPercentage((int)ElementarySchool.value, (int)ElementarySchool.subvalue);

                HighSchool.value = info.GetEducation2Capacity();
                HighSchool.subvalue = info.GetEducation2Need();
                HighSchool.extvalue = this.GetAvailabilityPercentage((int)HighSchool.value, (int)HighSchool.subvalue);

                University.value = info.GetEducation3Capacity();
                University.subvalue = info.GetEducation3Need();
                University.extvalue = this.GetAvailabilityPercentage((int)University.value, (int)University.subvalue);
            }
        }

        public void UpdateDate_1() // 1 秒 -- 适时数据
        {
            // 时间
            SimulationManager SM = Singleton<SimulationManager>.instance;

            GameTime.text = SM.m_currentGameTime.Date.ToString("yyyy-MM-dd", LocaleManager.cultureInfo);

            PlayingTime.text = (new DateTime()).AddSeconds(PlayingTime.extvalue).ToString("HH:mm:ss", LocaleManager.cultureInfo);
            UpdateDate_GameTime();

            // 人口
            Population.value = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].m_populationData.m_finalCount;
            Population.text = FormatScientific((int)Population.value, false);
            // 人口变化
            PopulationDelta.value = Singleton<DistrictManager>.instance.m_districts.m_buffer[0].m_populationData.GetWeeklyDelta();
            PopulationDelta.text = FormatScientific((int)PopulationDelta.value, true);
            // 金额
            CashAmount.value = Singleton<EconomyManager>.instance.LastCashAmount;
            CashAmount.text = FormatMoney(CashAmount.value, false); 
            // 每周收入
            CashDelta.value = Singleton<EconomyManager>.instance.LastCashDelta;
            CashDelta.text = FormatMoney(CashDelta.value, true);
            // 树
            TreeCount.value = Singleton<TreeManager>.instance.m_treeCount - 1;
            TreeCount.text = FormatScientific((int)TreeCount.value, false);
        }

        public void UpdateDate_3() // 3 秒 -- 即时数据
        {
            // 温度
            Temperatur.extvalue = Singleton<WeatherManager>.instance.m_currentTemperature;
            m_Temperature.Check(Temperatur.extvalue);
            Temperatur.text = m_Temperature.result;
            // 人口实例
            CitizenInstanceCount.value = Singleton<CitizenManager>.instance.m_instanceCount;
            CitizenInstanceCount.text = FormatScientific((int)CitizenInstanceCount.value, false);
        }

        public void UpdateDate_5()  // 5 秒  -- 天数据
        {
            DistrictManager DM = Singleton<DistrictManager>.instance;
            // 失业率
            Unemployment.value = DM.m_districts.m_buffer[0].GetUnemployment();
            Unemployment.text = FormatPercentage((int)Unemployment.value);
            // 车辆
            VehicleCount.value = Singleton<VehicleManager>.instance.m_vehicleCount;
            VehicleParked.value = Singleton<VehicleManager>.instance.m_parkedCount;
            VehicleCount.text = FormatScientific((int)VehicleCount.value, false);
            VehicleParked.text = FormatScientific((int)VehicleParked.value, false);
            // 天气
            //if (WeatherRainIntensity == 0)
            //{
            //    if (Singleton<WeatherManager>.exists)
            //        Singleton<WeatherManager>.instance.m_targetRain = 0;
            //}
            // 游客
            int num1 = (int)DM.m_districts.m_buffer[0].m_tourist1Data.m_averageCount;
            int num2 = (int)DM.m_districts.m_buffer[0].m_tourist2Data.m_averageCount;
            int num3 = (int)DM.m_districts.m_buffer[0].m_tourist3Data.m_averageCount;
            TouristsAverage.value = (num1 + num2) +num3;
            TouristsAverage.text = FormatScientific((int)TouristsAverage.value, false);
        }

        public void UpdateDate_10() // 10 秒 -- 周数据
        {
            DistrictManager DM = Singleton<DistrictManager>.instance;
            // 老年人
            Senior.value = GetPercentValue((int)DM.m_districts.m_buffer[0].m_seniorData.m_finalCount,
                (int)DM.m_districts.m_buffer[0].m_populationData.m_finalCount, false);
            Senior.text = FormatPercentage((int)Senior.value);
            // 幸福度
            Happiness.extvalue = (float)DM.m_districts.m_buffer[0].m_finalHappiness;
            Happiness.text = FormatPercentage((int)(Happiness.extvalue));
            // 健康率
            AvgHealth.value = DM.m_districts.m_buffer[0].m_residentialData.m_finalHealth;
            AvgHealth.text = FormatPercentage((int)AvgHealth.value);
            // 教育         
            int finalCount = (int)DM.m_districts.m_buffer[0].m_educated0Data.m_finalCount;
            int num11 = (int)DM.m_districts.m_buffer[0].m_educated1Data.m_finalCount;
            int num12 = (int)DM.m_districts.m_buffer[0].m_educated2Data.m_finalCount;
            int num13 = (int)DM.m_districts.m_buffer[0].m_educated3Data.m_finalCount;
            int total = ((finalCount + num11) + num12) + num13;
            EducatedLegend.value = GetPercentValue(num11, total, true);
            WellEducatedLegend.value = GetPercentValue(num12, total, true);
            HighlyEducatedLegend.value = GetPercentValue(num13, total, true);
            EducatedLegend.text = FormatPercentage((int)EducatedLegend.value);
            WellEducatedLegend.text = FormatPercentage((int)WellEducatedLegend.value);
            HighlyEducatedLegend.text = FormatPercentage((int)HighlyEducatedLegend.value);
            // 交通
            TransportManager TM = Singleton<TransportManager>.instance;
            BusCitizens.value = TM.m_passengers[0].m_residentPassengers.m_averageCount;
            MetroCitizens.value = TM.m_passengers[1].m_residentPassengers.m_averageCount;
            TrainCitizens.value = TM.m_passengers[2].m_residentPassengers.m_averageCount;
            ShipCitizens.value = TM.m_passengers[3].m_residentPassengers.m_averageCount;
            PlaneCitizens.value = TM.m_passengers[4].m_residentPassengers.m_averageCount;
            TaxiCitizens.value = TM.m_passengers[5].m_residentPassengers.m_averageCount;
            TramCitizens.value = TM.m_passengers[6].m_residentPassengers.m_averageCount;
            PublicTransportAverage.value = BusCitizens.value + MetroCitizens.value + TrainCitizens.value + ShipCitizens.value + PlaneCitizens.value + TaxiCitizens.value + TramCitizens.value;
            PublicTransportAverage.text = FormatScientific((int)PublicTransportAverage.value, false);
            // 进口
            ImportOil.value = (int)((DM.m_districts.m_buffer[0].m_importData.m_averageOil + 0x63) / 100);
            ImportOre.value = (int)((DM.m_districts.m_buffer[0].m_importData.m_averageOre + 0x63) / 100);
            ImportGoods.value = (int)((DM.m_districts.m_buffer[0].m_importData.m_averageGoods + 0x63) / 100);
            ImportForestry.value = (int)((DM.m_districts.m_buffer[0].m_importData.m_averageForestry + 0x63) / 100);
            ImportAgricultural.value = (int)((DM.m_districts.m_buffer[0].m_importData.m_averageAgricultural + 0x63) / 100);
            ImportTotal.value = (int)(ImportOil.value + ImportOre.value + ImportGoods.value + ImportForestry.value + ImportAgricultural.value);
            // 出口
            ExportOil.value = (int)((DM.m_districts.m_buffer[0].m_exportData.m_averageOil + 0x63) / 100);
            ExportOre.value = (int)((DM.m_districts.m_buffer[0].m_exportData.m_averageOre + 0x63) / 100);
            ExportGoods.value = (int)((DM.m_districts.m_buffer[0].m_exportData.m_averageGoods + 0x63) / 100);
            ExportForestry.value = (int)((DM.m_districts.m_buffer[0].m_exportData.m_averageForestry + 0x63) / 100);
            ExportAgricultural.value = (int)((DM.m_districts.m_buffer[0].m_exportData.m_averageAgricultural + 0x63) / 100);
            ExportTotal.value = (int)(ExportOil.value + ExportOre.value + ExportGoods.value + ExportForestry.value + ExportAgricultural.value);
            // 地价
            CityInfo.value = DM.m_districts.m_buffer[0].GetLandValue() * 100;
            // 城市名称
            string str = !Singleton<SimulationManager>.exists ? null : Singleton<SimulationManager>.instance.m_metaData.m_CityName;
            if (str == null)
            {
                str = "Cities: Skylines";
            }
            CityInfo.text = str;
            // 分区
            DistrictsCount.value = DM.m_districts.ItemCount() - 1;
            DistrictsCount.text = DistrictsCount.value.ToString();
            // 建筑
            BuildingCount.value = Singleton<BuildingManager>.instance.m_buildingCount - 1;
            BuildingCount.text = FormatScientific((int)BuildingCount.value, false);
            // 容量数据
            UpdateCapacities();
        }

        public void LateUpdate()
        {
            PlayingTime.extvalue += Time.deltaTime;

            if (!Singleton<SimulationManager>.exists || ExtendedInfoManager.infopanel == null || !ExtendedInfoManager.infopanel.isVisible)
                return;

            float deltaTime = Singleton<SimulationManager>.instance.m_simulationTimeDelta; // Time.deltaTime

            // 1s
            Time_1 += deltaTime;
            if (Time_1 >= 1.0f)
            {
                Time_1 = 0.0f;
                UpdateDate_1();
            }

            // 3s
            Time_3 += deltaTime;
            if (Time_3 >= 3.0f)
            {
                Time_3 = 0.0f;
                UpdateDate_3();
            }

            // 5s
            Time_5 += deltaTime;
            if (Time_5 >= 5.0f)
            {
                Time_5 = 0.0f;
                UpdateDate_5();
            }

            // 10s
            Time_10 += deltaTime;
            if (Time_10 >= 10.0f)
            {
                Time_10 = 0.0f;
                UpdateDate_10();
            }
        }

        public static int GetPercentValue(int value, int total, bool trunc)
        {
            float num = ((float)value) / ((float)total);
            if (trunc)
                return Mathf.Clamp(Mathf.RoundToInt(num * 100f), 0, 100);
            else
                return Mathf.Clamp(Mathf.FloorToInt(num * 100f), 0, 100);
        }

        private float GetAvailabilityPercentage(int capacity, int consumption, int consumptionMin = 45, int consumptionMax = 55)
        {
            if (capacity == 0)
            {
                return 0f;
            }
            else
            {
                float basePercent = capacity / (float)consumption;
                float percentModifier = (float)((consumptionMin + consumptionMax) / 2);
                return basePercent * percentModifier;
            }
        }


        public static string FormatScientific(int value, bool isMark)
        {
            return string.Format((isMark && value > 0) ? "+{0:N0}" : "{0:N0}", value);
        }

        public static string FormatPercentage(int p)
        {
            return string.Format("{0}%", p); //Locale.Get("VALUE_PERCENTAGE")
        }

        static UICurrencyWrapper CurrencyWrapper = new UICurrencyWrapper(0x7fffffffffffffffL);
        public static string FormatMoney(long value, bool isMark)
        {
            //CurrencyWrapper.Check(value, "N0"); //Settings.moneyFormatNoCents
            //var result = CurrencyWrapper.result;
            var result = "";
            if (value == 0x7fffffffffffffffL)
            {
                result = "∞";
            }
            else
            {
                result = string.Format("$ {0:N}", ((double)value) / 100.0);
            }

            return ((isMark && value > 0) ? "+ " + result : result);
        }

        private Color getUnemploymentColor()
        {
            Color result = ExtendedPanel.COLOR_DARK_GREEN;
            if (Unemployment.value >= 10f)
            {
                result = ExtendedPanel.COLOR_DARK_YELLOW;
                if (Unemployment.value >= 20f)
                    result = ExtendedPanel.COLOR_DARK_RED;
            }
            return result;
        }

        private Color getHappinessColor()
        {
            Color result = ExtendedPanel.COLOR_DARK_GREEN;
            if (Happiness.extvalue <= 70f)
            {
                result = ExtendedPanel.COLOR_DARK_YELLOW;
                if (Happiness.extvalue <= 50f)
                    result = ExtendedPanel.COLOR_DARK_RED;
            }
            return result;
        }


        private Color getPopulationDeltaColor()
        {
            Color result = (PopulationDelta.value <= 0) ? ((PopulationDelta.value >= 0) ? ExtendedPanel.COLOR_DARK_YELLOW : ExtendedPanel.COLOR_DARK_RED) : ExtendedPanel.COLOR_DARK_GREEN;
            return result;
        }

        private Color getCashDeltaColor()
        {
            Color result = (CashDelta.value <= 0) ? ((CashDelta.value >= 0) ? new Color(255, 69, 0) : new Color(128, 0, 0)) : ExtendedPanel.COLOR_DARK_GREEN;
            return result;
        }

        public static bool isGamePaused()
        {
            if (Singleton<SimulationManager>.exists)
            {
                return Singleton<SimulationManager>.instance.SimulationPaused || Singleton<SimulationManager>.instance.ForcedSimulationPaused;
            }
            else
                return true;
        }
    }
}
