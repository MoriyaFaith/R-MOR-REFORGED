using RoR2;
using UnityEngine;

namespace HANDMod.Modules
{
    public class Buffs
    {
        public static BuffDef CreateBuffDef(string name, bool canStack, bool isCooldown, bool isDebuff, Color color, Sprite iconSprite)
        {
            BuffDef bd = ScriptableObject.CreateInstance<BuffDef>();
            bd.name = name;
            bd.canStack = canStack;
            bd.isCooldown = isCooldown;
            bd.isDebuff = isDebuff;
            bd.buffColor = color;
            bd.iconSprite = iconSprite;

            HANDMod.Modules.ContentPacks.buffDefs.Add(bd);
            (bd as UnityEngine.Object).name = bd.name;
            return bd;
        }
    }
}
