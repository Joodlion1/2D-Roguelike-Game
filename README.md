# 2D Roguelike Game — SWE 402 Assignment 2

A turn-based roguelike game built with Unity 6, following the Unity Learn "Create a 2D Roguelike Game" course and extended with additional features.

## Features

- Procedurally generated 8x8 dungeon with random ground/wall tiles
- Turn-based movement with arrow key controls
- Enemy AI that chases and attacks the player
- Destructible walls with health and damage states
- Food resource system with starvation mechanic
- Level progression via exit cell
- Game Over screen with restart (Space key or Restart button)
- Sprite-based animations (Idle, Walk, Attack) for player and enemy
- UI Toolkit interface for food counter and game over panel

## Additional Requirements (Section 7)

- **Audio**: Background music via AudioSource on Main Camera; SFX using PlayOneShot() for player movement, wall attack, food pickup, enemy attack, enemy death, and game over
- **Visual Effects**: ParticleSystem effects for wall destruction, food collection, and enemy death, controlled via .Play() in code
- **Smooth Cell Movement**: Coroutine-based smooth movement that lerps objects between cells; input blocked during movement
- **Object Pooling**: CellObjects (food, walls, enemies) are pre-instantiated in pools and recycled via SetActive(false/true) on level transitions
- **Code Standards**: All Inspector-exposed fields use [SerializeField] on private variables; numeric tuning variables use [Range()] and [Tooltip()]

## Controls

- **Arrow Keys**: Move player
- **Space**: Restart game after Game Over

## Build Targets

- WebGL (index.html)
