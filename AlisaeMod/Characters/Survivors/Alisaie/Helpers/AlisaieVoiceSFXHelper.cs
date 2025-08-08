using System.Collections.Generic;
using UnityEngine;
using RoR2;

namespace AlisaieMod.Characters.Survivors.Alisaie.Helpers
{

    /**
     * Class allows us to play Voice SFX randomly from categories such as HurtSounds and BattleSounds and DeathSounds.
     * This also triggers a Stop all Voice SFX event to prevent overlap of voice lines.
     */
    public static class AlisaieVoiceSFXHelper
    {
        private const string StopVFXAll = "StopVFXAll";

        private static readonly List<string> HurtSounds = new List<string>
        {
            "Play_VFXHurt1",
            "Play_VFXHurt2",
            "Play_VFXHurt3",
            "Play_VFXHurt4",
            "Play_VFXHurt5",
            "Play_VFXHurt6"
        };

        private static readonly List<string> DeathSounds = new List<string>
        {
            "Play_VFXDeath1",
            "Play_VFXDeath2"
        };

        private static readonly List<string> BattleSounds = new List<string>
        {
            "Play_VFXBattle1",
            "Play_VFXBattle2",
            "Play_VFXBattle3",
            "Play_VFXBattle4",
            "Play_VFXBattle5",
            "Play_VFXBattle6",
            "Play_VFXBattle7"
        };

        // --- Misc / Named Lines ---
        public const string Play_VFXIntro = "Play_VFXIntro";
        public const string Play_VFXEnGarde = "Play_VFXEnGarde";
        public const string Play_VFXBackInTheGame = "Play_VFXBackInTheGame";
        public const string Play_VFXStillFight = "Play_VFXStillFight";
        public const string Play_VFXGiveThemAShow = "Play_VFXGiveThemAShow";
        public const string Play_VFXNotDone = "Play_VFXNotDone";
        public const string Play_VFXDamnYou = "Play_VFXDamnYou";
        public const string Play_VFXPayMeBack = "Play_VFXPayMeBack";
        public const string Play_VFXSmokingCrater = "Play_VFXSmokingCrater";
        public const string Play_VFXRightBehindYou = "Play_VFXRightBehindYou";
        public const string Play_VFXHealing = "Play_VFXHealing";

        // --- Core logic ---
        public static bool IsVoiceEnabled(CharacterBody body)
        {
            if (body == null || body.skillLocator == null)
                return true;
            // Find the voice option skill family
            var genericSkills = body.GetComponents<GenericSkill>();
            foreach (var skill in genericSkills)
            {
                if (skill.skillName == "VoiceOption" && skill.skillFamily != null)
                {
                    var selectedSkillDef = skill.skillDef;
                    return selectedSkillDef != null && selectedSkillDef.skillName == "VoiceOn";
                }
            }
            return true; // Default to enabled if not found
        }

        private static void TryPlayRandomVFX(GameObject obj, List<string> eventList, float chancePercent)
        {
            if (!obj || eventList == null || eventList.Count == 0)
                return;

            var body = obj.GetComponent<CharacterBody>();
            if (!IsVoiceEnabled(body))
                return;

            if (Random.value <= chancePercent)
            {
                AkSoundEngine.PostEvent(StopVFXAll, obj);

                int index = Random.Range(0, eventList.Count);
                AkSoundEngine.PostEvent(eventList[index], obj);
            }
        }

        // --- Public methods ---

        public static void PlayRandomHurtVFX(float chancePercent, GameObject obj)
        {
            TryPlayRandomVFX(obj, HurtSounds, chancePercent);
        }

        public static void PlayRandomDeathVFX(float chancePercent, GameObject obj)
        {
            TryPlayRandomVFX(obj, DeathSounds, chancePercent);
        }

        public static void PlayRandomBattleVFX(float chancePercent, GameObject obj)
        {
            TryPlayRandomVFX(obj, BattleSounds, chancePercent);
        }

        public static void PlayVFX(float chancePercent, string VFXName, GameObject obj)
        {
            if (!obj || string.IsNullOrEmpty(VFXName))
                return;

            var body = obj.GetComponent<CharacterBody>();
            if (!IsVoiceEnabled(body))
                return;

            if (Random.value <= chancePercent)
            {
                AkSoundEngine.PostEvent(StopVFXAll, obj);
                AkSoundEngine.PostEvent(VFXName, obj);
            }
        }

        public static void PlayRandomFromArrayVFX(float chancePercent, string[] eventArray, GameObject obj)
        {
            if (obj == null || eventArray == null || eventArray.Length == 0)
                return;

            var body = obj.GetComponent<CharacterBody>();
            if (!IsVoiceEnabled(body))
                return;

            if (Random.value <= chancePercent)
            {
                AkSoundEngine.PostEvent(StopVFXAll, obj);
                int index = Random.Range(0, eventArray.Length);
                AkSoundEngine.PostEvent(eventArray[index], obj);
            }
        }

    }

}
