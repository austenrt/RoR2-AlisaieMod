using AlisaieMod.Characters.Survivors.Alisaie.Components;
using AlisaieMod.Characters.Survivors.Alisaie.Helpers;
using AlisaieMod.Characters.Survivors.Alisaie.Projectiles;
using AlisaieMod.Characters.Survivors.Alisaie.UI;
using EntityStates;
using RoR2;
using RoR2.Orbs;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AlisaieMod.Survivors.Alisaie.SkillStates
{
    public class Verthunder : BaseSkillState
    {
        public float baseDuration = 1.2f; // Increased to allow endlag speedup
        public float damageCoefficient = AlisaieStaticValues.verthunderDamageCoefficient;
        public GameObject orbPrefab = AlisaieAssets.VFXcastVerthunder;
        public GameObject orbImpactPrefab = AlisaieAssets.VFXimpactVerthunder;
        public string castAnim = "cast_end2";

        private float duration;
        private bool hasFired;
        private AlisaieAutoAim autoAimComponent;
        private bool isCrit;
        private int orbsFired;
        private int totalOrbs;
        private float orbInterval;

        public override void OnEnter()
        {
            base.OnEnter(); 

            

            duration = baseDuration / attackSpeedStat;

            autoAimComponent = GetComponent<AlisaieAutoAim>(); 
            isCrit = characterBody.RollCrit();

            // Try to get a target early
            HurtBox target = autoAimComponent?.GetCurrentTarget();
            if (target && target.healthComponent)
            {
                PlayCrossfade("Gesture, Override", castAnim, 0.05f);
                AkSoundEngine.PostEvent(AlisaieStaticValues.PlayThunderCast, gameObject);
                AlisaieVoiceSFXHelper.PlayRandomBattleVFX(0.4f, gameObject);

                characterBody.SetAimTimer(duration + 1f);
                
                // Calculate orb count and timing
                totalOrbs = Mathf.Max(1, Mathf.FloorToInt(characterBody.level / 2.5f));
                orbInterval = Mathf.Max(0.15f, 0.4f / attackSpeedStat);


                AlisaieVFXHelper.PlayFocusHoldVFX(GetModelTransform().gameObject, "verthunder");
                AlisaieVFXHelper.PlayAuraVFX(GetModelTransform(), "VFXauraVerthunder");
            }
            else
            {
                // No target, add a stock back
                if (skillLocator != null && skillLocator.secondary != null)
                {
                    skillLocator.secondary.AddOneStock();
                }
                outer.SetNextStateToMain();
            }
        }




        private void FireSingleOrb()
        {
            // Get current target (allows retargeting between orbs)
            HurtBox target = autoAimComponent?.GetCurrentTarget();
            if (!target || !target.healthComponent) return;
            
            float orbDamageCoefficient = damageCoefficient;

            GameObject orbHolder = new GameObject("VerthunderOrb");
            orbHolder.transform.position = transform.position;

            var orb = orbHolder.AddComponent<AlisaieJoltDamageOrb>();

            orb.attacker = gameObject;
            orb.target = target;
            orb.origin = transform.position;
            orb.damageValue = orbDamageCoefficient * damageStat;
            orb.isCrit = isCrit;
            orb.teamIndex = TeamComponent.GetObjectTeam(gameObject);
            orb.procCoefficient = 1f;
            orb.damageColorIndex = DamageColorIndex.Void;
            
            // 30% chance to add stun
            orb.stunChance = 0.3f;
            
            orb.followVFXPrefab = orbPrefab;
            orb.impactVFXPrefab = orbImpactPrefab;
            orb.originVFXPrefab = null; // Handle origin VFX manually with EffectManager
            
            // Spawn origin sparking effect with EffectManager
            EffectManager.SpawnEffect(orbImpactPrefab, new EffectData
            {
                origin = transform.position,
                rotation = Quaternion.identity,
                scale = 1f
            }, false);
            orb.scale = 1f;
            orb.impactSfx = AlisaieStaticValues.PlayThunderImpact;
            orb.speed = 140f;
            orb.enableLightningJitter = true;
            orb.jitterIntensity = 1.5f;
            orb.jitterFrequency = 0.03f;

            
            
            // Only consume dualcast and add mana on first orb
            if (orbsFired == 0)
            {
                GetComponent<AlisaieSpellSwitcher>()?.ConsumeDualcast();

                var manaTracker = characterBody?.GetComponent<AlisaieManaTracker>();
                manaTracker?.AddBlackMana(7);
            }
        }




        public override void FixedUpdate()
        {
            base.FixedUpdate();
            
            // Fallback timing for high attack speed (fire at 80% of duration if animation event missed)
            if (!hasFired && fixedAge >= duration * 0.8f && isAuthority)
            {
                OnAnimationCastPoint();
            }
            
            // End early if all orbs fired and past minimum duration
            if (fixedAge >= 0.8f && orbsFired >= totalOrbs && isAuthority)
            {
                outer.SetNextStateToMain();
            }
            else if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }
        
        // Called by Animation Event
        public void OnAnimationCastPoint()
        {
            if (!hasFired)
            {
                hasFired = true; // Set first to prevent double-execution
                FireVerthunderOrb();
                AlisaieVFXHelper.PlayFocusBurstVFX(GetModelTransform().gameObject, "verthunder");
            }
        }
        
        public void FireVerthunderOrb()
        {
            if (orbsFired < totalOrbs)
            {
                FireSingleOrb();
                orbsFired++;
                
                // Schedule next orb if more to fire
                if (orbsFired < totalOrbs)
                {
                    characterBody.StartCoroutine(FireNextOrb());
                }
            }
        }
        
        private System.Collections.IEnumerator FireNextOrb()
        {
            yield return new WaitForSeconds(orbInterval/2f);
            FireVerthunderOrb();
        }

        public override void OnExit()
        {
            AlisaieVFXHelper.SetRealFocusActive(GetModelTransform().gameObject);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

    }

    
    }
