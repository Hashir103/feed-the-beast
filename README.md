## 🎮 Feed the Beast

## 📌 Overview
Feed the Beast is a survival horror/PvP cooking game where two players will fight for their survival by cooking meals for a frightening monster

## 🕹️ Core Gameplay
Assemble meals by finding ingredients hidden around the environment

## 🎯 Game Type
Survial horror, PvP, cooking, pet simulation

## 👥 Player Setup
There are two players in the game– locally connected. The two players are responsible for the same core gameplay mentioned above, however they will be working against each other for their own survival. Currently, the scope of our project is to be developed with the idea of two players playing split-screen on the same device. However, if it fits in our timeline, we’d like to include an online multiplayer option so that players can join from different devices and only have their own character’s point of view on their screen. Online connection would entail a user sharing a private code for their player two to join with. Each player will only be able to see a small portion of the overall map. Players will be able to navigate around the map using the WASD keys and perform actions like picking up, cooking, and serving.

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
There are a few scripted events in our game. The first one is the pet heading to sleep, indicating the time to cook food for it begins. Along with the pet sleeping, there will be multiple audio cues to indicate how much time is left (30 seconds, 10 seconds etc.) and the pet will wake up at the end of the timer. The end of timer would have another scripted event– the pet waking up and observing both players created meals before deciding its own action.

At the start of the timer, food will also drop for both users to collect for preparation. This scripted event will determine randomly the quality of the ingredients given for the users to cook with. 

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
