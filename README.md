# UPDATE
This repository is outdated for several years now. It used to be a portfolio project. Since then, Safe And Sound has developed into a full fledged stealth shooter prototype. See the video below.



https://github.com/bastingup/SafeAndSoundExampleScripts/assets/28649176/5de02424-c926-4069-b53d-5e5ca42346be




# SafeAndSoundExampleScripts
A collection of scripts demonstrating my work and approaches in creating my indie game Safe And Sound.

Safe And Sound is a cyberpunk stealth shooter in third person. You play as three charaters: Ember, Ezra, and Adam. You can switch between them at any given point in time, the two other character will follow the active one or stay put, depending on the command given.

The game is created with Unity3D/C#. The design of the systems follows the Singleton Pattern. These example scripts show some character controlling, management, and UI scripts.
The skill system is based on ScriptableObjects.

The derivation is the same for all characters:
- Creature
  - Character
    - PlayerController
      - Ember
      - Ezra
      - Adam
    - NPC
      - MilitaryPolice
      - GenericCivillian

For visual/dev content visist:
https://www.instagram.com/hopelessheartsgames/

Check it out on my homepage:
https://bastingup.github.io/html/sas.html

![alt text](./Images/sas_01.gif)

![alt text](./Images/sas_02.gif)
