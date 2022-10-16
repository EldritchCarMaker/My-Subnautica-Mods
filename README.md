# Subnautica Mods
 
### [Better Cyclops Lockers](https://github.com/Nagorogan/My-Subnautica-Mods/files/8884799/BetterCyclopsLockers.zip)


Increases the width and height of the cyclops lockers by a configurable amount (1-15 each on nexus, 1-100 each from this file. I trust github users a bit more, please don't disappoint.) and allows the lockers to function as an autosort target meaning that any placed autosortlocker within the cyclops can automatically deposit to the lockers. 

Dependencies: [AutosortLockers](https://www.nexusmods.com/subnautica/mods/31), [SmlHelper](https://www.nexusmods.com/subnautica/mods/113)
###### [Changelog](https://github.com/Nagorogan/My-Subnautica-Mods/blob/main/BetterCyclopsLockers/Changelog.md)


### [Cyclops Torpedoes](https://github.com/Nagorogan/My-Subnautica-Mods/files/9438326/CyclopsTorpedoes.zip)


Adds a new cyclops module that allows you to shoot torpedoes from the cyclops. Simply place torpedoes into the Cyclops' decoy tube and then enter the cyclops external cameras. From there, just press the torpedo button and, assuming you have the cyclops module equipped, the cyclops will shoot out the torpedo in whatever direction the camera is facing. Should work with modded torpedoes, although I'm not certain if it works with all of them or just some of them.

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113), [More Cyclops Upgrades](https://github.com/PrimeSonic/PrimeSonicSubnauticaMods/releases/tag/Aug5_2021)

###### No Changelog Yet


### [Cyclops Vehicle Upgrade Console](https://github.com/Nagorogan/My-Subnautica-Mods/files/9641307/CyclopsVehicleUpgradeConsole.zip)


Improves the vehicle management terminal inside the cyclops (the terminal above the docking bay) to also function as a vehicle upgrade console. Meaning you can change the color and name of your vehicle. More importantly, allows you to fabricate a new seamoth/prawn suit directly inside the cyclops' empty docking bay. Supports easy craft's pulling of resources from other containers. Also adds in a button on the left of the terminal to allow you to deconstruct any vehicle currently docked in the cyclops, giving your resources back.
![image](https://user-images.githubusercontent.com/97289845/180631717-a8fe5e74-df81-4ea1-b855-070497a76f98.png)
![image](https://user-images.githubusercontent.com/97289845/180631735-876381ca-79b8-4775-96d3-ac04670e7da7.png)
![image](https://user-images.githubusercontent.com/97289845/180631757-a43c7c33-76dc-4077-855c-cbba4f261c72.png)

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113)
###### [Changelog](https://github.com/Nagorogan/My-Subnautica-Mods/blob/main/CyclopsVehicleUpgradeConsole/Changelog.md)


### [Equivalent Exchange](https://github.com/Nagorogan/My-Subnautica-Mods/files/9778603/EquivalentExchange.zip)


A very cool mod allowing for transmutation of items into other items. After Picking up an ion cube, you can build the Item Research Station (mod still kinda beta, no real model yet. Temp one for now. Still looks nice tho) which will allow you to deposit items in in order to unlock them for exchange, and gain EMC from them. You can then open the Exchange Menu by pressing the two menu keys at the same time (temporary until I get a model in place) and then admire the beauty of it. Once you're done admiring, you can choose an item from one of the five tabs and, assuming you have the EMC required, spawn that item directly in your inventory. You can also hold shift while selecting an item to create five of those items, at five times the cost. In case you want a *lot* of copper.

The EMC cost for items is calculated based off of their recipes, with unknown items that don't have a recipe havin a default value of 100. This also allows the mod to work almost perfectly with modded items, with the only exceptions being modded items that may not have a recipe attached to them. I have tried to make this mod as balanced as I could, but if you aren't a fan of it, there are many configs in it to change things. You can set an inefficiency multiplier for making items cost slightly more than you get from them. You can change the costs of specific resources, or even fully crafted items if you desire by simply adding an entry into the BaseMaterialCosts config. You can also blacklist certain items from getting unlocked, or blacklist all items with a specific string in their name. This isn't super useful for casual play, but becomes incredibly good for combining with the `ExchangeUnlockAll` command to unlock all items in the exchange menu, and the `nocost` cheat to effectively turn it into a creative menu.

Whats more is this mod also pairs with the FCS mods if you have those installed (no worries if you don't, mod still works), and allows you to convert EMC to alterra credits and back. I tried to make the conversion alright, but admittedly it's not the best conversion, so there's a config for EMC => FCS credit conversion rate if you don't feel like the default rate is fair!

Look how nice the exchange menu is. I'm proud of this thing.
![image](https://user-images.githubusercontent.com/97289845/193378643-3c588a87-7dfd-4d9a-87b7-3d07acc86dfd.png)

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113)
###### [Changelog](https://github.com/Nagorogan/My-Subnautica-Mods/blob/main/EquivalentExchange/Changelog.md)

### [Auto Storage Transfer](https://github.com/EldritchCarMaker/My-Subnautica-Mods/files/9795861/AutoStorageTransfer.zip)


a very simple, yet powerful mod that allows you to link storages to automatically transfer to or from each other.

Simply craft the storage transfer controller with some magnetite, and then look at a storage container. From there, you can set it to transmit/recieve, and change the id of it. a storage set to transmitter will send items to any storages set to reciever that have the same id. You can also have multiple storages on transfer and only one on reciever.

Storages can only have one single channel, and the transfers do not have any form of sorting built in. However, this does not mean you can't sort stuff. What this means is that you'll have to be creative. Instead of having a container put all magnetite here and titanium there, you have a container automatically transfer to an [autosort locker](https://www.nexusmods.com/subnautica/mods/31) that then sorts the items, and you can then use auto storage transfer to send those now sorted items somewhere else

This mod also has compatibility with fcs mods, and you can set a base as a reciever and it'll put all items sent to it into the storage disks. To do this, just craft a C48 terminal, look at it with the storage transfer controller, and set the id to whatever you want. Any transmitters on that id will send items directly to any free storage disks.


Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113)
###### [Changelog](https://github.com/EldritchCarMaker/My-Subnautica-Mods/blob/main/AutoStorageTransfer/Changelog.md)


### [Auto Storage Transfer Compatibility](https://github.com/EldritchCarMaker/My-Subnautica-Mods/files/9795859/AutoStorageTransferCompatibility.zip)


This mod pairs alongside Auto Storage Transfer (linked above) and many other mods, including ion cube generator(I'll put a link here later) and even the cyclops reactor mods. Also allows auto storage transfer to work with many extra vanilla items, such as the water filtration unit and the base reactors.


Dependencies: Auto Storage Transfer
###### [Changelog](https://github.com/EldritchCarMaker/My-Subnautica-Mods/blob/main/AutoStorageTransferCompatibility/Changelog.md)


### [No FCS Drone Port](https://github.com/Nagorogan/My-Subnautica-Mods/files/9727254/NoFCSDronePort.zip)


Allows fcs mods to deliver to depots even if they don't have a drone port built. Works with cyclops, meaning you can now have items be delivered directly to your cyclops! or if you just don't like waiting for the drone to get to your base, this mod also solves that problem. Combined with equivalent exchange to act as a way to get alterra credits without an ore consumer you can fully utilize the FCS mods from within your very own cyclops

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113)
###### No Changelog Yet

### [Equippable Item Icons](https://github.com/Nagorogan/My-Subnautica-Mods/files/8865337/EquippableItemIcons.zip)


An API for creating new items and icons for said items. Very configurable, ask on discord for information on how to use. Still in progress, but it's basically done apart from adding new things and making sure its bug free.

Any mod that uses this API will have an icon next to the quickslots for each equipped item with a special icon. This icon may or may not drain, depending on the mod it comes from.

#### [All-In-One Package](https://github.com/Nagorogan/My-Subnautica-Mods/files/9689290/All-In-One.zip)

This file has every single mod I have made that utilizes Equippable Item Icons. Easier to use this than download each individually if you just want them all. Does not work with vortex/mod managers

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113)
###### [Changelog](https://github.com/Nagorogan/My-Subnautica-Mods/blob/main/EquippableItemIcons/Changelog.md)


### [Warp Chip](https://github.com/Nagorogan/My-Subnautica-Mods/files/9689291/WarpChip.zip)


adds a new equippable chip that warps you forward a slight distance, blueprint unlocks by scanning a warper. Key is configurable, distance is 15m out of base and 10m in base(configurable). Cooldown is 5 seconds (configurable) but is shortened if you don't teleport the full distance. Can also hold the warp key for about 5 seconds in order to teleport back to the last base you entered (including cyclops, and config for including lifepod).

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113), Equippable Item Icons
###### [Changelog](https://github.com/Nagorogan/My-Subnautica-Mods/blob/main/WarpChip/Changelog.md)


### [Spy Watch](https://github.com/Nagorogan/My-Subnautica-Mods/files/9689293/SpyWatch.zip)


Adds a new equippable watch(hud chip) that allows you to go invisible for a short amount of time. Creatures shouldn't be able to detect you while invisible, but if something saw you first they may still try to attack. The key is configurable, and after equipping the watch an icon should show up in the bottom right near the quickslots. When using the watch, the icon will slowly drain and this is your indication of how long you have left to be invisible.


Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113), equippable item icons
###### [Changelog](https://github.com/Nagorogan/My-Subnautica-Mods/blob/main/SpyWatch/Changelog.md)


### [Burst Fins](https://github.com/Nagorogan/My-Subnautica-Mods/files/9689294/BurstFins.zip)


Adds a new set of fins as an upgrade to the ultra glide fins. These ones have a short lasting but quick charging battery that can allow for short bursts of incredible speed even while using the seaglide

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113), equippable item icons
###### [Changelog](https://github.com/Nagorogan/My-Subnautica-Mods/blob/main/BurstFins/Changelog.md)


### [Shield Suit](https://github.com/Nagorogan/My-Subnautica-Mods/files/9689295/ShieldSuit.zip)


Adds a new suit that allows for the use of a toggleable personal shield. Doesn't last forever, but lasts long enough and recharges pretty quickly. 

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113), equippable item icons
###### No Changelog yet

### [Miniature Suit](https://github.com/Nagorogan/My-Subnautica-Mods/files/9689299/MiniatureSuit.zip)


Simple mod. Equip suit, press button, now smol ryley. Press button again, normal ryley.

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113), equippable item icons
###### No Changelog yet

### [Stasis Suit](https://github.com/Nagorogan/My-Subnautica-Mods/files/9689300/StasisSuit.zip)


This suit uses Stasis rifle technology to temporarily freeze internal organs in time, halting natural resource consumption. Meaning this suit can, temporarily, stop all need for food, water, oxygen, and even stop you from getting hurt!

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113), equippable item icons
###### No Changelog yet

### [Time Control Suit](https://github.com/Nagorogan/My-Subnautica-Mods/files/9689301/TimeControlSuit.zip)


This suit uses Stasis rifle technology but brings it to the max. For a very short amount of time, with a fairly long cooldown, can completely *stop time itself* all around you, while still allowing you to remain free. It's unknown exactly who made this suit, all you know is it randomly appeared in your blueprints once finding an ion cube and you can't possibly pass it up.

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113), equippable item icons
###### No Changelog yet

### [Sonar Chip](https://github.com/Nagorogan/My-Subnautica-Mods/files/9689297/SonarChip.zip)


Adds a new equippable chip that can be activated to let out a sonar burst. Has five charges that slowly regenerate and has a one second delay after use before recharge will begin.

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113), equippable item icons
###### [Changelog](https://github.com/Nagorogan/My-Subnautica-Mods/blob/main/SonarChip/Changelog.md)


### [Camera Drone Upgrades](https://github.com/Nagorogan/My-Subnautica-Mods/files/9173380/CameraDroneUpgrades.zip)



Another API for improving the camera drones. This mod by default also adds scanning functionality to all drones, without requiring an upgrade to be equipped.
Any mod that uses this API will use an upgrade module that must be inserted into the scanner room upgrades slot in order for the drones to be able to use the upgrades.

#### [All In One Package](https://github.com/Nagorogan/My-Subnautica-Mods/files/9541082/All-In-One.zip)


This zip file contains every single camera drone mod I have made up until the point of writing this. Just for if you don't feel like downloading each one individually. Does not work with vortex/mod managers

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113)
###### [Changelog](https://github.com/Nagorogan/My-Subnautica-Mods/blob/main/CameraDroneUpgrades/Changelog.md)


### [Camera Drone Defense Upgrade](https://github.com/Nagorogan/My-Subnautica-Mods/files/9541084/CameraDroneDefenseUpgrade.zip)


uses Camera Drone Upgrades to add an upgrade module that allows drones to automatically shock creatures that attempt to grab the drone.

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113), Camera Drone Upgrades
###### No Changelog Yet


### [Camera Drone Flight Upgrade](https://github.com/Nagorogan/My-Subnautica-Mods/files/9541086/CameraDroneFlightUpgrade.zip)


uses Camera Drone Upgrades to add an upgrade module that allows drones to fly and have a hover.

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113), Camera Drone Upgrades
###### No Changelog Yet


### [Camera Drone Repair Upgrade](https://github.com/Nagorogan/My-Subnautica-Mods/files/9541088/CameraDroneRepairUpgrade.zip)


uses Camera Drone Upgrades to add an upgrade module that allows drones to repair objects they are looking at.

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113), Camera Drone Upgrades
###### No Changelog Yet


### [Camera Drone Shield Upgrade](https://github.com/Nagorogan/My-Subnautica-Mods/files/9541089/CameraDroneShieldUpgrade.zip)


uses Camera Drone Upgrades to add an upgrade module that allows drones to use a small personal shield.

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113), Camera Drone Upgrades
###### No Changelog Yet


### [Camera Drone Speed Upgrade](https://github.com/Nagorogan/My-Subnautica-Mods/files/9541090/CameraDroneSpeedUpgrade.zip)


uses Camera Drone Upgrades to add an upgrade module that allows drones to use a speed boost at the cost of energy drain.

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113), Camera Drone Upgrades
###### No Changelog Yet


### [Camera Drone Stasis Upgrade](https://github.com/Nagorogan/My-Subnautica-Mods/files/9541091/CameraDroneStasisUpgrade.zip)


uses Camera Drone Upgrades to add an upgrade module that allows drones to Deploy a stasis bubble around them. Drones are also immune to the stasis bubble while they have this module equipped.

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113), Camera Drone Upgrades
###### No Changelog Yet


### [Camera Drone Stealth Upgrade](https://github.com/Nagorogan/My-Subnautica-Mods/files/9541092/CameraDroneStealthUpgrade.zip)


uses Camera Drone Upgrades to add an upgrade module that allows drones to toggle a stealth mode, in which creatures can't target them or grab them. If a creature already detects the drone before activating stealth, it might continue to attack the drone though. 

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113), Camera Drone Upgrades
###### No Changelog Yet
###### [To Do List](https://github.com/Nagorogan/My-Subnautica-Mods/blob/main/CameraDroneStealthUpgrade/ToDoList.md)


### [Pickupable Vehicles](https://github.com/Nagorogan/My-Subnautica-Mods/files/9641184/PickupableVehicles.zip)


Allows you to pick up vehicles. Has a config for whether it needs a special module to be equipped or if it's just an innate ability. Can change the inventory size of them. Warning: Picking up/dropping the prawn is quite buggy and thus is not allowed by default. You can enable this, but its not recommended for now. Just stick with the seamoth. With new update, also allows for picking up and dropping of the cyclops itself! May cause lighting issues, or misc issues so isn't recommended for constant use, more as a "oh no cyclops stuck" fix.

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113)
###### [Changelog](https://github.com/Nagorogan/My-Subnautica-Mods/blob/main/PickupableVehicles/Changelog.md)

### [Edible Everything](https://github.com/Nagorogan/My-Subnautica-Mods/files/9641187/EdibleEverything.zip)

joke mod made by request of a friend, turns every pickuppable item into food. May not work fully with every single item consistently, but does work well enough for the vast majority of things.

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113)
###### No Changelog.


### [Miniature Vehicles](https://github.com/Nagorogan/My-Subnautica-Mods/files/9541109/MiniatureVehicles.zip)


Adds in a vehicle module that allows you to shrink down to half size while in vehicles


Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113)
###### No Changelog yet


### [Seeds From Harvesting](https://github.com/Nagorogan/My-Subnautica-Mods/files/8841548/SeedsFromHarvesting.zip)


Gives you one seed whenever you harvest a plant like a mushroom or marble melon. Don't need to knife them if you just want to keep the farm sustained. If you want to expand the farm, still need to knife them to get all 4 seeds though. This mod also stops you from planting fully grown plants like the marble melon, now you need to plant the seeds instead. There is a config for this but if you set it to false you can just plant and harvest rapidly to duplicate as many seeds as you want. 


Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113)
###### [Changelog](https://github.com/Nagorogan/My-Subnautica-Mods/blob/main/SeedsFromHarvesting/Changelog.md)


### [ECM Mod Logger](https://github.com/Nagorogan/My-Subnautica-Mods/files/9690562/ECMModLogger.zip)


Keeps track of your most valuable mods, and alerts you to potential problems and flaws in your mod list! Keep your eyes open when starting the game.


No Dependencies
###### No Changelog yet


### [Cyclops Antenna Fix](https://github.com/Nagorogan/My-Subnautica-Mods/files/9541098/CyclopsAntennaFix.zip)


Small mod to simply fix the bug where the cyclops antennas will slowly move off to the left, eventually completely detaching from the sub altogether


Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113)
###### No Changelog yet


### [Sound Commands](https://github.com/Nagorogan/My-Subnautica-Mods/files/8841549/SoundCommand.zip)


Mostly for mod making, adds three commands to try out sounds. [Link to SN1 sounds](https://github.com/Nagorogan/ECCLibrary/blob/main/Wiki/Resources/SN/FMODEventPathsSN1.txt). Just copy paste one of those after the command PlaySound and it'll play the sound. For loops, use PlayLoopSound and StopSounds or else the sound will just go on forever. 

example: PlayLoopSound event:/sub/base/nuke_gen_loop


Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113)
###### No Changelog Yet


### [Recharger Chips](https://github.com/EldritchCarMaker/My-Subnautica-Mods/files/9795844/RechargerChips.zip)


Three new chips! Each will slowly charge any batteries stored within your inventory. One uses solar power to do it, another thermal, the final uses both.


Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113)
###### [Changelog](https://github.com/EldritchCarMaker/My-Subnautica-Mods/blob/main/RechargerChips/Changelog.md)


### [Cyclops Windows](https://github.com/Nagorogan/My-Subnautica-Mods/files/8841553/CyclopsWindows.zip)


Moves the cyclops lockers to the other side of the wall, replacing them with some super sweet windows. Got a cool button to toggle the windows too
![image](https://user-images.githubusercontent.com/97289845/165867823-b09aecdd-6cc8-4f7b-9292-608a6b5032a3.png)

picture of button location
![image](https://user-images.githubusercontent.com/97289845/172463513-9cda71e9-63cc-4949-8c07-05e2aecfd2fb.png)



###### [Changelog](https://github.com/Nagorogan/My-Subnautica-Mods/blob/main/CyclopsWindows/Changelog.md)


### [Invincible Docked Vehicles](https://github.com/Nagorogan/My-Subnautica-Mods/files/8841554/InvincibleDockedVehicles.zip)


Makes vehicles invincible when they're docked. More of a bandaid fix for a few bugs in game, but this is the only way I can think of to get them fixed for now at least.

###### No Dependencies
###### [Changelog](https://github.com/Nagorogan/My-Subnautica-Mods/blob/main/InvincibleDockedVehicles/Changelog.md)


### [Remote Control Vehicles](https://github.com/Nagorogan/My-Subnautica-Mods/files/9166554/RemoteControlVehicles.zip)


Adds three new items to the game; a remote control, a common vehicle module(usuable for seamoth and prawn), and a cyclops module. Both modules allow you to remotely control the vehicle it's equipped in but you do need to use the remote control to control them. Only one remote controllable vehicle is allowed of each type. ie; Only one cyclops can be equipped with the module, and only one prawn/seamoth can be equipped with the module. 

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113), [More Cyclops Upgrades](https://github.com/PrimeSonic/PrimeSonicSubnauticaMods/releases/tag/Aug5_2021)

###### [Changelog](https://github.com/Nagorogan/My-Subnautica-Mods/blob/main/RemoteControlVehicles/Changelog.md)
###### [To Do List](https://github.com/Nagorogan/My-Subnautica-Mods/blob/main/RemoteControlVehicles/ToDoList.md)


### [Drooping Stingers Nerf](https://github.com/Nagorogan/My-Subnautica-Mods/files/8227628/DroopingStingersNerf.zip):


Decreases the damage drooping stingers do while also fixing the bug that causes them to near instantly kill things.

### [Player Tool Changes_SN](https://github.com/Nagorogan/My-Subnautica-Mods/files/8841555/PlayerToolChanges_SN.zip)


Allows you to configure the stats of most tools in game. Must re-equip tool for changes to take effect.

Dependencies: [SmlHelper](https://www.nexusmods.com/subnautica/mods/113)
###### [Changelog](https://github.com/Nagorogan/My-Subnautica-Mods/blob/main/PlayerToolChanges_SN/Changelog.md)


### [Adaptive Teleporting Costs](https://github.com/EldritchCarMaker/My-Subnautica-Mods/files/9795838/AdaptiveTeleportingCosts.zip)


Changes the FCS quantum teleporter to drain power based on distance teleported rather than simply if the teleporter is in the same base or not. Also lets you configure the min and max costs and the scaling rate of cost over distance.

Dependencies: [FCS Home Solutions](http://fcstudioshub.com/subnautica/home-solutions/)
###### [Changelog](https://github.com/EldritchCarMaker/My-Subnautica-Mods/blob/main/SeaDragonHealthBuff/Changelog.md)

# Subnautica Below Zero Mods

### [Faster SnowFox Charging](https://github.com/Nagorogan/My-Subnautica-Mods/files/8227669/FasterSnowFoxCharging.zip):


Makes the snowfox docking pad recharge and repair at a configurable rate, also allows the pad to refill percentages of power/health rather than fixed values.

Dependencies: [SmlHelperZero](https://www.nexusmods.com/subnauticabelowzero/mods/34)
###### No Changelog yet


### [Flying SeaTruck](https://github.com/Nagorogan/My-Subnautica-Mods/files/8694119/FlyingSeaTruck.zip)


adds a module that lets the seatruck fly, has a config for whether the seatruck drops to the ground when not piloted or stays in air

Dependencies: [SmlHelperZero](https://www.nexusmods.com/subnauticabelowzero/mods/34)
###### No Changelog yet


### [Player Tool Changes_BZ](https://github.com/Nagorogan/My-Subnautica-Mods/files/8309959/PlayerToolChanges_BZ.zip):


Allows you to configure the stats of most tools in game. Must re-equip tool for changes to take effect. Has option for Air bladder changes, currently unused and too lazy to remove. Just pretend the air bladder config doesn't exist.

Dependencies: [SmlHelperZero](https://www.nexusmods.com/subnauticabelowzero/mods/34)
###### [Changelog](https://github.com/Nagorogan/My-Subnautica-Mods/blob/main/PlayerToolChanges_BZ/Changelog.md)


### [SnowFox Quantum Locker](https://github.com/Nagorogan/My-Subnautica-Mods/files/8227678/SnowFoxQuantumLocker.zip):


Adds a storage container to the snowfox. The storage container has a config to choose between three possible options. **ONLY THE STANDARD LOCKER HAS BEEN TESTED**. I believe that the quantum locker also works, however I am not certain to what degree. The snowfox shared locker does not work at all.

The three options are

standard: just a standard locker, hit the key in the config menu to open while riding a snowfox. 4x4 space.

Quantum: gives the snowfox access to the quantum locker network (each snowfox counts as a quantum locker for the purposes of mods such as [Quantum Locker Enhanced](https://www.nexusmods.com/subnauticabelowzero/mods/91)). Press the key in the config menu to open the quantum locker network while riding a snowfox.

Snowfox: **DOES NOT WORK, TO BE DONE LATER** Makes a unique locker that is shared between all snowfoxes on the map. Press the key in the config menu to open the snowfox shared locker network. 4x4 space.

Dependencies: [SmlHelperZero](https://www.nexusmods.com/subnauticabelowzero/mods/34)
###### No Changelog yet


### old versions 
[ShieldChip.zip](https://github.com/Nagorogan/My-Subnautica-Mods/files/8702937/ShieldChip.zip) - old version of shield suit from before I turned it into a suit, its just more buggy, less refined, and is a hud chip rather than a suit. Please use the shield suit instead, but if you really want a hud chip rather than a suit, here you are. I will not be providing updates to this mod as I don't even have the code for it anymore, it was all replaced by the shield suit. You're lucky I even have this much here



# Thanks!
Huge thanks to the subnautica modding discord, which is probably where you came from, for helping me with the vast majority if not all of these mods. Wouldn't have them without major help from people there.

Many icons and sprites were made by Tom Stone#0118 and Akari - アカリ#1302 on discord, so thanks to them for the great icons used in my mods!

## Patreon cause someone wanted to donate at one point

https://www.patreon.com/user?u=79717901 I don't even know if they still want to, I just made it so why not link it?

## Permissions and stuffs
I give permission to anyone and everyone to modify and or distribute my mods and the files contained within, under the following restrictions. 

I only ask that 

1; do not upload my files to nexus mods, even if modified. 

2; please give credit to me as the original author if possible. 

3; if I ask for you to remove one of my mods that you have put up somewhere, please do so.

In addition, I simply ask that if you upload one of my mods somewhere, let me know. You can do this by messaging me on discord (EldritchCarMaker#8108), or letting me know through github. Discord is preferable.
