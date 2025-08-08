using AlisaieMod.Modules;
using AlisaieMod.Survivors.Alisaie.Achievements;
using System;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.XR;

namespace AlisaieMod.Survivors.Alisaie
{
    public static class AlisaieTokens
    {
        // Color constants for token styling
        private const string COLOR_WHITE_MANA = "#AEEFFF"; // light blue
        private const string COLOR_BLACK_MANA = "#B266FF"; // purple
        private const string COLOR_ENCHANTED = "#FF4444"; // red
        private const string COLOR_FLAVOR = "#CCD3E0"; // flavor text

        public static void Init()
        {
            AddAlisaieTokens();

            ////uncomment this to spit out a lanuage file with all the above tokens that people can translate
            ////make sure you set Language.usingLanguageFolder and printingEnabled to true
            Language.PrintOutput("Alisaie.txt");
            ////refer to guide on how to build and distribute your mod with the proper folders
        }

        public static void AddAlisaieTokens()
        {
            string prefix = AlisaieSurvivor.ALISAIE_PREFIX;

            string desc = $"Alisaie is a nimble Red Mage who balances white and black magic with precise swordplay.<color={COLOR_FLAVOR}>" + Environment.NewLine + Environment.NewLine
             + "< ! > Strategically build up mana before starting a teleporter event." + Environment.NewLine + Environment.NewLine
             + "< ! > Each rapier combo step has a 10 second grace period to continue the combo. " + Environment.NewLine + Environment.NewLine
             + "< ! > Use Veraero to pull enemies closer together for massive rapier combo damage." + Environment.NewLine + Environment.NewLine
             + "< ! > Verholy does slightly less damage than Verflare but heals in a large radius. Verflare deals more damage and applies burn." + Environment.NewLine + Environment.NewLine;

            string outro = "\"I'll forge my own path, no matter what stands in my way!\"";
            string outroFailure = "\"This isn't how my story was supposed to end...\"";

            Language.Add(prefix + "NAME", "Alisaie");
            Language.Add(prefix + "DESCRIPTION", desc);
            Language.Add(prefix + "SUBTITLE", "Red Mage");
            Language.Add(prefix + "LORE", "The source of the distortion is undeniable. Traces of dynamis, twisted and amplified, saturate the very fabric of Petrichor V. " +
                "It's as if a grand-scale, desperate projection was cast, an image of a world consumed by sorrow. The \"Alisaie\" we observe is not merely a traveler from another dimension; " +
                "she is an echo, a fragment of a soul plucked from the very precipice of the Final Days, then cast into this artificial despair-scape by Meteion. The entity, in her final, " +
                "fractured moments, sought to understand the ultimate despair of a dead world, and in doing so, inadvertently brought a beacon of defiance to her own distorted creation. " +
                "Alisaie's struggle here is not just for survival, but for the very soul of this echoing reality. " +
                "Should she truly \"awaken,\" the implications for Meteion, and perhaps even for the very fabric of the cosmos, are… considerable." +
                "\r\n\r\n— Confidential Research Log, Project \"Echo Chamber\"");
            Language.Add(prefix + "OUTRO_FLAVOR", outro);
            Language.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            Language.Add(prefix + "MASTERY_SKIN_NAME", "Alternate");
            Language.Add(prefix + "ALT_SKIN_NAME", "Crimson");
            Language.Add(prefix + "IM_NOT_ALPHINAUD_SKIN_NAME", "Im not alphinaud!");
            #endregion

            #region Passive
            Language.Add(prefix + "PASSIVE_NAME", "Dualcast");
            Language.Add(prefix + "PASSIVE_DESCRIPTION", "Fully charging Jolt or using Acceleration grants <style=cIsUtility>Dualcast</style>, temporarily transforming your abilities into powerful elemental spells.");
            #endregion

            #region Primary

            Language.Add(prefix + "PRIMARY_JOLT_NAME", "Jolt");
            Language.Add(prefix + "PRIMARY_JOLT_DESCRIPTION", $"Charge up a projectile, dealing <style=cIsDamage>{30f * AlisaieStaticValues.maxJoltDamageCoefficient} - {100f * AlisaieStaticValues.maxJoltDamageCoefficient}% damage</style> and generating <style=cIsUtility><color={COLOR_WHITE_MANA}>2 white mana</color></style> and <style=cIsUtility><color={COLOR_BLACK_MANA}>2 black mana</color></style>. Full charge grants <style=cIsUtility>Dualcast</style>.");

            Language.Add(prefix + "PRIMARY_VERAERO_NAME", "Veraero");
            Language.Add(prefix + "PRIMARY_VERAERO_DESCRIPTION", $"Unleash wind-aspected magic, dealing <style=cIsDamage>{100f * AlisaieStaticValues.veraeroDamageCoefficient}% AOE damage</style> and generating <style=cIsUtility><color={COLOR_WHITE_MANA}>7 white mana</color></style>. Only usable during <style=cIsUtility>Dualcast</style>.");

            Language.Add(prefix + "PRIMARY_VERHOLY_NAME", "Verholy");
            Language.Add(prefix + "PRIMARY_VERHOLY_DESCRIPTION", $"Fire a concentrated blast of holy energy at your target, dealing <style=cIsDamage>{100f * AlisaieStaticValues.verholyDamageCoefficient}% AoE damage</style> and generating <style=cIsUtility><color={COLOR_WHITE_MANA}>11 white mana</color></style>. Heals all allies within a large radius for half the AoE damage. Only usable after an <style=cIsUtility><color={COLOR_ENCHANTED}>Enchanted</color> Combo</style>.");

            #endregion

            #region Secondary

            Language.Add(prefix + "SECONDARY_ACCELERATION_NAME", "Acceleration");
            Language.Add(prefix + "SECONDARY_ACCELERATION_DESCRIPTION", "Gain <style=cIsUtility>Dualcast</style> instantly and double mana gain for 10 seconds.");

            Language.Add(prefix + "SECONDARY_VERTHUNDER_NAME", "Verthunder");
            Language.Add(prefix + "SECONDARY_VERTHUNDER_DESCRIPTION", $"Fire <style=cIsDamage>1-10 lightning orbs</style> based on level, each dealing <style=cIsDamage>{100f * AlisaieStaticValues.verthunderDamageCoefficient}% damage</style>. Generates <style=cIsUtility><color={COLOR_BLACK_MANA}>7 black mana</color></style>. Only usable during <style=cIsUtility>Dualcast</style>.");

            Language.Add(prefix + "SECONDARY_VERFLARE_NAME", "Verflare");
            Language.Add(prefix + "SECONDARY_VERFLARE_DESCRIPTION", $"Unleash an explosion, applying burn and dealing <style=cIsDamage>{100f * AlisaieStaticValues.verflareDamageCoefficient}% AoE damage.</style> Generates <style=cIsUtility><color={COLOR_BLACK_MANA}>11 black mana</color></style>. Only usable after an <style=cIsUtility><color={COLOR_ENCHANTED}>Enchanted</color> Combo</style>.");
            #endregion

            #region Utility
            Language.Add(prefix + "UTILITY_CORPS_NAME", "Corps-A-Corps");
            Language.Add(prefix + "UTILITY_CORPS_DESCRIPTION", $"Rush forward with a sword attack for <style=cIsDamage>{100f * AlisaieStaticValues.corpsDamageCoefficient}% damage</style> and briefly dodge.");

            //displacement removed
            //Language.Add(prefix + "UTILITY_DISPLACEMENT_NAME", "Displacement");
            //Language.Add(prefix + "UTILITY_DISPLACEMENT_DESCRIPTION", "Jump backwards to safety. The faster it’s used after Corps-A-Corps, the shorter the cooldown.");
            #endregion

            #region Special

            Language.Add(prefix + "SPECIAL_RAPIER_COMBO_NAME", "Rapier Combo");
            Language.Add(prefix + "SPECIAL_RAPIER_COMBO_DESCRIPTION", $"Perform a series of rapier attacks, each dealing increasing damage. Combo becomes <color={COLOR_ENCHANTED}>Enchanted</color> above <color={COLOR_WHITE_MANA}>50</color>/<color={COLOR_BLACK_MANA}>50</color> mana. <color={COLOR_ENCHANTED}>Enchanted</color> combos grant access to Verholy/Verflare.");

            // Individual sword combo step tokens 
            Language.Add(prefix + "SPECIAL_RIPOSTE_NAME", "Riposte");
            Language.Add(prefix + "SPECIAL_RIPOSTE_NAME_ENCH", "Enchanted Riposte");
            Language.Add(prefix + "SPECIAL_RIPOSTE_DESCRIPTION", $"A swing and thrust with your rapier, dealing <style=cIsDamage>{100f * (AlisaieStaticValues.riposteDamageCoefficient / 2f):0.#}%</style> damage per hit.");
            Language.Add(prefix + "SPECIAL_RIPOSTE_DESCRIPTION_ENCH", $"A swing and thrust with your rapier, dealing <style=cIsDamage>{100f * (AlisaieStaticValues.riposteDamageCoefficient / 2f * AlisaieStaticValues.enchantedMultiplier):0.#}%</style> per hit.");

            Language.Add(prefix + "SPECIAL_ZWERCHHAU_NAME", "Zwerchhau");
            Language.Add(prefix + "SPECIAL_ZWERCHHAU_NAME_ENCH", "Enchanted Zwerchhau");
            Language.Add(prefix + "SPECIAL_ZWERCHHAU_DESCRIPTION", $"Three slashes with your rapier, each dealing <style=cIsDamage>{100f * (AlisaieStaticValues.zwerchhauDamageCoefficient / 3f):0.#}%</style> damage.");
            Language.Add(prefix + "SPECIAL_ZWERCHHAU_DESCRIPTION_ENCH", $"Three slashes with your rapier, each dealing <style=cIsDamage>{100f * (AlisaieStaticValues.zwerchhauDamageCoefficient / 3f * AlisaieStaticValues.enchantedMultiplier):0.#}%</style>.");

            Language.Add(prefix + "SPECIAL_REDOB_NAME", "Redoublement");
            Language.Add(prefix + "SPECIAL_REDOB_NAME_ENCH", "Enchanted Redoublement");
            Language.Add(prefix + "SPECIAL_REDOB_DESCRIPTION", $"Six rapid jabs with your rapier, each dealing <style=cIsDamage>{100f * (AlisaieStaticValues.redoublementDamageCoefficient / 6f):0.#}%</style> damage.");
            Language.Add(prefix + "SPECIAL_REDOB_DESCRIPTION_ENCH", $"Six rapid jabs with your rapier, each dealing <style=cIsDamage>{100f * (AlisaieStaticValues.redoublementDamageCoefficient / 6f * AlisaieStaticValues.enchantedMultiplier):0.#}%</style>. Enables Verholy/Verflare.");
            #endregion

            #region Skins
            Language.Add(prefix + "ENDWALKER_SKIN_NAME", "Footfalls");
            Language.Add(prefix + "STORMBLOOD_SKIN_NAME", "Revolutions");
            Language.Add(prefix + "SHADOWBRINGERS_SKIN_NAME", "Tomorrow and Tomorrow");
            Language.Add(prefix + "SUMMER_SKIN_NAME", "Island Paradise");
            Language.Add(prefix + "WINTER_SKIN_NAME", "Home Beyond the Horizon");
            #endregion

            #region Achievements
            Language.Add(Tokens.GetAchievementNameToken(AlisaieMasteryAchievement.identifier), "Alisaie: Mastery");
            Language.Add(Tokens.GetAchievementDescriptionToken(AlisaieMasteryAchievement.identifier), "As Alisaie, beat the game or obliterate on Monsoon.");
            Language.Add(Tokens.GetAchievementNameToken(AlisaieWinterSkinAchievement.identifier), "Alisaie: Better Prepared");
            Language.Add(Tokens.GetAchievementDescriptionToken(AlisaieWinterSkinAchievement.identifier), "As Alisaie, become frozen.");
            Language.Add(Tokens.GetAchievementNameToken(AlisaieShadowbringersSkinAchievement.identifier), "Alisaie: One Brings Shadow, One Brings Light");
            Language.Add(Tokens.GetAchievementDescriptionToken(AlisaieShadowbringersSkinAchievement.identifier), "As Alisaie, get a multikill of 5 or more.");
            Language.Add(Tokens.GetAchievementNameToken(AlisaieSummerSkinAchievement.identifier), "Alisaie: Easy Breezy");
            Language.Add(Tokens.GetAchievementDescriptionToken(AlisaieSummerSkinAchievement.identifier), "As Alisaie, complete a teleporter event without going below 50/50 mana.");
            #endregion
            Language.Add("KEYWORD_AUTOAIM", "Auto Aim");
            #region Keywords

            #endregion

            // Voice Option Skill Tokens
            Language.Add(prefix + "VOICE_ON_NAME", "Voice: On");
            Language.Add(prefix + "VOICE_ON_DESCRIPTION", "Alisaie will play voice lines during gameplay.");
            Language.Add(prefix + "VOICE_OFF_NAME", "Voice: Off");
            Language.Add(prefix + "VOICE_OFF_DESCRIPTION", "Alisaie will be silent during gameplay.");
        }

    }
}
