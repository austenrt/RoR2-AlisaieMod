using AlisaieMod.Characters.Survivors.Alisaie.Components;
using AlisaieMod.Characters.Survivors.Alisaie.Helpers;
using AlisaieMod.Characters.Survivors.Alisaie.Projectiles;
using AlisaieMod.Characters.Survivors.Alisaie.UI;
using EntityStates;
using RoR2;
using UnityEngine;

namespace AlisaieMod.Survivors.Alisaie.SkillStates
{
    public class Veraero : AOESpell
    {
        protected override float BaseDuration => 0.8f;
        protected override float DamageCoefficient => AlisaieStaticValues.veraeroDamageCoefficient;
        protected override string CastAnim => "cast_end3";
        protected override string AnimLayer => "Gesture, Override";
        protected override GameObject CastVFX => AlisaieAssets.VFXcastVeraero;
        protected override GameObject AoeVFX => AlisaieAssets.VFXaoeVeraero;
        protected override GameObject ImpactVFX => AlisaieAssets.VFXimpactVeraero;

        protected override string AuraVFXName => "VFXauraVeraero";
        protected override string CastSFX => AlisaieStaticValues.PlayAeroCast; 
        protected override string ImpactSFX => AlisaieStaticValues.PlayAeroImpact;
        protected override string SpellName => "veraero";
        protected override int ManaAmount => 7;
        protected override bool IsWhiteMana => true;
        protected override DamageType ExtraDamageType => DamageType.Generic;
        protected override float AOERadius => 18f;
        protected override float PullForce => 0f;
        protected override bool PullEnemies => true; // Pull enemies to centeree
        protected override float ProcCoefficient => 1f;
        protected override float OrbSpeed => 80f;

        protected override void OnAOEOrbFire()
        {
           GetComponent<AlisaieSpellSwitcher>()?.ConsumeDualcast();
           base.OnAOEOrbFire();
        }
    }
}