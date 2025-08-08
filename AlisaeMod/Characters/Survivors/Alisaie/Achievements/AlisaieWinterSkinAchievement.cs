using RoR2;
using RoR2.Achievements;
using AlisaieMod.Survivors.Alisaie;
using EntityStates;
using UnityEngine;

[RegisterAchievement(identifier, unlockableIdentifier, null, 5, null)]
public class AlisaieWinterSkinAchievement : BaseAchievement
{
    public const string identifier = AlisaieSurvivor.ALISAIE_PREFIX + "winterSkinAchievement";
    public const string unlockableIdentifier = AlisaieSurvivor.ALISAIE_PREFIX + "winterSkinUnlockable";

    public override BodyIndex LookUpRequiredBodyIndex() => BodyCatalog.FindBodyIndex(AlisaieSurvivor.instance.bodyName);

    private bool achieved = false;
    private EntityStateMachine trackedStateMachine;

    public override void OnBodyRequirementMet()
    {
        base.OnBodyRequirementMet();
        RoR2Application.onFixedUpdate += OnFixedUpdate;
        var body = this.localUser.cachedBody;
        if (body != null)
        {
            trackedStateMachine = EntityStateMachine.FindByCustomName(body.gameObject, "Body");
        }
    }

    public override void OnBodyRequirementBroken()
    {
        RoR2Application.onFixedUpdate -= OnFixedUpdate;
        trackedStateMachine = null;
        base.OnBodyRequirementBroken();
    }

    private void OnFixedUpdate()
    {
        var body = this.localUser.cachedBody;
        if (body == null)
        {
            return;
        }
        if (trackedStateMachine == null)
        {
            return;
        }

        if (!achieved && trackedStateMachine.state is EntityStates.FrozenState)
        {
            Grant();
            achieved = true;
        }
    }
}
