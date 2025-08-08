using EntityStates;
using UnityEngine;
using AlisaieMod.Survivors.Alisaie;
using RoR2.Skills;
using AlisaieMod.Characters.Survivors.Alisaie.Components;

namespace AlisaieMod.Survivors.Alisaie.SkillStates
{
    public class Zwerchhau : SwordComboStep
    {
        public override void OnEnter()
        {
            enchantedMultiplier = 1f;
            animName = "s2";
            float baseDmg = AlisaieStaticValues.zwerchhauDamageCoefficient / 3f;
            hitDamages = new float[] { baseDmg, baseDmg, baseDmg };
            swordSFX = AlisaieStaticValues.PlayS2Zwerch;
            base.OnEnter();
        }
    }
}
