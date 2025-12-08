// SPDX-FileCopyrightText: 2023-2025 Overtone contributors <https://github.com/ForNeVeR/overtone>
//
// SPDX-License-Identifier: MIT

namespace Overtone.Game.Config

open Overtone.Resources

// The file looks like that for :
//
// 1. Help (tonehelp.txt)
// 1. Building Names (bldname.txt)
// 1. Building Descriptions (bldtxt.txt)
// 1. Glyphs + Artifacts Descriptions (plotobj.txt)
// 1. New Game + Events texts : newtext.txt
// 1. New Game Tribe Description : newgame.txt
// 1. In Game text : gamey.txt
//
// BACKGRND
// DESCRIPTION
// END
//
// IWHallMove
// DESCRIPTION
// END
//
// ENDTEXT

type DescriptionConfiguration(descriptions: Map<string, string list>) =
    member _.GetDescription(objectName: string): string list option =
        descriptions |> Map.tryFind objectName

    static member Read(descriptionTxt: byte[]): DescriptionConfiguration =
        let lines = descriptionTxt |> TextConfiguration.extractLines |> Seq.toList
        let rec parse acc currentName currentDesc = function
            | [] ->
                match currentName, currentDesc with
                | Some name, desc when desc <> [] -> Map.add name (List.rev desc) acc
                | _ -> acc
            | line :: rest when line = "END" ->
                match currentName, currentDesc with
                | Some name, desc -> parse (Map.add name (List.rev desc) acc) None [] rest
                | _ -> parse acc None [] rest
            | line :: rest when currentName = None ->
                parse acc (Some line) [] rest
            | line :: rest ->
                parse acc currentName (line :: currentDesc) rest
        let descMap = parse Map.empty None [] lines
        // Debug print loaded descriptions
        printfn "Loaded descriptions:"
        descMap |> Map.iter (fun obj descs ->
            printfn "Object: %s" obj
            descs |> List.iter (printfn "  %s"))
        DescriptionConfiguration(descMap)
