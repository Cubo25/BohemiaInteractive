# Game Test Runner - Custom Testing Solution

## Overview
This is a custom testing solution for the QAA Platformer project that tests core game functionalities **without using Unity Testing Framework**. The tests run directly in the game scene and provide clear, executable results.

## Test Coverage
The test suite validates the following functionalities:
1. **Game can be launched** - Verifies GameManager initialization and game state
2. **Character can move** - Tests character movement system and physics
3. **Character gets killed when in collision with spike** - Validates damage system and death mechanics
4. **Level can be finished** - Tests portal trigger and level completion flow

## Setup Instructions

### Method 1: Automatic Testing (Recommended)
1. Open Unity Editor
2. Open the scene: `Assets/Scenes/MainScene.unity`
3. Create an empty GameObject in the scene (Right-click in Hierarchy → Create Empty)
4. Name it "TestRunner"
5. Add the `GameTestRunner` component to this GameObject:
   - Select the GameObject
   - In Inspector, click "Add Component"
   - Search for "GameTestRunner" and add it
6. The tests will run automatically when you press Play (▶️)

### Method 2: Manual Testing via Keyboard
1. Follow steps 1-5 from Method 1
2. Add the `TestExecutor` component to any GameObject (or create a new one)
3. Press **'T'** key during Play mode to manually trigger tests

### Method 3: UI Button Testing
1. Follow steps 1-5 from Method 1
2. Create a UI Button in your scene
3. Add the `TestExecutor` component to any GameObject
4. In the Button's OnClick event, drag the GameObject with TestExecutor and select `TestExecutor.RunTests()`

## Running Tests

### Automatic Execution
- Tests run automatically when the scene starts (if `Run Tests On Start` is enabled)
- Results are printed to the Unity Console

### Manual Execution
- Press **'T'** key during Play mode
- Or call `RunTests()` method programmatically

## Test Results

### Console Output
All test results are printed to the Unity Console with:
- ✓ PASS / ✗ FAIL status for each test
- Detailed messages explaining test outcomes
- Execution time for each test
- Final summary with pass/fail count and success rate

### Example Output:
```
========================================
GAME TEST RUNNER - Starting Tests
========================================
[TEST] Test 1: Game Can Be Launched - Starting...
[TEST] Test 1: Game Can Be Launched - PASSED: GameManager initialized and game is running (0.001s)
[TEST] Test 2: Character Can Move - Starting...
[TEST] Test 2: Character Can Move - PASSED: Character movement system is functional (0.015s)
[TEST] Test 3: Character Gets Killed On Spike Collision - Starting...
[TEST] Test 3: Character Gets Killed On Spike Collision - PASSED: Character took damage on spike collision (1.250s)
[TEST] Test 4: Level Can Be Finished - Starting...
[TEST] Test 4: Level Can Be Finished - PASSED: Level completion triggered successfully (0.020s)
========================================
TEST RESULTS SUMMARY
========================================
✓ PASS | Test 1: Game Can Be Launched
✓ PASS | Test 2: Character Can Move
✓ PASS | Test 3: Character Gets Killed On Spike Collision
✓ PASS | Test 4: Level Can Be Finished
========================================
TOTAL: 4/4 tests passed
SUCCESS RATE: 100.0%
========================================
```

## Test Details

### Test 1: Game Can Be Launched
- **Purpose**: Verifies that the game initializes correctly
- **Checks**: 
  - GameManager.Instance exists
  - GameManager.IsRunning is true
- **Expected Result**: GameManager is initialized and game is running

### Test 2: Character Can Move
- **Purpose**: Validates character movement functionality
- **Checks**:
  - Player GameObject exists
  - CharacterController2D component exists
  - Rigidbody2D is not static
  - Character can change position
- **Expected Result**: Character movement system is functional

### Test 3: Character Gets Killed On Spike Collision
- **Purpose**: Tests damage system and death mechanics
- **Checks**:
  - Player and Spike objects exist
  - Colliders are properly configured
  - Character takes damage when colliding with spike
  - Character health changes on collision
- **Expected Result**: Character receives damage and can die from spike collision
- **Note**: This test waits for the spike's damage interval (1 second)

### Test 4: Level Can Be Finished
- **Purpose**: Validates level completion flow
- **Checks**:
  - Player and Portal objects exist
  - Portal trigger is configured correctly
  - Level completion is triggered when player enters portal
  - Game state changes appropriately
- **Expected Result**: Level completion triggers successfully

## Configuration

### GameTestRunner Settings
- **Run Tests On Start**: Automatically run tests when scene starts (default: true)
- **Print Detailed Results**: Show detailed messages for each test (default: true)

### TestExecutor Settings
- **Test Key**: Keyboard key to trigger tests (default: 'T')
- **Test Runner Reference**: Reference to GameTestRunner component (auto-found if not set)

## Requirements
- Unity 2022.3.1f1 or compatible version
- Scene must contain:
  - Player GameObject with tag "Player"
  - At least one Portal GameObject
  - At least one Spike GameObject
  - GameManager GameObject with GameManager component

## Notes
- Tests run in the actual game scene without spawning additional objects
- Tests may modify player position temporarily but reset it after completion
- Some tests require physics updates, so they use coroutines with WaitForFixedUpdate
- The spike damage test waits 1.1 seconds to account for the damage interval

## Troubleshooting

### Tests don't run automatically
- Check that `Run Tests On Start` is enabled in GameTestRunner
- Ensure GameTestRunner component is attached to a GameObject in the scene

### "Player object not found" error
- Verify Player GameObject has the "Player" tag
- Check that Player GameObject is active in the scene

### "Portal/Spike not found" error
- Ensure Portal and Spike GameObjects exist in the scene
- Verify they have the Portal/Spike components attached

### Tests fail unexpectedly
- Check Unity Console for detailed error messages
- Verify all required components are attached to GameObjects
- Ensure colliders are properly configured (triggers enabled where needed)

## File Structure
```
Assets/Scripts/Testing/
├── GameTestRunner.cs    # Main test runner with all test logic
└── TestExecutor.cs      # Helper script for manual test triggering
```

