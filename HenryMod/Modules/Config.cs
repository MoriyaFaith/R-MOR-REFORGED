using BepInEx.Configuration;
using UnityEngine;

namespace HANDMod.Modules
{
    public static class Config
    {
        public static bool forceUnlock = false;
        public static void ReadConfig()
        {
            forceUnlock = HandPlugin.instance.Config.Bind("General", "Force Unlock", false, "Automatically unlock HAN-D and his skills by default.").Value;
        }

        // this helper automatically makes config entries for disabling survivors
        public static ConfigEntry<bool> CharacterEnableConfig(string characterName, string description = "Set to false to disable this character", bool enabledDefault = true) {

            return HandPlugin.instance.Config.Bind<bool>("General",
                                                          "Enable " + characterName,
                                                          enabledDefault,
                                                          description);
        }
    }
}