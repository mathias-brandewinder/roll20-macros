(*
Goal: write out macros that follow the Dungeon World Move template
*)

module Sheet =

    type Ability =
        | DEX
        | STR
        | INT
        | WIS
        | CHA
        | CON

    let score (ability: Ability) =
        match ability with
        | DEX -> "dexterity"
        | STR -> "strength"
        | INT -> "intelligence"
        | WIS -> "wisdom"
        | CHA -> "charisma"
        | CON -> "constitution"

    let modifier (ability: Ability) =
        $"{score ability}_mod"

    type Attribute = {
        Name: string
        Value: string
        }

[<RequireQualifiedAccess>]
module Format =

    let template (name: string) =
        $"&{{template:{name}}}"

    let prop (name: string) (value: string) =
        $"{{{{{name}={value}}}}}"

    let roll (desc: string) =
        $"[[{desc}]]"

open Sheet

/// Template for a Move
type Move = {
    /// Name of the Move
    Name: string
    /// Description of the Move Trigger
    Trigger: string
    /// What happens if the Move succeeds
    Success: string
    /// What happens if the Move is a partial success
    Partial: string
    /// What happens if the Move fails
    Miss: string
    /// Additional information about the Move
    Details: string
    /// Ability
    Ability: Option<Ability>
    }

let eval (move: Move) =
    let result =
        "@{selected|rolltype}"
        +
        (move.Ability
        |> Option.map (fun ability ->
            $" + @{{selected|{modifier ability}}}[{ability}]")
        |> Option.defaultValue "")
        +
        " + ?{Bonus|0}[bonus] + (@{selected|rollforward})[Forward] + @{selected|global_ongoing}[Ongoing]"
    [
        Format.template "move"
        Format.prop "movename" move.Name
        Format.prop "charname" "@{selected|character_name}"
        Format.prop "trigger" (move.Trigger + (move.Ability |> Option.map (fun ability -> $" + **{ability}**") |> Option.defaultValue ""))
        Format.prop "success" move.Success
        Format.prop "partial" move.Partial
        Format.prop "miss" move.Miss
        Format.prop "details" move.Details
        Format.prop "doroll" ("@{selected|rolltype}" |> Format.roll)
        Format.prop "result" (Format.roll result)
    ]
    |> String.concat " "

let scoutAhead: Move = {
    Name = "Test Scout Ahead"
    Trigger = "When you take point and look for anything out of the ordinary, roll"
    Ability = Some WIS
    Success = "Choose 2"
    Partial = "Choose 1"
    Miss = "Mark XP"
    Details = """• You get the drop on whatever lies ahead
• You discern a beneficial aspect of the terrain - shortcut, shelter, or tactical advantage (describe it)
• You make a **Discovery** (ask the GM)
• You notice sign of a nearby **Danger** - ask the GM what it is, and what it might signify
"""
    }

scoutAhead
|> eval
