## 🎮 Feed the Beast

## 📌 Overview
Feed the Beast is a survival horror/PvP cooking game where two players will fight for their survival by cooking meals for a frightening monster

## 🕹️ Core Gameplay
Assemble meals by finding ingredients hidden around the environment

## 🎯 Game Type
Survial horror, PvP, cooking, pet simulation

## 👥 Player Setup
Two player local co-op

## 🤖 AI Design
### Monster FSM
Idle
Mopving towards food
Sniffing food (deciding who to eat)
Moving towards target
Attacking target
Going to sleep

### Ingredient FSM
Idle when spawned on a surface
PickedUp set PlayerHandFree to False
InMeal when added to a plate

## 🎬 Scripted Events
Pet heading to sleep, indicating the time to cook food for it begins
Audio cues to indicate how much time is left (30 seconds, 10 seconds etc.) and the pet will wake up at the end of the timer
the pet waking up and observing both players created meals before deciding its own action
At the start of the timer, ingredients will drop for both users to collect for preparation

## 🌍 Environment


## 🧪 Physics Scope


## 🧠 FSM Scope


## 🧩 Systems and Mechanics


## 🎮 Controls (proposed)
W A S D move  
Mouse look  
Space jump  
E pick up/drop ingredient

## 📂 Project Setup aligned to course topics
