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
            string handPrefix = Content.HANDSurvivor.HANDSurvivor.HAND_PREFIX;
            LanguageAPI.Add(handPrefix + "NAME", "HAN-D");

            string HANDDesc = "";
            HANDDesc += "HAN-D is a tanky robot janitor whose powerful melee attacks are sure to leave a mess! <style=cSub>\r\n\r\n";
            HANDDesc += "< ! > HURT has increased knockback against airborne enemies. Use FORCED_REASSEMBLY to pop enemies in the air, then HURT them to send them flying!\r\n\r\n";
            HANDDesc += "< ! > FORCED_REASSEMBLY's self-knockback can be used to reach flying enemies.\r\n\r\n";
            HANDDesc += "< ! > OVERCLOCK lasts as long as you can keep hitting enemies.\r\n\r\n";
            HANDDesc += "< ! > Use DRONES to heal and stay in the fight.\r\n\r\n";
            LanguageAPI.Add(handPrefix + "DESCRIPTION", HANDDesc);

            LanguageAPI.Add(handPrefix + "SUBTITLE", "Unpaid Janitor");

            string tldr = "<style=cMono>\r\n//--AUTO-TRANSCRIPTION FROM BASED DEPARTMENT OF UES [REDACTED]--//</style>\r\n\r\n<i>*hits <color=#327FFF>Spinel Tonic</color>*</i>\n\nIs playing without the <color=#6955A6>Command</color> artifact the ultimate form of cuckoldry?\n\nI cannot think or comprehend of anything more cucked than playing without <color=#6955A6>Command</color>. Honestly, think about it rationally. You are shooting, running, jumping for like 60 minutes solely so you can get a fucking <color=#77FF16>Squid Polyp</color>. All that hard work you put into your run - dodging <style=cIsHealth>Stone Golem</style> lasers, getting annoyed by six thousand <style=cIsHealth>Lesser Wisps</color> spawning above your head, activating <color=#E5C962>Shrines of the Mountain</color> all for one simple result: your inventory is filled up with <color=#FFFFFF>Warbanners</color> and <color=#FFFFFF>Monster Tooth</color> necklaces which cost money.\n\nOn a god run? Great. A bunch of shitty items which add nothing to your run end up coming out of the <color=#E5C962>Chests</color> you buy. They get the benefit of your hard earned dosh that came from killing <style=cIsHealth>Lemurians</style>.\n\nAs a man who plays this game you are <style=cIsHealth>LITERALLY</style> dedicating two hours of your life to opening boxes and praying it's not another <color=#77FF16>Chronobauble</color>. It's the ultimate and final cuck. Think about it logically.\r\n<style=cMono>\r\nTranscriptions complete.\r\n</style>\r\n \r\n\r\n";
            LanguageAPI.Add(handPrefix + "LORE", tldr);

            LanguageAPI.Add(handPrefix + "OUTRO_FLAVOR", "..and so it left, servos pulsing with new life.");
            LanguageAPI.Add(handPrefix + "OUTRO_FAILURE", "..and so it vanished, unrewarded in all of its efforts.");

            LanguageAPI.Add(handPrefix + "PASSIVE_NAME", "SWARM_ARMOR");
            LanguageAPI.Add(handPrefix + "PASSIVE_DESC", "<style=cIsDamage>Reduce incoming damage</style> by <style=cIsDamage>0.3%</style> of your maximum health for every <style=cIsUtility>drone</style> on your team.");

            LanguageAPI.Add(handPrefix + "PRIMARY_NAME", "HURT");
            LanguageAPI.Add(handPrefix + "PRIMARY_DESC", "Apply force to all combatants for <style=cIsDamage>390% damage</style>. <style=cIsDamage>Stuns</style> while <style=cIsDamage>OVERCLOCK</style> is active.");

            LanguageAPI.Add(handPrefix + "PRIMARY_HAMMER_NAME", "SMASH");
            LanguageAPI.Add(handPrefix + "PRIMARY_HAMMER_DESC", "<style=cIsHealth>Slow down</style> and smash all combatants for <style=cIsDamage>600% damage</style>. <style=cIsDamage>Stuns</style> while <style=cIsDamage>OVERCLOCK</style> is active.");

            LanguageAPI.Add(handPrefix + "SECONDARY_NAME", "FORCED_REASSEMBLY");
            LanguageAPI.Add(handPrefix + "SECONDARY_DESC", "<style=cIsDamage>Stunning</style>. Charge up and apply great force to all combatants for <style=cIsDamage>600%-1500% damage</style>.");

            LanguageAPI.Add(handPrefix + "SECONDARY_SCEPTER_NAME", "UNETHICAL_REASSEMBLY");
            LanguageAPI.Add(handPrefix + "SECONDARY_SCEPTER_DESC", "<style=cIsDamage>Stunning</style>. Charge up and apply <style=cIsHealth>OVERWHELMING</style> force to all combatants for <style=cIsDamage>900%-2250% damage</style>.");

            LanguageAPI.Add(handPrefix + "UTILITY_NAME", "OVERCLOCK");
            LanguageAPI.Add(handPrefix + "UTILITY_DESC", "<style=cIsUtility>Boost</style>. Increase <style=cIsUtility>movement speed</style> and <style=cIsDamage>attack speed</style> by <style=cIsDamage>40%</style>. Hit enemies to <style=cIsUtility>increase duration</style>.");

            LanguageAPI.Add(handPrefix + "UTILITY_NEMESIS_NAME", "FOCUS");
            LanguageAPI.Add(handPrefix + "UTILITY_NEMESIS_DESC", "<style=cIsUtility>Boost</style>. Temporarily deal <style=cIsDamage>50% extra damage</style>. Hit enemies to <style=cIsUtility>increase duration</style>.");

            LanguageAPI.Add("KEYWORD_MOFFEINHANDOVERCLOCKED_BOOST", "<style=cKeywordName>Boost</style><style=cSub>The skill can be cancelled early to boost upwards.</style>");

            LanguageAPI.Add(handPrefix + "SPECIAL_NAME", "DRONE");
            LanguageAPI.Add(handPrefix + "SPECIAL_DESC", "Fire a drone that deals <style=cIsDamage>270% damage</style> and <style=cIsHealing>heals for 40% of the damage dealt</style>. Kills and melee hits <style=cIsUtility>reduce cooldown</style>.");

            LanguageAPI.Add("ACHIEVEMENT_MOFFEINHANDOVERCLOCKEDHAMMERPRIMARYUNLOCK_NAME", "HAN-D: Bonk!");
            LanguageAPI.Add("ACHIEVEMENT_MOFFEINHANDOVERCLOCKEDHAMMERPRIMARYUNLOCK_DESCRIPTION", "Flatten the final boss.");

            LanguageAPI.Add("ACHIEVEMENT_MOFFEINHANDOVERCLOCKEDNEMESISFOCUSUNLOCK_NAME", "HAN-D: Beatdown");
            LanguageAPI.Add("ACHIEVEMENT_MOFFEINHANDOVERCLOCKEDNEMESISFOCUSUNLOCK_DESCRIPTION", "Keep OVERCLOCK active for over 45 seconds.");

            LanguageAPI.Add("ACHIEVEMENT_MOFFEINHANDOVERCLOCKEDSURVIVORUNLOCK_NAME", "Handyman");
            LanguageAPI.Add("ACHIEVEMENT_MOFFEINHANDOVERCLOCKEDSURVIVORUNLOCK_DESCRIPTION", "Repair the robot janitor on Rallypoint Delta with an Escape Pod's Fuel Array.");

            string rmorPrefix = Content.RMORSurvivor.RMORSurvivor.RMOR_PREFIX;
            LanguageAPI.Add(rmorPrefix + "NAME", "R-MOR");

            LanguageAPI.Add(rmorPrefix + "PRIMARY_NAME", "ERADICATE");
            LanguageAPI.Add(rmorPrefix + "PRIMARY_DESC", "Charge up your cannons and fire a barrage of rockets for up to <style=cIsDamage>3x420% damage</style>.");
        }
    }
}
