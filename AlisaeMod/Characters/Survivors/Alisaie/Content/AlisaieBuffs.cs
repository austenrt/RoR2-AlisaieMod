using RoR2;
using UnityEngine;

namespace AlisaieMod.Survivors.Alisaie
{
    public static class AlisaieBuffs
    {
        // armor buff gained during roll
        public static BuffDef armorBuff;
        public static BuffDef dualCastBuff;
        public static BuffDef accelerationBuff;
        public static BuffDef finisherBuff;

        public static void Init(AssetBundle assetBundle)
        {
            armorBuff = Modules.Content.CreateAndAddBuff("AlisaieArmorBuff",
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/HiddenInvincibility").iconSprite,
                Color.white,
                false,
                false);

            dualCastBuff = Modules.Content.CreateAndAddBuff(
                "AlisaieDualcastBuff",
                assetBundle.LoadAsset<Sprite>("texIconDualcast"),
                Color.white,
                true,
                false
                );

            finisherBuff = Modules.Content.CreateAndAddBuff(
                "AlisaieFinisherBuff",
                assetBundle.LoadAsset<Sprite>("texIconFinisher"),
                Color.white,
                true,
                false
                );

            accelerationBuff = Modules.Content.CreateAndAddBuff(
                "AlisaieAccelerationBuff",
                assetBundle.LoadAsset<Sprite>("texIconAccelerationBuff"),
                Color.white,
                false,
                false
                );

        }
    }
}
