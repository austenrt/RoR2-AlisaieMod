using EntityStates;
using UnityEngine;
using AlisaieMod.Survivors.Alisaie;
using RoR2.Skills;
using AlisaieMod.Characters.Survivors.Alisaie.Components;

namespace AlisaieMod.Survivors.Alisaie.SkillStates
{
    public class EnchantedRiposte : SwordComboStep
    {
        public override void OnEnter()
        {
            manaCost = 20;
            enchantedMultiplier = AlisaieStaticValues.enchantedMultiplier;
            swordSFX = AlisaieStaticValues.PlayS1RiposteEnch; 
            animName = "s1";
            float baseDmg = AlisaieStaticValues.riposteDamageCoefficient / 2f;
            hitDamages = new float[] { baseDmg, baseDmg };
            //swordSFXDelay = 0.3f;
            base.OnEnter();
        }

    }
}
