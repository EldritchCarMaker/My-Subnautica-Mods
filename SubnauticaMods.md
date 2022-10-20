# My Subnautica Mods

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
