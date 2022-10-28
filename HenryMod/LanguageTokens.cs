using R2API;
using System;
using System.Collections.Generic;
using System.Text;

namespace HANDMod
{
    internal class LanguageTokens
    {
        public LanguageTokens()
        {
            string prefix = Content.HANDSurvivor.HANDSurvivor.HAND_PREFIX;
            LanguageAPI.Add(prefix + "NAME", "HAN-D");
            LanguageAPI.Add(prefix + "DESCRIPTION", "TODO");
            LanguageAPI.Add(prefix + "SUBTITLE", "Unpaid Janitor");
            LanguageAPI.Add(prefix + "LORE", "sample lore");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", "..and so it left, servos pulsing with new life.");
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", "..and so it vanished, unrewarded in all of its efforts.");

            LanguageAPI.Add(prefix + "UTILITY_NAME", "Overclock");
            LanguageAPI.Add(prefix + "UTILITY_DESC", "<style=cIsUtility>Springy</style>. Increase <style=cIsUtility>movement speed</style> and <style=cIsDamage>attack speed</style> by <style=cIsDamage>40%</style> and gain <style=cIsDamage>50% stun chance</style>. Hit enemies to increase duration.");

            LanguageAPI.Add(prefix + "KEYWORD_SPRINGY", "<style=cKeywordName>Springy</style><style=cSub>Spring upwards when using this skill.</style>");
        }
    }
}
