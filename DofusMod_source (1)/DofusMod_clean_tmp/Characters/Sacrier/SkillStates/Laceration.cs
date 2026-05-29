using DofusMod.Characters.Sacrier;
using EntityStates;
using RoR2;
using UnityEngine;

namespace DofusMod.Characters.Sacrier.SkillStates
{
    // ── Laceration ─────────────────────────────────────────────────────────
    // Wide melee arc. Heals 15% of damage dealt. If Angrr is fully charged,
    // this hit deals +50% bonus damage and triggers ConsumeCharge().
    public class Laceration : BaseSkillState
    {
        // Tune these from config or leave as constants
        public static float DamageCoeff    = 1.80f;
        public static float HealFraction   = 0.15f;
        public static float BonusCoeff     = 0.50f;   // Angrr bonus
        public static float HitRadius      = 4.5f;
        public static float Duration       = 0.50f;   // full animation length
        public static float HitWindowStart = 0.20f;   // when the swing hits
        public static float HitWindowEnd   = 0.35f;

        private bool   _hasFired;
        private float  _stopwatch;
        private Animator _animator;

        public override void OnEnter()
        {
            base.OnEnter();
            _hasFired  = false;
            _stopwatch = 0f;
            _animator  = GetModelAnimator();

            // 0.5s full swing animation — replace token with your custom anim
            PlayAnimation("FullBody, Override", "Slash", "Slash.playbackRate", Duration);

            if (isAuthority)
                characterBody.SetAimTimer(2f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            _stopwatch += Time.fixedDeltaTime;

            if (!_hasFired
                && _stopwatch >= HitWindowStart
                && _stopwatch <= HitWindowEnd)
            {
                _hasFired = true;
                FireSlash();
            }

            if (isAuthority && _stopwatch >= Duration)
                outer.SetNextStateToMain();
        }

        private void FireSlash()
        {
            var passive   = characterBody.GetComponent<SacrierPassiveController>();
            bool charged  = passive != null && passive.IsCharged;
            float coeff   = DamageCoeff + (charged ? DamageCoeff * BonusCoeff : 0f);

            // Arc overlap centered in front of the character
            var origin = characterBody.corePosition + characterDirection.forward * 1.5f;
            var blast  = new BlastAttack
            {
                attacker        = gameObject,
                inflictor       = gameObject,
                teamIndex       = teamComponent.teamIndex,
                position        = origin,
                radius          = HitRadius,
                baseDamage      = damageStat * coeff,
                baseForce       = 300f,
                crit            = RollCrit(),
                damageType      = DamageType.Generic,
                falloffModel    = BlastAttack.FalloffModel.None,
                procCoefficient = 1f
            };
            var result = blast.Fire();

            // Heal based on damage actually dealt
            if (result.hitCount > 0)
            {
                float healAmount = result.hitPoints * HealFraction;
                healthComponent.Heal(healAmount, default, true);
            }

            if (charged)
                passive.ConsumeCharge();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
            => InterruptPriority.Skill;
    }
}
