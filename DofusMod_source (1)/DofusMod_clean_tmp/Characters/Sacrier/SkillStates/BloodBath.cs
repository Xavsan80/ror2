using EntityStates;
using RoR2;
using UnityEngine;

namespace DofusMod.Characters.Sacrier.SkillStates
{
    // ── Blood Bath ──────────────────────────────────────────────────────────
    // Dash toward the nearest enemy. Damage scales with missing HP:
    //   Total = 300% + 1% per 1% missing health  (max ~400% at 1 HP)
    public class BloodBath : BaseSkillState
    {
        public static float BaseDamageCoeff    = 3.0f;
        public static float MissingHpCoeff     = 1.0f; // multiplied by missing% fraction
        public static float DashSpeed          = 40f;
        public static float DashDuration       = 0.30f;
        public static float BlastRadius        = 3.5f;
        public static float DashStartFraction  = 0.05f;

        private Vector3     _dashDirection;
        private float       _stopwatch;
        private bool        _hasFired;
        private HurtBox     _target;

        public override void OnEnter()
        {
            base.OnEnter();
            _stopwatch = 0f;
            _hasFired  = false;

            _target = FindNearestEnemy();

            if (_target != null)
                _dashDirection = (_target.transform.position - characterBody.corePosition).normalized;
            else
                _dashDirection = characterDirection.forward;

            PlayAnimation("FullBody, Override", "Dash");

            if (isAuthority)
                characterMotor.velocity = Vector3.zero;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            _stopwatch += Time.fixedDeltaTime;

            if (isAuthority)
            {
                // Propel body during dash
                if (_stopwatch < DashDuration)
                    characterMotor.rootMotion += _dashDirection * DashSpeed * Time.fixedDeltaTime;

                // Hit at ~70% through the dash
                if (!_hasFired && _stopwatch >= DashDuration * 0.70f)
                {
                    _hasFired = true;
                    FireImpact();
                }

                if (_stopwatch >= DashDuration)
                    outer.SetNextStateToMain();
            }
        }

        private void FireImpact()
        {
            float maxHp     = healthComponent.fullCombinedHealth;
            float curHp     = healthComponent.combinedHealth;
            float missingPct = (maxHp - curHp) / maxHp;           // 0–1
            float totalCoeff = BaseDamageCoeff + MissingHpCoeff * missingPct;

            var blast = new BlastAttack
            {
                attacker        = gameObject,
                inflictor       = gameObject,
                teamIndex       = teamComponent.teamIndex,
                position        = characterBody.corePosition,
                radius          = BlastRadius,
                baseDamage      = damageStat * totalCoeff,
                baseForce       = 600f,
                crit            = RollCrit(),
                damageType      = DamageType.Generic,
                falloffModel    = BlastAttack.FalloffModel.None,
                procCoefficient = 1f
            };
            blast.Fire();

            // Visual feedback — swap for a custom effect once you have assets
            EffectManager.SpawnEffect(
                LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/BeetleGuardSlamImpact"),
                new EffectData { origin = characterBody.corePosition, scale = 1.5f },
                true);
        }

        private HurtBox FindNearestEnemy()
        {
            var sphere = new SphereSearch
            {
                origin      = characterBody.corePosition,
                radius      = 30f,
                mask        = LayerIndex.entityPrecise.mask,
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

        public override void OnExit()
        {
            base.OnExit();
            // Restore normal friction
            if (characterMotor) characterMotor.velocity *= 0.2f;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
            => InterruptPriority.Skill;
    }
}
