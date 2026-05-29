using R2API;

namespace DofusMod.Characters.Sacrier
{
    internal static class SacrierTokens
    {
        internal static void Register()
        {
            LanguageAPI.Add("SACRIER_NAME",        "Sacrier");
            LanguageAPI.Add("SACRIER_SUBTITLE",    "Servant of Sacré");
            LanguageAPI.Add("SACRIER_DESC",
                "The Sacrier is a berserker who grows deadlier as wounds accumulate. " +
                "Pain is not a weakness — it is fuel.");
            LanguageAPI.Add("SACRIER_OUTRO_FLAVOR", "..and so she bled forward.");
            LanguageAPI.Add("SACRIER_FAIL_FLAVOR",  "..and so she fell, unblooded.");

            // Passive
            LanguageAPI.Add("SACRIER_PASSIVE_NAME", "Punishment");
            LanguageAPI.Add("SACRIER_PASSIVE_DESC",
                "<style=cIsHealing>Absorbing damage</style> charges <style=cIsDamage>Angrr</style>. " +
                "At max stacks your next hit deals <style=cIsDamage>+50% bonus damage</style> and heals you.");

            // Primary
            LanguageAPI.Add("SACRIER_PRIMARY_NAME", "Laceration");
            LanguageAPI.Add("SACRIER_PRIMARY_DESC",
                "Slash in a wide arc for <style=cIsDamage>180% damage</style>. " +
                "Heals for <style=cIsHealing>15%</style> of damage dealt.");

            // Secondary
            LanguageAPI.Add("SACRIER_SECONDARY_NAME", "Blood Bath");
            LanguageAPI.Add("SACRIER_SECONDARY_DESC",
                "Dash to a nearby enemy, dealing <style=cIsDamage>300% base damage + 1% per 1% missing health</style>. " +
                "<style=cIsUtility>6s cooldown.</style>");

            // Utility
            LanguageAPI.Add("SACRIER_UTILITY_NAME", "Transposition");
            LanguageAPI.Add("SACRIER_UTILITY_DESC",
                "<style=cIsUtility>Swap positions</style> with a targeted enemy, " +
                "briefly <style=cIsDamage>stunning</style> them. <style=cIsUtility>8s cooldown.</style>");

            // Special
            LanguageAPI.Add("SACRIER_SPECIAL_NAME",  "Sacrier's Heart");
            LanguageAPI.Add("SACRIER_SPECIAL_DESC",
                "Enter a <style=cIsDamage>berserker trance</style> for 6 seconds. " +
                "Cannot be reduced below <style=cIsHealing>1 HP</style>, " +
                "gain <style=cIsUtility>+60 armor</style>, and deal <style=cIsDamage>+25% damage</style>. " +
                "<style=cIsUtility>40s cooldown.</style>");
        }
    }
}
