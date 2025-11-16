// SPDX-FileCopyrightText: 2023-2025 Overtone contributors <https://github.com/ForNeVeR/overtone>
//
// SPDX-License-Identifier: MIT

namespace Overtone.Game.Scenes

open JetBrains.Lifetimes
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

open Overtone.Game
open Overtone.Game.Config

// Planet img : SMISLE
// Planet count/position : https://github.com/Fadoli/ToneRebellion_Raw/blob/master/original_content/WORLDPOS.TXT
// game size is 640x480

type Planets (lifetime: Lifetime, device: GraphicsDevice, textureManager: Textures.Manager) =
    
    // Gotta love sparkles !
    let sparkles:Sparkles = Sparkles(lifetime, device)
    let IslandsRendering = textureManager.LoadWholeShape(lifetime, Shapes.PlanetsRendering)
    let world:WorldDefinition = GameState.islands.worlds[GameState.currentMapSize]
    let colorMask = Color.White
    let mutable currentFrame = 0
    let mutable mouseState:MouseState = new MouseState()
    let mutable hoveredPlanet:int = 0

    interface IScene with

        member _.DrawBackground(batch: SpriteBatch): unit =
            sparkles.Draw(batch)

        member _.Draw(batch: SpriteBatch): unit =
            hoveredPlanet <- 0
            let baseOffset = Vector2(320f,240f)
            let baseAngle = (float32)currentFrame/12f
            world.islands
            |> Seq.sortBy(fun island -> cos(MathHelper.ToRadians(island.baseAngle-baseAngle))*island.distance)
            |> Seq.iter(fun island -> 
                let angle = MathHelper.ToRadians(island.baseAngle - baseAngle)
                let dist = island.distance /8f
                let mutable renderedIndex = island.shapeIndexDisplayed
                if not island.isVisible then
                    renderedIndex <- renderedIndex + GameData.IslandsCount
                let mutable currentPlanet = IslandsRendering[renderedIndex]
                let position = baseOffset + Vector2(sin(angle)*dist, cos(angle)*dist/2.5f)
                if (position-Vector2((float32)mouseState.X, (float32)mouseState.Y)).Length() < 32f && island.isVisible then
                    currentPlanet <- IslandsRendering[renderedIndex + 2*GameData.IslandsCount]
                    hoveredPlanet <- island.shapeIndexDisplayed
                batch.Draw(
                    currentPlanet.texture,
                    currentPlanet.offset+position,
                    colorMask
                )
            )
            currentFrame <- currentFrame + 1
            ()

        member _.Update(time: GameTime, mouse: MouseState): (int*int*int) =
            mouseState <- mouse
            sparkles.Update(time)

            // Check for planet click
            if mouse.LeftButton = ButtonState.Pressed then
                // Find hovered planet
                let hovered = hoveredPlanet
                if hovered <> 0 then
                    // Register planet id and switch to state 3 (GameView)
                    Overtone.Game.GameState.currentPlanetId <- hovered
                    (8003, hovered, 0) // 8003 is our custom event for planet selection
                else
                    (0,0,0)
            else
                (0,0,0)
