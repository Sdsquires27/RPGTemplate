# Copilot Instructions - RPG Template

## Architecture

**Service Layer**: `GameServices.cs` is a static registry. Access everything via `GameServices.GetTileManager()`, `GameServices.GetInputHandler()`, etc.

**Hex Grid**: Pointy-top orientation using cubic coordinates (x, y, z where x+y+z=0). 
- **Conversion**: `HexToWorldPosition()` / `WorldToHexPosition()` handle transforms
- **Key classes**: `HexGrid` (manager), `HexTile` (individual tile), `HexCoordinate` (coordinate struct), `HexPathfinding` (A* on hex)

**Actors**: All extend `ActorScript` (abstract). Dual-track continuous position (Rigidbody2D) + hex tile position (auto-synced in Update).
- **Required**: Implement `HandleMovement()` and `HandleActions()`
- **Key methods**: `SetHexTile()`, `GetCurrentHexTile()`, `GetHexPosition()`

## Patterns & Examples

**Create new actor**:
```csharp
public class EnemyScript : ActorScript {
    protected override void HandleMovement() { /* move via rb.linearVelocity */ }
    protected override void HandleActions() { /* implement AI logic */ }
}
```

**Query grid**:
```csharp
TileManager tm = GameServices.GetTileManager();
HexTile tile = tm.GetTile(coord);
HexTile[] neighbors = tm.GetWalkableNeighbors(coord);
```

**Place actor on tile**: `actor.SetHexTile(tile);` (auto-registers occupancy)

## Key Files

- `Scripts/Services/GameServices.cs` - Singleton registry
- `Scripts/TileScripts/HexGrid.cs` - Core grid logic (TileManager extends this)
- `Scripts/Actors/ActorScript.cs` - Actor base class
- `Scripts/Actors/PlayerScript.cs` - Reference implementation
- `Assets/Misc/DefaultInput.inputactions` - Input action triggers

## Do Not

- Edit `GameBootstrapper.cs` or `*.meta` files
- Create actors that don't inherit from `ActorScript`
- Access actors/tiles without going through GameServices or HexGrid

---
*Updated Feb 2026 for hex grid + actor system. Focus: new enemies, tile effects, pathfinding queries.*
