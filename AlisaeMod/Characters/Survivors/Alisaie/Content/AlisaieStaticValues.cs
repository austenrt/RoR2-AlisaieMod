using System;

namespace AlisaieMod.Survivors.Alisaie
{
    public static class AlisaieStaticValues
    {

        //jolt
        public const float maxJoltDamageCoefficient = 3.2f;

        // Dualcast spells
        public const float veraeroDamageCoefficient = 3.25f;       // AoE with inward pull
        public const float verthunderDamageCoefficient = 2.5f;    // multibeam single target

        //Utility
        public const float corpsDamageCoefficient = 3f;

        // Enchanted combo hits, enchanted should have a multiplier applied
        public const float riposteDamageCoefficient = 6f;
        public const float zwerchhauDamageCoefficient = 8f;
        public const float redoublementDamageCoefficient = 10f;
        public const float enchantedMultiplier = 2f;

        // Finishers
        public const float verholyDamageCoefficient = 16f;      // AoE finisher - white
        public const float verflareDamageCoefficient = 24f;     // AoE finisher - black 
        

        // === SOUND EFFECTS BY SKILL ===
        
        // PRIMARY - Jolt
        public const string PlayCastJolt = "Play_cast_jolt";
        public const string StopCastJolt = "Stop_cast_jolt";
        public const string PlayJoltImpact = "Play_jolt_impact";
        public const string StopJoltImpact = "Stop_jolt_impact";
        
        // PRIMARY - Veraero (Dualcast)
        public const string PlayAeroCast = "Play_aero_cast";
        public const string StopAeroCast = "Stop_aero_cast";
        public const string PlayAeroImpact = "Play_aero_impact";
        public const string StopAeroImpact = "Stop_aero_impact";
        
        // SECONDARY - Verthunder (Dualcast)
        public const string PlayThunderCast = "Play_thunder_cast";
        public const string StopThunderCast = "Stop_thunder_cast";
        public const string PlayThunderImpact = "Play_thunder_impact";
        public const string StopThunderImpact = "Stop_thunder_impact";
        
        // UTILITY - Corps-A-Corps / Displacement
        public const string PlayFlecheCast = "Play_fleche_cast";
        public const string StopFlecheCast = "Stop_fleche_cast";
        public const string PlayFlecheImpact = "Play_fleche_impact";
        public const string StopFlecheImpact = "Stop_fleche_impact";
        
        // SPECIAL - Sword Combo
        // Riposte (S1)
        public const string PlayS1Riposte = "Play_s1_riposte";
        public const string PlayS1RiposteEnch = "Play_s1_riposte_ench";
        public const string StopS1Riposte = "Stop_s1_riposte_ench";
        
        // Zwerchhau (S2)
        public const string PlayS2Zwerch = "Play_s2_zwerch";
        public const string StopS2Zwerch = "Stop_s2_zwerch";
        public const string PlayS2ZwerchEnch = "Play_s2_zwerch_ench";
        public const string StopS2ZwerchEnch = "Stop_s2_zwerch_ench";
        public const string PlayS2ZwerchEnchImpact = "Play_s2_zwerch_ench_impact";
        public const string StopS2ZwerchEnchImpact = "Stop_s2_zwerch_ench_impact";
        
        // Redoublement (S3)
        public const string PlayS3Redoublement = "Play_s3_redoublement";
        public const string StopS3Redoublement = "Stop_s3_redoublement";
        public const string PlayS3RedoublementEnch = "Play_s3_redoublement_ench";
        public const string StopS3RedoublementEnch = "Stop_s3_redoublement_ench";
        
        // FINISHERS - Verholy/Verflare
        public const string PlayFinisherHoly = "Play_finisher_holy";
        public const string StopFinisherHoly = "Stop_finisher_holy";
        public const string PlayFinisherFlare = "Play_finisher_flare";
        public const string StopFinisherFlare = "Stop_finisher_flare";
        
        // GENERAL - Buffs/Effects
        public const string PlayBuffAdd = "Play_buffAdd";
        public const string StopBuffAdd = "Stop_buffAdd";
        public const string PlayFullcast = "Play_fullcast";
        public const string StopFullcast = "Stop_fullcast";
        public const string PlayAcceleration = "Play_acceleration";
        public const string StopAcceleration = "Stop_acceleration";
        public const string PlayCorps = "Play_corps";
        public const string StopCorps = "Stop_corps";
        public const string Play5050Mana = "Play_5050mana";
        public const string Stop5050Mana = "Stop_5050mana";
        public const string PlayFinisherReady = "Play_finisher_ready";
        public const string StopFinisherReady = "Stop_finisher_ready";
        
        // SPECIAL - Sword Combo Short Versions
        public const string PlayS3RedoublementShort = "Play_s3_redoublement_short";
        public const string StopS3RedoublementShort = "Stop_s3_redoublement_short";
        public const string PlayS3RedoublementEnchShort = "Play_s3_redoublement_ench_short";
        public const string StopS3RedoublementEnchShort = "Stop_s3_redoublement_ench_short";
        
    }
}
