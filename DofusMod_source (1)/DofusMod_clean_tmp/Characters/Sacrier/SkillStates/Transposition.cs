using EntityStates;
using RoR2;
using UnityEngine;

namespace DofusMod.Characters.Sacrier.SkillStates
{
    // ── Transposition ───────────────────────────────────────────────────────
    // Teleport-swap positions with the nearest enemy within range and briefly stun.
    public class Transposition : BaseSkillState
    {
        public static float SearchRadius  = 25f;
        public static float StunDuration  = 1.5f;
        public static float SkillDuration = 0.40f;  // brief cast animation

        private float   _stopwatch;
        private bool    _swapped;

        public override void OnEnter()
        {
            base.OnEnter();
            _stopwatch = 0f;
            _swapped   = false;

            PlayAnimation("FullBody, Override", "UtilityActivate");

            // Flash effect on caster
            EffectManager.SpawnEffect(
                LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/TeleportOutBoom"),
                new EffectData { origin = characterBody.corePosition },
                true);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            _stopwatch += Time.fixedDeltaTime;

            if (!_swapped && _stopwatch >= SkillDuration * 0.4f)
            {
                _swapped = true;
                PerformSwap();
            }

            if (isAuthority && _stopwatch >= SkillDuration)
                outer.SetNextStateToMain();
        }

        private void PerformSwap()
        {
            if (!isAuthority) return;

            HurtBox target = FindNearestEnemy();
            if (target == null) return;

            var targetBody = target.healthComponent?.body;
            if (targetBody == null) return;

            Vector3 myPos     = characterBody.footPosition;
            Vector3 enemyPos  = targetBody.footPosition;

            // Move caster to where the enemy was
            TeleportHelper.TeleportBody(characterBody, enemyPos);

            // Move enemy to where caster was
            TeleportHelper.TeleportBody(targetBody, myPos);

            // Stun the enemy
            targetBody.AddTimedBuff(RoR2Content.Buffs.Slow80, StunDuration);
            SetStateOnHurt.SetStunOnObject(targetBody.gameObject, StunDuration);

            // Visual at destination
            EffectManager.SpawnEffect(
                LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/TeleportOutBoom"),
                new EffectData { origin = enemyPos },
                true);
        }

        private HurtBox FindNearestEnemy()
        {
            var sphere = new SphereSearch
            {
                origin  = characterBody.corePosition,
                radius  = SearchRadius,
                mask    = LayerIndex.entityPrecise.mask,
                queryTriggerInteraction = QueryTriggerInteraction.UseGlobal
            };
            sphere.RefreshCandidates();
            sphere.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(teamComponent.teamIndex));
            sphere.FilterCandidatesByDistinctHurtBoxEntities();
            sphere.OrderCandidatesByDistance();

            var results = new System.Collections.Generic.List<HurtBox>();
            sphere.GetHurtBoxes(results);
            return results.Count > 0 ? results[0] : null;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
            => InterruptPriority.PrioritySkill;
    }
}
