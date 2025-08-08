using RoR2;
using RoR2.Achievements;
using AlisaieMod.Survivors.Alisaie;
using UnityEngine;

[RegisterAchievement(identifier, unlockableIdentifier, null, 5, null)]
public class AlisaieShadowbringersSkinAchievement : BaseAchievement
{
    public const string identifier = AlisaieSurvivor.ALISAIE_PREFIX + "shadowbringersSkinAchievement";
    public const string unlockableIdentifier = AlisaieSurvivor.ALISAIE_PREFIX + "shadowbringersSkinUnlockable";

    public override BodyIndex LookUpRequiredBodyIndex() => BodyCatalog.FindBodyIndex(AlisaieSurvivor.instance.bodyName);

    private int lastMultiKillCount = 0;

    public override void OnBodyRequirementMet()
    {
        base.OnBodyRequirementMet();
        RoR2Application.onFixedUpdate += OnFixedUpdate;
    }

    public override void OnBodyRequirementBroken()
    {
        RoR2Application.onFixedUpdate -= OnFixedUpdate;
        base.OnBodyRequirementBroken();
    }

    private void OnFixedUpdate()
    {
        var body = this.localUser.cachedBody;
        if (body != null)
        {
            if (body.multiKillCount >= 5 && body.multiKillCount != lastMultiKillCount)
            {
                Grant();
                lastMultiKillCount = body.multiKillCount;
            }
        }
    }
}
