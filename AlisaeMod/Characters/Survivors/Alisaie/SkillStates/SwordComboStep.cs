using AlisaieMod.Characters.Survivors.Alisaie.Components;
using AlisaieMod.Modules.BaseStates;
using RoR2;
using UnityEngine;
using AlisaieMod.Survivors.Alisaie; // For AlisaieStaticValues
using AlisaieMod.Characters.Survivors.Alisaie.Helpers;
using AlisaieMod.Characters.Survivors.Alisaie.UI;
using RoR2BepInExPack.GameAssetPaths;
using RoR2.Skills;

namespace AlisaieMod.Survivors.Alisaie.SkillStates
{
    public abstract class SwordComboStep : BaseMeleeAttack
    {
        public int stepIndex; // 0 = Riposte, 1 = Zwerchhau, 2 = Redoublement
        //most of these protected fields are immplemented in derived classes
        protected float[] hitDamages;
        protected string animName;
        protected float enchantedMultiplier = 1f;
        protected string swordSFX;
        protected float swordSFXDelay = 0f;
        protected int currentHitIndex;
        protected bool hitboxActive;
        protected int manaCost = 0;
        protected System.Collections.Generic.HashSet<HealthComponent> hitEnemiesThisActivation = new System.Collections.Generic.HashSet<HealthComponent>();

        public override void OnEnter()
        {
            useAnimationEventHitbox = true;
            base.OnEnter();
            currentHitIndex = 0; 
            hitboxActive = false;
            hitEnemiesThisActivation.Clear();
            duration = baseDuration;
            if (!string.IsNullOrEmpty(animName))
            {
                PlayCrossfade("FullBody, Override", animName, "attackSpeedStat", baseDuration / attackSpeedStat, 0.1f);
            }
            PlayComboSound();
            if (characterMotor && characterDirection)
            {
                Vector3 forwardBoost = characterDirection.forward * 2f;
                characterMotor.velocity += forwardBoost;
            }
            var manaTracker = characterBody?.GetComponent<AlisaieManaTracker>();
            manaTracker?.SubtractBlackMana(manaCost);
            manaTracker?.SubtractWhiteMana(manaCost);
        }

        protected virtual void PlayComboSound()
        {
            if (!string.IsNullOrEmpty(swordSFX))
            {
                if (swordSFXDelay > 0f)
                    characterBody?.StartCoroutine(PlayDelayedSound(swordSFX, swordSFXDelay));
                else
                    AkSoundEngine.PostEvent(swordSFX, gameObject);
                    AlisaieVoiceSFXHelper.PlayRandomBattleVFX(0.5f, gameObject);
            }
        }

        protected System.Collections.IEnumerator PlayDelayedSound(string soundEvent, float delay)
        {
            yield return new WaitForSeconds(delay);
            AkSoundEngine.PostEvent(soundEvent, gameObject);
            AlisaieVoiceSFXHelper.PlayRandomBattleVFX(0.5f, gameObject);
        }

        public void OnAnimationActivateHitbox()
        {
            if (!hitboxActive)
            {
                hitboxActive = true;
                hitEnemiesThisActivation.Clear();
                FireHit(currentHitIndex);
                if (currentHitIndex < hitDamages.Length - 1)
                {
                    currentHitIndex++;
                }
            }
        }

        public void OnAnimationDeactivateHitbox()
        {
            hitboxActive = false;
            hitEnemiesThisActivation.Clear();
        }

        protected virtual void FireHit(int hitIndex)
        {
            if (!isAuthority) return;
            bool crit = characterBody.RollCrit();
            float hitDamage = hitDamages[hitIndex] * enchantedMultiplier;

            var attack = new OverlapAttack();
            attack.attacker = gameObject;
            attack.damage = hitDamage * damageStat;
            attack.damageColorIndex = DamageColorIndex.Default;
            attack.damageType = DamageType.Generic;
            attack.hitBoxGroup = FindHitBoxGroup("SwordGroup");
            attack.hitEffectPrefab = null;
            attack.inflictor = gameObject;
            attack.isCrit = crit;
            attack.procCoefficient = procCoefficient;
            attack.pushAwayForce = pushForce;
            attack.teamIndex = GetTeam();

            var hitResults = new System.Collections.Generic.List<HurtBox>();
            if (attack.Fire(hitResults) && hitResults.Count > 0)
            {
                var filteredResults = new System.Collections.Generic.List<HurtBox>();
                foreach (var hurtBox in hitResults)
                {
                    if (hurtBox?.healthComponent && hitEnemiesThisActivation.Add(hurtBox.healthComponent))
                    {
                        filteredResults.Add(hurtBox);
                    }
                }
                hitResults = filteredResults;
            }

            if (hitResults.Count > 0)
            {
                OnHitEnemyAuthority();
                AddRecoil(-0.5f * enchantedMultiplier, 0.5f * enchantedMultiplier, -0.3f * enchantedMultiplier, 0.3f * enchantedMultiplier);
                foreach (var hurtBox in hitResults)
                {
                    if (hurtBox && hurtBox.transform && AlisaieAssets.VFXimpactSword)
                    {
                        EffectManager.SpawnEffect(AlisaieAssets.VFXimpactSword, new EffectData
                        {
                            origin = hurtBox.transform.position,
                            rotation = Quaternion.LookRotation((hurtBox.transform.position - transform.position).normalized),
                            scale = enchantedMultiplier
                        }, true);
                    }
                }
            }
        }

        public void SetStep(int i)
        {
            stepIndex = i;
        }

        public override void OnExit()
        {
            base.OnExit();
            var model = characterBody?.modelLocator?.modelTransform;
            if (model)
            {
                var animEventHandler = model.GetComponent<AlisaieAnimationEventHandler>();
                animEventHandler?.CleanupSwordEvents();
            }
        }
    }
}
