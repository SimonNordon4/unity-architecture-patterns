# Project Overview

The sample game is a 'Survivors' like game where your character must survive 14 waves consisting of a variety of enemies which get progressively more difficult. Collecting chests will grant you upgrades to keep up with this difficulty.

## Scene Structure

There are 5 scenes to consider.

- Main Menu
- Game
- Game Won
- Game Lost
- Dungeon

The reason for these scenes is to showcase how we can handle inter-scene dependencies. While it's not neccesary in this small game, there are good reasons to break up large games into multiple scenes. 'Dungeon' is a shared scene among all Patterns, which gives an opportunity ot introduce additive scene loading.

## Application Management

**Application Lifecycle:** At the moment we can only quit the game.

**Scene Management:** Handles the loading of our 5 different scenes.

**User Settings:** Stores settings like Volume in the Player Prefs and persists between sessions.

## Game Management

The Game is won when all waves of enemies have been killed.

The Game is lost when the player loses all their health.

The Game can be paused, resumed, restarted or quit.

The total amount of enemies killed, and time will be displayed on the Win or Lose Screen.

## Player Controller

The Player has different Stat attributes, such as Speed, Damage, Crit Chance etc which effect it's behaviour.

The Player can collect Items, that containt modifiers to enhance these stats by picking up Chests that spawn during the game.

Chests spawn every 25 seconds, but this time is lowered whenever an enemy is killed.



