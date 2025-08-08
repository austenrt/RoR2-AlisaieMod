using AlisaieMod.Survivors.Alisaie;
using RoR2;
using UnityEngine;
using RoR2.Skills;
using AlisaieMod.Modules;
using AlisaieMod.Characters.Survivors.Alisaie.Helpers;


namespace AlisaieMod.Characters.Survivors.Alisaie.Components
{
    /**
     * Manages spell-based skill switching mechanics for Alisaie.
     * 
     * This component handles:
     * - Dualcast skill switching (Primary/Secondary → Veraero/Verthunder)
     * - Finisher skill switching (Primary/Secondary → Verholy/Verflare)
     * - Cooldown preservation across skill switches
     */

    public class AlisaieSpellSwitcher : MonoBehaviour
    {
        private CharacterBody characterBody;
        private GenericSkill secondarySkill;
        private GenericSkill primarySkill;

        private BuffDef dualcastBuff;
        private SkillDef verthunderSkillDef;
        private SkillDef veraeroSkillDef;

        private SkillDef originalPrimarySkillDef;
        private SkillDef originalSecondarySkillDef;
        private bool isSecondarySwapped = false;
        private bool isPrimarySwapped = false;
        
        // Finisher skill switching
        private BuffDef finisherBuff;
        private SkillDef verholySkillDef;
        private SkillDef verflareSkillDef;
        private bool isFinisherSwapped = false;
        
        // Cooldown preservation
        private float savedCooldownRemaining = 0f;
        private float cooldownSaveTime = 0f;
        
        // Track if last dualcast was from acceleration
        private bool lastDualcastFromAcceleration = false;
        
        // Track original stock count to prevent duplication
        private int originalSecondaryStock = 0;

        void Awake()
        {
            characterBody = GetComponent<CharacterBody>();
            if (characterBody?.skillLocator == null) return;

            primarySkill = characterBody.skillLocator.primary;
            secondarySkill = characterBody.skillLocator.secondary;
            
            if (primarySkill != null)
            {
                originalPrimarySkillDef = primarySkill.skillDef;
            }
            if (secondarySkill != null)
            {
                originalSecondarySkillDef = secondarySkill.skillDef;
            }

            dualcastBuff = AlisaieBuffs.dualCastBuff;
            finisherBuff = AlisaieBuffs.finisherBuff;
            veraeroSkillDef = AlisaieSurvivor.instance?.veraeroSkillDef;
            verthunderSkillDef = AlisaieSurvivor.instance?.verthunderSkillDef;
            verholySkillDef = AlisaieSurvivor.instance?.verholySkillDef;
            verflareSkillDef = AlisaieSurvivor.instance?.verflareSkillDef;
        }

        void Update()
        {
            HandleDualcastSkillSwitching();
            HandleFinisherSkillSwitching();
        }
        
        private void HandleDualcastSkillSwitching()
        {
            if (characterBody == null || primarySkill == null || secondarySkill == null || 
                verthunderSkillDef == null || veraeroSkillDef == null || 
                originalPrimarySkillDef == null || originalSecondarySkillDef == null)
                return;

            bool hasDualcast = characterBody.GetBuffCount(dualcastBuff) > 0;

            if (hasDualcast && !isSecondarySwapped)
            {
                // Handle acceleration cooldown logic (dualcast from acceleration)
                HandleAccelerationCooldownOnSkillSwitch(true, true, true);
                
                // Switch both skills
                primarySkill.SetSkillOverride(this, veraeroSkillDef, GenericSkill.SkillOverridePriority.Replacement);
                secondarySkill.SetSkillOverride(this, verthunderSkillDef, GenericSkill.SkillOverridePriority.Replacement);
                isPrimarySwapped = true;
                isSecondarySwapped = true;
            }
            else if (!hasDualcast && isSecondarySwapped)
            {
                // Switch both skills back
                primarySkill.UnsetSkillOverride(this, veraeroSkillDef, GenericSkill.SkillOverridePriority.Replacement);
                secondarySkill.UnsetSkillOverride(this, verthunderSkillDef, GenericSkill.SkillOverridePriority.Replacement);
                isPrimarySwapped = false;
                isSecondarySwapped = false;
                
                // Handle acceleration cooldown logic (dualcast from acceleration)
                HandleAccelerationCooldownOnSkillSwitch(false, true, true);
            }
        }
        
        private void HandleFinisherSkillSwitching()
        {
            if (characterBody == null || primarySkill == null || secondarySkill == null || 
                verholySkillDef == null || verflareSkillDef == null || finisherBuff == null)
                return;

            bool hasFinisher = characterBody.GetBuffCount(finisherBuff) > 0;

            if (hasFinisher && !isFinisherSwapped)
            {
                // Handle acceleration cooldown logic (finishers behave like Jolt - preserve but don't interfere)
                HandleAccelerationCooldownOnSkillSwitch(true, true, false);
                
                // Play finisher ready sound
                AkSoundEngine.PostEvent(AlisaieStaticValues.PlayFinisherReady, gameObject);
                
                // Switch to finisher skills
                primarySkill.SetSkillOverride(this, verholySkillDef, GenericSkill.SkillOverridePriority.Replacement);
                secondarySkill.SetSkillOverride(this, verflareSkillDef, GenericSkill.SkillOverridePriority.Replacement);
                isFinisherSwapped = true;
            }
            else if (!hasFinisher && isFinisherSwapped)
            {
                // Switch back to original skills
                primarySkill.UnsetSkillOverride(this, verholySkillDef, GenericSkill.SkillOverridePriority.Replacement);
                secondarySkill.UnsetSkillOverride(this, verflareSkillDef, GenericSkill.SkillOverridePriority.Replacement);
                isFinisherSwapped = false;
                
                // Handle acceleration cooldown logic (finishers behave like Jolt - preserve but don't interfere)
                HandleAccelerationCooldownOnSkillSwitch(false, true, false);
            }
        }
        
        private void HandleAccelerationCooldownOnSkillSwitch(bool isSwitchingTo, bool canAffectAcceleration = true, bool isFromAcceleration = false)
        {
            if (!canAffectAcceleration) return;
            
            if (isSwitchingTo)
            {
                // Save original stock count before any modifications
                originalSecondaryStock = secondarySkill.stock;
                
                // Only save cooldown if this is from acceleration (not from Jolt or Finisher)
                if (isFromAcceleration && lastDualcastFromAcceleration)
                {
                    savedCooldownRemaining = Mathf.Max(savedCooldownRemaining, secondarySkill.cooldownRemaining);
                    cooldownSaveTime = Time.time;

                    // Consume one stock immediately when switching from acceleration
                    if (secondarySkill.stock > 0)
                    {
                        secondarySkill.stock -= 1;
                        originalSecondaryStock = secondarySkill.stock; // Update tracked stock
                    }
                }
                else
                {
                    // Don't save cooldown for Jolt-granted dualcast or finisher buffs
                    float currentRemaining = GetCurrentSavedCooldown();
                    string source = isFromAcceleration ? "Jolt dualcast" : "Finisher buff";
                }
            }
            else
            {
                // Restore original stock count to prevent duplication
                secondarySkill.stock = originalSecondaryStock;
                
                // Only restore cooldown if it was from acceleration
                float currentRemaining = GetCurrentSavedCooldown();
                if (currentRemaining > 0f)
                {
                    // Set the cooldown time based on how much time has passed
                    secondarySkill.rechargeStopwatch = secondarySkill.finalRechargeInterval - currentRemaining;
                    
                    // Force refresh the skill state
                    secondarySkill.RunRecharge(0f);
                    
                    // Reset saved values after use to prevent interference with future uses
                    savedCooldownRemaining = 0f;
                    cooldownSaveTime = 0f;
                    originalSecondaryStock = 0;
                }
                else
                {
                    // Reset saved values even when no cooldown to restore
                    originalSecondaryStock = 0;
                }
            }
        }

        public void ConsumeDualcast()
        {
            ConsumeDualcast(false); // Default to dualcast consumption
        }
        
        public void ConsumeDualcast(bool isFinisher)
        {
            if (isFinisher)
            {
                // Consume finisher buff
                if (characterBody != null && characterBody.GetBuffCount(finisherBuff) > 0)
                {
                    characterBody.RemoveOldestTimedBuff(finisherBuff);
                }
            }
            else
            {
                // Consume dualcast buff
                if (characterBody != null && characterBody.GetBuffCount(dualcastBuff) > 0)
                {
                    // Handle acceleration cooldown if applicable
                    HandleAccelerationCooldown();
                    
                    characterBody.RemoveOldestTimedBuff(dualcastBuff); // Remove only one stack
                }
            }
        }
        
        public void SetAccelerationDualcast()
        {
            // Called specifically when Acceleration skill grants dualcast
            lastDualcastFromAcceleration = true;
        }
        
        private float GetCurrentSavedCooldown()
        {
            if (savedCooldownRemaining <= 0f) return 0f;
            
            // Calculate how much time has passed since we saved the cooldown
            float timePassed = Time.time - cooldownSaveTime;
            float currentRemaining = savedCooldownRemaining - timePassed;
            
            return Mathf.Max(0f, currentRemaining);
        }
        
        public void ConsumeFinisher()
        {
            ConsumeDualcast(true); // Use overloaded method for finisher consumption
        }
        
        private void HandleAccelerationCooldown()
        {
            // Only apply acceleration cooldown if this dualcast was from acceleration
            if (lastDualcastFromAcceleration)
            {
                float oldCooldown = savedCooldownRemaining;
                savedCooldownRemaining = Mathf.Max(savedCooldownRemaining, 12f); // Don't reduce existing cooldown
                lastDualcastFromAcceleration = false; // Reset flag
            }
        }

    }

}
