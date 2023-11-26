# R-MOR is a devastating machine made for critical encounters: extreme armor, retractable blades and missile launchers. An unstoppable behemoth.

[![](https://i.imgur.com/GOHd9Tb.jpg)]()
[![](https://i.imgur.com/qtxtlgM.jpg)]()
[![](https://i.imgur.com/0K50bEI.jpg)]()
[![](https://i.imgur.com/gJ2VPrN.jpg)]()
[![](https://i.imgur.com/N4ex8oo.png)]()

## For Skin Creators

To add custom DRONE models to your R-MOR skin, refer to the prefab in the Unity project on the GitHub. The DRONE model is a disabled GameObject attached to the prefab, and the game will attempt to load the textures/mesh from that.

Basically the same as HAN-D's DRONE, just without a saw.

## Installation

Place the RMOR_Reforged folder in /Risk of Rain 2/BepInEx/plugins/  

## To Do
- Fix up all animations
- Emotes
- Better Ability Icons
- Fix Royal Capacitor's Item Display
- Better Starstorm 2 Integration?
- More Skins
- More Abilities
- More polish

## Known Issues
Ragdoll kind of explodes and goes wacky. 

Mastery Skin Assets are not visible to other players.

If anything is found, please contact me on the RoR2 modding server!

## Resources Used

東方 Project - V2 Reimu Hakurei Fumo (3D scan) (https://skfb.ly/oqJsH) by Renafox is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).

DOOM (1993) - Sound Effects

Gensou Sky Drift - Extra parts for Mastery Model

## Credits

MoriyaLuna - Main Dev of R-MOR

Moffein - Major Coding Help, Leaving R-MOR model in HAN-D's Unity project for me to see and lose my shit over

TheTimesweeper - HenryMod template, help with Ragdoll

lui - R-MOR Model and Animations

dotflare - Default Skin Textures

## Credits for HAN-D

Moffein - Main Dev of HAN-D

TheTimesweeper - HenryMod template, help with Unity/Animations, general polish. You're a lifesaver!

dotflare - HAN-D Model + Anims

LucidInceptor - DRONE Model

PapaZach - Skill Icons

Tera - FOCUS Skill Icon

Vale-X - SWARM_ARMOR Buff Icon

KoobyKarasu - SMASH Skill Icon

Jaysian - Bankroller

Sounds taken from Risk of Rain 1 and Starstorm

# 1.3.0 - Huge Balance Update
### GENERAL CHANGES
`R-MOR simply used to have way too much damage reduction to be killable when combined with FORTIFY. As a result, I had to lower it even further than it originally was when it increased per level.`
- Armor decreased from 50 (*33% damage reduction*) to 30 (*23% damage reduction*)
- Base damage increased from 12 to 13
- Health regeneration decreased from 0.3 to 0.2 
- Projectiles increase FORTIFY time by a greater amount
### ERADICATE
`Added an effect with OVERCLOCK like HAN-D and R-MOR's other primaries`
- Now stuns on hit with FORTIFY or OVERCLOCK activated
- Rockets now fly much faster with FORTIFY or OVERCLOCK active
### CLUSTER_CANNON
`CLUSTER_CANNON's damage just seemed rather weak to me. Maybe this is the Vergil player in me coming out when I see a single flaw in a character, but it just felt a little weak. I also really want to redo animations for this attack.`
- Base damage increased from 600% to 800%
- Base charge time increased from 1.5 seconds to 1.8 seconds
- Cooldown increased from 5 seconds to 7 seconds
### FORTIFY
`As it was, FORTIFY was inherently powerful as its health regeneration scaled with player HP, which was unique among any type of healing in the game to my knowledge. As a result, I changed its effect to use Barrier, and have a longer Cooldown.`
- All effects changed to blue
- Health Regeneration buff removed
- Now builds Barrier at a rate of 12 barrier/sec
- Disables Barrier decay while active
- Cooldown increased form 7 seconds to 10 seconds
- Buff time decreased from 4 seconds to 3 seconds
### MISSILE
`Missiles were originally really simple, just doing a large amount of damage. However, they would proc Bands, and didn't have any ability for support like HAN-D's drones do. This makes a more offensive-based support to help R-MOR's purpose as a machine made for combat`
- Damage reduced from 600% to 250%
- Now applies Weak, which will likely be replaced by a custom debuff at some point

## Other Changelogs
`1.3.1`
- Fixed Several Achievements heavily lagging and flooding logs
- Fixed SKEWER not animating when seen by other players

`1.3.2`
- Fixed Formatting error in text.
- Fixed Keywords not appearing.
- Fortify now scales with level
- Fixed Eradicate not actually stunning enemies

`1.2.0`
- Fixed Skewer not unlocking if pre-requisites were broken on a stage prior to the game closing
- R-MOR's rockets should now have slightly better collision online, inform me if I'm incorrect
- Should have fixed any more frame drop problems
- Projectiles now all have unique speeds. Hitting Eradicate should now be easier on airborne enemies
- CLUSTER_CANNON's charge animation properly scales with attack speed
- CLUSTER_CANNON and ERADICATE's animations no longer lock R-MOR's bottom half in-place
- ERADICATE and CLUSTER_CANNON now feature kickback, getting stronger at higher charge levels
- Removed custom explosion effects and lowered radi of all explosions. Range of explosions is no longer deceptively large

`1.1.0`
- Fixed Skewer unlocking irregardless of whether the conditions were met or not
- Fixed ERADICATE erroneously being Agile
- Entirely redid sounds

- **FORTIFY CHANGES**
- Originally, FORTIFY was rediculously powerful and made R-MOR hard to kill despite his weaknesses. In turn, FORTIFY's effectiveness was heavily turned down.
- Added Armor reduced from 200 to 100
- Added Health regeneration reduced from 10% of max health per second to 5% of max health per second

`1.0.0`
- New model texture by dotflare
- More New animations
- CSS Animation Fixed, now uses sounds and screenshake
- All projectiles have new effects and meshes
- Mastery Skin with custom drones and projectiles
- Large amount of new sounds
- Unlocks for all alt skills

- **BALANCE CHANGEs**
- MISSILE Damage reduced from 800% to 600%
- ERADICATE Damage reduced from 420% to 390% to prevent proccing bands

`0.2.5`
- **Removed heavy frame dropping in certain maps.** It's still there to a minor degree, but I do think it's improved quite a bit from before.
- Updated Textures
- New Projectile Particle effects
- Added Effects on using Fortify
- Added Sound and Screen shake on Character Select Screen
- Fixed Overclock not being able to be unlocked

`0.2.2`
- Fixed Readme

`0.2.1`
- Fixed game crashing if Ancient Scepter wasn't installed

`0.2.0`

- Entirely new animations (Thanks, Lui!)
- All projectiles restore Utilities less
- Minor update to CLUSTER_CANNON VFX
- New thruster VFX from missile drones. Active missiles no longer have a particle effect.
- New skill icons!
- SWARM_ASSAULT buff icon has a new color
- SWARM_ASSAULT Attack speed bonus: 10% -> 5%
- FORTIFY Armor: 300 -> 200
- MISSILE: Targeting now works just like HAN-D's DRONE skill
- MISSILE Damage: 300% -> 800%
- Starting Armor: 40 -> 50
- No longer gains armor on Level-Up
- Began work on new ItemDisplays to replace HAN-D's
- Fixed a massive bug where Coil Golems would completely break
- Fixed bug where all stocks of CLUSTER_CANNON would be consumed immediately

`0.1.1`

- I'm stupid (Fixed Language Tokens)
- Sort order defaults to 4.51 (always after HAN-D)
- Removed broken mastery skin

`0.1.0`

- Initial Mod Upload

