using AlisaieMod.Characters.Survivors.Alisaie.Helpers;
using RoR2;
using UnityEngine;

namespace AlisaieMod.Characters.Survivors.Alisaie.Projectiles
{
    /**
     * A projectile component for Alisaie's spells.
     * 
     * This orb handles the movement, collision detection, and damage application
     * for Jolt, Verthunder, and other spells. It supports both straight-line
     * and arcing movement patterns, and manages VFX trails that persist briefly after impact.
     * 
     * I created this custom orb because the default orb system in RoR2 caused the trails to disappear instantly and
     * it looked bad for my spell VFX.
     *
     * 
     */
    public class AlisaieJoltDamageOrb : MonoBehaviour
    {
        [Header("Target & Motion")]
        public HurtBox target;
        public float speed = 80f;
        public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public float delayBeforeFiring = 0f;

        [Header("Damage")]
        public float damageValue;
        public float procCoefficient = 1f;
        public DamageColorIndex damageColorIndex = DamageColorIndex.Default;
        public bool isCrit;
        public GameObject attacker;
        public TeamIndex teamIndex;
        public float stunChance = 0f;
        public DamageType extraDamageType = DamageType.Generic;

        [Header("Visuals")]
        public GameObject originVFXPrefab;
        public GameObject followVFXPrefab;
        public GameObject impactVFXPrefab;
        public float scale = 1f;
        public float lingerTime = 0f;
        public string impactSfx = "Play_jolt_impact";


        protected GameObject vfxInstance;
        public Vector3 origin;

        [Tooltip("Enable parabolic arc in movement.")]
        public bool enableArc = false;
        public float arcHeight = 3f;
        public bool useRandomArc = false;

        [Tooltip("Enable lightning jitter effect.")]
        public bool enableLightningJitter = false;
        public float jitterIntensity = 0.5f;
        public float jitterFrequency = 0.1f; // How often to jitter (lower = less frequent)

        private float lastJitterTime;
        private Vector3 currentJitterOffset;
        private Vector3 lastJitterPosition;

        public float maxLifetime = 5f;
        private float lifetime;

        private float distanceToTarget;
        private float duration;
        private float elapsed;
        private float delayTimer;
        protected bool hasHit;
        private bool hasStartedMoving;

        void Start()
        {
            if (!target || !target.transform)
            {
                Destroy(gameObject);
                return;
            }

            origin = transform.position;
            distanceToTarget = Vector3.Distance(origin, target.transform.position);
            duration = distanceToTarget / speed;
            delayTimer = delayBeforeFiring;
            hasStartedMoving = delayBeforeFiring <= 0f;

            if (originVFXPrefab)
            {
                GameObject castEffect = Instantiate(originVFXPrefab, origin, Quaternion.identity);
                Destroy(castEffect, 2f);
            }

            if (followVFXPrefab)
            {
                vfxInstance = Instantiate(followVFXPrefab, transform.position, Quaternion.identity);
                vfxInstance.transform.localScale *= scale;
                vfxInstance.transform.parent = transform;
            }
        }

        void Update()
        {
            lifetime += Time.deltaTime;
            if (lifetime > maxLifetime)
            {
                OnFail();
                return;
            }

            if (hasHit || target == null || target.transform == null)
                return;

            if (target.healthComponent == null || !target.healthComponent.alive)
            {
                OnFail();
                return;
            }

            if (!hasStartedMoving)
            {
                delayTimer -= Time.deltaTime;
                if (delayTimer <= 0f)
                {
                    hasStartedMoving = true;
                    elapsed = 0f;
                }
                else
                {
                    return;
                }
            }

            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            Vector3 nextPosition;
            if (enableArc)
            {
                nextPosition = AlisaieVFXHelper.GetCombinedArcPosition(
                    origin,
                    target.transform.position,
                    t,
                    sideArcWidth: 2f,
                    verticalArcHeight: arcHeight,
                    useRandomArc: useRandomArc);
            }
            else
            {
                float curvedT = movementCurve.Evaluate(t);
                nextPosition = Vector3.Lerp(origin, target.transform.position, curvedT);
            }

            // Add lightning jitter effect - dramatic but infrequent
            if (enableLightningJitter)
            {
                float timeRemaining = duration - elapsed;

                // First jitter period: straight line from origin
                if (elapsed <= jitterFrequency)
                {
                    // No jitter, straight line
                }
                // Final jitter period: straight line to target
                else if (timeRemaining <= jitterFrequency)
                {
                    // No jitter, straight to target
                }
                // Middle periods: random jitter
                else
                {
                    // Only update jitter occasionally for dramatic fork effect
                    if (Time.time - lastJitterTime > jitterFrequency)
                    {
                        // Random movement: sometimes towards target, sometimes random direction
                        if (UnityEngine.Random.Range(0f, 1f) < 0.5f)
                        {
                            // Move towards target
                            Vector3 toTarget = (target.transform.position - transform.position).normalized;
                            currentJitterOffset = toTarget * jitterIntensity * UnityEngine.Random.Range(0.3f, 1f);
                        }
                        else
                        {
                            // Random direction
                            currentJitterOffset = FindValidJitterOffset(nextPosition);
                        }
                        lastJitterTime = Time.time;
                    }
                    nextPosition += currentJitterOffset;
                }
            }

            // Rotate VFX to face movement direction
            Vector3 movementDirection = (nextPosition - transform.position).normalized;
            if (movementDirection != Vector3.zero && vfxInstance)
            {
                vfxInstance.transform.rotation = Quaternion.LookRotation(movementDirection);
            }

            transform.position = nextPosition;

            if (t >= 1f)
            {
                OnHit();
            }
        }

        protected virtual void OnHit()
        {
            if (hasHit) return;
            hasHit = true;

            CleanupVFX();

            if (target && target.healthComponent)
            {
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.damage = damageValue;
                damageInfo.attacker = attacker;
                damageInfo.inflictor = gameObject;
                damageInfo.procCoefficient = procCoefficient;
                damageInfo.position = target.transform.position;
                damageInfo.crit = isCrit;
                damageInfo.damageColorIndex = damageColorIndex;
                damageInfo.damageType = extraDamageType;

                // Apply stun if chance is set
                if (stunChance > 0f && UnityEngine.Random.Range(0f, 1f) <= stunChance)
                {
                    damageInfo.damageType |= DamageType.Stun1s;
                }

                target.healthComponent.TakeDamage(damageInfo);
            }

            if (!string.IsNullOrEmpty(impactSfx))
                AkSoundEngine.PostEvent(impactSfx, target.gameObject);

            if (impactVFXPrefab)
            {
                EffectManager.SpawnEffect(impactVFXPrefab, new EffectData
                {
                    origin = target.transform.position,
                    rotation = Quaternion.identity,
                    scale = 1f
                }, true);
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

        private System.Collections.IEnumerator DestroyAfterLinger()
        {
            yield return new WaitForSeconds(lingerTime);
            Destroy(gameObject);
        }

        private void OnFail()
        {
            if (hasHit) return;
            hasHit = true;

            CleanupVFX();
            Destroy(gameObject);
        }

        protected void CleanupVFX()
        {
            if (vfxInstance)
            {
                vfxInstance.transform.parent = null;

                // Find head and tail children using ChildLocator
                var childLocator = vfxInstance.GetComponent<ChildLocator>();
                Transform headChild = childLocator ? childLocator.FindChild("head") : null;
                Transform tailChild = childLocator ? childLocator.FindChild("tail") : null;

                // Instantly destroy head and all its children
                if (headChild)
                {
                    Destroy(headChild.gameObject);
                }

                // Handle tail - stop emission/looping, let fade naturally
                if (tailChild)
                {
                    foreach (var trail in tailChild.GetComponentsInChildren<TrailRenderer>())
                    {
                        trail.autodestruct = true;
                        trail.emitting = false;
                    }

                    foreach (var ps in tailChild.GetComponentsInChildren<ParticleSystem>())
                    {
                        var emission = ps.emission;
                        emission.enabled = false;

                        var main = ps.main;
                        main.loop = false;

                        ps.Stop(false, ParticleSystemStopBehavior.StopEmitting);
                    }

                    foreach (var light in tailChild.GetComponentsInChildren<Light>())
                    {
                        StartCoroutine(FadeLight(light, 0.3f));
                    }
                }

                // Destroy the main VFX object after fade time
                Destroy(vfxInstance, 1f);
            }
        }

        private Vector3 FindValidJitterOffset(Vector3 basePosition)
        {
            // Try up to 8 different jitter directions to find one without collision
            for (int attempts = 0; attempts < 8; attempts++)
            {
                Vector3 jitterOffset = new Vector3(
                    Random.Range(-jitterIntensity, jitterIntensity),
                    Random.Range(-jitterIntensity, jitterIntensity),
                    Random.Range(-jitterIntensity, jitterIntensity)
                );

                Vector3 testPosition = basePosition + jitterOffset;

                // Check if this jittered position would collide with stage geometry
                if (!Physics.Linecast(transform.position, testPosition, LayerIndex.world.mask))
                {
                    return jitterOffset;
                }
            }

            // If all attempts failed, return no jitter (stay on safe path)
            return Vector3.zero;
        }

        private System.Collections.IEnumerator FadeLight(Light light, float fadeTime)
        {
            if (!light) yield break;

            float startIntensity = light.intensity;
            float elapsed = 0f;

            while (elapsed < fadeTime && light)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeTime;
                light.intensity = Mathf.Lerp(startIntensity, 0f, t);
                yield return null;
            }

            if (light)
                light.intensity = 0f;
        }
    }
}