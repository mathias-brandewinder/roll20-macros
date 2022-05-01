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

"Resolved" macro:

```
&{template:move} {{charname=@{TEST|character_name}}} {{movename=Volley}} {{trigger=When you take aim and shoot at an enemy at range}} {{success=You have a clear shot—deal your damage.}} {{partial=Deal your damage, and choose one:
• You have to move to get the shot placing you in danger of the GM's choice
• You have to take what you can get: -1d6 damage.
• You have to take several shots, reducing your ammo by one.}} {{miss=Miss!}} {{doroll=[[1]]}} {{result=[[@{TEST|rolltype} + @{TEST|dexterity_mod}[dexterity] +0 + (@{TEST|rollforward})[forward] + @{TEST|global_ongoing}[ongoing global]]]}} {{details=}}
```