using EntityStates;
using UnityEngine;
using AlisaieMod.Survivors.Alisaie;
using RoR2.Skills;
using AlisaieMod.Characters.Survivors.Alisaie.Components;

namespace AlisaieMod.Survivors.Alisaie.SkillStates
{
    public class Redoublement : SwordComboStep
    {
        public override void OnEnter()
        {
            animName = "s3";
            enchantedMultiplier = 1f;
            float baseDmg = AlisaieStaticValues.redoublementDamageCoefficient / 6f;
            hitDamages = new float[] { baseDmg, baseDmg, baseDmg, baseDmg, baseDmg, baseDmg };
            swordSFX = AlisaieStaticValues.PlayS3RedoublementShort;
            base.OnEnter();
        }
    }
}
