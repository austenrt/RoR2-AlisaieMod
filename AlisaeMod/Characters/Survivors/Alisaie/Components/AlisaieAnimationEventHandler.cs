using UnityEngine;
using RoR2;
using AlisaieMod.Survivors.Alisaie.SkillStates;
using System.Collections.Generic;

namespace AlisaieMod.Characters.Survivors.Alisaie.Components
{
    //goes on model prefab, handles animation events
    public class AlisaieAnimationEventHandler : MonoBehaviour
    {
        private static Dictionary<GameObject, CharacterBody> registeredBodies = new Dictionary<GameObject, CharacterBody>();

        public void TestEvent() { }

        public static void RegisterCharacterBody(GameObject modelObject, CharacterBody characterBody)
        {
            registeredBodies[modelObject] = characterBody;
        }

        public static void UnregisterCharacterBody(GameObject modelObject)
        {
            if (registeredBodies.ContainsKey(modelObject))
            {
                registeredBodies.Remove(modelObject);
            }
        }


        public void OnCastPoint()
        {
            if (registeredBodies.TryGetValue(gameObject, out CharacterBody characterBody))
            {
                var primaryState = characterBody.skillLocator?.primary?.stateMachine?.state;
                var secondaryState = characterBody.skillLocator?.secondary?.stateMachine?.state;

                if (primaryState is Jolt joltState)
                {
                    joltState.OnAnimationCastPoint();
                }
                else if (primaryState is Veraero veraeroState)
                {
                    veraeroState.OnAnimationCastPoint();
                }
                else if (secondaryState is Verthunder verthunderState)
                {
                    verthunderState.OnAnimationCastPoint();
                }
                else if (secondaryState is Verholy verholyState)
                {
                    verholyState.OnAnimationCastPoint();
                }
                else if (secondaryState is Verflare verflareState)
                {
                    verflareState.OnAnimationCastPoint();
                }
            }
        }

        public void ActivateSwordHitbox()
        {
            if (registeredBodies.TryGetValue(gameObject, out CharacterBody characterBody))
            {
                var specialState = characterBody.skillLocator?.special?.stateMachine?.state;
                if (specialState is SwordComboStep swordComboStep)
                {
                    swordComboStep.OnAnimationActivateHitbox();
                }
            }
        }

        public void DeactivateSwordHitbox()
        {
            if (registeredBodies.TryGetValue(gameObject, out CharacterBody characterBody))
            {
                var specialState = characterBody.skillLocator?.special?.stateMachine?.state;
                if (specialState is SwordComboStep swordComboStep)
                {
                    swordComboStep.OnAnimationDeactivateHitbox();
                }
            }
        }

        public void ActivateSwordTrailVFX()
        {
            var childLocator = GetComponent<ChildLocator>();
            if (childLocator == null) return;
            // Check for enchanted status
            bool enchanted = false;
            if (registeredBodies.TryGetValue(gameObject, out CharacterBody characterBody))
            {
                var specialState = characterBody.skillLocator?.special?.stateMachine?.state;
                enchanted = specialState is EnchantedRiposte || specialState is EnchantedZwerchhau || specialState is EnchantedRedoublement;
            }
            var vfxTrail = childLocator.FindChild(enchanted ? "VFXswordTrailEnch" : "VFXswordTrail");
            if (vfxTrail == null) return;
            foreach (var ps in vfxTrail.GetComponentsInChildren<ParticleSystem>(true))
            {
                ps.Play(true);
            }
        }

        public void DeactivateSwordTrailVFX()
        {
            var childLocator = GetComponent<ChildLocator>();
            if (childLocator == null) return;
            // Deactivate both trails
            var vfxTrail = childLocator.FindChild("VFXswordTrail");
            if (vfxTrail != null)
            {
                foreach (var ps in vfxTrail.GetComponentsInChildren<ParticleSystem>(true))
                {
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }
            var vfxTrailEnch = childLocator.FindChild("VFXswordTrailEnch");
            if (vfxTrailEnch != null)
            {
                foreach (var ps in vfxTrailEnch.GetComponentsInChildren<ParticleSystem>(true))
                {
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }
        }

        public void PlaySwordStepVFX(int step, bool enchanted)
        {
            var childLocator = GetComponent<ChildLocator>();
            if (childLocator == null) return;
            string vfxName = $"VFXs{(enchanted ? step - 3 : step)}{(enchanted ? "Ench" : "")}Attack";
            var vfxObj = childLocator.FindChild(vfxName);
            if (vfxObj == null) return;
            foreach (var ps in vfxObj.GetComponentsInChildren<ParticleSystem>(true))
            {
                ps.Play(true);
            }
        }

        public void PlaySwordStepVFXEvent()
        {
            if (registeredBodies.TryGetValue(gameObject, out CharacterBody characterBody))
            {
                var specialState = characterBody.skillLocator?.special?.stateMachine?.state;
                int step = 0;
                bool enchanted = false;
                if (specialState is SwordComboStep swordComboStep)
                {
                    step = swordComboStep.stepIndex;
                    enchanted = specialState is EnchantedRiposte || specialState is EnchantedZwerchhau || specialState is EnchantedRedoublement;
                }
                PlaySwordStepVFX(step, enchanted);
            }
        }

        public void CleanupSwordEvents()
        {
            DeactivateSwordTrailVFX();
            DeactivateSwordHitbox();
        }

        private string GetTransformPath(Transform t)
        {
            string path = t.name;
            while (t.parent != null)
            {
                t = t.parent;
                path = t.name + "/" + path;
            }
            return path;
        }

        private void LogHierarchy(Transform t, int depth)
        {
            if (depth < 3)
            {
                for (int i = 0; i < t.childCount; i++)
                {
                    LogHierarchy(t.GetChild(i), depth + 1);
                }
            }
        }

    }
}