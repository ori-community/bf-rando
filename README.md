# Ori DE Randomiser

A new randomiser implementation.

See [orirando.com](https://orirando.com) for the complete, up to date rando.

> :warning: **Requires python 3.10+, download [here](https://www.python.org/downloads/)**

## Installation

Use the [Mod Manager](https://github.com/Kirefel/bf-mod-manager)

## Manual Installation

Requires [Ori DE Mod Loader](https://github.com/ori-community/bf-modloader)

TODO

## Current state

Implemented:

* Everything in a typical Standard Clues or Shards seed, unless listed below
* In game seed generation - simply start a new file to generate a Standard Clues with Force Trees game

Upcoming:

* World Tour
* Forlorn escape plant clues
* In-game seed generation options
* Sharable seeds

## FAQ

### Why write another randomiser?

* To separate out QoL and accessibility features into different mods, allowing you to play with them without it being rando
* For a better developer workflow
* To make it compatible with other mods

### Where is the spoiler/seed file?

In the mod's install location (default `%APPDATA%\ori-bf-mod-manager\mods\RandoBeta\seeds`). The folders are named by date. These will be deleted whenever the mod is updated so be sure to save a copy of any you want to keep. The seed is also stored in the save file of the game itself so it won't be affected if the `randomizer0.dat` file is deleted.

This will become more easily accessible later.

### Sein text is back!

You need the [QoL mod](https://github.com/Kirefel/OriDeQol)
