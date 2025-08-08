# Alisaie Survivor for RoR2

A custom survivor mod for Risk of Rain 2 that adds the character Alisaie from Final Fantasy XIV

Cast spells to gain mana, then spend mana through enchanted rapier combos to gain access to more powerful spells.

# Screenshots
![20250804174915_1](https://github.com/user-attachments/assets/47568cd5-5132-45df-80c2-802c177907b4)

![20250804175108_1](https://github.com/user-attachments/assets/8b1589aa-a38b-4ac8-918a-36a5e3b96b49)

![20250804174800_1](https://github.com/user-attachments/assets/715096bc-8612-426c-b1bf-8c71ced44ec5)

![20250721140709_1](https://github.com/user-attachments/assets/c2351a54-fc50-4afe-bdf0-6f0af965992f)

# Skills
## Primary

### Jolt
Charge up a projectile, dealing up to **320% damage** and generating 2 white mana and 2 black mana. Full charge grants **Dualcast**.

<img width="80" height="80" alt="texIconJolt" src="https://github.com/user-attachments/assets/48269a7c-9644-4a69-a39f-e7384cbaccdc" />

---

## Dualcast Spells
During dualcast, primary and secondary are replaced with the following instant-cast spells.

<img width="48" height="64" alt="texIconDualcast" src="https://github.com/user-attachments/assets/c1232f7b-e5a6-4e2a-8346-4c26c6fb22bb" />

### Veraero
Unleash wind-aspected magic, dealing **325% AOE damage** and generating 7 white mana. Only usable during **Dualcast**.

<img width="80" height="80" alt="texIconVeraero" src="https://github.com/user-attachments/assets/3afabc7f-e3b9-455e-a14a-f17e81156374" />

### Verthunder
Fire 1–10 lightning orbs based on level, each dealing **250% damage**. Generates 7 black mana. Only usable during **Dualcast**.

<img width="80" height="80" alt="texIconVerthunder" src="https://github.com/user-attachments/assets/c2602e99-5d4e-4896-a3c1-a08da3b30896" />

---

## Finisher Spells
After an enchanted combo, primary and secondary are replaced with the following powerful instant-cast spells.

<img width="48" height="64" alt="texIconFinisher" src="https://github.com/user-attachments/assets/dfa40f06-f1af-45cd-a77a-5c3a717f31a5" />

### Verholy
Fire a concentrated blast of holy energy at your target, dealing **1600% AoE damage** and generating 11 white mana. Heals all allies within a large radius for half the AoE damage. Only usable after an **Enchanted Combo**.

<img width="80" height="80" alt="texIconVerholy" src="https://github.com/user-attachments/assets/464008c1-2a62-42d3-a07e-857b9d3c1eb0" />

### Verflare
Unleash an explosion, applying burn and dealing **2400% AoE damage**. Generates 11 black mana. Only usable after an **Enchanted Combo**.

<img width="80" height="80" alt="texIconVerflare" src="https://github.com/user-attachments/assets/d917dff2-bc2b-409e-8408-7421bb6b141c" />

---

## Secondary

### Acceleration
Gain **Dualcast** instantly and double mana gain for 10 seconds.

<img width="80" height="80" alt="texIconAcceleration" src="https://github.com/user-attachments/assets/1a3fd828-d1ed-484d-976d-56ecbc0468ed" />

---

## Utility

### Corps-A-Corps
Rush forward with a sword attack for **300% damage** and briefly dodge.

<img width="80" height="80" alt="texIconCorpsACorps" src="https://github.com/user-attachments/assets/540620bf-68ae-4485-87f7-aff726f5018f" />

---

## Rapier Combo
Perform a series of rapier attacks, each dealing increasing damage. Combo becomes **Enchanted** above 50/50 mana. **Enchanted** combos grant access to Verholy/Verflare.

### Riposte
A swing and thrust with your rapier, dealing **300% damage per hit** (**600% total**, 2 hits).

<img width="80" height="80" alt="texIconS1Riposte" src="https://github.com/user-attachments/assets/c7da2432-711f-483e-b373-db3c4218e6e5" />

### Zwerchhau
Three slashes with your rapier, each dealing **267% damage** (**800% total**, 3 hits).

<img width="80" height="80" alt="texIconS2Zwerchhau" src="https://github.com/user-attachments/assets/af14357d-3fb6-47e1-bf28-e7b0f5e47d0a" />

### Redoublement
Six rapid jabs with your rapier, each dealing **167% damage** (**1000% total**, 6 hits).

<img width="80" height="80" alt="texIconS3Redoublement" src="https://github.com/user-attachments/assets/44a5d83e-464b-4e19-9f26-7f658c9f937e" />

---

## Enchanted Rapier Combo
Enchanted combos are available above 50/50 mana and grant access to Verholy/Verflare.

### Enchanted Riposte
A swing and thrust with your rapier, dealing **600% damage per hit** (**1200% total**, 2 hits). Costs 20 Black and White Mana.

<img width="80" height="80" alt="texIconES1Riposte" src="https://github.com/user-attachments/assets/507575cd-a158-45d8-910e-34005e1918d3" />

### Enchanted Zwerchhau
Three slashes with your rapier, each dealing **533% damage** (**1600% total**, 3 hits). Costs 15 Black and White Mana.

<img width="80" height="80" alt="texIconES2Zwerchhau" src="https://github.com/user-attachments/assets/55b8af09-2814-41cb-90b3-9dde1b190f5f" />

### Enchanted Redoublement
Six rapid jabs with your rapier, each dealing **333% damage** (**2000% total**, 6 hits). Costs 15 Black and White Mana. Enables **finisher spells.**

<img width="80" height="80" alt="texIconES3Redoublement" src="https://github.com/user-attachments/assets/182467af-8d15-4ffc-a112-203cfd39d485" />

---
# Known Issues
- Secondary and Special do not behave as expected with multiple stocks
- If an enemy dies while a spell is midair the spell projectile might hang for a few seconds.
- Getting resurrected with Seed of Life causes the mana gauge to disappear
- Only the host in a multiplayer game can see the mana gauge
- Corps-A-Corps sound effect plays globally in multiplayer

# Feedback
If you find any issues, feel free to open an issue here on github, or message me directly on discord @emocoded. For any other comments or questions please message me on Discord.

# Credits
WhiteMageSunny - DeviantArt - Original endwalker model I began working off of

PassiveModding/Ramen - FFXIV Meddle tool

rob/arcph1r3, thetimesweeper - henry tutorial - This was my first time creating a RoR2 mod, couldn't have done it without this tutorial/ survivor template!

Psyche/psychebomb - helping with some material opacity stuff

.score - helped fix dependency bug

general credits to all of the FFXIV and RoR2 modding communities, this would have been impossible without them!

