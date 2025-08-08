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
    public class Verholy : AOESpell
    {
        public static event System.Action<CharacterBody, GameObject> OnAlisaieFinisherKill;

        protected override float BaseDuration => 1.0f;
        protected override float DamageCoefficient => AlisaieStaticValues.verholyDamageCoefficient;
        protected override string CastAnim => "cast_end4";
        protected override string AnimLayer => "Gesture, Override";
        protected override GameObject CastVFX => AlisaieAssets.VFXcastVerholy;
        protected override GameObject AoeVFX => AlisaieAssets.VFXaoeVerholy;
        protected override GameObject ImpactVFX => AlisaieAssets.VFXimpactVerholy;

        protected override string AuraVFXName => "VFXauraVerholy";
        protected override string CastSFX => AlisaieStaticValues.PlayFinisherHoly;
        protected override string ImpactSFX => AlisaieStaticValues.PlayFinisherHoly;
        protected override string SpellName => "verholy";
        protected override int ManaAmount => 11;
        protected override bool IsWhiteMana => true;
        protected override DamageType ExtraDamageType => DamageType.BonusToLowHealth;
        protected override float AOERadius => 12f;
        protected override float PullForce => 0f;
        protected override float ProcCoefficient => 1f;
        protected override float OrbSpeed => 80f;
        protected override GameObject AOEHitboxPrefab => AlisaieAssets.HitboxVerholyAoeGroup;
        protected override bool UseAOEHitbox => true;
        public override void OnEnter()
        {
            base.OnEnter();
            
            string[] voices = new string[]
            {
                "Play_VFXGiveThemAShow",
                //"Play_VFXHealing"
            };
            AlisaieVoiceSFXHelper.PlayRandomFromArrayVFX(0.5f, voices, gameObject);
        }
        protected override void OnAOEOrbFire()
        {
            GetComponent<AlisaieSpellSwitcher>()?.ConsumeFinisher();
            Vector3 healCenter = transform.position;
            var target = autoAimComponent?.GetCurrentTarget();
            if (target && target.transform)
                healCenter = target.transform.position;
            HealAlliesInRadius(healCenter);

            base.OnAOEOrbFire();
        }

        private void HealAlliesInRadius(Vector3 center)
        {
            // Heal all allies (including self) in 24f radiu
            float healRadius = 24f;
            float healAmount = (DamageCoefficient * damageStat) / 2f;
            TeamIndex myTeam = TeamComponent.GetObjectTeam(gameObject);
            var sphereSearch = new SphereSearch();
            sphereSearch.origin = center;
            sphereSearch.radius = healRadius;
            sphereSearch.mask = LayerIndex.entityPrecise.mask;
            sphereSearch.queryTriggerInteraction = QueryTriggerInteraction.Collide;
            sphereSearch.RefreshCandidates();
            TeamMask teamMask = new TeamMask();
            teamMask.AddTeam(myTeam);
            sphereSearch.FilterCandidatesByHurtBoxTeam(teamMask);
            var results = new List<HurtBox>();
            sphereSearch.GetHurtBoxes(results);

            var healed = new HashSet<HealthComponent>();
            foreach (var hurtBox in results)
            {
                var hc = hurtBox.healthComponent;
                if (hc != null && healed.Add(hc))
                {
                    hc.Heal(healAmount, default, true);
                    //heal vfx spawn
                    if (AlisaieAssets.VFXhealVerholy && hc.body != null)
                    {
                        Vector3 vfxPos = hc.body.corePosition;
                        if (vfxPos == Vector3.zero)
                            vfxPos = hc.body.transform.position;
                        EffectManager.SpawnEffect(
                            AlisaieAssets.VFXhealVerholy,
                            new EffectData { origin = vfxPos, scale = 1f },
                            true
                        );
                    }
                }
            }
        }

    }
}