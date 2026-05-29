using DofusMod.Modules;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace DofusMod.Characters.Xelor
{
    internal static class XelorCharacter
    {
        internal static GameObject bodyPrefab;
        internal static GameObject displayPrefab;

        internal static void Create()
        {
            BuildBody();
            RegisterSkills();
            ContentAddition.AddBody(bodyPrefab);
            Log.Info("Xelor body registered.");
        }

        // ── Body ────────────────────────────────────────────────────────────
        private static void BuildBody()
        {
            // Clone Artificer as the ranged base — swap mesh for Xelor's assets.
            bodyPrefab = PrefabAPI.InstantiateClone(
                Helpers.Load<GameObject>("RoR2/Base/Mage/MageBody.prefab"),
                "XelorBody");

            var body = bodyPrefab.GetComponent<CharacterBody>();
            body.baseNameToken       = "XELOR_NAME";
            body.subtitleNameToken   = "XELOR_SUBTITLE";
            body.portraitIcon        = null;
            body.bodyColor           = new Color(0.20f, 0.55f, 0.85f); // time-blue

            // ── Base stats ──────────────────────────────────────────────────
            // Xelor is a glass cannon: lower HP, higher damage.
            body.baseMaxHealth       = 110f;
            body.levelMaxHealth      = 30f;
            body.baseRegen           = 1.0f;
            body.levelRegen          = 0.2f;
            body.baseMoveSpeed       = 7f;
            body.baseAcceleration    = 80f;
            body.baseJumpPower       = 15f;
            body.baseDamage          = 16f;
            body.levelDamage         = 3.6f;
            body.baseAttackSpeed     = 1f;
            body.baseArmor           = 0f;
            body.baseJumpCount       = 1;

            // ── Passive component ───────────────────────────────────────────
            bodyPrefab.AddComponent<XelorDevotionController>();

            displayPrefab = PrefabAPI.InstantiateClone(
                Helpers.Load<GameObject>("RoR2/Base/Mage/MageDisplay.prefab"),
                "XelorDisplay", false);
        }

        // ── Skills ──────────────────────────────────────────────────────────
        private static void RegisterSkills()
        {
            var skillLocator = bodyPrefab.GetComponent<SkillLocator>();

            skillLocator.passiveSkill.enabled               = true;
            skillLocator.passiveSkill.skillNameToken        = "XELOR_PASSIVE_NAME";
            skillLocator.passiveSkill.skillDescriptionToken = "XELOR_PASSIVE_DESC";

            // Primary — Temporal Dust
            var primaryFamily = ScriptableObject.CreateInstance<SkillFamily>();
            primaryFamily.variants = new SkillFamily.Variant[1];
            ContentAddition.AddSkillFamily(primaryFamily);
            var primaryDef = Helpers.CreateSkillDef(
                typeof(SkillStates.TemporalDust),
                "Weapon",
                "XELOR_PRIMARY_NAME",
                "XELOR_PRIMARY_DESC",
                null, 0f);
            primaryFamily.variants[0] = new SkillFamily.Variant { skillDef = primaryDef };
            skillLocator.primary.SetFieldValue("_skillFamily", primaryFamily);

            // Secondary — Sandglass
            var secondaryFamily = ScriptableObject.CreateInstance<SkillFamily>();
            secondaryFamily.variants = new SkillFamily.Variant[1];
            ContentAddition.AddSkillFamily(secondaryFamily);
            var secondaryDef = Helpers.CreateSkillDef(
                typeof(SkillStates.XelorSandglass),
                "Weapon",
                "XELOR_SECONDARY_NAME",
                "XELOR_SECONDARY_DESC",
                null, 8f);
            secondaryFamily.variants[0] = new SkillFamily.Variant { skillDef = secondaryDef };
            skillLocator.secondary.SetFieldValue("_skillFamily", secondaryFamily);

            // Utility — Teleportation
            var utilityFamily = ScriptableObject.CreateInstance<SkillFamily>();
            utilityFamily.variants = new SkillFamily.Variant[1];
            ContentAddition.AddSkillFamily(utilityFamily);
            var utilityDef = Helpers.CreateSkillDef(
                typeof(SkillStates.XelorTeleportation),
                "Weapon",
                "XELOR_UTILITY_NAME",
                "XELOR_UTILITY_DESC",
                null, 6f);
            utilityFamily.variants[0] = new SkillFamily.Variant { skillDef = utilityDef };
            skillLocator.utility.SetFieldValue("_skillFamily", utilityFamily);

            // Special — Temporal Rift
            var specialFamily = ScriptableObject.CreateInstance<SkillFamily>();
            specialFamily.variants = new SkillFamily.Variant[1];
            ContentAddition.AddSkillFamily(specialFamily);
            var specialDef = Helpers.CreateSkillDef(
                typeof(SkillStates.TemporalRift),
                "Weapon",
                "XELOR_SPECIAL_NAME",
                "XELOR_SPECIAL_DESC",
                null, 50f);
            specialFamily.variants[0] = new SkillFamily.Variant { skillDef = specialDef };
            skillLocator.special.SetFieldValue("_skillFamily", specialFamily);
        }
    }

    // ── Passive: Devotion — kill → 30% chance to reset a random cooldown ───
    public class XelorDevotionController : MonoBehaviour
    {
        private const float PROC_CHANCE = 0.30f;

        private CharacterBody   _body;
        private SkillLocator    _skills;

        private void Start()
        {
            _body   = GetComponent<CharacterBody>();
            _skills = GetComponent<SkillLocator>();
            GlobalEventManager.onCharacterDeathGlobal += OnAnyDeath;
        }

        private void OnDestroy()
        {
            GlobalEventManager.onCharacterDeathGlobal -= OnAnyDeath;
        }

        private void OnAnyDeath(DamageReport report)
        {
            if (report.attackerBody != _body) return;
            if (!Util.CheckRoll(PROC_CHANCE * 100f, _body.master)) return;

            // Collect skills that are on cooldown and pick one at random
            var candidates = new System.Collections.Generic.List<GenericSkill>();
            if (_skills.primary   != null && _skills.primary.rechargeStopwatch   < _skills.primary.finalRechargeInterval)   candidates.Add(_skills.primary);
            if (_skills.secondary != null && _skills.secondary.rechargeStopwatch < _skills.secondary.finalRechargeInterval) candidates.Add(_skills.secondary);
            if (_skills.utility   != null && _skills.utility.rechargeStopwatch   < _skills.utility.finalRechargeInterval)   candidates.Add(_skills.utility);
            if (_skills.special   != null && _skills.special.rechargeStopwatch   < _skills.special.finalRechargeInterval)   candidates.Add(_skills.special);

            if (candidates.Count == 0) return;

            var chosen = candidates[Random.Range(0, candidates.Count)];
            chosen.rechargeStopwatch = chosen.finalRechargeInterval;

            Chat.AddMessage($"<color=#3388ff>Xelor's Devotion reset {chosen.skillNameToken}!</color>");
        }
    }
}
