using RoR2;
using RoR2.CharacterAI;
using RoR2.Skills;
using UnityEngine;

namespace AlisaieMod.Survivors.Alisaie
{
    public static class AlisaieAI
    {
        public static void Init(GameObject bodyPrefab, string masterName)
        {
            GameObject master = Modules.Prefabs.CreateBlankMasterPrefab(bodyPrefab, masterName);

            BaseAI baseAI = master.GetComponent<BaseAI>();
            baseAI.aimVectorDampTime = 0.1f;
            baseAI.aimVectorMaxSpeed = 360;

            var skillLocator = bodyPrefab.GetComponent<SkillLocator>();
            var primaryFamily = skillLocator.primary.skillFamily;
            var secondaryFamily = skillLocator.secondary.skillFamily;
            var utilityFamily = skillLocator.utility.skillFamily;
            var specialFamily = skillLocator.special.skillFamily;

            // Find SkillDefs by name
            SkillDef joltSkillDef = null, veraeroSkillDef = null, verholySkillDef = null, verthunderSkillDef = null, verflareSkillDef = null, rapierComboSkillDef = null, corpsSkillDef = null;
            foreach (var sd in primaryFamily.variants)
            {
                if (sd.skillDef.skillName == "Jolt") joltSkillDef = sd.skillDef;
                if (sd.skillDef.skillName == "AlisaieVeraero") veraeroSkillDef = sd.skillDef;
                if (sd.skillDef.skillName == "AlisaieVerholy") verholySkillDef = sd.skillDef;
            }
            foreach (var sd in secondaryFamily.variants)
            {
                if (sd.skillDef.skillName == "AlisaieVerthunder") verthunderSkillDef = sd.skillDef;
                if (sd.skillDef.skillName == "AlisaieVerflare") verflareSkillDef = sd.skillDef;
            }
            foreach (var sd in utilityFamily.variants)
            {
                if (sd.skillDef.skillName == "CorpsACorps") corpsSkillDef = sd.skillDef;
            }
            foreach (var sd in specialFamily.variants)
            {
                if (sd.skillDef.skillName == "RapierCombo") rapierComboSkillDef = sd.skillDef;
            }

            // --- Verholy/Verflare Finisher Driver ---
            AISkillDriver finisherDriver = master.AddComponent<AISkillDriver>();
            finisherDriver.customName = "Use Verholy/Verflare Finisher";
            finisherDriver.skillSlot = SkillSlot.Primary; // Assuming Verholy/Verflare are mapped to Primary/Secondary
            finisherDriver.requireSkillReady = true;
            finisherDriver.minDistance = 0;
            finisherDriver.maxDistance = 20;
            finisherDriver.selectionRequiresTargetLoS = false;
            finisherDriver.selectionRequiresOnGround = false;
            finisherDriver.selectionRequiresAimTarget = false;
            finisherDriver.maxTimesSelected = -1;
            finisherDriver.requiredSkill = verholySkillDef; // Use Verholy as finisher
            finisherDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            finisherDriver.activationRequiresTargetLoS = false;
            finisherDriver.activationRequiresAimTargetLoS = false;
            finisherDriver.activationRequiresAimConfirmation = true;
            finisherDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            finisherDriver.moveInputScale = 1;
            finisherDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            finisherDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            // --- Dualcast Spell Driver (Veraero/Verthunder) ---
            AISkillDriver dualcastDriver = master.AddComponent<AISkillDriver>();
            dualcastDriver.customName = "Use Dualcast Spell";
            dualcastDriver.skillSlot = SkillSlot.Secondary; // Or Secondary if mapped
            dualcastDriver.requireSkillReady = true;
            dualcastDriver.minDistance = 8;
            dualcastDriver.maxDistance = 20;
            dualcastDriver.selectionRequiresTargetLoS = false;
            dualcastDriver.selectionRequiresOnGround = false;
            dualcastDriver.selectionRequiresAimTarget = false;
            dualcastDriver.maxTimesSelected = -1;
            dualcastDriver.requiredSkill = verthunderSkillDef; // Use Verthunder for dualcast
            dualcastDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            dualcastDriver.activationRequiresTargetLoS = false;
            dualcastDriver.activationRequiresAimTargetLoS = false;
            dualcastDriver.activationRequiresAimConfirmation = true;
            dualcastDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            dualcastDriver.moveInputScale = 1;
            dualcastDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            dualcastDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            // --- Jolt Driver (Full Charge for Dualcast) ---
            AISkillDriver joltDriver = master.AddComponent<AISkillDriver>();
            joltDriver.customName = "Use Jolt (Full Charge)";
            joltDriver.skillSlot = SkillSlot.Primary;
            joltDriver.requireSkillReady = true;
            joltDriver.minDistance = 8;
            joltDriver.maxDistance = 20;
            joltDriver.selectionRequiresTargetLoS = false;
            joltDriver.selectionRequiresOnGround = false;
            joltDriver.selectionRequiresAimTarget = false;
            joltDriver.maxTimesSelected = -1;
            joltDriver.requiredSkill = joltSkillDef;
            joltDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            joltDriver.activationRequiresTargetLoS = false;
            joltDriver.activationRequiresAimTargetLoS = false;
            joltDriver.activationRequiresAimConfirmation = true;
            joltDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            joltDriver.moveInputScale = 1;
            joltDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            joltDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            // --- Sword Combo Driver (Enchanted if possible) ---
            AISkillDriver swordComboDriver = master.AddComponent<AISkillDriver>();
            swordComboDriver.customName = "Use Sword Combo (Enchanted if possible)";
            swordComboDriver.skillSlot = SkillSlot.Special;
            swordComboDriver.requireSkillReady = true;
            swordComboDriver.minDistance = 0;
            swordComboDriver.maxDistance = 8;
            swordComboDriver.selectionRequiresTargetLoS = false;
            swordComboDriver.selectionRequiresOnGround = false;
            swordComboDriver.selectionRequiresAimTarget = false;
            swordComboDriver.maxTimesSelected = -1;
            swordComboDriver.requiredSkill = rapierComboSkillDef;
            swordComboDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            swordComboDriver.activationRequiresTargetLoS = false;
            swordComboDriver.activationRequiresAimTargetLoS = false;
            swordComboDriver.activationRequiresAimConfirmation = false;
            swordComboDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            swordComboDriver.moveInputScale = 1;
            swordComboDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            swordComboDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            // --- Corps-A-Corps Driver (Utility) ---
            AISkillDriver corpsDriver = master.AddComponent<AISkillDriver>();
            corpsDriver.customName = "Use Corps-A-Corps";
            corpsDriver.skillSlot = SkillSlot.Utility;
            corpsDriver.requireSkillReady = true;
            corpsDriver.minDistance = 15;
            corpsDriver.maxDistance = 30;
            corpsDriver.selectionRequiresTargetLoS = false;
            corpsDriver.selectionRequiresOnGround = false;
            corpsDriver.selectionRequiresAimTarget = false;
            corpsDriver.maxTimesSelected = -1;
            corpsDriver.requiredSkill = corpsSkillDef;
            corpsDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            corpsDriver.activationRequiresTargetLoS = false;
            corpsDriver.activationRequiresAimTargetLoS = false;
            corpsDriver.activationRequiresAimConfirmation = false;
            corpsDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            corpsDriver.moveInputScale = 1;
            corpsDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            corpsDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            // --- Fallback Chase Driver ---
            AISkillDriver chaseDriver = master.AddComponent<AISkillDriver>();
            chaseDriver.customName = "Chase";
            chaseDriver.skillSlot = SkillSlot.None;
            chaseDriver.requireSkillReady = false;
            chaseDriver.minDistance = 0;
            chaseDriver.maxDistance = float.PositiveInfinity;
            chaseDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            chaseDriver.activationRequiresTargetLoS = false;
            chaseDriver.activationRequiresAimTargetLoS = false;
            chaseDriver.activationRequiresAimConfirmation = false;
            chaseDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            chaseDriver.moveInputScale = 1;
            chaseDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            chaseDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
        }
        //recommend taking these for a spin in game, messing with them in runtimeinspector to get a feel for what they should do at certain ranges and such
    }
}
