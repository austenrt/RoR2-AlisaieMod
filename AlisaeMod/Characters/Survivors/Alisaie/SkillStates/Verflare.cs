using AlisaieMod.Characters.Survivors.Alisaie.Components;
using AlisaieMod.Characters.Survivors.Alisaie.Helpers;
using AlisaieMod.Characters.Survivors.Alisaie.Projectiles;
using AlisaieMod.Characters.Survivors.Alisaie.UI;
using EntityStates;
using RoR2;
using UnityEngine;
using System.Collections.Generic;

namespace AlisaieMod.Survivors.Alisaie.SkillStates
{
    public class Verflare : AOESpell    
    {
        public static event System.Action<CharacterBody, GameObject> OnAlisaieFinisherKill;

        protected override float BaseDuration => 1.0f;
        protected override float DamageCoefficient => AlisaieStaticValues.verflareDamageCoefficient;
        protected override string CastAnim => "cast_end6";
        protected override string AnimLayer => "Gesture, Override";
        protected override GameObject CastVFX => AlisaieAssets.VFXcastVerflare;
        protected override GameObject AoeVFX => AlisaieAssets.VFXaoeVerflare;
        protected override GameObject ImpactVFX => AlisaieAssets.VFXimpactVerflare;
        
        protected override string AuraVFXName => "VFXauraVerflare";
        protected override string CastSFX => AlisaieStaticValues.PlayFinisherFlare;
        protected override string ImpactSFX => AlisaieStaticValues.PlayFinisherFlare;
        protected override string SpellName => "verflare";
        protected override int ManaAmount => 11;
        protected override bool IsWhiteMana => false; 
        protected override DamageType ExtraDamageType => DamageType.IgniteOnHit;
        protected override float AOERadius => 12f;
        protected override float PullForce => 0f;
        protected override float ProcCoefficient => 1f;
        protected override float OrbSpeed => 500f;

        public override void OnEnter()
        {
            base.OnEnter();
            AlisaieVoiceSFXHelper.PlayVFX(0.5f, "Play_VFXSmokingCrater", gameObject);
        }
        protected override void OnAOEOrbFire()
        {
            GetComponent<AlisaieSpellSwitcher>()?.ConsumeFinisher();
            Vector3 burnCenter = transform.position;
            var target = autoAimComponent?.GetCurrentTarget();
            if (target && target.transform)
                burnCenter = target.transform.position;
            ApplyBurnInLargeRadius(burnCenter);
            base.OnAOEOrbFire();
        }

        private void ApplyBurnInLargeRadius(Vector3 center)
        {
            // Find all enemies in 24f radius and apply burn (only once per enemy) 
            float burnRadius = 24f;
            TeamIndex myTeam = TeamComponent.GetObjectTeam(gameObject);
            TeamMask enemyTeams = TeamMask.GetEnemyTeams(myTeam);
            var sphereSearch = new SphereSearch();
            sphereSearch.origin = center;
            sphereSearch.radius = burnRadius;
            sphereSearch.mask = LayerIndex.entityPrecise.mask;
            sphereSearch.queryTriggerInteraction = QueryTriggerInteraction.Collide;
            sphereSearch.RefreshCandidates();
            sphereSearch.FilterCandidatesByHurtBoxTeam(enemyTeams);
            var results = new List<HurtBox>();
            sphereSearch.GetHurtBoxes(results);

            var selfHealthComponent = characterBody?.healthComponent;
            var burned = new HashSet<HealthComponent>();
            foreach (var hurtBox in results)
            {
                var hc = hurtBox.healthComponent;
                if (hc != null && hc != selfHealthComponent && burned.Add(hc))
                {
                    DotController.InflictDot(hc.gameObject, gameObject, DotController.DotIndex.Burn, 3f, 1, null);
                }
            }
        }
    }
}