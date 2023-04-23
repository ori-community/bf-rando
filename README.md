# Ori DE Randomiser

A new randomiser implementation.

See [orirando.com](https://orirando.com) for the complete, up to date rando.

> :warning: **Requires python 3.10+ for seed generation, download [here](https://www.python.org/downloads/)**
>
> Not required for Archipelago or pre-generated seeds

## Installation

Use the [Mod Manager](https://github.com/Kirefel/bf-mod-manager)

## Current state

Implemented:

* Clues, Shards, Force Trees, World Tour, Warmth Fragments, Standard, Expert, Master, Glitched
* In game seed generation with configurable parameters
* [Archipelago Multiworld](https://archipelago.gg/) (beta, see FAQ for more info)

Upcoming:

* More detailed seed generation options
* Logic filter
* Sharable seeds
* Statistics

## FAQ

### Why write another randomiser?

* To separate out QoL and accessibility features into different mods, allowing you to play with them without it being rando
* For a better developer workflow
* To make it compatible with other mods

### What's different in this version?

There are several new features:

* In-game seed generation options
* Automatic updates through the mod manager
* Sharable seeds
* More QOL features
* Completely controller friendly
* Archipelago

Some things are not available, though they might be added later:

* orirando.com multiplayer
* Bingo
* Expanded bonus skills (e.g. skill velocity upgrade)

### Archipelago?

Yes! There are some caveats currently:

* Only Force Trees with no key mode is supported
* There are no clues (for keys or skills)
* No logic filter
* No bonus pickups e.g. Extra Air Dash
* No teleporters
* Archipelago uses its own generation algorithm, which will result in very different item placements
* Some locations will grant nothing (Archipelago thinkgs plants and cutscenes are items you can receive)
* Keystone duplication does not work
* If you pick up an item and then die without saving, you will keep the item but it will remain on the map. Picking it up grants nothing.

#### **How to play Archipelago**

1. The host starts a game using the Ori and the Blind Forest settings along with any other games
1. Install the AP beta in the mod manager by right clicking "Rando 2 (beta)" and selecting "Install version..."
1. Launch the game and go to the settings. Under Randomiser, enable `Enable Archipelago` and restart the game.
1. When the game loads, there will be a Multiplayer option in the main menu.
1. Enter your name and the appropriate hostname and port. The host will know these details.
1. Click `Connect`
1. Start a new game in a new file using Quick Start when all players are ready

If you ever quit, you will need to reconnect.

If you ever find that you're missing an item you should have, save and quit. You will get it upon loading back in.

### Why does it need python?

Python is used to generate the seed. It's not required if using Archipelago, or if the seed has already been generated elsewhere.

### Where is the spoiler/seed file?

Go to the save file in the main menu and press the indicated button (default: `Tab`/`Back`)

These files are lost when updating the mod or overwriting a save slot, so be sure to make a copy of any you want to keep.

### Sein text is back!

You need the [QoL mod](https://github.com/Kirefel/OriDeQol)
