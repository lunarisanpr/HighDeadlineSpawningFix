# High Deadline Spawning Fix

A simple Lethal Company mod intended to fix entities not spawning when the amount of days before deadline is above a certain number. Meant to be used with mods that increase the quota deadline, such as [ProgressiveDeadline](https://thunderstore.io/c/lethal-company/p/LethalOrg/ProgressiveDeadline/).

This mod should not have any effect if you are not the server host, but may or may not cause desyncs if not all players have the mod (untested).

## About Vanilla Entity Spawning

[Various formulas](https://lethal.miraheze.org/wiki/Mechanics#Spawning_algorithm_(Outdoor_Entities)) related to the minimum number of spawned outdoor and indoor entities depend on the amount of days before deadline. In particular, the calculated **minimum spawn count** for both outdoor and indoor spawns follow the given form:

`adjustedSpawnCurve + abs(daysUntilDeadline-3) / 1.6 - deviation`

while the **maximum spawn count** follows the given form:

`adjustedSpawnCurve + deviation`

During normal, vanilla gameplay, this results in entities being more likely to spawn closer to the deadline, as `daysUntilDeadline` is typically a value between `0` and `3`. However, when `daysUntilDeadline` exceeds `3` (either via save file editing or by deadline-altering mods), the spawn count formula starts to behave in unexpected ways (For the sake of simplicity, days until deadline will be referred to as 'days'):

- At days `4` to `14`, entities start spawning more frequently the further the deadline is, completely opposite of the behavior between days `0` to `3`.
- At day `15` and above, entities cease to spawn.

This is due to the spawning algorithm attempting to choose a number of entities to spawn between the **integer value** (floor) of the minimum spawn count and the maximum spawn count, using `System.Random`. However, starting day `15`, `(int) minimumSpawnCount` becomes **larger** than `(int) maximumSpawnCount`, causing an `ArgumentOutOfRangeException`. 

**High Deadline Spawning Fix** fixes that by modifying the `daysUntilDeadline` values used in the entity spawning formulas, without actually modifying the days until deadline. The way the mod achieves this is via either clamping the `daysUntilDeadline` value used in the calculation to a maximum value (default 3), or by using the modulo (mod 4) of the `daysUntilDeadline` value, configurable in the plugin's config file.

## Config

The plugin has two configuration options:

- **Clamp Type** - Sets the clamp type.
  - None: Vanilla behaviour
  - Clamp: Clamps `daysUntilDeadline` to the maximum value specified by **Clamp Value**
  - Modulo: gets the mod 4 of the daysUntilDeadline used in calculations (Default).

For a better understanding of how Modulo works, it treats higher deadline days as looping cycles of values between `0` and `3` days:

| Actual Days Before Deadline | Entities Spawn As If Days Before Deadline Was... |
| -------- | -------- |
| 0 | 0 |
| 1 | 1 |
| 2 | 2 |
| 3 | 3 |
| 4 | 0 |
| 5 | 1 |
| 6 | 2 |
| 7 | 3 |
| 8 | 0 |
| ... | ... |

- **Clamp Value (Clamp)** - Only used when **Clamp Type** is set to **Clamp**. Sets the maximum amount of days left before deadline. (Default is 3)

## About

This mod was made for v81 and is originally intended for use in a personal duos modpack, and will likely not be maintained in the future. Please do not contact me, as I am a very busy person.

This mod may not be compatible with other mods that modify the game's entity spawning logic.
