# 👽 Space spies
## 📝 Concept
A serious game designed to teach Virtual Reality concepts.. You play an alien infiltrating human society. Your mission: to gather intelligence through espionage.
There are different phases of gameplay such as espionage, infiltration and crossword puzzles to validate what has been learned.

## 🔃 Recovery guide 

### 🛠️ Installation and Prerequisites 
- **Unity Version** : 6000.2.9f1
- **Plateforme :** PC / WebGL.

### ▶️ Steps
1. Clone the repository
2. Open Unity Hub and the project
3. Load the project in the "Menu" scene

## 📁 Architecture 
```
Asset/
├── Scenes
├── Font
├── Packages
├── Prefabs
├── Resources/
│   ├── 2D
│   ├── 3D
│   ├── Materials
│   ├── Shaders
│   └── Sounds
├── Scriptables
└── Scripts/
    ├── Camera
    ├── Cinematics
    ├── Detection
    ├── Dialogues
    ├── Grid
    ├── Interactions
    ├── Managers
    ├── Movement
    ├── Player
    ├── SpaceMainRoom
    └── UI
```

## ✏️ Editable scriptables VR objects
In the ``Asset/Scriptables/`` there are a few cards describing objects that can be easily edited in Unity

<img width="800" height="719" alt="image" src="https://github.com/user-attachments/assets/fef7003a-6b8e-4546-810c-e5dd536cdba7" />
<img width="781" height="811" alt="image" src="https://github.com/user-attachments/assets/ff0fb9cc-7744-4a23-93de-1b750938e1e5" />

## 🗄️Crosswords
Crosswords are grids generated from a list of words. The size of the Grid is 20 (width) by 14 (height). Words may contain a hyphen.
Each word requires:
- Word string
- Starting X-coordinate
- Starting Y-coordinate
- Direction of the word horizontal / vertical
- Clue to help guessing the word when the cursor hover the cells
<img width="585" height="388" alt="image" src="https://github.com/user-attachments/assets/52738088-361f-4851-8331-e87fc88260b6" />
