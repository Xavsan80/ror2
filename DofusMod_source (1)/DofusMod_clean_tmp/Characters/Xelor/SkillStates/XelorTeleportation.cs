using EntityStates;
using RoR2;
using UnityEngine;

namespace DofusMod.Characters.Xelor.SkillStates
{
    // ── Teleportation ───────────────────────────────────────────────────────
    // Instant blink to the aimed cursor position (max range capped).
    public class XelorTeleportation : BaseSkillState
    {
        public static float MaxRange     = 35f;
        public static float Duration     = 0.35f;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("FullBody, Override", "UtilityActivate");

            if (isAuthority) Blink();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority && fixedAge >= Duration)
                outer.SetNextStateToMain();
        }

        private void Blink()
        {
            var aimRay = GetAimRay();

            // Raycast to find a surface or use MaxRange if nothing is hit
            Vector3 dest;
            if (Physics.Raycast(aimRay, out RaycastHit hit, MaxRange,
                LayerIndex.world.mask | LayerIndex.defaultLayer.mask,
                QueryTriggerInteraction.Ignore))
            {
                dest = hit.point + Vector3.up * 1.5f;
            }
            else
            {
                dest = aimRay.origin + aimRay.direction * MaxRange;
            }

            // Spawn out-flash at current position
            EffectManager.SpawnEffect(
                LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/TeleportOutBoom"),
                new EffectData { origin = characterBody.footPosition },
                true);

            TeleportHelper.TeleportBody(characterBody, dest);

            // Spawn in-flash at destination
            EffectManager.SpawnEffect(
                LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/TeleportOutBoom"),
                new EffectData { origin = dest },
                true);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
            => InterruptPriority.PrioritySkill;
    }
}
