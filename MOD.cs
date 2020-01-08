using ICities;
using IINS.UIExt;
using ColossalFramework.PlatformServices;

namespace IINS.ExtendedInfo
{

    //2016.12.12 13:30: locked for steam version


    public class ExtendedInfoMod : MODLoadingExtension, IUserMod
    {
        public static string Version = "(1.0.2)";

        public ExtendedInfoMod()
        {
            Debugger.Prefix = "[" + Name + " " + Version + "] "; 
        }

        public string Name
        {
            get
            {
                return "Extended InfoPanel" + " " + Version;
            }
        }
        public string Description
        {
            get
            {
                return "\nShow more information data in game bottom panel.";
            }
        }

        public override void OnReleased() 
        {
            base.OnReleased();
            ExtendedInfoManager.stop();    
        }


        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (mode == LoadMode.LoadGame || mode == LoadMode.NewGame)
                if (PlatformService.apiBackend == APIBackend.Steam)
                    ExtendedInfoManager.run();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();                   
        }

    }
}
