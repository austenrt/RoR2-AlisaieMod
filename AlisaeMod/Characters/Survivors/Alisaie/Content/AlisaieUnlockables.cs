using AlisaieMod.Survivors.Alisaie.Achievements;
using RoR2;
using UnityEngine;

namespace AlisaieMod.Survivors.Alisaie
{
    public static class AlisaieUnlockables
    {
        public static UnlockableDef characterUnlockableDef = null;
        public static UnlockableDef masterySkinUnlockableDef = null;
        public static UnlockableDef winterSkinUnlockableDef = null;
        public static UnlockableDef shadowbringersSkinUnlockableDef = null;
        public static UnlockableDef summerSkinUnlockableDef = null;

        public static void Init()
        {
            masterySkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                AlisaieMasteryAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(AlisaieMasteryAchievement.identifier),
                AlisaieSurvivor.instance.assetBundle.LoadAsset<Sprite>("texStormbloodSkin"));

            winterSkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                AlisaieWinterSkinAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(AlisaieWinterSkinAchievement.identifier),
                AlisaieSurvivor.instance.assetBundle.LoadAsset<Sprite>("texWinterSkin"));
            
            shadowbringersSkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                AlisaieShadowbringersSkinAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(AlisaieShadowbringersSkinAchievement.identifier),
                AlisaieSurvivor.instance.assetBundle.LoadAsset<Sprite>("texShadowbringersSkin"));

            summerSkinUnlockableDef = Modules.Content.CreateAndAddUnlockbleDef(
                AlisaieSummerSkinAchievement.unlockableIdentifier,
                Modules.Tokens.GetAchievementNameToken(AlisaieSummerSkinAchievement.identifier),
                AlisaieSurvivor.instance.assetBundle.LoadAsset<Sprite>("texSummerSkin"));
        }
    }
}
