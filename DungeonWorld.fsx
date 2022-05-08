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

type Modifier =
    | Ability of Ability
    | Prompt of string

type Roll = {
    /// Ability
    Modifier: Option<Modifier>
    /// What happens if the Move succeeds
    Success: string
    /// What happens if the Move is a partial success
    Partial: string
    /// What happens if the Move fails
    Miss: string
    }

/// Template for a Move
type Move = {
    /// Name of the Move
    Name: string
    /// Description of the Move Trigger
    Trigger: string
    /// Additional information about the Move
    Details: string
    /// Description of the Roll, if any
    Roll: Option<Roll>
    }
    with
    member this.FormatTrigger =
        match this.Roll with
        | None -> this.Trigger
        | Some roll ->
            this.Trigger
            +
            ", roll + "
            +
            match roll.Modifier with
            | None -> "nothing"
            | Some modifier ->
                match modifier with
                | Ability ability -> $"**{ability}**"
                | Prompt prompt -> $"**{prompt}**"
    member this.Result =
        match this.Roll with
        | None -> ""
        | Some roll ->
            "@{selected|rolltype}"
            +
            match roll.Modifier with
            | None -> ""
            | Some modifier ->
                match modifier with
                | Ability ability -> $" + @{{selected|{Sheet.modifier ability}}}[{ability}]"
                | Prompt prompt -> $" + ?{{{prompt}|0}}[{prompt}]"
            +
            " + ?{Bonus (Forward @{selected|rollforward}, Global @{selected|global_ongoing})|0}[Bonus] + (@{selected|rollforward})[Forward] + @{selected|global_ongoing}[Ongoing]"
    member this.Success =
        match this.Roll with
        | None -> ""
        | Some roll -> roll.Success
    member this.Partial =
        match this.Roll with
        | None -> ""
        | Some roll -> roll.Partial
    member this.Miss  =
        match this.Roll with
        | None -> ""
        | Some roll -> roll.Miss

let eval (move: Move) =
    [
        Format.template "move"
        Format.prop "movename" move.Name
        Format.prop "charname" "@{selected|character_name}"
        Format.prop "trigger" (move.FormatTrigger)
        Format.prop "details" move.Details
        if move.Roll |> Option.isSome
        then
            Format.prop "success" move.Success
            Format.prop "partial" move.Partial
            Format.prop "miss" move.Miss
            Format.prop "doroll" ("@{selected|rolltype}" |> Format.roll)
            Format.prop "result" (move.Result |> Format.roll)
    ]
    |> String.concat " "

module Moves =

    let scoutAhead: Move = {
        Name = "Scout Ahead"
        Trigger = "When you **take point and look for anything out of the ordinary**"
        Roll = Some {
            Modifier = Some (Ability WIS)
            Success = "Choose 2"
            Partial = "Choose 1"
            Miss = "Mark XP"
            }
        Details = """• You get the drop on whatever lies ahead
    • You discern a beneficial aspect of the terrain - shortcut, shelter, or tactical advantage (describe it)
    • You make a **Discovery** (ask the GM)
    • You notice sign of a nearby **Danger** - ask the GM what it is, and what it might signify
    """
        }

    let navigate: Move = {
        Name = "Navigate"
        Trigger = "When you **plot the best course through dangerous or unfamiliar lands**"
        Roll = Some {
            Modifier = Some (Ability INT)
            Success = "You avoid dangers and distractions and make good time, reaching a point of the GM's choosing before you need to **Make Camp**"
            Partial = "GM chooses 1 from the list below"
            Miss = "Mark XP"
            }
        Details = """• You happen upon a **Discovery** missed by the Scout
• The going is slow, or you wander off course. The GM says which, and where you end up on the map
• You encounter a **Danger**; whether or not you are surprised depends on whether the Scout has the drop on it
    """
        }

    let staySharp: Move = {
        Name = "Stay Sharp"
        Trigger = "When you **are on watch and something approaches**"
        Roll = Some {
            Modifier = Some (Ability WIS)
            Success = "You notice in time to alert everyone and prepare a response, all party members take +1 forward"
            Partial = "You manage to sound the alarm"
            Miss = "Mark XP, and whatever approaches has the drop on you"
            }
        Details = ""
        }

    let makeCamp: Move = {
        Name = "Make Camp"
        Trigger = "When you **settle in to rest**, choose one member of the party to **Manage Provisions**. Then, if you eat and drink and have enough XP, you may Level Up. If you are bedding down in dangerous territory, decide on a watch order. Then, the GM chooses one person to **take watch** during the night"
        Roll = None
        Details = ""
        }

    let takeWatch: Move = {
        Name = "Take Watch"
        Trigger = "When you **take watch during the night**"
        Roll = Some {
            Modifier = None
            Success = "The night passes without incident"
            Partial = "GM chooses 1 from the list below"
            Miss = "Everyone marks XP, and a Danger manifests. You'd better **Stay Sharp**!"
            }
        Details = """• The person on watch notices a nearby Discovery
• One party member of the GM's choice suffers a restless night
• One or more followers causes trouble
• A Danger approaches. It's not immediately hostile, but whoever is on watch had better Stay Sharp anyways
When you wake from at least a few hours of uninterrupted sleep, and you ate and drank the night before, heal damage equal to half of your max HP."""
        }

    let manageProvisions: Move = {
        Name = "Manage Provisions"
        Trigger = "When you **prepare and distribute food for the party**"
        Roll = Some {
            Modifier = Some (Ability WIS)
            Success = "Choose 1 from the list below"
            Partial = "The party consumes the expected amount of rations"
            Miss = "Mark XP"
            }
        Details = """• Careful management reduces the amount of rations consumed (ask the GM by how much)
• The party consumes the expected amount and the food you prepare is excellent - describe it, and everyone who licks their lips takes +1 forward"""
        }

    let orderFollower: Move = {
        Name = "Order Follower"
        Trigger = "When you **order or expect a follower to do something dangerous, degrading, or contrary to their instinct**"
        Roll = Some {
            Modifier = Some (Prompt "Loyalty")
            Success = "They do it, now"
            Partial = "They do it, but the GM picks one from the list below"
            Miss = "Mark XP"
            }
        Details = """• Decrease the follower's **Loyalty** by 1
• They complain loudly, now or later, and demand something in return
• Caution, laziness or fear makes them take a long time to get it done"""
        }

    let journey: Move = {
        Name = "Journey"
        Trigger = """When you **travel by a safe route**, through safe of dangerous lands,
indicate your destination on the map. the GM will tell you how long the trip takes, and what
-- if anything -- happens along the way. When you reach your destination, choose someone to
**Manage Provisions** to determine how many rations were consumed over the course of the trip."""
        Roll = None
        Details = ""
        }

    let undertakePerilousJourney: Move = {
        Name = "Undertake a Perilous Journey"
        Trigger = """When you **travel through dangerous lands**, and not on a safe route,
indicate the course you want to take on the map and ask the GM how far you should be able to get
before needing to **Make Camp**. If you are exploring with no set destination, indicate which way you go.
Then, choose one party member to **Scout Ahead**, and one to **Navigate**, resolving these moves in that order."""
        Roll = None
        Details = ""
        }

Moves.scoutAhead |> eval

Moves.navigate |> eval

Moves.staySharp |> eval

Moves.makeCamp |> eval

Moves.manageProvisions |> eval

Moves.orderFollower |> eval

Moves.takeWatch |> eval

Moves.journey |> eval

Moves.undertakePerilousJourney |> eval