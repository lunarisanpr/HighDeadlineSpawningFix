using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HighDeadlineSpawningFix.Hooks;
using MonoMod.RuntimeDetour;
using MonoMod.Cil;
using BepInEx.Configuration;

namespace HighDeadlineSpawningFix
{
    public enum DeadlineClampType
    {
        None,
        Clamp,
        Modulo
    }

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class HighDeadlineSpawningFix : BaseUnityPlugin
    {
        public static HighDeadlineSpawningFix Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;

        public static ConfigFile Config { get; private set; }

        public static ConfigEntry<DeadlineClampType> ClampType { get; private set; }
        public static ConfigEntry<int> ClampValue { get; private set; }

        private static void InitConfig()
        {
            ClampType = Config.Bind<DeadlineClampType>(
                "High Deadline Spawning Fix Settings", 
                "Clamp Type", 
                DeadlineClampType.Modulo, 
                "Sets the clamp type.\nNone = Disables clamping (vanilla behavior)\nClamp = clamps " +
                "daysUntilDeadline used in calculations to a maximum value\nModulo = gets the mod 4 " +
                "of the daysUntilDeadline used in calculations (e.g. 4 Days Before Deadline will be treated as " +
                "0 Days Before Deadline by the spawning script)."
             );
            ClampValue = Config.Bind<int>(
                "High Deadline Spawning Fix Settings",
                "Clamp Value (Clamp)",
                3,
                "When Clamp Type is set to Clamp, sets the maximum amount of days left before deadline. " +
                "Has no effect if None or Modulo is selected."
             );
        }

        private void Awake()
        {
            Config = base.Config;
            InitConfig();
            
            Logger = base.Logger;
            Instance = this;

            Hook();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }

        internal static void Hook()
        {
            Logger.LogDebug("Hooking...");

            IL.RoundManager.PredictAllOutsideEnemies += RoundManagerHook.PatchDaysUntilDeadlineAccess;
            IL.RoundManager.SpawnEnemiesOutside += RoundManagerHook.PatchDaysUntilDeadlineAccess;
            IL.RoundManager.PlotOutEnemiesForNextHour += RoundManagerHook.PatchDaysUntilDeadlineAccess;
            IL.RoundManager.AdvanceHourAndSpawnNewBatchOfEnemies += RoundManagerHook.PatchDaysUntilDeadlineAccess;

            Logger.LogDebug("Finished Hooking!");
        }

        internal static void Unhook()
        {
            Logger.LogDebug("Unhooking...");

            IL.RoundManager.PredictAllOutsideEnemies -= RoundManagerHook.PatchDaysUntilDeadlineAccess;
            IL.RoundManager.SpawnEnemiesOutside -= RoundManagerHook.PatchDaysUntilDeadlineAccess;
            IL.RoundManager.PlotOutEnemiesForNextHour -= RoundManagerHook.PatchDaysUntilDeadlineAccess;
            IL.RoundManager.AdvanceHourAndSpawnNewBatchOfEnemies -= RoundManagerHook.PatchDaysUntilDeadlineAccess;

            Logger.LogDebug("Finished Unhooking!");
        }
    }
}
