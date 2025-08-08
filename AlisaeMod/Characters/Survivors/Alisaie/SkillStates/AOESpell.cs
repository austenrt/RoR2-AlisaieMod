using AlisaieMod.Characters.Survivors.Alisaie.Components;
using AlisaieMod.Characters.Survivors.Alisaie.Helpers;
using AlisaieMod.Characters.Survivors.Alisaie.Projectiles;
using AlisaieMod.Characters.Survivors.Alisaie.UI;
using EntityStates;
using RoR2;
using UnityEngine;

namespace AlisaieMod.Survivors.Alisaie.SkillStates
{
    /**
     * Abstract base class for Alisaie's AOE spells.
     */
    public abstract class AOESpell : BaseSkillState
    {
        protected abstract float BaseDuration { get; }
        protected abstract float DamageCoefficient { get; }
        protected abstract string CastAnim { get; }
        protected abstract string AnimLayer { get; }
        protected abstract GameObject CastVFX { get; }
        protected abstract GameObject AoeVFX { get; }
        protected abstract GameObject ImpactVFX { get; }
        protected abstract string AuraVFXName { get; }
        protected abstract string SpellName { get; }
        protected abstract string CastSFX { get; }
        protected abstract string ImpactSFX { get; }
        protected abstract int ManaAmount { get; }
        protected abstract bool IsWhiteMana { get; }
        protected abstract DamageType ExtraDamageType { get; }

        protected virtual float OrbSpeed => 80f;
        protected virtual float AOERadius => 12f;
        protected virtual float PullForce => 0f;
        protected virtual bool PullEnemies => false;
        protected virtual float ProcCoefficient => 1f;
        protected virtual bool UseAOEHitbox => false;
        protected virtual GameObject AOEHitboxPrefab => null;


        protected float duration;
        protected bool hasFired;
        protected AlisaieAutoAim autoAimComponent;
        protected bool isCrit;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = BaseDuration / attackSpeedStat;
            hasFired = false;
            autoAimComponent = GetComponent<AlisaieAutoAim>();
            isCrit = Util.CheckRoll(characterBody.crit, characterBody.master);
            HurtBox target = autoAimComponent?.GetCurrentTarget();
            if (target && target.healthComponent)
            {
                PlayCrossfade(AnimLayer, CastAnim, 0.05f);
                AkSoundEngine.PostEvent(CastSFX, gameObject);
                AlisaieVoiceSFXHelper.PlayRandomBattleVFX(0.4f, gameObject);
                characterBody.SetAimTimer(duration + 1f);

                var modelTransform = GetModelTransform();
                AlisaieVFXHelper.PlayAuraVFX(modelTransform, AuraVFXName);
                AlisaieVFXHelper.PlayFocusHoldVFX(modelTransform.gameObject, SpellName);
            }
            else
            {
                // No target, add a stock back to the correct skill slot
                if (activatorSkillSlot != null)
                {
                    activatorSkillSlot.AddOneStock();
                }
                outer.SetNextStateToMain();
            }

        }

        protected virtual void FireAOEOrb(Vector3 targetPosition)
        {
            GameObject orbHolder = new GameObject(GetType().Name + "Orb");
            orbHolder.transform.position = transform.position;
            var orb = orbHolder.AddComponent<AlisaieAOEOrb>();
            orb.target = autoAimComponent?.GetCurrentTarget();
            orb.origin = transform.position;
            orb.attacker = gameObject;
            orb.teamIndex = TeamComponent.GetObjectTeam(gameObject);
            orb.damageValue = DamageCoefficient * damageStat;
            orb.isCrit = isCrit;
            orb.procCoefficient = ProcCoefficient;
            orb.damageColorIndex = DamageColorIndex.Default;
            orb.speed = OrbSpeed;
            orb.aoeRadius = AOERadius;
            orb.followVFXPrefab = CastVFX;
            orb.aoeVFXPrefab = AoeVFX;
            orb.impactVFXPrefab = ImpactVFX;
            orb.impactSfx = ImpactSFX;
            orb.extraDamageType = ExtraDamageType;
            orb.pullEnemies = PullEnemies;
            orb.useAOEHitbox = UseAOEHitbox;
            orb.aoeHitboxPrefab = AOEHitboxPrefab;
            OnAOEOrbFire();
            hasFired = true;
        }

        protected virtual void OnAOEOrbFire()
        {
            var manaTracker = characterBody?.GetComponent<AlisaieManaTracker>();
            if (IsWhiteMana)
                manaTracker?.AddWhiteMana(ManaAmount);
            else
                manaTracker?.AddBlackMana(ManaAmount);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!hasFired && fixedAge >= duration * 0.7f && isAuthority)
            {
                OnAnimationCastPoint();
            }
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public void OnAnimationCastPoint()
        {
            if (!hasFired)
            {
                hasFired = true;
                HurtBox target = autoAimComponent?.GetCurrentTarget();
                if (target && target.healthComponent)
                {
                    FireAOEOrb(target.transform.position);
                }
                AlisaieVFXHelper.PlayFocusBurstVFX(GetModelTransform().gameObject, SpellName);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void OnExit()
        {
            //ensure real focus is enabled at end of spell
            AlisaieVFXHelper.SetRealFocusActive(GetModelTransform().gameObject);
            base.OnExit();
        }
    }
}
