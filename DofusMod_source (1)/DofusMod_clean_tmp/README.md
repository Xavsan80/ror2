# DofusMod — Sacrier & Xelor for Risk of Rain 2

Adds two playable survivors inspired by the Dofus universe, adapted for RoR2's action gameplay.

---

## ⚡ Installation (r2modman / Thunderstore app)

1. Download **DofusMod.zip** from the [latest GitHub Actions run](../../actions)
   → Go to the workflow run → Click **DofusMod-thunderstore** under Artifacts
2. Open **r2modman** or the **Thunderstore app**
3. Select your Risk of Rain 2 profile
4. Click **Settings → Import local mod** → select `DofusMod.zip`
5. Launch the game from the mod manager — done!

> All dependencies (BepInEx, R2API) are listed in `manifest.json` and will be
> auto-installed by the mod manager.

---

## Sacrier — Servant of Sacré

A berserker who weaponizes pain. The more she's hit, the more dangerous she becomes.

**Base Stats**
| Stat | Base | Per level |
|------|------|-----------|
| HP | 200 | +54 |
| Damage | 14 | +3.2 |
| Speed | 7 m/s | — |
| Armor | 20 | — |

### Skills

**Passive — Punishment (Angrr)**
Taking damage charges Angrr (1 stack per ~5% max HP lost, up to 10).
At max stacks your next Laceration deals +50% bonus damage and heals 8% max HP.

**Primary — Laceration** *(no cooldown)*
Wide melee arc, 180% damage. Heals 15% of damage dealt.

**Secondary — Blood Bath** *(6s cooldown)*
Dash to the nearest enemy. Deals 300% + up to 100% bonus based on missing HP.

**Utility — Transposition** *(8s cooldown)*
Instantly swap positions with the nearest enemy within 25m. Stuns them for 1.5s.

**Special — Sacrier's Heart** *(40s cooldown)*
6-second trance: cannot drop below 1 HP, +60 armor, +25% damage.

---

## Xelor — Master of Time

A glass-cannon mage who slows, freezes, and erases foes with temporal magic.

**Base Stats**
| Stat | Base | Per level |
|------|------|-----------|
| HP | 110 | +30 |
| Damage | 16 | +3.6 |
| Speed | 7 m/s | — |
| Armor | 0 | — |

### Skills

**Passive — Devotion**
Killing an enemy has a 30% chance to reset a random skill cooldown.

**Primary — Temporal Dust** *(no cooldown)*
3-shot burst, 3×60% damage. Each hit slows targets by 80% for 1.5s.

**Secondary — Xelor's Sandglass** *(8s cooldown)*
Throw a time bomb that detonates after 2s for 600% damage in a large area.

**Utility — Teleportation** *(6s cooldown)*
Instant blink to cursor position (max 35m).

**Special — Temporal Rift** *(50s cooldown)*
Place a 12m zone for 5 seconds that freezes all enemies inside for 3s.

---

## Building from source

The project uses **NuGet packages only** — no local RoR2 installation required to build.

```bash
git clone https://github.com/YOUR_NAME/DofusMod
cd DofusMod
dotnet build -c Release
```

The compiled DLL lands in `bin/Release/netstandard2.0/DofusMod.dll`.

Alternatively, every push to `main` triggers a GitHub Actions build that
produces a ready-to-import `DofusMod.zip` under the workflow's Artifacts.

---

## Adding custom assets

Currently uses Commando / Artificer meshes as placeholders.
To add proper Sacrier/Xelor visuals:
- Build a Unity AssetBundle with your models and reference it in `SacrierCharacter.cs` / `XelorCharacter.cs`
- Assign `portraitIcon` and `SkillDef.icon` sprites
- Replace `LegacyResourcesAPI.Load<GameObject>(...)` VFX prefabs

## Dependencies
- BepInExPack
- R2API (Core, Prefab, Language, Sound, Networking, Loadout, DamageType, ContentManagement)

## Credits
- Ankama Games — Sacrier and Xelor from Dofus
- The RoR2 Modding Discord
