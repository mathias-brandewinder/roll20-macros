# Roll20 macros

Goal: make it less painful to work with Roll 20 macros

## Relevant links / docs

[Roll20 macros](https://help.roll20.net/hc/en-us/articles/360037256794-Macros)

[Macros and roll templates](https://help.roll20.net/hc/en-us/articles/360037257334-How-to-Make-Roll-Templates)

[Dice rolls](https://help.roll20.net/hc/en-us/articles/360037773133-Dice-Reference)

[Rolls and character sheets](https://help.roll20.net/hc/en-us/articles/4403865972503-Custom-Roll-Parsing-for-Character-Sheets)

> Tip: pressing up-arrow in the chat window will show the latest macro used, if any.

## Examples: Dungeon World

[Dungeon World Move Template](https://github.com/Roll20/roll20-character-sheets/blob/master/Dungeon%20World%20by%20Roll20/Dungeon%20World.html#L747-L811)

Unresolved macro

```
&{template:move} {{charname=@{selected|character_name}}} {{movename=Scout Ahead}} {{trigger=When you take point and look for anything out of the ordinary, roll + WIS}}  {{success=Choose 2}} {{partial=Choose 1}} {{miss=}} {{doroll=[[2d6]]}} {{result=[[@{selected|rolltype} + @{selected|wisdom_mod}[wisdom] + ?{Bonus|0}[bonus] + (@{selected|rollforward})[forward] + @{selected|global_ongoing}[ongoing global]]]}} {{details=• You get the drop on whatever lies ahead
• You discern a beneficial aspect of the terrain - shortcut, shelter, or tactical advantage (describe it)
• You make a **Discovery** (ask the GM)
• You notice sign of a nearby **Danger** - ask the GM what it is, and what it might signify}}
```

"Resolved" macro:

```
&{template:move} {{charname=@{selected|character_name}}} {{movename=Scout Ahead}} {{trigger=When you take point and look for anything out of the ordinary, roll + WIS}}  {{success=Choose 2}} {{partial=Choose 1}} {{miss=}} {{doroll=[[2d6]]}} {{result=[[@{selected|rolltype} + @{selected|wisdom_mod}[wisdom] + ?{Bonus|0}[bonus] + (@{selected|rollforward})[forward] + @{selected|global_ongoing}[ongoing global]]]}} {{details=• You get the drop on whatever lies ahead
• You discern a beneficial aspect of the terrain - shortcut, shelter, or tactical advantage (describe it)
• You make a **Discovery** (ask the GM)
• You notice sign of a nearby **Danger** - ask the GM what it is, and what it might signify}}
```