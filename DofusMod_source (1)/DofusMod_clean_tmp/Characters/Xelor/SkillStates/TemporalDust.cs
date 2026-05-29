using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace DofusMod.Characters.Xelor.SkillStates
{
    // ── Temporal Dust ───────────────────────────────────────────────────────
    // 3-shot burst of slowing projectiles. Each hit applies Slow80 for 1.5s.
    public class TemporalDust : BaseSkillState
    {
        public static float    DamageCoeff   = 0.60f;
        public static float    FireRate      = 0.12f;  // seconds between shots
        public static float    ExitDelay     = 0.20f;
        public static int      ShotCount     = 3;
        public static float    Speed         = 80f;
        public static float    SlowDuration  = 1.5f;

        private int   _shotsFired = 0;
        private float _fireTimer  = 0f;
        private float _exitTimer  = 0f;
        private bool  _finished   = false;

        // Use Artificer's ice bolt as a placeholder; replace with a custom
        // blue time-stream prefab once assets are available.
        private static GameObject _projectilePrefab =>
            LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageIceBolt");

        public override void OnEnter()
        {
            base.OnEnter();
            _shotsFired = 0;
            _fireTimer  = 0f;
            _exitTimer  = 0f;
            _finished   = false;

            PlayAnimation("Gesture, Additive", "FireSeekingShot");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!_finished)
            {
                _fireTimer += Time.fixedDeltaTime;
                if (isAuthority && _fireTimer >= FireRate)
                {
                    _fireTimer -= FireRate;
                    FireShot();
                    _shotsFired++;
                    if (_shotsFired >= ShotCount) _finished = true;
                }
            }
            else
            {
                _exitTimer += Time.fixedDeltaTime;
                if (isAuthority && _exitTimer >= ExitDelay)
                    outer.SetNextStateToMain();
            }
        }

        private void FireShot()
        {
            var aimRay = GetAimRay();

            // Small spread between shots
            float spread = Random.Range(-2f, 2f);
            var dir = Quaternion.Euler(0f, spread, 0f) * aimRay.direction;

            ProjectileManager.instance.FireProjectile(
                _projectilePrefab,
                aimRay.origin,
                Util.QuaternionSafeLookRotation(dir),
                gameObject,
                damageStat * DamageCoeff,
                0f,
                RollCrit(),
                DamageColorIndex.Default,
                null,
                Speed);
        }

        // We apply the slow via an On.RoR2.GlobalEventManager hook registered
        // in DofusPlugin.Awake so it doesn't get duplicated per-state-instance.
        // Alternatively attach a SlowOnHit component to the projectile prefab.

        public override InterruptPriority GetMinimumInterruptPriority()
            => InterruptPriority.Skill;
    }
}
