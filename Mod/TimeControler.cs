using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework;
using System;


namespace IINS.ExtendedInfo
{
    public class TimeControler : MonoBehaviour, ISimulationManager
    {
        [Serializable]
        public struct TimeSpeed
        {
            public uint num;
            public uint den;
        }

        private uint tick;
        public static TimeSpeed[] TimeSpeeds = {
            new TimeSpeed(){num=0, den=1},
            new TimeSpeed(){num=1, den=1},
        };

        public TimeSpeed speed;

        public int speedIndex = 0;
        private static bool exists;

        public TimeControler()
        {
            exists = true;
            name = "IINS-TimeControler";          
        }

        public void Awake()
        {
            SimulationManager.RegisterSimulationManager(this);
            speed = new TimeSpeed() { num = 1, den = 1 }; // TimeSpeeds[speedIndex];
        }

        public void OnDestroy()
        {
            exists = false;
            GameObject.Destroy(gameObject);
        }

        public void SimulationStep(int subStep)
        {
            var SM = Singleton<SimulationManager>.instance;


            if (SM.m_enableDayNight && (CityInfoDatas.TimeWarpMod_sunManager == null) && exists)
            {
                uint dayOffsetFrames = SM.m_dayTimeOffsetFrames;

                if (!SM.SimulationPaused && !SM.ForcedSimulationPaused)
                {

                    if (tick == 0)
                    {
                        dayOffsetFrames = (dayOffsetFrames + (uint)speed.num - 1) % SimulationManager.DAYTIME_FRAMES;
                    }
                    else
                    {
                        dayOffsetFrames = (dayOffsetFrames - 1) % SimulationManager.DAYTIME_FRAMES;
                    }

                    tick = (tick + 1) % speed.den;
                }

                SM.m_dayTimeOffsetFrames = dayOffsetFrames;
            }
        }

        public ThreadProfiler GetSimulationProfiler()
        {
            return null;
        }

        public void UpdateData(SimulationManager.UpdateMode mode)
        {

        }
        public void LateUpdateData(SimulationManager.UpdateMode mode)
        {
        }

        public void GetData(FastList<ColossalFramework.IO.IDataContainer> data)
        {

        }

        public void EarlyUpdateData()
        {

        }

        public string GetName()
        {
            return "IINS TimeControler";
        }
    }

}
