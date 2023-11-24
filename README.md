# GYARougelike

# RoomTypes
| Index | RoomType   |
| ------| :--------: |
| 0     | StartRoom  |
| 1     | EnemyRoom  |
| 2     | LootRoom   |
| 3     | CampRoom   |
| 4     | BossRoom   |

Finns också en bool HiddenType som har 20% chans att vara true. Denna gör att spelaren inte kan se vilken typ av rum det är i förväg

# CardTypes
| Index | Type            | Info                                                      |
| ------| :-------------: | --------------------------------------------------------- |
| 0     | Slash           | Attacks the first enemy in line                           |
| 1     | Block           | Gives block to self                                       |
| 2     | Thorns          | Deals back damage to first attacker                       |
| 3     | Cleave          | Attacks every enemy                                       |
| 4     | Bash            | Stuns first enemy in line                                 |
| 5     | Shielded Charge | Give block to self and deal damage to first enemy in line |
| 6     | Roid-Rage       | Buff the other player                                     |

# EnemyTypes
| Index | Type        |
| ------| :---------: |
| 0     | Boss        |
| 1     | BasicGrunt  |
| 2     | Buff/Debuff |
| 3     | Summoner    |
| 4     | Tank        |
| 5     | Summon      |
