# 🌀 Living Labyrinth

🔗 [Read the accompanying blog series](https://maxheinze.com/dev-blog)

A Unity-based exploration of procedural level generation inspired by the classic board game **Verrückte Labyrinth** (*The aMAZEing Labyrinth*).  
This project accompanies my blog series, where I rebuild and extend the game’s shifting maze mechanics step-by-step — from tile logic to dynamic grid updates and procedural layouts.

---

## 🎯 Project Overview

**Living Labyrinth** is a learning and experimentation project designed to demonstrate:
- Procedural generation with tile-based systems  
- Dynamic grid manipulation (sliding tiles, shifting rows)  
- Data-driven design using ScriptableObjects  
- Visual and logical rotation of interconnected elements  

The game’s core idea:
> A labyrinth made of movable tiles — each turn, a spare tile is pushed into the grid, reshaping the maze.

---

## 🧱 Development Phases

Each development phase corresponds to a post in the **blog series**, with code and commits tagged accordingly.

| Phase | Blog Post | Description |
|-------|------------|-------------|
| **1** | [Designing a Dynamic Maze Tile System in Unity](#) | Tile data model, rotations, and grid visualization |
| **2** | *(Coming Soon)* Building the Living Labyrinth Grid | Shifting rows and handling the spare tile |
| **3** | *(Planned)* Procedural Maze Generation | Creating valid, random labyrinth layouts |
| **4** | *(Planned)* Pathfinding & Player Movement | Navigating the shifting maze |
| **5** | *(Planned)* Expansions | Items, objectives, and advanced procedural rules |

Each blog phase is committed to the **main branch** for easy reference.  
Active work happens on the **dev branch**, then merged into main once the post is published.

---

## 🧩 Current Progress

**Phase 1 — Tile System**

Implemented:
- `TileConnection` enum using bit flags for direction handling
- `TileType` ScriptableObject defining base tile shapes
- `Tile` MonoBehaviour for rotation and logic
- `TileGridTest` script to spawn a random grid of tiles

✅ Displays a fully randomized 7×7 grid of rotating tiles on Play. Use Spacebar to regenerate a new random grid.


---

## 🛠️ Setup Instructions

**Requirements**
- Unity 6000.2.9f1
- .NET Standard 2.1 / C# 9 support (default in Unity)

**Steps**
1. Clone the repository  
   ```bash
   git clone https://github.com/maximoh-mmo/labyrinth.git
