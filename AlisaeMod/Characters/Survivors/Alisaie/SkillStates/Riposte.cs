using EntityStates;
using UnityEngine;
using AlisaieMod.Survivors.Alisaie;
using RoR2.Skills;
using AlisaieMod.Characters.Survivors.Alisaie.Components;

namespace AlisaieMod.Survivors.Alisaie.SkillStates
{
    public class Riposte : SwordComboStep
    {
        public override void OnEnter()
        {
            
            enchantedMultiplier = 1f; 
            animName = "s1";
            float baseDmg = AlisaieStaticValues.riposteDamageCoefficient / 2f;
            hitDamages = new float[] { baseDmg, baseDmg };
            swordSFX = AlisaieStaticValues.PlayS1Riposte;
            swordSFXDelay = 0.3f;
            base.OnEnter();

        }
    }
}
