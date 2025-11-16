// SPDX-FileCopyrightText: 2023-2025 Overtone contributors <https://github.com/ForNeVeR/overtone>
//
// SPDX-License-Identifier: MIT

namespace Overtone.Game

open Overtone.Utils.Constants
open Overtone.Game.Config
open Overtone.Resources

//
// This holds the gamestate
//

module GameState =
    let mutable currentRace: int = -1
    let mutable currentDifficulty: int = 0
    let mutable currentMapSize: int = 0
    let mutable discRoot: string = ""
    let mutable disc: Option<GameDisc> = None
    
    let islands: IslandsConfiguration= new IslandsConfiguration()

    // Mutable variables for description configurations
    let mutable helpDescriptions: DescriptionConfiguration = DescriptionConfiguration(Map.empty)
    let mutable buildingNamesDescriptions: DescriptionConfiguration = DescriptionConfiguration(Map.empty)
    let mutable buildingDescriptions: DescriptionConfiguration = DescriptionConfiguration(Map.empty)
    let mutable glyphsDescriptions: DescriptionConfiguration = DescriptionConfiguration(Map.empty)
    let mutable newGameEventsDescriptions: DescriptionConfiguration = DescriptionConfiguration(Map.empty)
    let mutable newGameTribeDescriptions: DescriptionConfiguration = DescriptionConfiguration(Map.empty)
    let mutable inGameTextDescriptions: DescriptionConfiguration = DescriptionConfiguration(Map.empty)

    let mutable soundsConfig: SoundsConfiguration= new SoundsConfiguration(Map.empty)
    let mutable shapesConfig: ShapesConfiguration= new ShapesConfiguration(Map.empty)


    let init(rootPath): unit =
        discRoot <- rootPath
        let currentDisc = new GameDisc(rootPath)
        disc <- Some(currentDisc)
        islands.Read <| currentDisc.GetData "data\worldpos.txt"
        soundsConfig <- SoundsConfiguration.Read <| currentDisc.GetConfig "sound.txt"
        shapesConfig <- ShapesConfiguration.Read <| currentDisc.GetConfig "shapes.txt"
        // Load all description files after disc is initialized
        helpDescriptions <- DescriptionConfiguration.Read(currentDisc.GetConfig "tonehelp.txt")
        buildingNamesDescriptions <- DescriptionConfiguration.Read(currentDisc.GetConfig "bldname.txt")
        buildingDescriptions <- DescriptionConfiguration.Read(currentDisc.GetConfig "bldtxt.txt")
        glyphsDescriptions <- DescriptionConfiguration.Read(currentDisc.GetConfig "plotobj.txt")
        newGameEventsDescriptions <- DescriptionConfiguration.Read(currentDisc.GetConfig "newtext.txt")
        newGameTribeDescriptions <- DescriptionConfiguration.Read(currentDisc.GetConfig "newgame.txt")
        inGameTextDescriptions <- DescriptionConfiguration.Read(currentDisc.GetConfig "gamey.txt")

    let getDisc(): GameDisc=
        match disc with
            | Some(output) -> output
            | None ->
                let output = new GameDisc(discRoot)
                output


    let handleButtonEvent() =
        //
        printfn("stuff !")

    let ChangeDifficulty() =
        currentDifficulty <- currentDifficulty + 1
        if (currentDifficulty >= GameData.DifficultyCount) then
            currentDifficulty <- 0
            
    let ChangeWorldSize() =
        currentMapSize <- currentMapSize + 1
        if (currentMapSize >= GameData.WorldSizeCount) then
            currentMapSize <- 0
            
    let SelectRace(newRace: int) =
        if newRace = currentRace then
            currentRace <- -1
        else
            currentRace <- newRace

    let StartGame(): bool =
        
        islands.worlds[currentMapSize].islands
        |> Seq.iter (fun (island) -> island.isVisible <- false)
        // Change only if a race is selected !
        if (currentRace <> -1) then
            islands.worlds[currentMapSize].islands[currentRace].isVisible <- true
            true
        else
            false
