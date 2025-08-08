using UnityEngine;
using RoR2;
using RoR2.Achievements;
using AlisaieMod.Survivors.Alisaie;
using AlisaieMod.Characters.Survivors.Alisaie.Components;

[RegisterAchievement(identifier, unlockableIdentifier, null, 5, null)]
public class AlisaieSummerSkinAchievement : BaseAchievement
{
    public const string identifier = AlisaieSurvivor.ALISAIE_PREFIX + "summerSkinAchievement";
    public const string unlockableIdentifier = AlisaieSurvivor.ALISAIE_PREFIX + "summerSkinUnlockable";

    public override BodyIndex LookUpRequiredBodyIndex() => BodyCatalog.FindBodyIndex(AlisaieSurvivor.instance.bodyName);

    private bool manaRequirementMet = true;
    private AlisaieManaTracker manaTracker;

    public override void OnInstall()
    {
        base.OnInstall();
        RoR2.TeleporterInteraction.onTeleporterChargedGlobal += OnTeleporterChargedGlobal;
        RoR2.CharacterBody.onBodyStartGlobal += OnBodyStartGlobal;
    }

    public override void OnUninstall()
    {
        RoR2.TeleporterInteraction.onTeleporterChargedGlobal -= OnTeleporterChargedGlobal;
        RoR2.CharacterBody.onBodyStartGlobal -= OnBodyStartGlobal;
        if (manaTracker != null)
            manaTracker.OnManaChanged -= OnManaChanged;
        base.OnUninstall();
    }

    private void OnBodyStartGlobal(CharacterBody body)
    {
        if (body == this.localUser.cachedBody)
        {
            manaRequirementMet = true;
            manaTracker = body.GetComponent<AlisaieManaTracker>();
            if (manaTracker != null)
                manaTracker.OnManaChanged += OnManaChanged;
        }
    }

    private void OnManaChanged()
    {
        if (manaTracker != null && (manaTracker.WhiteMana < 50 || manaTracker.BlackMana < 50))
        {
            manaRequirementMet = false;
        }
    }

    private void OnTeleporterChargedGlobal(TeleporterInteraction tp)
    {
        if (manaRequirementMet)
        {
            Grant();
        }
    }
}
