using DofusMod.Modules;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace DofusMod.Characters.Sacrier
{
    internal static class SacrierCharacter
    {
        internal static GameObject bodyPrefab;
        internal static GameObject displayPrefab;

        internal static void Create()
        {
            BuildBody();
            RegisterSkills();
            ContentAddition.AddBody(bodyPrefab);
            Log.Info("Sacrier body registered.");
        }

        // ── Body ────────────────────────────────────────────────────────────
        private static void BuildBody()
        {
            // Clone Commando as a base prefab — replace with a custom mesh once
            // you have Sacrier assets. All stats and skill families are then
            // swapped out below.
            bodyPrefab = PrefabAPI.InstantiateClone(
                Helpers.Load<GameObject>("RoR2/Base/Commando/CommandoBody.prefab"),
                "SacrierBody");

            var body = bodyPrefab.GetComponent<CharacterBody>();
            body.baseNameToken          = "SACRIER_NAME";
            body.subtitleNameToken      = "SACRIER_SUBTITLE";
            body.portraitIcon           = null; // assign your Sprite asset here
            body.bodyColor              = new Color(0.72f, 0.10f, 0.10f); // blood red

            // ── Base stats ──────────────────────────────────────────────────
            body.baseMaxHealth          = 200f;
            body.levelMaxHealth         = 54f;
            body.baseRegen              = 1.5f;
            body.levelRegen             = 0.3f;
            body.baseMoveSpeed          = 7f;
            body.baseAcceleration       = 80f;
            body.baseJumpPower          = 15f;
            body.baseDamage             = 14f;
            body.levelDamage            = 3.2f;
            body.baseAttackSpeed        = 1f;
            body.baseArmor              = 20f;
            body.baseJumpCount          = 1;

            // ── Attach passive component ────────────────────────────────────
            bodyPrefab.AddComponent<SacrierPassiveController>();

            // ── Display prefab (character select / logbook) ─────────────────
            displayPrefab = PrefabAPI.InstantiateClone(
                Helpers.Load<GameObject>("RoR2/Base/Commando/CommandoDisplay.prefab"),
                "SacrierDisplay", false);
        }

        // ── Skills ──────────────────────────────────────────────────────────
        private static void RegisterSkills()
        {
            var skillLocator = bodyPrefab.GetComponent<SkillLocator>();

            // Passive (informational — no SkillDef needed, handled by component)
            skillLocator.passiveSkill.enabled         = true;
            skillLocator.passiveSkill.skillNameToken  = "SACRIER_PASSIVE_NAME";
            skillLocator.passiveSkill.skillDescriptionToken = "SACRIER_PASSIVE_DESC";

            // Primary — Laceration
            var primaryFamily = ScriptableObject.CreateInstance<SkillFamily>();
            primaryFamily.variants = new SkillFamily.Variant[1];
            ContentAddition.AddSkillFamily(primaryFamily);

            var primaryDef = Helpers.CreateSkillDef(
                typeof(SkillStates.Laceration),
                "Weapon",
                "SACRIER_PRIMARY_NAME",
                "SACRIER_PRIMARY_DESC",
                null,           // icon sprite
                cooldown: 0f);  // primary has no cooldown

            primaryFamily.variants[0] = new SkillFamily.Variant { skillDef = primaryDef };
            skillLocator.primary.SetFieldValue("_skillFamily", primaryFamily);

            // Secondary — Blood Bath
            var secondaryFamily = ScriptableObject.CreateInstance<SkillFamily>();
            secondaryFamily.variants = new SkillFamily.Variant[1];
            ContentAddition.AddSkillFamily(secondaryFamily);

            var secondaryDef = Helpers.CreateSkillDef(
                typeof(SkillStates.BloodBath),
                "Weapon",
                "SACRIER_SECONDARY_NAME",
                "SACRIER_SECONDARY_DESC",
                null,
                cooldown: 6f);

            secondaryFamily.variants[0] = new SkillFamily.Variant { skillDef = secondaryDef };
            skillLocator.secondary.SetFieldValue("_skillFamily", secondaryFamily);

            // Utility — Transposition
            var utilityFamily = ScriptableObject.CreateInstance<SkillFamily>();
            utilityFamily.variants = new SkillFamily.Variant[1];
            ContentAddition.AddSkillFamily(utilityFamily);

            var utilityDef = Helpers.CreateSkillDef(
                typeof(SkillStates.Transposition),
                "Weapon",
                "SACRIER_UTILITY_NAME",
                "SACRIER_UTILITY_DESC",
                null,
                cooldown: 8f);

            utilityFamily.variants[0] = new SkillFamily.Variant { skillDef = utilityDef };
            skillLocator.utility.SetFieldValue("_skillFamily", utilityFamily);

            // Special — Sacrier's Heart
            var specialFamily = ScriptableObject.CreateInstance<SkillFamily>();
            specialFamily.variants = new SkillFamily.Variant[1];
            ContentAddition.AddSkillFamily(specialFamily);

            var specialDef = Helpers.CreateSkillDef(
                typeof(SkillStates.SacriersHeart),
                "Weapon",
                "SACRIER_SPECIAL_NAME",
                "SACRIER_SPECIAL_DESC",
                null,
                cooldown: 40f);

            specialFamily.variants[0] = new SkillFamily.Variant { skillDef = specialDef };
            skillLocator.special.SetFieldValue("_skillFamily", specialFamily);
        }
    }

    // ── Passive: Punishment / Angrr stacks ──────────────────────────────────
    // The component lives on the body and is queried by the skill states.
    public class SacrierPassiveController : MonoBehaviour
    {
        public const int MAX_STACKS = 10;
        public int Stacks { get; private set; } = 0;
        public bool IsCharged => Stacks >= MAX_STACKS;

        private CharacterBody _body;
        private HealthComponent _health;
        private float _lastHp;

        private void Start()
        {
            _body   = GetComponent<CharacterBody>();
            _health = GetComponent<HealthComponent>();
            _lastHp = _health != null ? _health.combinedHealth : 0f;
        }

        private void FixedUpdate()
        {
            if (_health == null) return;
            float current = _health.combinedHealth;
            float delta   = _lastHp - current;

            if (delta > 0f)  // damage was taken
            {
                // 1 stack per ~5 % max HP lost (feels responsive without being trivial)
                int gained = Mathf.Clamp(Mathf.FloorToInt(delta / (_health.fullCombinedHealth * 0.05f)), 0, MAX_STACKS);
                AddStacks(gained);
            }
            _lastHp = current;
        }

        public void AddStacks(int count)
        {
            Stacks = Mathf.Min(Stacks + count, MAX_STACKS);
        }

        // Call when the charged hit fires — resets stacks and heals.
        public void ConsumeCharge()
        {
            if (!IsCharged) return;
            Stacks = 0;
            if (_health != null)
                _health.Heal(_health.fullCombinedHealth * 0.08f, default, true);
        }
    }
}
