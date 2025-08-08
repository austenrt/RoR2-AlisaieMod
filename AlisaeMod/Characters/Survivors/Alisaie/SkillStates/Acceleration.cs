using AlisaieMod.Characters.Survivors.Alisaie.Helpers;
using AlisaieMod.Characters.Survivors.Alisaie.UI; // Assuming this might be needed for UI interactions, though not directly used in this basic version
using AlisaieMod.Survivors.Alisaie; // For AlisaieStaticValues
using AlisaieMod.Characters.Survivors.Alisaie.Components;
using EntityStates;
using RoR2;
using UnityEngine;

namespace AlisaieMod.Survivors.Alisaie.SkillStates
{
    public class Acceleration : BaseSkillState
    {
        public float baseDuration = 0.2f;

        public string animString = "cast_acceleration";

        public float buffDuration = 10f;

        public override void OnEnter()
        {
            base.OnEnter();


            PlayCrossfade("FullBody, Override", animString, 0.1f);

            AkSoundEngine.PostEvent(AlisaieStaticValues.PlayAcceleration, gameObject);

            if (characterBody != null)
            {
                if (AlisaieBuffs.dualCastBuff != null)
                {
                    AlisaieBuffHelper.AddBuff(characterBody, gameObject, AlisaieBuffs.dualCastBuff, true, buffDuration);
                }
                
                if (AlisaieBuffs.accelerationBuff != null)
                {
                    AlisaieBuffHelper.AddBuff(characterBody, gameObject, AlisaieBuffs.accelerationBuff, true, buffDuration);
                }
                
                var spellSwitcher = GetComponent<AlisaieSpellSwitcher>();
                spellSwitcher?.SetAccelerationDualcast();
            }
            AlisaieVFXHelper.PlayAuraVFX(GetModelTransform(), "VFXauraAcceleration");
            characterBody?.SetAimTimer(baseDuration + 0.5f); //aimbuffer
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!isAuthority) return;

            if (fixedAge >= baseDuration)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
