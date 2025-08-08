using BepInEx.Configuration;
using AlisaieMod.Modules;
using AlisaieMod.Modules.Characters;
using AlisaieMod.Characters.Survivors.Alisaie.Components;
using AlisaieMod.Characters.Survivors.Alisaie.Helpers;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using R2API;
using EntityStates;
using AlisaieMod.Survivors.Alisaie.SkillDefs;

namespace AlisaieMod.Survivors.Alisaie
{
    public class AlisaieSurvivor : SurvivorBase<AlisaieSurvivor>
    {
        //used to load the assetbundle for this character. must be unique
        public override string assetBundleName => "mdlalisaie"; 

        //the name of the prefab we will create. conventionally ending in "Body". must be unique
        public override string bodyName => "AlisaieBody"; 

        //name of the ai master for vengeance and goobo. must be unique
        public override string masterName => "AlisaieMonsterMaster";

        public override string modelPrefabName => "mdlAlisaie";
        public override string displayPrefabName => "AlisaieDisplay";

        public const string ALISAIE_PREFIX = AlisaiePlugin.DEVELOPER_PREFIX + "_ALISAIE_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => ALISAIE_PREFIX;

        //skilldefs
        public SkillDef veraeroSkillDef;
        public SkillDef verthunderSkillDef;
        public SkillDef verholySkillDef;
        public SkillDef verflareSkillDef;
        public SkillDef enchantedSwordComboSkillDef;
        public SkillDef rapierComboSkillDef;
        public SkillDef voiceOnSkillDef;
        public SkillDef voiceOffSkillDef;
        public SkillFamily voiceOptionsSkillFamily;

        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = ALISAIE_PREFIX + "NAME",
            subtitleNameToken = ALISAIE_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("texIconAlisaiePortrait"),
            bodyColor = Color.red,
            sortPosition = 100,

            crosshair = Asset.LoadCrosshair("Standard"),
            podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 110f,
            healthRegen = 1.5f,
            armor = 0f,

            jumpCount = 1,
        };

        public override CustomRendererInfo[] customRendererInfos => new CustomRendererInfo[]
        {
            new CustomRendererInfo { childName = "ew_earring" },
            new CustomRendererInfo { childName = "ew_eyes" },
            new CustomRendererInfo { childName = "ew_face" },
            new CustomRendererInfo { childName = "ew_hair" },
            new CustomRendererInfo { childName = "ew_hair_acc" },
            new CustomRendererInfo { childName = "ew_hands" },
            new CustomRendererInfo { childName = "ew_lashes" },
            new CustomRendererInfo { childName = "ew_top" },
            new CustomRendererInfo { childName = "extraRenderer1" },
            new CustomRendererInfo { childName = "extraRenderer2" },
            new CustomRendererInfo { childName = "extraRenderer3" },
            new CustomRendererInfo { childName = "extraRenderer4" },
            new CustomRendererInfo { childName = "extraRenderer5" },
            new CustomRendererInfo { childName = "w2351b0012 Part 0.0" },
            new CustomRendererInfo { childName = "w2351b0012 Part 0.1" },
            new CustomRendererInfo { childName = "w2301b0016 Part 0.0" },
            new CustomRendererInfo { childName = "w2301b0016 Part 0.1" },
            new CustomRendererInfo { childName = "fakeFocus1", material = null },
            new CustomRendererInfo { childName = "fakeFocus2", material = null }
        };

        public override UnlockableDef characterUnlockableDef => AlisaieUnlockables.characterUnlockableDef;
        
        public override ItemDisplaysBase itemDisplays => new AlisaieItemDisplays();

        //set in base classes
        public override AssetBundle assetBundle { get; protected set; }

        public override GameObject bodyPrefab { get; protected set; }
        public override CharacterBody prefabCharacterBody { get; protected set; }
        public override GameObject characterModelObject { get; protected set; }
        public override CharacterModel prefabCharacterModel { get; protected set; }
        public override GameObject displayPrefab { get; protected set; }

        public override void Initialize()
        {
            //uncomment if you have multiple characters
            //ConfigEntry<bool> characterEnabled = Config.CharacterEnableConfig("Survivors", "Henry");

            //if (!characterEnabled.Value)
            //    return;

            base.Initialize();
        }

        public override void InitializeCharacter()
        {
            //need the character unlockable before you initialize the survivordef
            AlisaieUnlockables.Init();

            base.InitializeCharacter();

            AlisaieConfig.Init();
            AlisaieStates.Init();
            AlisaieTokens.Init();

            AlisaieAssets.Init(assetBundle);
            AlisaieBuffs.Init(assetBundle);

            InitializeEntityStateMachines();
            InitializeSkills();
            InitializeSkins();
            InitializeCharacterMaster();

            AdditionalBodySetup();

            AddHooks();
        }

        private void AdditionalBodySetup()
        {
            AddHitboxes();
            bodyPrefab.AddComponent<AlisaieAutoAim>();
            bodyPrefab.AddComponent<AlisaieSpellSwitcher>();
            bodyPrefab.AddComponent<AlisaieAnimationEventRegistrar>();
            bodyPrefab.AddComponent<AlisaieManaTracker>();
        }
     
        public void AddHitboxes()
        {
            //example of how to create a HitBoxGroup. see summary for more details 
            Prefabs.SetupHitBoxGroup(characterModelObject, "SwordGroup", "SwordHitbox");
        }

        public override void InitializeEntityStateMachines() 
        {
            //clear existing state machines from your cloned body (probably commando)
            //omit all this if you want to just keep theirs
            Prefabs.ClearEntityStateMachines(bodyPrefab);

            //the main "Body" state machine has some special properties
            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(EntityStates.GenericCharacterMain), typeof(EntityStates.SpawnTeleporterState));
            //if you set up a custom main characterstate, set it up here
                //don't forget to register custom entitystates in your HenryStates.cs

            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon2");
        }

        #region skills
        public override void InitializeSkills()
        {
            //remove the genericskills from the commando body we cloned
            Skills.ClearGenericSkills(bodyPrefab);
            //add our own
            //AddPassiveSkill();
            AddPrimarySkills();
            AddSecondarySkills();
            AddUtiitySkills();
            AddSpecialSkills();
            AddVoiceOptionSkills();
        }

        //skip if you don't have a passive
        //also skip if this is your first look at skills
        private void AddPassiveSkill()
        {
            //option 1. fake passive icon just to describe functionality we will implement elsewhere
            bodyPrefab.GetComponent<SkillLocator>().passiveSkill = new SkillLocator.PassiveSkill
            {
                enabled = true,
                skillNameToken = ALISAIE_PREFIX + "PASSIVE_NAME",
                skillDescriptionToken = ALISAIE_PREFIX + "PASSIVE_DESCRIPTION",
                keywordToken = "KEYWORD_STUNNING",
                icon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),
            };

            //option 2. a new SkillFamily for a passive, used if you want multiple selectable passives
            GenericSkill passiveGenericSkill = Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, "PassiveSkill");
            SkillDef passiveSkillDef1 = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "HenryPassive",
                skillNameToken = ALISAIE_PREFIX + "PASSIVE_NAME",
                skillDescriptionToken = ALISAIE_PREFIX + "PASSIVE_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_AGILE" },
                skillIcon = assetBundle.LoadAsset<Sprite>("texPassiveIcon"),

                //unless you're somehow activating your passive like a skill, none of the following is needed.
                //but that's just me saying things. the tools are here at your disposal to do whatever you like with

                //activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Shoot)),
                //activationStateMachineName = "Weapon1",
                //interruptPriority = EntityStates.InterruptPriority.Skill,

                //baseRechargeInterval = 1f,
                //baseMaxStock = 1,

                //rechargeStock = 1,
                //requiredStock = 1,
                //stockToConsume = 1,

                //resetCooldownTimerOnUse = false,
                //fullRestockOnAssign = true,
                //dontAllowPastMaxStocks = false,
                //mustKeyPress = false,
                //beginSkillCooldownOnSkillEnd = false,

                //isCombatSkill = true,
                //canceledFromSprinting = false,
                //cancelSprintingOnActivation = false,
                //forceSprintDuringState = false,

            });
            Skills.AddSkillsToFamily(passiveGenericSkill.skillFamily, passiveSkillDef1);
        }

        //if this is your first look at skilldef creation, take a look at Secondary first
        private void AddPrimarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Primary);

            SkillDef joltSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Jolt",
                skillNameToken = ALISAIE_PREFIX + "PRIMARY_JOLT_NAME",
                skillDescriptionToken = ALISAIE_PREFIX + "PRIMARY_JOLT_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_AUTOAIM" },
                skillIcon = assetBundle.LoadAsset<Sprite>("texIconJolt"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Jolt)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 1f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = true,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = true,
                forceSprintDuringState = false,
            });

            veraeroSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "AlisaieVeraero",
                skillNameToken = ALISAIE_PREFIX + "PRIMARY_VERAERO_NAME",
                skillDescriptionToken = ALISAIE_PREFIX + "PRIMARY_VERAERO_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texIconVeraero"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Veraero)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 2f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            verholySkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "AlisaieVerholy",
                skillNameToken = ALISAIE_PREFIX + "PRIMARY_VERHOLY_NAME",
                skillDescriptionToken = ALISAIE_PREFIX + "PRIMARY_VERHOLY_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texIconVerholy"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Verholy)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 1f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            Skills.AddPrimarySkills(bodyPrefab, joltSkillDef);
        }

        private void AddSecondarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Secondary);

            SkillDef accelerationSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "Acceleration",
                skillNameToken = ALISAIE_PREFIX + "SECONDARY_ACCELERATION_NAME",
                skillDescriptionToken = ALISAIE_PREFIX + "SECONDARY_ACCELERATION_DESCRIPTION",
                keywordTokens = new string[] {},
                skillIcon = assetBundle.LoadAsset<Sprite>("texIconAcceleration"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Acceleration)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 12f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            verthunderSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "AlisaieVerthunder",
                skillNameToken = ALISAIE_PREFIX + "SECONDARY_VERTHUNDER_NAME",
                skillDescriptionToken = ALISAIE_PREFIX + "SECONDARY_VERTHUNDER_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_AUTOAIM" },
                skillIcon = assetBundle.LoadAsset<Sprite>("texIconVerthunder"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Verthunder)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 2f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            verflareSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "AlisaieVerflare",
                skillNameToken = ALISAIE_PREFIX + "SECONDARY_VERFLARE_NAME",
                skillDescriptionToken = ALISAIE_PREFIX + "SECONDARY_VERFLARE_DESCRIPTION",
                keywordTokens = new string[] { },
                skillIcon = assetBundle.LoadAsset<Sprite>("texIconVerflare"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Verflare)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Skill,

                baseRechargeInterval = 1f,
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = false,
            });

            Skills.AddSecondarySkills(bodyPrefab, accelerationSkillDef);
        }

        private void AddUtiitySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Utility);

            SkillDef corpsSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "CorpsACorps",
                skillNameToken = ALISAIE_PREFIX + "UTILITY_CORPS_NAME",
                skillDescriptionToken = ALISAIE_PREFIX + "UTILITY_CORPS_DESCRIPTION",
                keywordTokens = new string[] { "KEYWORD_AGILE" },
                skillIcon = assetBundle.LoadAsset<Sprite>("texIconCorpsACorps"),

                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.CorpsACorps)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,

                baseRechargeInterval = 6f, 
                baseMaxStock = 1,

                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,

                resetCooldownTimerOnUse = false,
                fullRestockOnAssign = true,
                dontAllowPastMaxStocks = false,
                mustKeyPress = true,
                beginSkillCooldownOnSkillEnd = false,

                isCombatSkill = true,
                canceledFromSprinting = false,
                cancelSprintingOnActivation = false,
                forceSprintDuringState = true,
            });

            Skills.AddUtilitySkills(bodyPrefab, corpsSkillDef);
        }

        private void AddSpecialSkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Special);
            #region rapierComboSkillDef

            var rapierComboSkillDef = ScriptableObject.CreateInstance<AlisaieComboSkillDef>();
            rapierComboSkillDef.skillName = "RapierCombo";
            rapierComboSkillDef.skillNameToken = ALISAIE_PREFIX + "SPECIAL_RAPIER_COMBO_NAME";
            rapierComboSkillDef.skillDescriptionToken = ALISAIE_PREFIX + "SPECIAL_RAPIER_COMBO_DESCRIPTION";
            rapierComboSkillDef.icon = assetBundle.LoadAsset<Sprite>("texIconS1Riposte");
            rapierComboSkillDef.activationStateMachineName = "Weapon";
            rapierComboSkillDef.interruptPriority = EntityStates.InterruptPriority.Skill;
            rapierComboSkillDef.baseMaxStock = 1;
            rapierComboSkillDef.baseRechargeInterval = 1f;
            rapierComboSkillDef.isCombatSkill = true;
            rapierComboSkillDef.mustKeyPress = true;
            rapierComboSkillDef.rechargeStock = 1;
            rapierComboSkillDef.stockToConsume = 1;

            // Setup new 7-step state machine arrays
            rapierComboSkillDef.comboStates = new SerializableEntityStateType[] {
                new SerializableEntityStateType(typeof(SkillStates.RapierComboIdle)),         // Cooldown
                new SerializableEntityStateType(typeof(SkillStates.Riposte)),                 // Riposte
                new SerializableEntityStateType(typeof(SkillStates.Zwerchhau)),               // Zwerchhau
                new SerializableEntityStateType(typeof(SkillStates.Redoublement)),            // Redoublement
                new SerializableEntityStateType(typeof(SkillStates.EnchantedRiposte)),        // EnchRiposte
                new SerializableEntityStateType(typeof(SkillStates.EnchantedZwerchhau)),      // EnchZwerchhau
                new SerializableEntityStateType(typeof(SkillStates.EnchantedRedoublement))    // EnchRedoublement
            };
            rapierComboSkillDef.comboIcons = new Sprite[] {
                assetBundle.LoadAsset<Sprite>("texIconS1Riposte"),        // Cooldown
                assetBundle.LoadAsset<Sprite>("texIconS1Riposte"),            // Riposte
                assetBundle.LoadAsset<Sprite>("texIconS2Zwerchhau"),          // Zwerchhau
                assetBundle.LoadAsset<Sprite>("texIconS3Redoublement"),       // Redoublement
                assetBundle.LoadAsset<Sprite>("texIconES1Riposte"),           // EnchRiposte
                assetBundle.LoadAsset<Sprite>("texIconES2Zwerchhau"),         // EnchZwerchhau
                assetBundle.LoadAsset<Sprite>("texIconES3Redoublement")       // EnchRedoublement
            };
            rapierComboSkillDef.comboSkillNameTokens = new string[] {
                ALISAIE_PREFIX + "SPECIAL_RAPIER_COMBO_NAME",         // Cooldown
                ALISAIE_PREFIX + "SPECIAL_RIPOSTE_NAME",              // Riposte
                ALISAIE_PREFIX + "SPECIAL_ZWERCHHAU_NAME",            // Zwerchhau
                ALISAIE_PREFIX + "SPECIAL_REDOB_NAME",                // Redoublement
                ALISAIE_PREFIX + "SPECIAL_RIPOSTE_NAME_ENCH",         // EnchRiposte
                ALISAIE_PREFIX + "SPECIAL_ZWERCHHAU_NAME_ENCH",       // EnchZwerchhau
                ALISAIE_PREFIX + "SPECIAL_REDOB_NAME_ENCH"            // EnchRedoublement
            };
            rapierComboSkillDef.comboSkillDescriptionTokens = new string[] {
                ALISAIE_PREFIX + "SPECIAL_RAPIER_COMBO_DESCRIPTION",      // Cooldown
                ALISAIE_PREFIX + "SPECIAL_RIPOSTE_DESCRIPTION",           // Riposte
                ALISAIE_PREFIX + "SPECIAL_ZWERCHHAU_DESCRIPTION",         // Zwerchhau
                ALISAIE_PREFIX + "SPECIAL_REDOB_DESCRIPTION",             // Redoublement
                ALISAIE_PREFIX + "SPECIAL_RIPOSTE_DESCRIPTION_ENCH",      // EnchRiposte
                ALISAIE_PREFIX + "SPECIAL_ZWERCHHAU_DESCRIPTION_ENCH",    // EnchZwerchhau
                ALISAIE_PREFIX + "SPECIAL_REDOB_DESCRIPTION_ENCH"         // EnchRedoublement
            };

            Skills.AddSpecialSkills(bodyPrefab, rapierComboSkillDef);
            this.rapierComboSkillDef = rapierComboSkillDef;
            #endregion rapierComboSkillDef
        }

        private void AddVoiceOptionSkills()
        {
            // Create a hidden generic skill for voice options
            GenericSkill voiceOptionSkill = Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, "VoiceOption", true);
            voiceOptionsSkillFamily = voiceOptionSkill.skillFamily;

            // Create Voice On SkillDef
            voiceOnSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "VoiceOn",
                skillNameToken = ALISAIE_PREFIX + "VOICE_ON_NAME",
                skillDescriptionToken = ALISAIE_PREFIX + "VOICE_ON_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texIconVoiceOn"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "Body",
                interruptPriority = EntityStates.InterruptPriority.Any,
                baseRechargeInterval = 0f,
                baseMaxStock = 1,
                isCombatSkill = false,
                mustKeyPress = false,
            });

            // Create Voice Off SkillDef
            voiceOffSkillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "VoiceOff",
                skillNameToken = ALISAIE_PREFIX + "VOICE_OFF_NAME",
                skillDescriptionToken = ALISAIE_PREFIX + "VOICE_OFF_DESCRIPTION",
                skillIcon = assetBundle.LoadAsset<Sprite>("texIconVoiceOff"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "Body",
                interruptPriority = EntityStates.InterruptPriority.Any,
                baseRechargeInterval = 0f,
                baseMaxStock = 1,
                isCombatSkill = false,
                mustKeyPress = false,
            });

            Skills.AddSkillsToFamily(voiceOptionsSkillFamily, voiceOnSkillDef, voiceOffSkillDef);
        }
        #endregion skills

        #region skins
        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();
            #region endwalkerSkin
            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;
            List<SkinDef> skins = new List<SkinDef>();

            // --- Endwalker (Default) Skin ---
            //Skins materials and mesh orders are based on the customrendererinfo above
            Material[] ewMaterials = new Material[]
            {
                assetBundle.LoadMaterial("mat_ew_earring"),    // ew_earring
                assetBundle.LoadMaterial("mat_summer_eye"),    // summer_eye
                assetBundle.LoadMaterial("mat_summer_face"),   // summer_face
                assetBundle.LoadMaterial("mat_ew_hair"),       // ew_hair
                assetBundle.LoadMaterial("mat_ew_hair_acc"),   // ew_hair_acc
                assetBundle.LoadMaterial("mat_ew_skin"),       // ew_hands
                assetBundle.LoadMaterial("mat_ew_lashes"),     // ew_lashes
                assetBundle.LoadMaterial("mat_ew_top"),        // ew_top
                assetBundle.LoadMaterial("mat_summer_shorts"), // extraRenderer1
                assetBundle.LoadMaterial("mat_shb_shoes"),     // extraRenderer2
                assetBundle.LoadMaterial("mat_shb_rings"),     // extraRenderer3
                assetBundle.LoadMaterial("mat_summer_shoes"),  // extraRenderer4
                assetBundle.LoadMaterial("mat_summer_eyeglass"), // extraRenderer5
                assetBundle.LoadMaterial("mat_ew_focus"),      // w2351b0012 Part 0.0 (focus)
                assetBundle.LoadMaterial("mat_ew_focus"),      // w2351b0012 Part 0.1 (focus)
                assetBundle.LoadMaterial("mat_ew_weapon"),     // w2301b0016 Part 0.0 (weapon)
                assetBundle.LoadMaterial("mat_ew_weapon"),     // w2301b0016 Part 0.1 (weapon)
                assetBundle.LoadMaterial("mat_ew_focus"),      // fakeFocus1
                assetBundle.LoadMaterial("mat_ew_focus")       // fakeFocus2
            };

            string[] ewMeshes = new string[]
            {
                "ew_earring",        // ew_earring
                "summer_eye",        // ew_eyes
                "summer_face",       // ew_face
                "ew_hair",           // ew_hair
                "ew_hair_acc",       // ew_hair_acc
                "ew_hands",          // ew_hands
                "ew_lashes",         // ew_lashes
                "ew_top",            // ew_top
                "nullMesh",          // extraRenderer1
                "nullMesh",          // extraRenderer2
                "nullMesh",          // extraRenderer3
                "nullMesh",          // extraRenderer4
                "nullMesh",          // extraRenderer5
                "w2351b0012 Part 0.0", // w2351b0012 Part 0.0 (focus)
                "w2351b0012 Part 0.1", // w2351b0012 Part 0.1 (focus)
                "w2301b0016 Part 0.0", // w2301b0016 Part 0.0 (weapon)
                "w2301b0016 Part 0.1", // w2301b0016 Part 0.1 (weapon)
                "w2351b0012 Part 0.0", // fakeFocus1
                "w2351b0012 Part 0.1"  // fakeFocus2
            };

            CharacterModel.RendererInfo[] ewRendererInfos = Modules.Skins.getRendererMaterials(defaultRendererinfos, ewMaterials);
            SkinDef.MeshReplacement[] ewMeshReplacements = Modules.Skins.getMeshReplacements(
                assetBundle,
                defaultRendererinfos,
                ewMeshes
            );

            SkinDef ewSkin = Modules.Skins.CreateSkinDef(
                ALISAIE_PREFIX + "ENDWALKER_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texEndwalkerSkin"),
                ewRendererInfos,
                prefabCharacterModel.gameObject
            );
            ewSkin.meshReplacements = ewMeshReplacements;
            skins.Add(ewSkin);

            #endregion

            #region ShadowbringersSkin
            // --- Shadowbringers Skin ---
            Material[] shbMaterials = new Material[]
            {
                assetBundle.LoadMaterial("mat_ew_earring"),    // ew_earring
                assetBundle.LoadMaterial("mat_summer_eye"),    // summer_eye
                assetBundle.LoadMaterial("mat_summer_face"),   // summer_face
                assetBundle.LoadMaterial("mat_ew_hair"),       // ew_hair
                assetBundle.LoadMaterial("mat_ew_hair_acc"),   // ew_hair_acc
                assetBundle.LoadMaterial("mat_summer_skin"),   // shb_hands
                assetBundle.LoadMaterial("mat_ew_lashes"),     // ew_lashes
                assetBundle.LoadMaterial("mat_shb_top"),       // shb_top
                assetBundle.LoadMaterial("mat_shb_rings"),     // extraRenderer1
                assetBundle.LoadMaterial("mat_shb_shorts"),    // extraRenderer2
                assetBundle.LoadMaterial("mat_shb_shoes"),     // extraRenderer3
                assetBundle.LoadMaterial("mat_summer_shoes"),  // extraRenderer4
                assetBundle.LoadMaterial("mat_summer_eyeglass"), // extraRenderer5
                assetBundle.LoadMaterial("mat_shb_focus"),     // w2351b0012 Part 0.0 (focus)
                assetBundle.LoadMaterial("mat_ew_focus"),      // w2351b0012 Part 0.1 (focus)
                assetBundle.LoadMaterial("mat_shb_weapon"),    // w2301b0016 Part 0.0 (weapon)
                assetBundle.LoadMaterial("mat_ew_weapon"),     // w2301b0016 Part 0.1 (weapon)
                assetBundle.LoadMaterial("mat_shb_focus"),     // fakeFocus1
                assetBundle.LoadMaterial("mat_ew_focus")       // fakeFocus2
            };

            string[] shbMeshes = new string[]
            {
                "ew_earring",    // ew_earring
                "summer_eye",    // ew_eyes
                "summer_face",   // ew_face
                "ew_hair",       // ew_hair
                "ew_hair_acc",   // ew_hair_acc
                "shb_hands",     // ew_hands
                "ew_lashes",     // ew_lashes
                "shb_top",       // ew_top
                "shb_rings",     // extraRenderer1
                "shb_shorts",    // extraRenderer2
                "shb_shoes",     // extraRenderer3
                "nullMesh",      // extraRenderer4
                "nullMesh",      // extraRenderer5
                "shb_focus",     // w2351b0012 Part 0.0 (focus)
                "nullMesh",      // w2351b0012 Part 0.1 (focus)
                "shb_weapon",    // w2301b0016 Part 0.0 (weapon)
                "nullMesh",      // w2301b0016 Part 0.1 (weapon)
                "shb_focus",     // fakeFocus1
                "nullMesh"       // fakeFocus2
            };

            CharacterModel.RendererInfo[] shbRendererInfos = Modules.Skins.getRendererMaterials(defaultRendererinfos, shbMaterials);
            SkinDef.MeshReplacement[] shbMeshReplacements = Modules.Skins.getMeshReplacements(
                assetBundle,
                defaultRendererinfos,
                shbMeshes
            );

            SkinDef shbSkin = Modules.Skins.CreateSkinDef(
                ALISAIE_PREFIX + "SHADOWBRINGERS_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texShadowbringersSkin"),
                shbRendererInfos,
                prefabCharacterModel.gameObject,
                AlisaieUnlockables.shadowbringersSkinUnlockableDef
            );
            shbSkin.meshReplacements = shbMeshReplacements;
            skins.Add(shbSkin);
            #endregion

            #region SummerSkin
            // --- Summer Skin ---
            Material[] summerMaterials = new Material[]
            {
                assetBundle.LoadMaterial("mat_ew_earring"),      // ew_earring
                assetBundle.LoadMaterial("mat_summer_eye"),      // summer_eye
                assetBundle.LoadMaterial("mat_summer_face"),     // summer_face
                assetBundle.LoadMaterial("mat_ew_hair"),         // ew_hair
                assetBundle.LoadMaterial("mat_ew_hair_acc"),     // ew_hair_acc
                assetBundle.LoadMaterial("mat_summer_skin"),     // summer_arms
                assetBundle.LoadMaterial("mat_ew_lashes"),       // ew_lashes
                assetBundle.LoadMaterial("mat_summer_top"),      // summer_top
                assetBundle.LoadMaterial("mat_summer_gloves"),   // summer_gloves
                assetBundle.LoadMaterial("mat_summer_shorts"),   // summer_shorts
                assetBundle.LoadMaterial("mat_summer_skin"),     // summer_legs
                assetBundle.LoadMaterial("mat_summer_shoes"),    // summer_shoes
                assetBundle.LoadMaterial("mat_summer_eyeglass"), // summer_eyeglass
                assetBundle.LoadMaterial("mat_summer_focus"),    // w2351b0012 Part 0.0 (focus)
                assetBundle.LoadMaterial("mat_ew_focus"),        // w2351b0012 Part 0.1 (focus)
                assetBundle.LoadMaterial("mat_summer_weapon"),   // w2301b0016 Part 0.0 (weapon)
                assetBundle.LoadMaterial("mat_ew_weapon"),       // w2301b0016 Part 0.1 (weapon)
                assetBundle.LoadMaterial("mat_summer_focus"),    // fakeFocus1
                assetBundle.LoadMaterial("mat_ew_focus")         // fakeFocus2
            };

            string[] summerMeshes = new string[]
            {
                "ew_earring",      // ew_earring
                "summer_eye",      // ew_eyes
                "summer_face",     // ew_face
                "ew_hair",         // ew_hair 
                "ew_hair_acc",     // ew_hair_acc
                "summer_arms",     // ew_hands
                "ew_lashes",       // ew_lashes
                "summer_top",      // ew_top
                "summer_gloves",   // extraRenderer1
                "summer_shorts",   // extraRenderer2 
                "summer_legs",     // extraRenderer3
                "summer_shoes",    // extraRenderer4
                "summer_eyeglass", // extraRenderer5
                "summer_focus",    // w2351b0012 Part 0.0 (focus)
                "nullMesh",        // w2351b0012 Part 0.1 (focus)
                "summer_weapon",   // w2301b0016 Part 0.0 (weapon)
                "nullMesh",        // w2301b0016 Part 0.1 (weapon)
                "summer_focus",    // fakeFocus1
                "nullMesh"         // fakeFocus2
            };

            CharacterModel.RendererInfo[] summerRendererInfos = Modules.Skins.getRendererMaterials(defaultRendererinfos, summerMaterials);
            SkinDef.MeshReplacement[] summerMeshReplacements = Modules.Skins.getMeshReplacements(
                assetBundle,
                defaultRendererinfos,
                summerMeshes
            );

            SkinDef summerSkin = Modules.Skins.CreateSkinDef(
                ALISAIE_PREFIX + "SUMMER_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texSummerSkin"),
                summerRendererInfos,
                prefabCharacterModel.gameObject,
                AlisaieUnlockables.summerSkinUnlockableDef
            );
            summerSkin.meshReplacements = summerMeshReplacements;
            skins.Add(summerSkin);
            #endregion

            #region WinterSkin
            // --- Winter Skin ---
            Material[] winterMaterials = new Material[]
            {
                assetBundle.LoadMaterial("mat_ew_earring"),      // ew_earring
                assetBundle.LoadMaterial("mat_summer_eye"),      // ew_eyes
                assetBundle.LoadMaterial("mat_summer_face"),     // ew_face
                assetBundle.LoadMaterial("mat_ew_hair"),         // ew_hair
                assetBundle.LoadMaterial("mat_ew_hair_acc"),     // ew_hair_acc
                assetBundle.LoadMaterial("mat_winter_gloves"),   // ew_hands
                assetBundle.LoadMaterial("mat_ew_lashes"),       // ew_lashes
                assetBundle.LoadMaterial("mat_winter_top"),      // ew_top
                assetBundle.LoadMaterial("mat_winter_shoes"),    // extraRenderer1
                assetBundle.LoadMaterial("mat_winter_shorts"),   // extraRenderer2
                assetBundle.LoadMaterial("mat_shb_shoes"),       // extraRenderer3
                assetBundle.LoadMaterial("mat_summer_shoes"),    // extraRenderer4
                assetBundle.LoadMaterial("mat_summer_eyeglass"), // extraRenderer5
                assetBundle.LoadMaterial("mat_ew_focus"),        // w2351b0012 Part 0.0 (focus)
                assetBundle.LoadMaterial("mat_ew_focus"),        // w2351b0012 Part 0.1 (focus)
                assetBundle.LoadMaterial("mat_ew_weapon"),       // w2301b0016 Part 0.0 (weapon)
                assetBundle.LoadMaterial("mat_ew_weapon"),       // w2301b0016 Part 0.1 (weapon)
                assetBundle.LoadMaterial("mat_ew_focus"),        // fakeFocus1
                assetBundle.LoadMaterial("mat_ew_focus")         // fakeFocus2
            };

            string[] winterMeshes = new string[]
            {
                "ew_earring",      // ew_earring
                "summer_eye",      // ew_eyes
                "summer_face",     // ew_face
                "ew_hair",         // ew_hair
                "ew_hair_acc",     // ew_hair_acc
                "winter_gloves",   // ew_hands
                "ew_lashes",       // ew_lashes
                "winter_top",      // ew_top
                "winter_shoes",    // extraRenderer1
                "winter_shorts",   // extraRenderer2
                "nullMesh",        // extraRenderer3
                "nullMesh",        // extraRenderer4
                "nullMesh",        // extraRenderer5
                "w2351b0012 Part 0.0", // w2351b0012 Part 0.0 (focus)
                "w2351b0012 Part 0.1", // w2351b0012 Part 0.1 (focus)
                "w2301b0016 Part 0.0", // w2301b0016 Part 0.0 (weapon)
                "w2301b0016 Part 0.1", // w2301b0016 Part 0.1 (weapon)
                "w2351b0012 Part 0.0", // fakeFocus1
                "w2351b0012 Part 0.1"  // fakeFocus2
            };

            CharacterModel.RendererInfo[] winterRendererInfos = Modules.Skins.getRendererMaterials(defaultRendererinfos, winterMaterials);
            SkinDef.MeshReplacement[] winterMeshReplacements = Modules.Skins.getMeshReplacements(
                assetBundle,
                defaultRendererinfos,
                winterMeshes
            );

            SkinDef winterSkin = Modules.Skins.CreateSkinDef(
                ALISAIE_PREFIX + "WINTER_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texWinterSkin"),
                winterRendererInfos,
                prefabCharacterModel.gameObject,
                AlisaieUnlockables.winterSkinUnlockableDef
            );
            winterSkin.meshReplacements = winterMeshReplacements;
            skins.Add(winterSkin);
            #endregion

            #region StormbloodSkin
            // --- Stormblood Skin ---
            Material[] sbMaterials = new Material[]
            {
                assetBundle.LoadMaterial("mat_ew_earring"),      // ew_earring
                assetBundle.LoadMaterial("mat_summer_eye"),      // ew_eyes
                assetBundle.LoadMaterial("mat_summer_face"),     // ew_face
                assetBundle.LoadMaterial("mat_ew_hair"),         // ew_hair
                assetBundle.LoadMaterial("mat_ew_hair_acc"),     // ew_hair_acc
                assetBundle.LoadMaterial("mat_summer_skin"),     // sb_hands
                assetBundle.LoadMaterial("mat_ew_lashes"),       // ew_lashes
                assetBundle.LoadMaterial("mat_sb_top"),          // sb_top
                assetBundle.LoadMaterial("mat_sb_shorts"),       // extraRenderer1
                assetBundle.LoadMaterial("mat_sb_shoes"),        // extraRenderer2
                assetBundle.LoadMaterial("mat_sb_top2"),         // extraRenderer3
                assetBundle.LoadMaterial("mat_summer_shoes"),    // extraRenderer4
                assetBundle.LoadMaterial("mat_summer_eyeglass"), // extraRenderer5
                assetBundle.LoadMaterial("mat_ew_focus"),        // w2351b0012 Part 0.0 (focus)
                assetBundle.LoadMaterial("mat_ew_focus"),        // w2351b0012 Part 0.1 (focus)
                assetBundle.LoadMaterial("mat_ew_weapon"),       // w2301b0016 Part 0.0 (weapon)
                assetBundle.LoadMaterial("mat_ew_weapon"),       // w2301b0016 Part 0.1 (weapon)
                assetBundle.LoadMaterial("mat_ew_focus"),        // fakeFocus1
                assetBundle.LoadMaterial("mat_ew_focus")         // fakeFocus2
            };

            string[] sbMeshes = new string[]
            {
                "ew_earring",      // ew_earring
                "summer_eye",      // ew_eyes
                "summer_face",     // ew_face
                "ew_hair",         // ew_hair
                "ew_hair_acc",     // ew_hair_acc
                "sb_hands",        // ew_hands
                "ew_lashes",       // ew_lashes
                "sb_top",          // ew_top
                "sb_shorts",       // extraRenderer1
                "sb_shoes",        // extraRenderer2
                "sb_top2",         // extraRenderer3
                "nullMesh",        // extraRenderer4
                "nullMesh",        // extraRenderer5
                "w2351b0012 Part 0.0", // w2351b0012 Part 0.0 (focus) 
                "w2351b0012 Part 0.1", // w2351b0012 Part 0.1 (focus)
                "w2301b0016 Part 0.0", // w2301b0016 Part 0.0 (weapon)
                "w2301b0016 Part 0.1", // w2301b0016 Part 0.1 (weapon)
                "w2351b0012 Part 0.0", // fakeFocus1
                "w2351b0012 Part 0.1"  // fakeFocus2
            };

            CharacterModel.RendererInfo[] sbRendererInfos = Modules.Skins.getRendererMaterials(defaultRendererinfos, sbMaterials);
            SkinDef.MeshReplacement[] sbMeshReplacements = Modules.Skins.getMeshReplacements(
                assetBundle,
                defaultRendererinfos,
                sbMeshes
            );

            SkinDef sbSkin = Modules.Skins.CreateSkinDef(
                ALISAIE_PREFIX + "STORMBLOOD_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texStormbloodSkin"),
                sbRendererInfos,
                prefabCharacterModel.gameObject,
                AlisaieUnlockables.masterySkinUnlockableDef
            );
            sbSkin.meshReplacements = sbMeshReplacements;
            skins.Add(sbSkin);
            #endregion

            skinController.skins = skins.ToArray();

        }
        #endregion skins

        //Character Master is what governs the AI of your character when it is not controlled by a player (artifact of vengeance, goobo)
        public override void InitializeCharacterMaster()
        {
            //you must only do one of these. adding duplicate masters breaks the game.

            //if you're lazy or prototyping you can simply copy the AI of a different character to be used
            //Modules.Prefabs.CloneDopplegangerMaster(bodyPrefab, masterName, "Merc");

            //how to set up AI in code
            AlisaieAI.Init(bodyPrefab, masterName);

            //how to load a master set up in unity, can be an empty gameobject with just AISkillDriver components
            //assetBundle.LoadMaster(bodyPrefab, masterName);
        }

        private void AddHooks()
        {
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }
        
        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            orig(self, damageInfo);
            
            // Check if this is Alisaie taking damage
            if (self.body && self.body.baseNameToken == ALISAIE_PREFIX + "NAME" && damageInfo.damage > 0)
            {
                // Check if this damage would kill Alisaie
                if (self.health <= 0 && self.alive)
                {
                    AlisaieVoiceSFXHelper.PlayRandomDeathVFX(1f, self.gameObject);
                }
                else
                {
                    // 5% chance to play hurt sound when damaged but not killed
                    AlisaieVoiceSFXHelper.PlayRandomHurtVFX(0.05f, self.gameObject);
                }
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {

            if (sender.HasBuff(AlisaieBuffs.armorBuff))
            {
                args.armorAdd += 300;
            }
        }
    }
}