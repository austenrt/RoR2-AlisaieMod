using BepInEx;
using AlisaieMod.Survivors.Alisaie;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using AlisaieMod.Characters.Survivors.Alisaie.UI;
using System;
using System.Reflection;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]


namespace AlisaieMod
{
    //[BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    public class AlisaiePlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.austenrt.AlisaieMod";
        public const string MODNAME = "AlisaieMod";
        public const string MODVERSION = "1.0.3";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string DEVELOPER_PREFIX = "austenrt";

        public static AlisaiePlugin instance;

        void Awake()
        {
            instance = this;

            //easy to use logger 
            Log.Init(Logger);

            // used when you want to properly set up language folders
            Modules.Language.Init();

            // character initialization
            new AlisaieSurvivor().Initialize();

            // make a content pack and add it. this has to be last
            new Modules.ContentPacks().Initialize();

            AlisaieCastBarController.Init();

            AlisaieManaBarController.Init();
        }
    }
}
