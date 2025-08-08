using EntityStates;
using UnityEngine;
using AlisaieMod.Survivors.Alisaie;
using RoR2.Skills;
using AlisaieMod.Characters.Survivors.Alisaie.Components;

namespace AlisaieMod.Survivors.Alisaie.SkillStates
{
    public class EnchantedZwerchhau : SwordComboStep
    {
        public override void OnEnter()
        {
            manaCost = 15;
            enchantedMultiplier = AlisaieStaticValues.enchantedMultiplier;
            swordSFX = AlisaieStaticValues.PlayS2ZwerchEnch;
            animName = "s2";
            float baseDmg = AlisaieStaticValues.zwerchhauDamageCoefficient / 3f;
            hitDamages = new float[] { baseDmg, baseDmg, baseDmg };
            base.OnEnter();
        }

    }
}
