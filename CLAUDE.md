# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Legends is a game development project built on the MonoGame framework using C#/.NET 8.0. The project uses a multi-layered architecture with a custom game engine, content pipeline, editor tools, and the main application.

## Project Structure

The solution consists of five main projects:

- **Legends.App** - Main game application (WinExe targeting net8.0)
- **Legends.Engine** - Core game engine library with graphics, animation, collision, and input systems
- **Legends.Content.Pipeline** - Content processing pipeline for game assets
- **Legends.Content.Auto** - Automated content generation tools
- **Legends.Editor** - Game editor and development tools

## Build Commands

The project uses standard .NET build commands:

```bash
# Build the entire solution
dotnet build Legends.sln

# Build and publish
dotnet publish Legends.sln

# Watch for changes and rebuild
dotnet watch run --project Legends.sln

# Build MonoGame content pipeline
dotnet mgcb ./content.mgcb /clean /launchdebugger
```

## Development Setup

### VS Code Configuration
The project includes VS Code configuration with:
- Build tasks for solution build/publish/watch
- Launch configurations for the main app and editor
- Content pipeline build task for MonoGame assets

### Running the Application
- **Main Game**: Run from `src/cs/Legends.App/` - launches the game application
- **Editor**: Run from `src/cs/Legends.Editor/` - launches development tools

## Architecture

### Core Services
The game uses a service-based architecture with these key services:
- `IRenderService` / `DefaultRenderService` - Handles 2D sprite rendering and graphics
- `GameManagementService` - Manages game state and screen management
- `CollisionService` - Handles collision detection and physics
- `InputHandlerService` - Processes keyboard and mouse input
- `ScreenManager` - Manages game screens/states (MonoGame.Extended)

### Service Registration
Services are registered through MonoGame's built-in service container and accessed via `Services` property.

### Graphics System
The engine includes a custom 2D graphics system with:
- Layer-based rendering with depth sorting
- Camera and viewport management
- Animation system with keyframe support
- Spatial organization for efficient rendering
- Material and texture management

### Content Pipeline
Uses MonoGame's content pipeline with custom processors for:
- Scene data serialization
- Animation data processing
- Spatial and camera data handling

### Screen System
Uses MonoGame.Extended's screen management:
- `TitleScreen` - Main menu/title screen
- `MapScreen` - In-game world/map display
- Screen transitions and state management

## Key Dependencies

- **MonoGame.Framework.DesktopGL** (3.8.2.1105) - Core game framework
- **MonoGame.Extended** (4.0.3) - Extended functionality for MonoGame
- **Microsoft.CodeAnalysis.CSharp** (4.0.1) - Dynamic code compilation
- **Newtonsoft.Json** (13.0.3) - JSON serialization
- **Autofac** - Dependency injection container
- **SharpDX** - DirectX wrapper
- **AssimpNet** - 3D model loading

## Development Notes

### Content Management
- Content root is set to "Content" directory
- Asset watching is enabled for automatic reloading during development
- Content logger can be enabled for debugging content loading issues

### Performance Settings
- Configurable VSync and fixed timestep settings
- Default resolution: 1280x1024
- Mouse visibility enabled by default

### Animation System
The engine includes a comprehensive animation system supporting:
- Keyframe-based animations
- Sprite animations with frame sets
- Translation, color, and tile animations
- Animation controllers and channels
- Loop types and timing control

### Input System
Event-driven input handling with:
- Keyboard event listeners
- Extensible input manager
- Input event types and processing

This is a MonoGame-based game engine project focused on 2D graphics and game development tools.