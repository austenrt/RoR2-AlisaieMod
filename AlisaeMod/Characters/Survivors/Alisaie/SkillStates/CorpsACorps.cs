using AlisaieMod.Characters.Survivors.Alisaie.Helpers;
using AlisaieMod.Characters.Survivors.Alisaie.Components;
using AlisaieMod.Modules.BaseStates;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace AlisaieMod.Survivors.Alisaie.SkillStates
{
    public class CorpsACorps : BaseMeleeAttack
    {
        private const float DASH_SPEED = 120f;
        private const float DASH_DURATION = 0.3f;

        private enum Phase { Start, Dash, End }
        private Phase currentPhase = Phase.Start;
        private float phaseTimer;
        private Vector3 dashDirection;
        private bool hasHitEnemy;
        private GameObject corpsGlowVFXInstance;

        public override void OnEnter()
        {
            hitboxGroupName = "SwordGroup";
            damageType = DamageType.Generic;
            damageCoefficient = AlisaieStaticValues.corpsDamageCoefficient;
            procCoefficient = 1f;
            pushForce = 0f; // Prevent extra force on hit
            bonusForce = Vector3.zero;
            baseDuration = DASH_DURATION + 0.05f; // for skill system
            attackStartPercentTime = 0.1f;
            attackEndPercentTime = 0.99f;
            hitStopDuration = 0.015f;
            attackRecoil = 0.5f;
            hitHopVelocity = 0f; // Prevent hop on hit
            swingSoundString = "HenryRoll";
            hitSoundString = "";
            muzzleString = "SwingForward";
            playbackRateParam = "Corps.playbackRate";
            swingEffectPrefab = AlisaieAssets.swordSwingEffect;
            hitEffectPrefab = null;
            impactSound = AlisaieAssets.swordHitSoundEvent.index;

            AlisaieVoiceSFXHelper.PlayVFX(0.05f, "Play_VFXEnGarde", gameObject);
            AkSoundEngine.PostEvent(AlisaieStaticValues.PlayCorps, gameObject);

            if (isAuthority && inputBank)
                dashDirection = inputBank.aimDirection;

            base.OnEnter();
            PlayStartAnimation();
            SetCameraFOV(75f);
            EnableGlowVFX();
            phaseTimer = 0f;
            currentPhase = Phase.Start;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            // Zero movement after hitstop if we've hit an enemy
            if (hasHitEnemy && !inHitPause && characterMotor != null) 
            {
                characterMotor.velocity = Vector3.zero;
                characterMotor.rootMotion = Vector3.zero;
                characterMotor.moveDirection = Vector3.zero;
                characterMotor.Motor.BaseVelocity = Vector3.zero;
            }
            phaseTimer += Time.fixedDeltaTime;
            switch (currentPhase)
            {
                case Phase.Start:
                    TransitionToDash();
                    break;
                case Phase.Dash:
                    if (inHitPause) break;
                    if (phaseTimer >= DASH_DURATION)
                    {
                        StopDashMovement();
                        outer.SetNextStateToMain();
                    }
                    else
                    {
                        UpdateDashMovement();
                    }
                    break;
                case Phase.End:
                    outer.SetNextStateToMain();
                    break;
            }
            UpdateCameraFOV();
        }

        private void PlayStartAnimation()
        {
            PlayCrossfade("FullBody, Override", "Corps-Start", playbackRateParam, 0.15f, 0.05f);
        }

        private void TransitionToDash()
        {
            currentPhase = Phase.Dash;
            phaseTimer = 0f;
            PlayDashAnimation();
            StartDashMovement();
            ApplyDashBuffs();
        }

        private void PlayDashAnimation()
        {
            PlayCrossfade("FullBody, Override", "Corps-Dash", playbackRateParam, DASH_DURATION, 0.05f);
        }

        private void PlayEndAnimation()
        {
            PlayCrossfade("FullBody, Override", "Corps-End", playbackRateParam, 0.05f, 0.05f);
        }

        private void StartDashMovement()
        {
            if (characterMotor && dashDirection != Vector3.zero)
            {
                float scaledDashSpeed = DASH_SPEED * (characterBody.moveSpeed / characterBody.baseMoveSpeed);
                characterMotor.velocity = dashDirection.normalized * scaledDashSpeed;
            }
        }

        private void UpdateDashMovement()
        {
            if (hasHitEnemy || !characterMotor || dashDirection == Vector3.zero) return;
            var autoAim = GetComponent<AlisaieAutoAim>();
            var target = autoAim?.GetCurrentTarget();
            Vector3 direction = dashDirection;
            if (target && target.transform)
            {
                Vector3 toTarget = (target.transform.position - transform.position).normalized;
                float curveStrength = Mathf.Clamp01(phaseTimer / DASH_DURATION);
                direction = Vector3.Slerp(dashDirection, toTarget, curveStrength * 0.8f).normalized;
            }
            float scaledDashSpeed = DASH_SPEED * (characterBody.moveSpeed / characterBody.baseMoveSpeed);
            characterMotor.velocity = direction * scaledDashSpeed;
            if (characterDirection && direction != Vector3.zero)
                characterDirection.forward = direction;
        }

        private void StopDashMovement()
        {
            if (characterMotor)
            {
                characterMotor.velocity = Vector3.zero;
                characterMotor.rootMotion = Vector3.zero;
            }
        }

        private void ApplyDashBuffs()
        {
            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(AlisaieBuffs.armorBuff, 1.5f);
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 0.5f);
            }
        }

        private void SetCameraFOV(float fov)
        {
            if (cameraTargetParams)
                cameraTargetParams.fovOverride = fov;
        }

        private void UpdateCameraFOV()
        {
            if (cameraTargetParams)
            {
                float ageRatio = phaseTimer / (DASH_DURATION + 0.2f);
                cameraTargetParams.fovOverride = Mathf.Lerp(75f, 60f, ageRatio);
            }
        }

        private void EnableGlowVFX()
        {
            var modelTransform = GetModelTransform();
            if (modelTransform)
            {
                var childLocator = modelTransform.GetComponent<ChildLocator>();
                var glowTransform = childLocator ? childLocator.FindChild("VFXcorpsGlow") : null;
                if (glowTransform)
                {
                    corpsGlowVFXInstance = glowTransform.gameObject;
                    corpsGlowVFXInstance.SetActive(true);
                }
            }
        }

        protected override void RemoveHitstop()
        {
            base.RemoveHitstop();
            if (characterMotor)
            {
                characterMotor.velocity = Vector3.zero;
                characterMotor.rootMotion = Vector3.zero;
                characterMotor.moveDirection = Vector3.zero;
                characterMotor.Motor.BaseVelocity = Vector3.zero;
            }
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
            hasHitEnemy = true;
            storedVelocity = Vector3.zero; // Ensure hitstop restores zero velocity
            StopDashMovement();
            if (characterMotor)
            {
                characterMotor.velocity = Vector3.zero;
                characterMotor.rootMotion = Vector3.zero;
                characterMotor.useGravity = true;
                characterMotor.moveDirection = Vector3.zero;
                characterMotor.Motor.ForceUnground();
                characterMotor.Motor.BaseVelocity = Vector3.zero;
            }
            var rb = characterBody?.rigidbody;
            if (rb != null)
                rb.velocity = Vector3.zero;
            PlayEndAnimation();
            Transform swordTransform = FindModelChild("Sword");
            if (swordTransform && AlisaieAssets.VFXimpactSword)
            {
                EffectManager.SpawnEffect(AlisaieAssets.VFXimpactSword, new EffectData
                {
                    origin = swordTransform.position,
                    rotation = swordTransform.rotation
                }, true);
            }
            currentPhase = Phase.End;
            phaseTimer = 0f;
        }

        public override void OnExit()
        {
            SetCameraFOV(-1f);
            StopDashMovement();
            if (corpsGlowVFXInstance)
                corpsGlowVFXInstance.SetActive(false);
            characterBody.modelLocator.modelTransform.GetComponent<AlisaieAnimationEventHandler>().CleanupSwordEvents();
            base.OnExit();
        }
    }
}
