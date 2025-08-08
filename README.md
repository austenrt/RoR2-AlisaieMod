# Alisaie Survivor for RoR2

A custom survivor mod for Risk of Rain 2 that adds the character Alisaie from Final Fantasy XIV

Cast spells to gain mana, then spend mana through enchanted rapier combos to gain access to more powerful spells.

# Screenshots

![20250804174915_1](https://github.com/user-attachments/assets/01d8b75f-42e6-4ab1-b003-7e0946a00d84)

![20250804174800_1](https://github.com/user-attachments/assets/1a542baf-79d7-45c8-b24f-6a0b99841c88)

![20250721140709_1](https://github.com/user-attachments/assets/8e30e343-2d15-4286-b30d-a7fe2ddbf03a)

![20250804175108_1](https://github.com/user-attachments/assets/e536a5e3-58b1-43e2-9530-2e2912056682)



# Skills
## Primary

### Jolt
Charge up a projectile, dealing up to **320% damage** and generating 2 white mana and 2 black mana. Full charge grants **Dualcast**.

<img width="80" height="80" alt="texIconJolt" src="https://github.com/user-attachments/assets/d48fca54-fc00-4331-b8c3-37b7109a48e5" />


---

## Dualcast Spells
During dualcast, primary and secondary are replaced with the following instant-cast spells.

<img width="48" height="64" alt="texIconDualcast" src="https://github.com/user-attachments/assets/dd406297-3d89-419c-9d94-2be9ea057764" />


### Veraero
Unleash wind-aspected magic, dealing **325% AOE damage** and generating 7 white mana. Only usable during **Dualcast**.

<img width="80" height="80" alt="texIconVeraero" src="https://github.com/user-attachments/assets/0eb969e7-5666-4056-8a8f-07fde5b97d8a" />


### Verthunder
Fire 1–10 lightning orbs based on level, each dealing **250% damage**. Generates 7 black mana. Only usable during **Dualcast**.

<img width="80" height="80" alt="texIconVerthunder" src="https://github.com/user-attachments/assets/c31c13d1-7505-4508-b30d-49619c64e66b" />


---

## Finisher Spells
After an enchanted combo, primary and secondary are replaced with the following powerful instant-cast spells.

<img width="48" height="64" alt="texIconFinisher" src="https://github.com/user-attachments/assets/82cd1a08-293f-41fa-b0e8-9e63b98c33ff" />


### Verholy
Fire a concentrated blast of holy energy at your target, dealing **1600% AoE damage** and generating 11 white mana. Heals all allies within a large radius for half the AoE damage. Only usable after an **Enchanted Combo**.

<img width="80" height="80" alt="texIconVerholy" src="https://github.com/user-attachments/assets/ced48a1b-d397-4f59-96ba-d7ab23628598" />

### Verflare
Unleash an explosion, applying burn and dealing **2400% AoE damage**. Generates 11 black mana. Only usable after an **Enchanted Combo**.

<img width="80" height="80" alt="texIconVerflare" src="https://github.com/user-attachments/assets/36fbf26c-6914-4b16-8598-cde893dd5f10" />

---

## Secondary

### Acceleration
Gain **Dualcast** instantly and double mana gain for 10 seconds.

<img width="80" height="80" alt="texIconJolt" src="https://github.com/user-attachments/assets/793bc12c-a54b-4503-977b-45e53d78fb8c" />

---

## Utility

### Corps-A-Corps
Rush forward with a sword attack for **300% damage** and briefly dodge.

<img width="80" height="80" alt="texIconCorpsACorps" src="https://github.com/user-attachments/assets/66e9def7-bcb3-4b0c-832f-25882b4484d1" />

---

## Rapier Combo
Perform a series of rapier attacks, each dealing increasing damage. Combo becomes **Enchanted** above 50/50 mana. **Enchanted** combos grant access to Verholy/Verflare.

### Riposte
A swing and thrust with your rapier, dealing **300% damage per hit** (**600% total**, 2 hits).

<img width="80" height="80" alt="texIconS1Riposte" src="https://github.com/user-attachments/assets/fbb1f050-4c5d-4f2c-b64f-7feb5a925e70" />

### Zwerchhau
Three slashes with your rapier, each dealing **267% damage** (**800% total**, 3 hits).

<img width="80" height="80" alt="texIconS2Zwerchhau" src="https://github.com/user-attachments/assets/d2e146cd-e804-487c-a9d1-545ae055ff36" />


### Redoublement
Six rapid jabs with your rapier, each dealing **167% damage** (**1000% total**, 6 hits).

<img width="80" height="80" alt="texIconS3Redoublement" src="https://github.com/user-attachments/assets/3392a652-729d-4a6a-92ff-ba93c764e282" />

---

## Enchanted Rapier Combo
Enchanted combos are available above 50/50 mana and grant access to Verholy/Verflare.

### Enchanted Riposte
A swing and thrust with your rapier, dealing **600% damage per hit** (**1200% total**, 2 hits). Costs 20 Black and White Mana.

<img width="80" height="80" alt="texIconES1Riposte" src="https://github.com/user-attachments/assets/81fefaee-2260-43ab-baa3-ee733de7f115" />

### Enchanted Zwerchhau
Three slashes with your rapier, each dealing **533% damage** (**1600% total**, 3 hits). Costs 15 Black and White Mana.

<img width="80" height="80" alt="texIconES2Zwerchhau" src="https://github.com/user-attachments/assets/051e43cb-7c48-4d35-86b0-119c6bee0247" />

### Enchanted Redoublement
Six rapid jabs with your rapier, each dealing **333% damage** (**2000% total**, 6 hits). Costs 15 Black and White Mana. Enables **finisher spells.**

<img width="80" height="80" alt="texIconES3Redoublement" src="https://github.com/user-attachments/assets/cfb46b99-1487-46ef-8d7a-809f32f77209" />

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

