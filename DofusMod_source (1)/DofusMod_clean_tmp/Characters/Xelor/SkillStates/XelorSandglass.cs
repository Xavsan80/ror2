using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace DofusMod.Characters.Xelor.SkillStates
{
    // ── Xelor's Sandglass ───────────────────────────────────────────────────
    // Throw a bomb that sticks to where it lands and detonates after 2s
    // for 600% damage in a large radius.
    //
    // The projectile itself is a MonoBehaviour (SandglassBomb) that
    // self-destructs after the fuse burns. We spawn it via ProjectileManager.
    public class XelorSandglass : BaseSkillState
    {
        public static float CastDuration  = 0.60f;
        public static float FireFraction  = 0.45f;   // fire at 45% of cast
        public static float ThrowSpeed    = 30f;
        public static float DamageCoeff   = 6.0f;
        public static float BlastRadius   = 14f;
        public static float FuseTime      = 2.0f;

        private bool _fired;

        // Placeholder — use RoR2's sticky bomb prefab until you make a custom one
        private static GameObject _stickPrefab =>
            LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/StickyBomb");

        public override void OnEnter()
        {
            base.OnEnter();
            _fired = false;
            PlayAnimation("Gesture, Additive", "ThrowBomb", "ThrowBomb.playbackRate", CastDuration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!_fired && isAuthority && fixedAge >= CastDuration * FireFraction)
            {
                _fired = true;
                FireBomb();
            }

            if (isAuthority && fixedAge >= CastDuration)
                outer.SetNextStateToMain();
        }

        private void FireBomb()
        {
            var aimRay = GetAimRay();

            // We pass damage=0 to the projectile itself and let
            // SandglassBombController handle the blast with our coefficients.
            // For a quick prototype, reuse StickyBomb with scaled damage.
            var info = new FireProjectileInfo
            {
                projectilePrefab  = _stickPrefab,
                position          = aimRay.origin,
                rotation          = Util.QuaternionSafeLookRotation(aimRay.direction),
                owner             = gameObject,
                damage            = damageStat * DamageCoeff,
                force             = 0f,
                crit              = RollCrit(),
                speedOverride     = ThrowSpeed
            };
            ProjectileManager.instance.FireProjectile(info);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
            => InterruptPriority.Skill;
    }
}
