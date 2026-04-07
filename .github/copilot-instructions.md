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

## NPC Personality & Quest System

**Overview**: NPCs are controlled entirely through data via `PersonalityData` scriptable objects. No code changes needed to modify NPC behavior, items they want, or quest chains.

**Core Data Structures**:
- `PersonalityData`: ScriptableObject with array of `QuestState` entries
- `QuestState`: Defines what items an NPC tracks and what GameState objectives matter
- Automatic quest switching via `activationKey` (GameState int/bool triggers)

**How It Works**:
1. Assign `PersonalityData` to NPC prefab in Inspector
2. Define `QuestState` entries with:
   - `questName`: identifier
   - `activationKey`: GameState bool that auto-triggers this quest (empty = default)
   - `desiredItems[]`: ItemData the NPC tracks (doesn't pick up automatically)
   - `objectiveKeys[]`: GameState bools the quest cares about (e.g., "temple_rebuilt")
   - `questPriority`: 0-1 urgency
3. AI actions run automatically:
   - `TrackItemAction`: Scans grid for desired items
   - `MonitorObjectiveAction`: Checks if objectives are completed
4. `NPCScript` auto-switches quests every 0.5s when activation conditions are met

**Example Usage**:
```csharp
// In any code that drives story progression:
GameState.SetBool("texts_collected", true);
// NPC with quest activation key "texts_collected" auto-switches to that quest
```

**Key Files**:
- `Scripts/ActorS/AI/Quest/PersonalityData.cs`
- `Scripts/ActorS/AI/Quest/QuestState.cs`
- `Scripts/ActorS/AI/Utility/Actions/TrackItemAction.cs`
- `Scripts/ActorS/AI/Utility/Actions/MonitorObjectiveAction.cs`
- `Scripts/ActorS/AI/NPCScript.cs`

**Key Methods**:
- `GameState.SetBool(key, value)` / `GetBool(key)` - Drive state transitions
- `npc.SetActiveQuest(questName)` - Manual override (rarely needed)
- `npc.GetPersonality()` - Access quest data at runtime

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
- Modify NPC quest behavior via code instead of PersonalityData
- Manually call quest activation—use `GameState.SetBool()` instead

---
*Updated Apr 2026 for hex grid + actor system + NPC personality system. Focus: new enemies, tile effects, pathfinding queries, NPC quest chains via GameState.*

