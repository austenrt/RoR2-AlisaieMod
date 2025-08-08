using AlisaieMod.Survivors.Alisaie;
using RoR2;
using UnityEngine;

namespace AlisaieMod.Characters.Survivors.Alisaie.Helpers
{
    /**
     * Utility class for safe and reliable buff application and removal.
     */
    public static class AlisaieBuffHelper
    {
        /// <summary>
        /// Safely applies a non-stacking timed buff to the given characterBody.
        /// </summary>
        public static void AddBuff(CharacterBody characterBody, GameObject source, BuffDef buff, bool playSFX, float duration, bool applyDelay = false)
        {
            if (characterBody == null || buff == null)
            {
                return;
            }

            // Only apply buffs on authority to prevent network issues
            if (!characterBody.hasAuthority)
            {
                return;
            }

            if (applyDelay)
            {
                characterBody.StartCoroutine(DelayedBuffApplication(characterBody, source, buff, playSFX, duration));
                return;
            }


            characterBody.AddTimedBuff(buff, duration);

            if (characterBody.HasBuff(buff))
            {
                if (playSFX && source != null)
                {
                    AkSoundEngine.PostEvent(AlisaieStaticValues.PlayBuffAdd, source);
                }
            }
        }

        private static System.Collections.IEnumerator DelayedBuffApplication(CharacterBody characterBody, GameObject source, BuffDef buff, bool playSFX, float duration)
        {
            yield return null; // Wait 1 frame
            if (!characterBody) yield break;

            AddBuff(characterBody, source, buff, playSFX, duration, applyDelay: false);
        }

        public static void RemoveBuff(CharacterBody characterBody, BuffDef buff)
        {
            if (characterBody == null || buff == null) return;

            while (characterBody.HasBuff(buff))
            {
                characterBody.RemoveBuff(buff);
            }
        }
    }
}
