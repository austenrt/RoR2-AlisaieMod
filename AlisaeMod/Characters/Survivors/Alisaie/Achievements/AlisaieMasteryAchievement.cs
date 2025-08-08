using RoR2;
using AlisaieMod.Modules.Achievements;

namespace AlisaieMod.Survivors.Alisaie.Achievements
{
    //automatically creates language tokens "ACHIEVMENT_{identifier.ToUpper()}_NAME" and "ACHIEVMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10, null)]
    public class AlisaieMasteryAchievement : BaseMasteryAchievement
    {
        public const string identifier = AlisaieSurvivor.ALISAIE_PREFIX + "masteryAchievement";
        public const string unlockableIdentifier = AlisaieSurvivor.ALISAIE_PREFIX + "masteryUnlockable";

        public override string RequiredCharacterBody => AlisaieSurvivor.instance.bodyName;

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
    }
}