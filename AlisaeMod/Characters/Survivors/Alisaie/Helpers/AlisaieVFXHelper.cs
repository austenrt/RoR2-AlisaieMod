using UnityEngine;

namespace AlisaieMod.Characters.Survivors.Alisaie.Helpers
{
    public static class AlisaieVFXHelper
    {
        // Stops emission on all ParticleSystems in the object and destroys it after their lifetime ends.
        // This is to stop the particle effects from just disappearing abruptly.
        public static void StopAndDestroy(GameObject vfxObject, float bufferTime = 0.25f)
        {
            if (!vfxObject) return;

            float maxLifetime = 0f;
            var systems = vfxObject.GetComponentsInChildren<ParticleSystem>();

            foreach (var ps in systems)
            {
                var main = ps.main;
                ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);

                float duration = 0f;

                if (main.loop)
                {
                    var mainModule = ps.main;
                    mainModule.loop = false;
                }
                else
                {
                    var lifetime = main.startLifetime;
                    if (lifetime.mode == ParticleSystemCurveMode.TwoConstants)
                        duration = Mathf.Max(lifetime.constantMax, 0.5f);
                    else
                        duration = Mathf.Max(lifetime.constant, 0.5f);

                }

                maxLifetime = Mathf.Max(maxLifetime, duration);
            }

            Object.Destroy(vfxObject, maxLifetime + bufferTime);
        }

        // Returns a position along a combined arc between origin and target, with adjustable sideways and vertical arc.
        public static Vector3 GetCombinedArcPosition(
            Vector3 origin,
            Vector3 target,
            float t,
            float sideArcWidth,
            float verticalArcHeight,
            Vector3? sideArcDirection = null,
            Vector3? verticalArcDirection = null,
            bool useRandomArc = false)
        {
            Vector3 forward = (target - origin).normalized;

            Vector3 sideDir = sideArcDirection ?? Vector3.zero;
            Vector3 verticalDir = verticalArcDirection ?? Vector3.zero;

            if (useRandomArc)
            {
                // Compute random directions inside here
                Vector3 up = Vector3.up;
                Vector3 perpendicular = Vector3.Cross(forward, up).normalized;

                sideDir = Random.value > 0.5f ? perpendicular : -perpendicular;
                verticalDir = Random.value > 0.5f ? Vector3.up : Vector3.down;
            }

            Vector3 flat = Vector3.Lerp(origin, target, t);

            float arcFactor = 4f * t * (1f - t);

            Vector3 sideOffset = sideDir.normalized * sideArcWidth * arcFactor;
            Vector3 verticalOffset = verticalDir.normalized * verticalArcHeight * arcFactor;

            return flat + sideOffset + verticalOffset;
        }


        // Generates a random sideways arc direction (perpendicular to origin-target vector and up).
        public static Vector3 GetRandomSideArcDirection(Vector3 origin, Vector3 target)
        {
            Vector3 forward = (target - origin).normalized;
            Vector3 up = Vector3.up;
            Vector3 perpendicular = Vector3.Cross(forward, up).normalized;

            // Random left or right
            return Random.value > 0.5f ? perpendicular : -perpendicular;
        }

        // Generates a random vertical arc direction (up or down).
        public static Vector3 GetRandomVerticalArcDirection()
        {
            return Random.value > 0.5f ? Vector3.up : Vector3.down;
        }

        //Plays the aura VFX by finding it in the model transform and playing its Particle System
        //The aura VFX is a child of the model transform and is the VFX that surrounds alisaie when casting a spell.
        public static void PlayAuraVFX(Transform modelTransform, string auraVfxName)
        {
            var auraVfx = modelTransform.Find(auraVfxName);
            var ps = auraVfx?.GetComponent<ParticleSystem>();
            ps.Play(true);
        }

        // Enables the Focus child and disables the FakeFocus child in the ChildLocator
        public static void SetRealFocusActive(GameObject modelObject)
        {
            var childLocator = modelObject.GetComponent<ChildLocator>();
            if (childLocator == null) return;
            var realFocus = childLocator.FindChild("Focus");
            var fakeFocus = childLocator.FindChild("FakeFocus");
            if (realFocus) realFocus.gameObject.SetActive(true);
            if (fakeFocus) fakeFocus.gameObject.SetActive(false);
        }

        // Enables the FakeFocus child and disables the Focus child in the ChildLocator
        public static void SetFakeFocusActive(GameObject modelObject)
        {
            var childLocator = modelObject.GetComponent<ChildLocator>();
            if (childLocator == null) return;
            var realFocus = childLocator.FindChild("Focus");
            var fakeFocus = childLocator.FindChild("FakeFocus");
            if (realFocus) realFocus.gameObject.SetActive(false);
            if (fakeFocus) fakeFocus.gameObject.SetActive(true);
        }

        // Plays the Hold child particle system (and its children) under VFX{spellName}FocusEffect in focusVFXAnchor
        public static void PlayFocusHoldVFX(GameObject modelObject, string spellName)
        {
            var childLocator = modelObject.GetComponent<ChildLocator>();
            if (childLocator == null) return;
            var vfxAnchor = childLocator.FindChild("focusVFXAnchor");
            if (vfxAnchor == null) return;
            var focusEffect = vfxAnchor.Find($"VFX{spellName}FocusEffect");
            if (focusEffect == null) return;
            var hold = focusEffect.Find("Hold");
            if (hold != null)
            {
                foreach (var ps in hold.GetComponentsInChildren<ParticleSystem>(true))
                    ps.Play(true);
            }
        }

        // Plays the Burst child particle system (and its children) and stops the Hold one under VFX{spellName}FocusEffect in focusVFXAnchor
        public static void PlayFocusBurstVFX(GameObject modelObject, string spellName)
        {
            var childLocator = modelObject.GetComponent<ChildLocator>();
            if (childLocator == null) return;
            var vfxAnchor = childLocator.FindChild("focusVFXAnchor");
            if (vfxAnchor == null) return;
            var focusEffect = vfxAnchor.Find($"VFX{spellName}FocusEffect");
            if (focusEffect == null) return;
            var hold = focusEffect.Find("Hold");
            var burst = focusEffect.Find("Burst");
            if (hold != null)
            {
                foreach (var ps in hold.GetComponentsInChildren<ParticleSystem>(true))
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
            if (burst != null)
            {
                foreach (var ps in burst.GetComponentsInChildren<ParticleSystem>(true))
                    ps.Play(true);
            }
        }
    }
}
