using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using AlisaieMod.Modules;
using System;
using RoR2.Projectile;
using R2API;

namespace AlisaieMod.Survivors.Alisaie
{
    public static class AlisaieAssets
    {

        //Henry leftovers

        public static GameObject swordSwingEffect;
        public static GameObject swordHitImpactEffect;
        public static GameObject bombExplosionEffect;
        public static NetworkSoundEventDef swordHitSoundEvent;

        //effectmanager effects
        //public static GameObject dummyOrbEffect;
        
        // VFX - Impact
        public static GameObject VFXimpactJolt;
        public static GameObject VFXimpactVerthunder;
        public static GameObject VFXimpactSword;
        public static GameObject VFXimpactVeraero;
        public static GameObject VFXimpactVerholy;
        public static GameObject VFXhealVerholy;
        public static GameObject VFXimpactVerflare;

        //AOE VFX
        public static GameObject VFXaoeVeraero;
        public static GameObject VFXaoeVerholy;
        public static GameObject VFXaoeVerflare;

        // VFX - projectiles
        public static GameObject VFXcastJolt;
        public static GameObject VFXcastVerthunder;
        public static GameObject VFXcastVeraero;
        public static GameObject VFXcastVerholy;
        public static GameObject VFXcastVerflare;

        //HUD & UI
        public static GameObject reticlePrefab;
        public static GameObject castBar;
        public static GameObject manaBar;

        public static GameObject HitboxVerholyAoeGroup;

        private static AssetBundle _assetBundle;

        private static AssetBundle assetBundle => _assetBundle;

        public static void Init(AssetBundle assetBundle)
        {

            _assetBundle = assetBundle;

            swordHitSoundEvent = Content.CreateAndAddNetworkSoundEventDef("HenrySwordHit");
            

            //VFX - projectiles 
            VFXcastJolt = _assetBundle.LoadAsset<GameObject>("VFXcastJolt");
            VFXcastVerthunder = _assetBundle.LoadAsset<GameObject>("VFXcastVerthunder");
            VFXcastVeraero = _assetBundle.LoadAsset<GameObject>("VFXcastVeraero");
            VFXcastVerholy = _assetBundle.LoadAsset<GameObject>("VFXcastVerholy");
            VFXcastVerflare = _assetBundle.LoadAsset<GameObject>("VFXcastVerflare");



            //VFX - Impact
            VFXimpactVerthunder = _assetBundle.LoadAsset<GameObject>("VFXimpactVerthunder");
            VFXimpactJolt = _assetBundle.LoadAsset<GameObject>("VFXimpactJolt");
            VFXimpactVeraero = _assetBundle.LoadAsset<GameObject>("VFXimpactVeraero");
            VFXimpactVerholy = _assetBundle.LoadAsset<GameObject>("VFXimpactVerholy");
            VFXhealVerholy = _assetBundle.LoadAsset<GameObject>("VFXhealVerholy");
            VFXimpactVerflare = _assetBundle.LoadAsset<GameObject>("VFXimpactVerflare");
            VFXimpactSword = _assetBundle.LoadAsset<GameObject>("VFXimpactSword");

            //VFX - AOE
            VFXaoeVeraero = _assetBundle.LoadAsset<GameObject>("VFXaoeVeraero");
            VFXaoeVerholy = _assetBundle.LoadAsset<GameObject>("VFXaoeVerholy");
            VFXaoeVerflare = _assetBundle.LoadAsset<GameObject>("VFXaoeVerflare");

            HitboxVerholyAoeGroup = _assetBundle.LoadAsset<GameObject>("hitboxVerholyAoeGroup");

            //HUD
            reticlePrefab = _assetBundle.LoadAsset<GameObject>("AlisaieReticle");
            castBar = _assetBundle.LoadAsset<GameObject>("castBar");
            manaBar = _assetBundle.LoadAsset<GameObject>("manaBar");



            CreateEffects();

            CreateProjectiles();

        }

        public static AssetBundle GetAssetBundle()
        {
            return _assetBundle;
        }

        #region effects
        private static void CreateEffects()
        {

            swordSwingEffect = _assetBundle.LoadEffect("HenrySwordSwingEffect", true);
            swordHitImpactEffect = _assetBundle.LoadEffect("ImpactHenrySlash");
            
            // Add all impact and AOE VFX as effects with automatic component setup
            RegisterImpactEffect(VFXimpactSword);
            RegisterImpactEffect(VFXimpactJolt);
            RegisterImpactEffect(VFXimpactVerthunder);
            RegisterImpactEffect(VFXimpactVeraero);
            RegisterImpactEffect(VFXimpactVerholy);
            RegisterImpactEffect(VFXhealVerholy);
            RegisterImpactEffect(VFXimpactVerflare);

            RegisterImpactEffect(VFXaoeVeraero);
            RegisterImpactEffect(VFXaoeVerholy);
            RegisterImpactEffect(VFXaoeVerflare);

        }

        /**
         * I don't like using unity editor to add these components so im doing it here 
         */
        private static void RegisterImpactEffect(GameObject impactVFX)
        {
            if (!impactVFX) return;
            
            // Add EffectComponent if it doesn't exist
            if (!impactVFX.GetComponent<EffectComponent>())
            {
                var effectComponent = impactVFX.AddComponent<EffectComponent>();
                effectComponent.applyScale = true;
                effectComponent.parentToReferencedTransform = false;
            }
            
            // Add VFXAttributes for priority
            if (!impactVFX.GetComponent<VFXAttributes>())
            {
                var vfxAttributes = impactVFX.AddComponent<VFXAttributes>();
                vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            }
            
            ContentAddition.AddEffect(impactVFX);
        }
        #endregion effects

        #region projectiles
        private static void CreateProjectiles()
        {
            //CreateBombProjectile();
            //Content.AddProjectilePrefab(bombProjectilePrefab);
        }

        //Henry leftovers, not used in Alisaie
        //private static void CreateBombProjectile()
        //{
        //    //highly recommend setting up projectiles in editor, but this is a quick and dirty way to prototype if you want
        //    bombProjectilePrefab = Asset.CloneProjectilePrefab("CommandoGrenadeProjectile", "HenryBombProjectile");

        //    //remove their ProjectileImpactExplosion component and start from default values
        //    UnityEngine.Object.Destroy(bombProjectilePrefab.GetComponent<ProjectileImpactExplosion>());
        //    ProjectileImpactExplosion bombImpactExplosion = bombProjectilePrefab.AddComponent<ProjectileImpactExplosion>();

        //    bombImpactExplosion.blastRadius = 16f;
        //    bombImpactExplosion.blastDamageCoefficient = 1f;
        //    bombImpactExplosion.falloffModel = BlastAttack.FalloffModel.None;
        //    bombImpactExplosion.destroyOnEnemy = true;
        //    bombImpactExplosion.lifetime = 12f;
        //    bombImpactExplosion.impactEffect = bombExplosionEffect;
        //    bombImpactExplosion.lifetimeExpiredSound = Content.CreateAndAddNetworkSoundEventDef("HenryBombExplosion");
        //    bombImpactExplosion.timerAfterImpact = true;
        //    bombImpactExplosion.lifetimeAfterImpact = 0.1f;

        //    ProjectileController bombController = bombProjectilePrefab.GetComponent<ProjectileController>();

        //    if (_assetBundle.LoadAsset<GameObject>("HenryBombGhost") != null)
        //        bombController.ghostPrefab = _assetBundle.CreateProjectileGhostPrefab("HenryBombGhost");

        //    bombController.startSound = "";
        //}
        #endregion projectiles

    }
}
