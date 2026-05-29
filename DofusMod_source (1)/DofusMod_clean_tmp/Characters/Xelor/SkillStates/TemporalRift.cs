using EntityStates;
using RoR2;
using UnityEngine;

namespace DofusMod.Characters.Xelor.SkillStates
{
    // ── Temporal Rift ───────────────────────────────────────────────────────
    // Place a lingering zone (5s) that freezes enemies inside for 3s.
    // The cast state exits quickly; the zone stays as a world GameObject.
    public class TemporalRift : BaseSkillState
    {
        public static float CastDuration  = 0.70f;
        public static float ZoneDuration  = 5.0f;
        public static float ZoneRadius    = 12f;
        public static float FreezeDuration = 3.0f;

        private bool _spawned;

        public override void OnEnter()
        {
            base.OnEnter();
            _spawned = false;
            PlayAnimation("FullBody, Override", "SpecialActivate");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!_spawned && isAuthority && fixedAge >= CastDuration * 0.5f)
            {
                _spawned = true;
                SpawnRift();
            }

            if (isAuthority && fixedAge >= CastDuration)
                outer.SetNextStateToMain();
        }

        private void SpawnRift()
        {
            // Aim at the ground in front of the caster
            var aimRay = GetAimRay();
            Vector3 origin;

            if (Physics.Raycast(aimRay, out RaycastHit hit, 60f,
                LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
                origin = hit.point;
            else
                origin = characterBody.corePosition + aimRay.direction * 12f;

            // Instantiate the persistent zone driver
            var zoneObj = new GameObject("XelorTemporalRiftZone");
            zoneObj.transform.position = origin;
            zoneObj.AddComponent<TemporalRiftZone>().Init(
                gameObject,
                teamComponent.teamIndex,
                damageStat,
                ZoneDuration,
                ZoneRadius,
                FreezeDuration);

            // Ring effect placeholder
            EffectManager.SpawnEffect(
                LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/NullifierSpawnEffect"),
                new EffectData { origin = origin, scale = ZoneRadius * 0.1f },
                true);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
            => InterruptPriority.PrioritySkill;
    }

    // ── Zone MonoBehaviour ──────────────────────────────────────────────────
    // Runs server-side, polling in a sphere and applying Frozen to victims.
    public class TemporalRiftZone : MonoBehaviour
    {
        private GameObject _owner;
        private TeamIndex  _ownerTeam;
        private float      _damage;
        private float      _lifespan;
        private float      _radius;
        private float      _freezeDur;
        private float      _age;
        private float      _tickInterval = 0.5f;
        private float      _tickTimer;

        public void Init(GameObject owner, TeamIndex team, float damage,
                         float lifespan, float radius, float freezeDur)
        {
            _owner     = owner;
            _ownerTeam = team;
            _damage    = damage;
            _lifespan  = lifespan;
            _radius    = radius;
            _freezeDur = freezeDur;
        }

        private void FixedUpdate()
        {
            _age       += Time.fixedDeltaTime;
            _tickTimer += Time.fixedDeltaTime;

            if (_tickTimer >= _tickInterval)
            {
                _tickTimer = 0f;
                TickZone();
            }

            if (_age >= _lifespan) Destroy(gameObject);
        }

        private void TickZone()
        {
            var sphere = new SphereSearch
            {
                origin  = transform.position,
                radius  = _radius,
                mask    = LayerIndex.entityPrecise.mask,
                queryTriggerInteraction = QueryTriggerInteraction.UseGlobal
            };
            sphere.RefreshCandidates();
            sphere.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(_ownerTeam));
            sphere.FilterCandidatesByDistinctHurtBoxEntities();

            var hits = new System.Collections.Generic.List<HurtBox>();
            sphere.GetHurtBoxes(hits);

            foreach (var hb in hits)
            {
                var body = hb.healthComponent?.body;
                if (body == null) continue;

                // Apply freeze via the standard SetFrozen mechanism
                SetStateOnHurt.SetFrozenOnObject(body.gameObject, _freezeDur);
            }
        }
    }
}
