using EntityStates;
using RoR2;
using UnityEngine;

namespace DofusMod.Characters.Sacrier.SkillStates
{
    // ── Sacrier's Heart ─────────────────────────────────────────────────────
    // 6-second trance: cannot drop below 1 HP, +60 armor, +25% damage.
    // Implemented as an enter-state that applies a buff, and a matching
    // passive hook that clamps incoming damage while the buff is active.
    public class SacriersHeart : BaseSkillState
    {
        public static float TranceDuration = 6f;
        public static float CastDuration   = 0.50f;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("FullBody, Override", "SpecialActivate");

            if (isAuthority)
            {
                characterBody.AddTimedBuff(SacrierBuffs.HeartTrance,    TranceDuration);
                characterBody.AddTimedBuff(RoR2Content.Buffs.ArmorBoost, TranceDuration);
                // ArmorBoost gives +200; we patch down via the component
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority && fixedAge >= CastDuration)
                outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
            => InterruptPriority.PrioritySkill;
    }

    // ── Buff catalogue for Sacrier ──────────────────────────────────────────
    public static class SacrierBuffs
    {
        public static BuffDef HeartTrance;
        public static BuffDef Angrr;        // visual stack counter

        public static void Register()
        {
            HeartTrance = ScriptableObject.CreateInstance<BuffDef>();
            HeartTrance.name                   = "bdSacrierHeart";
            HeartTrance.buffColor              = new Color(0.8f, 0.1f, 0.1f);
            HeartTrance.canStack               = false;
            HeartTrance.isDebuff               = false;
            HeartTrance.iconSprite             = null; // assign your icon
            R2API.ContentAddition.AddBuffDef(HeartTrance);

            Angrr = ScriptableObject.CreateInstance<BuffDef>();
            Angrr.name        = "bdSacrierAngrr";
            Angrr.buffColor   = new Color(0.9f, 0.3f, 0.0f);
            Angrr.canStack    = true;
            Angrr.isDebuff    = false;
            Angrr.iconSprite  = null;
            R2API.ContentAddition.AddBuffDef(Angrr);
        }
    }

    // ── Damage-clamp hook — attached to the body in SacrierCharacter.Create ─
    // Ensures Sacrier cannot die while Sacrier's Heart is active.
    public class SacrierInvulnController : MonoBehaviour
    {
        private HealthComponent _health;

        private void Start()
        {
            _health = GetComponent<HealthComponent>();
            On.RoR2.HealthComponent.TakeDamage += OnTakeDamage;
        }

        private void OnDestroy()
        {
            On.RoR2.HealthComponent.TakeDamage -= OnTakeDamage;
        }

        private void OnTakeDamage(
            On.RoR2.HealthComponent.orig_TakeDamage orig,
            HealthComponent self,
            DamageInfo info)
        {
            if (self == _health && self.body.HasBuff(SacrierBuffs.HeartTrance))
            {
                // Clamp damage so HP cannot fall below 1
                float maxAllowed = self.combinedHealth - 1f;
                if (info.damage > maxAllowed && maxAllowed > 0f)
                    info.damage = maxAllowed;
            }
            orig(self, info);
        }
    }
}
