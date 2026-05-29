using R2API;

namespace DofusMod.Characters.Xelor
{
    internal static class XelorTokens
    {
        internal static void Register()
        {
            LanguageAPI.Add("XELOR_NAME",        "Xelor");
            LanguageAPI.Add("XELOR_SUBTITLE",    "Master of Time");
            LanguageAPI.Add("XELOR_DESC",
                "Xelor bends the flow of time itself, slowing enemies to a crawl " +
                "and erasing them with temporal explosions.");
            LanguageAPI.Add("XELOR_OUTRO_FLAVOR", "..and so he stepped outside of time.");
            LanguageAPI.Add("XELOR_FAIL_FLAVOR",  "..and so his clock ran out.");

            // Passive
            LanguageAPI.Add("XELOR_PASSIVE_NAME", "Devotion");
            LanguageAPI.Add("XELOR_PASSIVE_DESC",
                "<style=cIsUtility>Killing an enemy</style> has a <style=cIsDamage>30% chance</style> " +
                "to reset a random skill's cooldown.");

            // Primary
            LanguageAPI.Add("XELOR_PRIMARY_NAME", "Temporal Dust");
            LanguageAPI.Add("XELOR_PRIMARY_DESC",
                "Fire a rapid burst of time-laced projectiles for <style=cIsDamage>3×60% damage</style>. " +
                "Each hit <style=cIsUtility>slows</style> enemies by 80% for 1.5s.");

            // Secondary
            LanguageAPI.Add("XELOR_SECONDARY_NAME", "Xelor's Sandglass");
            LanguageAPI.Add("XELOR_SECONDARY_DESC",
                "Throw a <style=cIsDamage>time bomb</style> that detonates after 2 seconds " +
                "for <style=cIsDamage>600% damage</style> in a large area. " +
                "<style=cIsUtility>8s cooldown.</style>");

            // Utility
            LanguageAPI.Add("XELOR_UTILITY_NAME", "Teleportation");
            LanguageAPI.Add("XELOR_UTILITY_DESC",
                "<style=cIsUtility>Blink</style> to your cursor's position instantly. " +
                "<style=cIsUtility>6s cooldown.</style>");

            // Special
            LanguageAPI.Add("XELOR_SPECIAL_NAME",  "Temporal Rift");
            LanguageAPI.Add("XELOR_SPECIAL_DESC",
                "Conjure a rift that <style=cIsUtility>freezes</style> all enemies inside " +
                "for <style=cIsDamage>3 seconds</style>. Lasts 5 seconds. " +
                "<style=cIsUtility>50s cooldown.</style>");
        }
    }
}
