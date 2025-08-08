using RoR2;
using UnityEngine;
using System.Collections.Generic;

namespace AlisaieMod.Characters.Survivors.Alisaie.Projectiles
{
    public class AlisaieAOEOrb : AlisaieJoltDamageOrb
    {
        [Header("AOE Settings")]

        public float aoeRadius = 12f;
        public GameObject aoeVFXPrefab;
        public GameObject aoeHitboxPrefab; // Prefab with HitBoxGroup for AOE
        public bool pullEnemies = false; // If true, pull enemies to center
        public bool useAOEHitbox = false;
        
        // Override the OnHit method to fire AOE blast instead of single target damage
        protected override void OnHit()
        {
            if (hasHit) return;
            hasHit = true;

            CleanupVFX();

            if (target && target.transform)
            {
                Vector3 impactPosition = target.transform.position;
                
                // Play impact sound
                if (!string.IsNullOrEmpty(impactSfx))
                    AkSoundEngine.PostEvent(impactSfx, target.gameObject);

                // Spawn Aoe VFX
                if (aoeVFXPrefab)
                {
                    EffectManager.SpawnEffect(aoeVFXPrefab, new EffectData
                    {
                        origin = impactPosition,
                        rotation = Quaternion.identity,
                        scale = 1f
                    }, true);
                }
                
                // Fire AOE blast at impact location
                FireAOEBlast(impactPosition);
            }

            if (lingerTime > 0f)
            {
                StartCoroutine(DestroyAfterLinger());
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void FireAOEBlast(Vector3 position)
        {
            if (useAOEHitbox && aoeHitboxPrefab)
            {
                var hitboxInstance = Instantiate(aoeHitboxPrefab, position, Quaternion.identity);
                var hitboxGroup = hitboxInstance.GetComponentInChildren<HitBoxGroup>();
                if (hitboxGroup != null)
                {
                    var overlapAttack = new OverlapAttack();
                    overlapAttack.attacker = attacker;
                    overlapAttack.inflictor = gameObject;
                    overlapAttack.teamIndex = teamIndex;
                    overlapAttack.damage = damageValue;
                    overlapAttack.isCrit = isCrit;
                    overlapAttack.procCoefficient = procCoefficient;
                    overlapAttack.damageColorIndex = damageColorIndex;
                    overlapAttack.damageType = DamageType.Generic;
                    overlapAttack.hitBoxGroup = hitboxGroup;
                    overlapAttack.hitEffectPrefab = impactVFXPrefab;
                    overlapAttack.Fire();
                }
                Destroy(hitboxInstance);
                return;
            }
            // Sphere AOE logic
            var blast = new BlastAttack();
            blast.attacker = attacker;
            blast.baseDamage = damageValue;
            blast.baseForce = 0f; // We'll handle pull separately
            blast.bonusForce = Vector3.zero;
            blast.crit = isCrit;
            blast.damageColorIndex = damageColorIndex;
            blast.damageType = DamageType.Generic;
            blast.falloffModel = BlastAttack.FalloffModel.None;
            blast.inflictor = attacker;
            blast.position = position;
            blast.procCoefficient = procCoefficient;
            blast.radius = aoeRadius;
            blast.teamIndex = teamIndex;
            var result = blast.Fire();
            if (result.hitCount > 0)
            {
                foreach (var hitPoint in result.hitPoints)
                {
                    if (impactVFXPrefab && hitPoint.hurtBox != null)
                    {
                        EffectManager.SpawnEffect(impactVFXPrefab, new EffectData
                        {
                            origin = hitPoint.hurtBox.transform.position,
                            rotation = Quaternion.identity,
                            scale = 1f
                        }, true);
                    }
                    if (hitPoint.hurtBox?.healthComponent?.body != null)
                    {
                        var body = hitPoint.hurtBox.healthComponent.body;
                        if (pullEnemies)
                        {
                            bool isBoss = body.isBoss;
                            bool isBig = body.characterMotor && body.characterMotor.mass > 300f;
                            if (!isBoss && !isBig)
                            {
                                Vector3 startPos = body.transform.position;
                                Vector3 targetPos = position;
                                body.StartCoroutine(SmoothPullToCenter(body, startPos, targetPos, 0.3f));
                            }
                        }
                    }
                }
            }
        }
        
        private System.Collections.IEnumerator DestroyAfterLinger()
        {
            yield return new WaitForSeconds(lingerTime);
            Destroy(gameObject);
        }

        // coroutine for smooth enemy pull movement
        private System.Collections.IEnumerator SmoothPullToCenter(CharacterBody body, Vector3 startPos, Vector3 targetPos, float duration)
        {
            float elapsed = 0f;
            var motor = body.GetComponent<CharacterMotor>();
            while (elapsed < duration && body)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Vector3 newPos = Vector3.Lerp(startPos, targetPos, t);
                if (motor)
                {
                    motor.velocity = (targetPos - body.transform.position) / (duration - elapsed + 0.01f);
                }
                else
                {
                    body.transform.position = newPos;
                }
                yield return null;
            }
            // Snap to center at the end
            if (motor)
            {
                motor.velocity = Vector3.zero;
            }
            body.transform.position = targetPos;
        }
    }
}