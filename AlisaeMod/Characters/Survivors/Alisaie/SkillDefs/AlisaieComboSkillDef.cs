using RoR2.Skills;
using RoR2;
using EntityStates;
using UnityEngine;
using JetBrains.Annotations;
using AlisaieMod.Characters.Survivors.Alisaie.Components;

namespace AlisaieMod.Survivors.Alisaie.SkillDefs
{
    /// <summary>
    /// This is a custom skill definition for Alisaie's sword combo system.
    /// 
    /// check docs/SwordComboStateMachine.png for the state machine diagram this was based on
    /// 
    /// </summary>
    public class AlisaieComboSkillDef : SkillDef
    {
        public enum ComboStep
        {
            Cooldown = 0,
            Riposte = 1,
            Zwerchhau = 2,
            Redoublement = 3,
            EnchRiposte = 4,
            EnchZwerchhau = 5,
            EnchRedoublement = 6
        }

        public SerializableEntityStateType[] comboStates;
        public Sprite[] comboIcons;
        public string[] comboSkillNameTokens;
        public string[] comboSkillDescriptionTokens; 
        public float stepGraceDuration = 10f;

        public class InstanceData : BaseSkillInstanceData
        {
            public ComboStep currentStep = ComboStep.Cooldown;
            public bool enchanted = false;
            public float stepGraceCountDownTimer = 0f;
            public float cooldownCountdownTimer = 0f; // Track cooldown period
            public int trueStock = 0; // Cache for real stock during combo
        }

        public override BaseSkillInstanceData OnAssigned(GenericSkill skillSlot)
        {
            var data = new InstanceData();
            data.currentStep = ComboStep.Cooldown;
            data.enchanted = false;
            data.stepGraceCountDownTimer = 0f;
            data.cooldownCountdownTimer = 0f;
            skillSlot.skillInstanceData = data;
            return data;
        }

        public override EntityState InstantiateNextState(GenericSkill skillSlot)
        {
            var instanceData = skillSlot.skillInstanceData as InstanceData;
            int stepIndex = (int)instanceData.currentStep;
            if (comboStates != null && stepIndex >= 0 && stepIndex < comboStates.Length)
            {
                var stateType = comboStates[stepIndex];
                EntityState entityState = (EntityState)System.Activator.CreateInstance(stateType.stateType);
                var swordComboStep = entityState as AlisaieMod.Survivors.Alisaie.SkillStates.SwordComboStep;
                if (swordComboStep != null)
                {
                    swordComboStep.stepIndex = stepIndex;
                }
                return entityState;
            }
            return base.InstantiateNextState(skillSlot);
        }

        public override void OnExecute(GenericSkill skillSlot)
        {
            var instanceData = skillSlot.skillInstanceData as InstanceData;
            AlisaieManaTracker manaTracker = skillSlot.characterBody?.GetComponent<AlisaieManaTracker>();
            instanceData.stepGraceCountDownTimer = stepGraceDuration;

            ComboStep nextStep = instanceData.currentStep;
            switch (instanceData.currentStep)
            {
                case ComboStep.Cooldown:
                    nextStep = HasBalancedMana(manaTracker) ? ComboStep.EnchRiposte : ComboStep.Riposte;
                    break;
                case ComboStep.Riposte:
                    nextStep = ComboStep.Zwerchhau;
                    break;
                case ComboStep.Zwerchhau:
                    nextStep = ComboStep.Redoublement;
                    break;
                case ComboStep.Redoublement:
                    nextStep = ComboStep.Cooldown;
                    break;
                case ComboStep.EnchRiposte:
                    nextStep = ComboStep.EnchZwerchhau;
                    break;
                case ComboStep.EnchZwerchhau:
                    nextStep = ComboStep.EnchRedoublement;
                    break;
                case ComboStep.EnchRedoublement:
                    nextStep = ComboStep.Cooldown;
                    break;
            }

            skillSlot.stateMachine.SetInterruptState(InstantiateNextState(skillSlot), interruptPriority);


            switch (nextStep)
            {
                case ComboStep.Cooldown:
                    ToCooldown(skillSlot);
                    break;
                case ComboStep.Riposte:
                    ToRiposte(skillSlot);
                    break;
                case ComboStep.Zwerchhau:
                    ToZwerchhau(skillSlot);
                    break;
                case ComboStep.Redoublement:
                    ToRedoublement(skillSlot);
                    break;
                case ComboStep.EnchRiposte:
                    ToEnchRiposte(skillSlot);
                    break;
                case ComboStep.EnchZwerchhau:
                    ToEnchZwerchhau(skillSlot);
                    break;
                case ComboStep.EnchRedoublement:
                    ToEnchRedoublement(skillSlot);
                    break;
            }
        }

        private bool HasBalancedMana(AlisaieManaTracker manaTracker)
        {
            bool balanced = manaTracker != null && manaTracker.WhiteMana >= 50 && manaTracker.BlackMana >= 50;
            return balanced;
        }

        public override void OnFixedUpdate([NotNull] GenericSkill skillSlot, float deltaTime)
        {
            skillSlot.RunRecharge(deltaTime);
            var instanceData = skillSlot.skillInstanceData as InstanceData;
            if (instanceData == null) return;
            UpdateStepVisuals(skillSlot, instanceData.currentStep);

            var manaTracker = skillSlot.characterBody?.GetComponent<AlisaieManaTracker>();

            if (instanceData.currentStep == ComboStep.Cooldown)
            {
                instanceData.cooldownCountdownTimer -= deltaTime;
                if (instanceData.cooldownCountdownTimer <= 0f)
                {

                    instanceData.trueStock = skillSlot.stock; //cache stock before changing state
                    // After cooldown, transition to One of the entry swordcombo states (Riposte or EnchRiposte)
                    if (HasBalancedMana(manaTracker))
                    {
                        ToEnchRiposte(skillSlot);
                    }
                    else
                    {
                        ToRiposte(skillSlot);
                    }
                    skillSlot.SetBlockedCooldownSkillState(false);
                }
            }
            // If mana is in balance and we are in a normal combo, switch to enchanted riposte
            else if (instanceData.currentStep == ComboStep.Riposte || instanceData.currentStep == ComboStep.Zwerchhau || instanceData.currentStep == ComboStep.Redoublement)
            {
                if (HasBalancedMana(manaTracker))
                {
                    ToEnchRiposte(skillSlot);
                }
            }
            //Timeout logic: Riposte and Enchanted Riposte should not time out. Cooldown shouldn't either (obviously)
            if (instanceData.currentStep != ComboStep.Riposte && instanceData.currentStep != ComboStep.EnchRiposte && instanceData.currentStep != ComboStep.Cooldown)
            {
                instanceData.stepGraceCountDownTimer -= deltaTime;
                if (instanceData.stepGraceCountDownTimer <= 0f)
                {
                    ToCooldown(skillSlot);
                }
            }
        }

        private void UpdateStepVisuals(GenericSkill skillSlot, ComboStep step)
        {
            int stepIndex = (int)step;
            if (comboIcons != null && stepIndex >= 0 && stepIndex < comboIcons.Length)
                this.icon = comboIcons[stepIndex];
            if (comboSkillNameTokens != null && stepIndex >= 0 && stepIndex < comboSkillNameTokens.Length)
                this.skillNameToken = comboSkillNameTokens[stepIndex];
            if (comboSkillDescriptionTokens != null && stepIndex >= 0 && stepIndex < comboSkillDescriptionTokens.Length)
                this.skillDescriptionToken = comboSkillDescriptionTokens[stepIndex];
        }

        private void TransitionToState(GenericSkill skillSlot, ComboStep step, bool enchanted, float rechargeStopwatch, float cooldownOverride, float stepGrace = 0f)
        {
            var instanceData = skillSlot.skillInstanceData as InstanceData;
            instanceData.currentStep = step;
            instanceData.enchanted = enchanted;
            skillSlot.rechargeStopwatch = rechargeStopwatch;
            skillSlot.cooldownOverride = cooldownOverride;
            instanceData.stepGraceCountDownTimer = stepGrace;
            UpdateStepVisuals(skillSlot, step);
        }

        private void ToRiposte(GenericSkill skillSlot)
        {
            TransitionToState(skillSlot, ComboStep.Riposte, false, 0f, 1f, stepGraceDuration);
        }
        private void ToZwerchhau(GenericSkill skillSlot)
        {
            skillSlot.stock = 0; // Prevent spamming during combo
            TransitionToState(skillSlot, ComboStep.Zwerchhau, false, 0f, 1f, stepGraceDuration);
        }
        private void ToRedoublement(GenericSkill skillSlot)
        {
            skillSlot.stock = 0;
            TransitionToState(skillSlot, ComboStep.Redoublement, false, 0f, 1f, stepGraceDuration);
        }
        private void ToEnchRiposte(GenericSkill skillSlot)
        {
            skillSlot.stock = 0;
            TransitionToState(skillSlot, ComboStep.EnchRiposte, true, 0f, 1f, stepGraceDuration);
        }
        private void ToEnchZwerchhau(GenericSkill skillSlot)
        {
            skillSlot.stock = 0;
            TransitionToState(skillSlot, ComboStep.EnchZwerchhau, true, 0f, 1f, stepGraceDuration);
        }
        private void ToEnchRedoublement(GenericSkill skillSlot)
        {
            skillSlot.stock = 0;
            TransitionToState(skillSlot, ComboStep.EnchRedoublement, true, 0f, 1f, stepGraceDuration);
        }
        private void ToCooldown(GenericSkill skillSlot)
        {
            var instanceData = skillSlot.skillInstanceData as InstanceData;
            skillSlot.stock = instanceData.trueStock; // Restore cached stock
            TransitionToState(skillSlot, ComboStep.Cooldown, false, 0f, 8f, 0f);
            skillSlot.SetBlockedCooldownSkillState(true);
            skillSlot.stock -= stockToConsume; // Only consume stock here
            if (skillSlot.stock > 0)
            {
                instanceData.cooldownCountdownTimer = 0f;
            }
            else
            {
                instanceData.cooldownCountdownTimer = 8f;
            }
        }
    }
}
