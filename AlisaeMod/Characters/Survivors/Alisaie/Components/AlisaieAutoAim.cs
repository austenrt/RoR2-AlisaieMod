using UnityEngine;
using RoR2;
using AlisaieMod.Survivors.Alisaie;

namespace AlisaieMod.Characters.Survivors.Alisaie.Components
{
    //goes on characterbody prefab, handles auto-aim tracking
    public class AlisaieAutoAim : HuntressTracker
    {
        public float customMaxTrackingDistance = 60f;
        public float customMaxTrackingAngle = 35f;
        public float customTrackerUpdateFrequency = 15f;

        private new void Awake()
        {
            trackingPrefab = AlisaieAssets.reticlePrefab;
            base.Awake();
        }

        private new void Start()
        {
            base.Start();
            this.maxTrackingDistance = customMaxTrackingDistance;
            this.maxTrackingAngle = customMaxTrackingAngle;
            this.trackerUpdateFrequency = customTrackerUpdateFrequency;
        }

        public HurtBox GetCurrentTarget()
        {
            return GetTrackingTarget();
        }
    }
}
