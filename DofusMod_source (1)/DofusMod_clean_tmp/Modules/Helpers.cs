using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DofusMod.Modules
{
    // Shared helpers used by both character setups.
    internal static class Helpers
    {
        // ── Skill Def factory ────────────────────────────────────────────────
        internal static SkillDef CreateSkillDef(
            System.Type activationState,
            string stateMachine,
            string nameToken,
            string descToken,
            Sprite icon,
            float cooldown,
            int stock            = 1,
            bool isCombatSkill   = true,
            bool mustKeyPress    = true,
            int maxStock         = 1,
            int rechargeStock    = 1,
            bool cancelOnExit   = false)
        {
            var def = ScriptableObject.CreateInstance<SkillDef>();
            def.activationState          = new EntityStates.SerializableEntityStateType(activationState);
            def.activationStateMachineName = stateMachine;
            def.skillNameToken           = nameToken;
            def.skillDescriptionToken    = descToken;
            def.icon                     = icon;
            def.baseRechargeInterval     = cooldown;
            def.baseMaxStock             = maxStock;
            def.rechargeStock            = rechargeStock;
            def.beginSkillCooldownOnSkillEnd = cancelOnExit;
            def.canceledFromSprinting    = false;
            def.forceSprintDuringState   = false;
            def.fullRestockOnAssign      = true;
            def.interruptPriority        = EntityStates.InterruptPriority.Skill;
            def.isCombatSkill            = isCombatSkill;
            def.mustKeyPress             = mustKeyPress;
            def.cancelSprintingOnActivation = true;
            def.stockToConsume           = stock;
            R2API.ContentAddition.AddSkillDef(def);
            return def;
        }

        // ── Addressable shorthand ────────────────────────────────────────────
        internal static T Load<T>(string address) where T : UnityEngine.Object
            => Addressables.LoadAssetAsync<T>(address).WaitForCompletion();

        // ── Basic sphere overlap blast ────────────────────────────────────────
        internal static void SphereDamage(
            GameObject attacker,
            Vector3 origin,
            float radius,
            float damageCoeff,
            DamageType damageType = DamageType.Generic)
        {
            var body = attacker.GetComponent<CharacterBody>();
            if (body == null) return;

            var blast = new BlastAttack
            {
                attacker           = attacker,
                inflictor          = attacker,
                teamIndex          = TeamComponent.GetObjectTeam(attacker),
                position           = origin,
                radius             = radius,
                baseDamage         = body.damage * damageCoeff,
                baseForce          = 400f,
                bonusForce         = Vector3.zero,
                crit               = body.RollCrit(),
                damageType         = damageType,
                falloffModel       = BlastAttack.FalloffModel.None,
                procCoefficient    = 1f
            };
            blast.Fire();
        }
    }
}
