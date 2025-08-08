using EntityStates;
using UnityEngine;
using AlisaieMod.Survivors.Alisaie;
using RoR2.Skills;
using AlisaieMod.Characters.Survivors.Alisaie.Components;

namespace AlisaieMod.Survivors.Alisaie.SkillStates
{
    public class EnchantedRedoublement : SwordComboStep
    {
        public override void OnEnter()
        {
            manaCost = 15;
            animName = "s3";
            enchantedMultiplier = AlisaieStaticValues.enchantedMultiplier;
            swordSFX = AlisaieStaticValues.PlayS3RedoublementEnchShort;
            characterBody.AddTimedBuff(AlisaieBuffs.finisherBuff, 60f); 
            float baseDmg = AlisaieStaticValues.redoublementDamageCoefficient / 6f;
            hitDamages = new float[] { baseDmg, baseDmg, baseDmg, baseDmg, baseDmg, baseDmg };
            base.OnEnter();
        }

    }
}
