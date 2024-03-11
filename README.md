# GYARougelike
A simple multiplayer roguelike game inspired by other roguelikes like "Slay the Spire".

# Architecture

### UI
Setting visibility of UI elements **HAS** to be done via ClientRpc's so they properly update visibility for all users.

Client specific UI is instantiated on each client individually and synced with the help of SyncVars. The state of each UI element is saved in the corresponding SyncVar and the values of these SyncVars **HAVE** to be set and changed in Commands or else they wont update for all clients. The corresponding SyncVar Hook is called for each instance of the script, thus removing the need of updating UI state via ClientRpc's.

### Rooms
When entering or exiting rooms, the functions for entering and exiting **should only** be called on one instance of the script, but setting visibility of said room is done via ClientRpc's as mentioned above. 

# Types

### RoomTypes
| Index | RoomType   | Count |
| ------| :--------: | :---: |
| 0     | StartRoom  | 1     |
| 1     | EnemyRoom  | 5     |
| 2     | LootRoom   | 2     |
| 3     | CampRoom   | 3     |
| 4     | BossRoom   | 1     |
|       |            |       |
| Bool  | HiddenType | 2     |

### CardTypes
| Index | Type            | Info                                                      |
| ------| :-------------: | --------------------------------------------------------- |
| 0     | Slash           | Attacks the first enemy in line                           |
| 1     | Block           | Gives block to self                                       |
| 2     | Thorns          | Deals back damage to first attacker                       |
| 3     | Cleave          | Attacks every enemy                                       |
| 4     | Bash            | Stuns first enemy in line                                 |
| 5     | Shielded Charge | Give block to self and deal damage to first enemy in line |
| 6     | Roid-Rage       | Buff the other player                                     |

### EnemyTypes
| Index | Type        |
| ------| :---------: |
| 0     | Boss        |
| 1     | BasicGrunt  |
| 2     | Buff/Debuff |
| 3     | Summoner    |
| 4     | Tank        |
| 5     | Summon      |
