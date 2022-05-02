(*
Goal: write out macros that follow the Dungeon World Move template
*)

[<RequireQualifiedAccess>]
module Format =

    let template (name: string) =
        $"&{{template:{name}}}"

    let prop (name: string) (value: string) =
        $"{{{{{name}={value}}}}}"

    let roll (desc: string) =
        $"[[{desc}]]"


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
    /// Base Roll
    Roll: Option<string>
    }

let eval (move: Move) =
    [
        Format.template "move"
        Format.prop "movename" move.Name
        Format.prop "charname" "@{selected|character_name}"
        Format.prop "trigger" move.Trigger
        Format.prop "success" move.Success
        Format.prop "partial" move.Partial
        Format.prop "miss" move.Miss
        Format.prop "details" move.Details
        Format.prop "doroll" (move.Roll |> Option.defaultValue "@{selected|rolltype}" |> Format.roll)
        Format.prop
            "result"
            (move.Roll
            |> Option.map (fun exp -> exp + " + @{selected|strength_mod}[Strength!]")
            |> Option.defaultValue "@{selected|rolltype}"
            |> Format.roll)
    ]
    |> String.concat " "

let scoutAhead: Move = {
    Name = "Test Scout Ahead"
    Trigger = "When you take point and look for anything out of the ordinary, roll + WIS"
    Roll = Some "2d6"
    Success = "Choose 2"
    Partial = "Choose 1"
    Miss = "Mark XP"
    Details = """• You get the drop on whatever lies ahead
• You discern a beneficial aspect of the terrain - shortcut, shelter, or tactical advantage (describe it)
• You make a **Discovery** (ask the GM)
• You notice sign of a nearby **Danger** - ask the GM what it is, and what it might signify
"""
    }

let herculeanAppetites: Move = {
    Name = "Herculean Appetites"
    Trigger = "While pursuing one of your appetites if you would roll for a move, instead of rolling 2d6 you roll 1d6+1d8"
    Roll = Some "1d6 + 1d8"
    Success = "Success"
    Partial = "Partial"
    Miss = "Mark XP"
    Details = "If the d6 is the higher die of the pair, the GM will also introduce a complication or danger that comes about due to your heedless pursuits."
    }

scoutAhead
|> eval

herculeanAppetites
|> eval