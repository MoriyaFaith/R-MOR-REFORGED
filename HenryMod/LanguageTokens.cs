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
            string prefix = HANDSurvivor.HAND_PREFIX;
            LanguageAPI.Add(prefix + "NAME", "HAN-D");
            LanguageAPI.Add(prefix + "DESCRIPTION", "TODO");
            LanguageAPI.Add(prefix + "SUBTITLE", "Unpaid Janitor");
            LanguageAPI.Add(prefix + "LORE", "sample lore");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", "..and so it left, servos pulsing with new life.");
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", "..and so it vanished, unrewarded in all of its efforts.");
        }
    }
}
