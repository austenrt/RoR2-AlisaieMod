using AlisaieMod.Characters.Survivors.Alisaie.Components;
using AlisaieMod.Characters.Survivors.Alisaie.Helpers;
using AlisaieMod.Characters.Survivors.Alisaie.Projectiles;
using AlisaieMod.Characters.Survivors.Alisaie.UI;
using EntityStates;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace AlisaieMod.Survivors.Alisaie.SkillStates
{
    public class Jolt : BaseSkillState
    {
        public float baseTotalCastDuration = 1.0f;
        public float minCastDuration = 0.1f;
        public float startAnimCoeff = 0.2f;
        public float endAnimCoeff = 0.3f;
        public float minChargePercent = 0.1f;

        private float totalCastDuration;
        private float startAnimDuration;
        private float endAnimDuration;
        private float castChargeDuration;

        public float damageCoefficient = AlisaieStaticValues.maxJoltDamageCoefficient;
        public float minDamagePercent = 0.3f;

        public string animStart = "cast_start";
        public string animLoop = "cast_loop";
        public string animEnd = "cast_end1";

        public string vfxAttachName = "focusVFXAnchor";
        public GameObject vfxPrefab = AlisaieAssets.VFXcastJolt;
        public GameObject vfxImpactPrefab = AlisaieAssets.VFXimpactJolt;
        protected GameObject vfxInstance;

        private float stateAge;
        private bool castCompleted;
        protected bool hasFired;
        private bool hasSwitchedToLoop;
        private bool isFullyCharged = false;
        private float fullChargeCoeff = 0.99f;
        private bool shouldApplyDualcast = false;
        private bool hasExited = false;

        private enum JoltPhase { Start, Loop, End }
        private JoltPhase phase;

        private HurtBox currentTarget;
        private AlisaieAutoAim autoAimComponent;
        private Transform focusTransform;
        protected bool isCrit;
        protected float damageToDeal;

        public override void OnEnter()
        {
            base.OnEnter();

            totalCastDuration = Mathf.Max(minCastDuration, baseTotalCastDuration / attackSpeedStat);
            startAnimDuration = totalCastDuration * startAnimCoeff;
            endAnimDuration = totalCastDuration * endAnimCoeff;
            castChargeDuration = totalCastDuration - endAnimDuration;

            phase = JoltPhase.Start;
            stateAge = 0f;
            hasFired = false;
            castCompleted = false;
            hasSwitchedToLoop = false;
            isFullyCharged = false;
            hasExited = false;

            PlayCrossfade("Gesture, Override", animStart, 0.1f);

            AkSoundEngine.PostEvent(AlisaieStaticValues.StopFullcast, gameObject);
            AkSoundEngine.PostEvent(AlisaieStaticValues.PlayFullcast, gameObject);

            autoAimComponent = GetComponent<AlisaieAutoAim>();
            isCrit = Util.CheckRoll(characterBody.crit, characterBody.master);

            var modelTransform = GetModelTransform();
            if (modelTransform)
            {
                var childLocator = modelTransform.GetComponent<ChildLocator>();
                focusTransform = childLocator ? childLocator.FindChild(vfxAttachName) : FindModelChild(vfxAttachName);

                AlisaieVFXHelper.PlayAuraVFX(modelTransform, "VFXauraJolt");
            }

            CreateCastVFX();

            var localUser = LocalUserManager.GetFirstLocalUser();
            if (localUser != null && localUser.cachedBody == characterBody)
            {
                AlisaieCastBarController.instance.gameObject.SetActive(true);
                AlisaieCastBarController.instance.SetFill(0f);
            }

            characterBody.SetAimTimer(totalCastDuration + 1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stateAge += Time.fixedDeltaTime;
            if (!isAuthority || hasExited) return;

            switch (phase)
            {
                case JoltPhase.Start:
                    if (stateAge >= startAnimDuration)
                    {
                        phase = JoltPhase.Loop;
                        Animator animator = GetModelAnimator();
                        if (animator) animator.SetFloat("cast.playbackRate", 1f);
                        PlayCrossfade("Gesture, Override", animLoop, 0.1f);
                    }
                    break;
                case JoltPhase.Loop:
                    float chargeTime = Mathf.Clamp(stateAge - startAnimDuration, 0f, castChargeDuration);
                    float fillRatio = chargeTime / castChargeDuration;
                    var localUser = LocalUserManager.GetFirstLocalUser();
                    if (localUser != null && localUser.cachedBody == characterBody)
                    {
                        AlisaieCastBarController.instance?.SetFill(fillRatio);
                    }
                    isFullyCharged = fillRatio >= fullChargeCoeff;
                    if (vfxInstance)
                    {
                        float scale = Mathf.Lerp(0.01f, 1f, fillRatio);
                        vfxInstance.transform.localScale = Vector3.one * scale;
                    }
                    if (!castCompleted && (!inputBank.skill1.down || isFullyCharged))
                    {
                        BeginCastEnd(chargeTime);
                        phase = JoltPhase.End;
                    }
                    break;
                case JoltPhase.End:
                    if (stateAge >= endAnimDuration)
                    {
                        hasExited = true;
                        outer.SetNextStateToMain();
                    }
                    break;
            }
        }

        private void BeginCastEnd(float chargeTime)
        {
            castCompleted = true;
            stateAge = 0f;
            Animator animator = GetModelAnimator();
            PlayCrossfade("Gesture, Override", animEnd, 0.05f);
            float chargeRatio = Mathf.Clamp01(chargeTime / castChargeDuration);
            if (chargeRatio >= fullChargeCoeff)
            {
                damageToDeal = damageStat * damageCoefficient;
                shouldApplyDualcast = true;
            }
            else
            {
                float scaledDamagePercent = Mathf.Lerp(minDamagePercent, 1f, chargeRatio);
                damageToDeal = damageStat * damageCoefficient * scaledDamagePercent;
                shouldApplyDualcast = false;
            }
        }

        protected virtual void FireOrb()
        {
            if (!autoAimComponent)
            {
                if (vfxInstance)
                {
                    AlisaieVFXHelper.StopAndDestroy(vfxInstance);
                }
                return;
            }

            currentTarget = autoAimComponent.GetCurrentTarget();

            if (!currentTarget || !currentTarget.healthComponent)
            {
                if (vfxInstance)
                {
                    AlisaieVFXHelper.StopAndDestroy(vfxInstance);
                }
                return;
            }

            float finalVFXScale = vfxInstance ? vfxInstance.transform.localScale.x : 1f;

            if (vfxInstance)
            {
                AlisaieVFXHelper.StopAndDestroy(vfxInstance);
                vfxInstance = null;
            }

            GameObject newOrbGameObject = new GameObject("AlisaieJoltOrb");
            newOrbGameObject.transform.position = transform.position;
            AlisaieJoltDamageOrb orb = newOrbGameObject.AddComponent<AlisaieJoltDamageOrb>();
            orb.target = currentTarget;
            orb.attacker = gameObject;
            orb.origin = newOrbGameObject.transform.position;
            orb.damageValue = damageToDeal;
            orb.isCrit = isCrit;
            orb.teamIndex = TeamComponent.GetObjectTeam(gameObject);
            orb.procCoefficient = 1f;
            orb.damageColorIndex = DamageColorIndex.Default;
            orb.followVFXPrefab = vfxPrefab;
            orb.impactVFXPrefab = vfxImpactPrefab;
            orb.scale = finalVFXScale;
            orb.delayBeforeFiring = 0f;
            orb.enableArc = true;
            orb.arcHeight = 3f;

            AkSoundEngine.PostEvent(AlisaieStaticValues.PlayCastJolt, gameObject);
            AlisaieVoiceSFXHelper.PlayRandomBattleVFX(0.3f, gameObject);

            var manaTracker = characterBody?.GetComponent<AlisaieManaTracker>();
            manaTracker?.AddWhiteMana(2);
            manaTracker?.AddBlackMana(2);

            hasFired = true;
        }

        private void CreateCastVFX()
        {
            if (!focusTransform || !vfxPrefab) return;

            vfxInstance = Object.Instantiate(vfxPrefab, focusTransform.position, focusTransform.rotation, focusTransform);
            if (vfxInstance)
            {
                vfxInstance.transform.localScale = Vector3.one * 0.01f;

                ChildLocator childLocator = vfxInstance.GetComponent<ChildLocator>();
                if (childLocator)
                {
                    Transform tailChild = childLocator.FindChild("tail");
                    if (tailChild)
                    {
                        Object.Destroy(tailChild.gameObject);
                    }
                }

                foreach (var ps in vfxInstance.GetComponentsInChildren<ParticleSystem>())
                    ps.Play();
            }
        }

        private string GetCurrentAnimationName()
        {
            var modelAnimator = GetModelAnimator();
            if (modelAnimator != null)
            {
                var clipInfo = modelAnimator.GetCurrentAnimatorClipInfo(0);
                if (clipInfo.Length > 0)
                    return clipInfo[0].clip.name;
            }
            return string.Empty;
        }

        public override void OnExit()
        {
            isFullyCharged = false;
            var localUser = LocalUserManager.GetFirstLocalUser();
            if (localUser != null && localUser.cachedBody == characterBody)
            {
                AlisaieCastBarController.instance.SetFill(0f);
                AlisaieCastBarController.instance.gameObject.SetActive(false);
            }
            if (vfxInstance)
            {
                AlisaieVFXHelper.StopAndDestroy(vfxInstance);
                vfxInstance = null;
            }
            if (shouldApplyDualcast && characterBody)
            {
                AlisaieBuffHelper.AddBuff(characterBody, gameObject, AlisaieBuffs.dualCastBuff, true, 10f, applyDelay: true);
            }
            var modelTransform = GetModelTransform();
            AlisaieVFXHelper.SetRealFocusActive(modelTransform.gameObject);

            base.OnExit();
        }

        public void OnAnimationCastPoint()
        {
            if (!hasFired)
            {
                FireOrb();
                hasFired = true;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
