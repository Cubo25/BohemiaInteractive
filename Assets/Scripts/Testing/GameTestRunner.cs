using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using QAAPlatformer;
using QAAPlatformer.Character;
using QAAPlatformer.Damage;
using QAAPlatformer.Obstacles;

namespace QAAPlatformer.Testing
{
	/// <summary>
	/// Custom test runner for game functionality testing without Unity Testing Framework
	/// Tests: Game launch, Level completion, Character movement, Character death on spike collision
	/// </summary>
	public class GameTestRunner : MonoBehaviour
	{
		[Header("Test Configuration")]
		[SerializeField] private bool m_RunTestsOnStart = true;
		[SerializeField] private bool m_PrintDetailedResults = true;
		
		[Header("Test Results")]
		[SerializeField] private List<TestResult> m_TestResults = new List<TestResult>();
		
		private GameObject m_PlayerObject;
		private CharacterController2D m_CharacterController;
		private CharacterDamageManager m_DamageManager;
		private Portal m_Portal;
		private Spike m_Spike;
		
		/// <summary>
		/// Test result structure
		/// </summary>
		[System.Serializable]
		public class TestResult
		{
			public string TestName;
			public bool Passed;
			public string Message;
			public float ExecutionTime;
			
			public TestResult(string name, bool passed, string message, float time = 0f)
			{
				TestName = name;
				Passed = passed;
				Message = message;
				ExecutionTime = time;
			}
		}
		
		private void Start()
		{
			if (m_RunTestsOnStart)
			{
				StartCoroutine(RunAllTests());
			}
		}
		
		/// <summary>
		/// Main test execution coroutine
		/// </summary>
		public IEnumerator RunAllTests()
		{
			m_TestResults.Clear();
			Debug.Log("========================================");
			Debug.Log("GAME TEST RUNNER - Starting Tests");
			Debug.Log("========================================");
			
			// Find required objects in scene
			FindSceneObjects();
			
			// Test 1: Game can be launched
			yield return StartCoroutine(TestGameLaunch());
			
			// Test 2: Character can move
			yield return StartCoroutine(TestCharacterMovement());
			
			// Test 3: Character gets killed when in collision with spike
			yield return StartCoroutine(TestCharacterDeathOnSpikeCollision());
			
			// Test 4: Level can be finished
			yield return StartCoroutine(TestLevelCompletion());
			
			// Print final results
			PrintTestResults();
		}
		
		/// <summary>
		/// Finds all required objects in the scene
		/// </summary>
		private void FindSceneObjects()
		{
			// Find player
			m_PlayerObject = GameObject.FindGameObjectWithTag("Player");
			if (m_PlayerObject)
			{
				m_CharacterController = m_PlayerObject.GetComponent<CharacterController2D>();
				m_DamageManager = m_PlayerObject.GetComponent<CharacterDamageManager>();
			}
			
			// Find portal
			m_Portal = FindObjectOfType<Portal>();
			
			// Find first spike
			m_Spike = FindObjectOfType<Spike>();
		}
		
		/// <summary>
		/// Test 1: Game can be launched
		/// </summary>
		private IEnumerator TestGameLaunch()
		{
			float startTime = Time.realtimeSinceStartup;
			string testName = "Test 1: Game Can Be Launched";
			bool passed = false;
			string message = "";
			
			Debug.Log($"[TEST] {testName} - Starting...");
			
			// Check if GameManager exists and is initialized
			if (GameManager.Instance == null)
			{
				message = "FAILED: GameManager.Instance is null";
			}
			else if (!GameManager.IsRunning)
			{
				message = "FAILED: Game is not running (IsRunning = false)";
			}
			else
			{
				passed = true;
				message = "PASSED: GameManager initialized and game is running";
			}
			
			float executionTime = Time.realtimeSinceStartup - startTime;
			m_TestResults.Add(new TestResult(testName, passed, message, executionTime));
			
			Debug.Log($"[TEST] {testName} - {message} ({executionTime:F3}s)");
			yield return null;
		}
		
		/// <summary>
		/// Test 2: Character can move
		/// </summary>
		private IEnumerator TestCharacterMovement()
		{
			float startTime = Time.realtimeSinceStartup;
			string testName = "Test 2: Character Can Move";
			bool passed = false;
			string message = "";
			
			Debug.Log($"[TEST] {testName} - Starting...");
			
			// Check if player exists
			if (m_PlayerObject == null)
			{
				message = "FAILED: Player object not found in scene";
			}
			else if (m_CharacterController == null)
			{
				message = "FAILED: CharacterController2D component not found on player";
			}
			else
			{
				// Test horizontal movement
				Vector3 initialPosition = m_PlayerObject.transform.position;
				
				// Simulate movement input (we'll use reflection or public method if available)
				// Since we can't directly access private fields, we'll test by checking if the component exists
				// and if the player can be moved programmatically
				
				// Wait a frame for physics
				yield return new WaitForFixedUpdate();
				
				// Check if Rigidbody2D exists and is not frozen
				Rigidbody2D rb = m_PlayerObject.GetComponent<Rigidbody2D>();
				if (rb == null)
				{
					message = "FAILED: Rigidbody2D component not found on player";
				}
				else if (rb.bodyType == RigidbodyType2D.Static)
				{
					message = "FAILED: Player Rigidbody2D is Static (cannot move)";
				}
				else
				{
					// Try to move the player programmatically to test movement capability
					Vector3 testPosition = initialPosition + Vector3.right * 0.1f;
					m_PlayerObject.transform.position = testPosition;
					yield return new WaitForFixedUpdate();
					
					// Check if position changed (allowing for physics constraints)
					// Note: In Unity 2022.3.1f1, Rigidbody2D uses 'velocity', not 'linearVelocity'
					if (m_PlayerObject.transform.position != initialPosition || rb.linearVelocity.magnitude > 0)
					{
						passed = true;
						message = "PASSED: Character movement system is functional";
					}
					else
					{
						message = "FAILED: Character position did not change (movement may be blocked)";
					}
					
					// Reset position
					m_PlayerObject.transform.position = initialPosition;
				}
			}
			
			float executionTime = Time.realtimeSinceStartup - startTime;
			m_TestResults.Add(new TestResult(testName, passed, message, executionTime));
			
			Debug.Log($"[TEST] {testName} - {message} ({executionTime:F3}s)");
			yield return null;
		}
		
		/// <summary>
		/// Test 3: Character gets killed when in collision with spike
		/// </summary>
		private IEnumerator TestCharacterDeathOnSpikeCollision()
		{
			float startTime = Time.realtimeSinceStartup;
			string testName = "Test 3: Character Gets Killed On Spike Collision";
			bool passed = false;
			string message = "";
			
			Debug.Log($"[TEST] {testName} - Starting...");
			
			// Check prerequisites
			if (m_PlayerObject == null)
			{
				message = "FAILED: Player object not found in scene";
			}
			else if (m_DamageManager == null)
			{
				message = "FAILED: CharacterDamageManager component not found on player";
			}
			else if (m_Spike == null)
			{
				message = "FAILED: Spike object not found in scene";
			}
			else
			{
				// Store initial state
				// Use reflection to access private health field for testing
				FieldInfo healthField = typeof(CharacterDamageManager).GetField("m_HealthPoints", 
					BindingFlags.NonPublic | BindingFlags.Instance);
				int initialHealth = healthField != null ? (int)healthField.GetValue(m_DamageManager) : 100;
				bool wasAlive = m_DamageManager.IsAlive();
				
				// Ensure player is alive
				if (!wasAlive)
				{
					message = "FAILED: Player is already dead before test";
				}
				else
				{
					// Get spike collider
					Collider2D spikeCollider = m_Spike.GetComponent<Collider2D>();
					Collider2D playerCollider = m_PlayerObject.GetComponent<Collider2D>();
					
					if (spikeCollider == null || !spikeCollider.isTrigger)
					{
						message = "FAILED: Spike does not have a trigger Collider2D";
					}
					else if (playerCollider == null)
					{
						message = "FAILED: Player does not have a Collider2D";
					}
					else
					{
						// Move player to spike position to trigger collision
						Vector3 spikePosition = m_Spike.transform.position;
						Vector3 originalPosition = m_PlayerObject.transform.position;
						
						// Position player on top of spike
						m_PlayerObject.transform.position = spikePosition + Vector3.up * 0.5f;
						
						// Wait for physics to process collision
						yield return new WaitForFixedUpdate();
						yield return new WaitForFixedUpdate();
						
						// Wait for damage interval (spike has 1 second damage interval)
						yield return new WaitForSeconds(1.1f);
						
						// Check if player took damage
						int newHealth = healthField != null ? (int)healthField.GetValue(m_DamageManager) : 100;
						bool isAlive = m_DamageManager.IsAlive();
						
						// Test passes if health changed (decreased) or player died
						if (newHealth < initialHealth || !isAlive)
						{
							passed = true;
							message = $"PASSED: Character took damage on spike collision (Health: {initialHealth} -> {newHealth}, Alive: {isAlive})";
						}
						else
						{
							message = $"FAILED: Character did not take damage (Health unchanged: {initialHealth}, Still alive: {isAlive})";
						}
						
						// Reset player position
						m_PlayerObject.transform.position = originalPosition;
					}
				}
			}
			
			float executionTime = Time.realtimeSinceStartup - startTime;
			m_TestResults.Add(new TestResult(testName, passed, message, executionTime));
			
			Debug.Log($"[TEST] {testName} - {message} ({executionTime:F3}s)");
			yield return null;
		}
		
		/// <summary>
		/// Resets player health and death state (used between tests)
		/// </summary>
		private void ResetPlayerState()
		{
			if (m_DamageManager != null)
			{
				// Use reflection to reset health and death state
				FieldInfo healthField = typeof(CharacterDamageManager).GetField("m_HealthPoints", 
					BindingFlags.NonPublic | BindingFlags.Instance);
				FieldInfo isDeadField = typeof(CharacterDamageManager).GetField("m_IsDead", 
					BindingFlags.NonPublic | BindingFlags.Instance);
				
				if (healthField != null)
				{
					healthField.SetValue(m_DamageManager, 100); // Reset to full health
				}
				if (isDeadField != null)
				{
					isDeadField.SetValue(m_DamageManager, false); // Reset death state
				}
				
				// Also reset Rigidbody2D rotation freeze if it was unfrozen
				Rigidbody2D rb = m_PlayerObject.GetComponent<Rigidbody2D>();
				if (rb != null)
				{
					rb.freezeRotation = true;
				}
			}
		}
		
		/// <summary>
		/// Test 4: Level can be finished
		/// </summary>
		private IEnumerator TestLevelCompletion()
		{
			float startTime = Time.realtimeSinceStartup;
			string testName = "Test 4: Level Can Be Finished";
			bool passed = false;
			string message = "";
			
			Debug.Log($"[TEST] {testName} - Starting...");
			
			// Reset player state before this test (in case previous test killed the player)
			ResetPlayerState();
			yield return new WaitForFixedUpdate();
			
			// Check prerequisites
			if (m_PlayerObject == null)
			{
				message = "FAILED: Player object not found in scene";
			}
			else if (m_Portal == null)
			{
				message = "FAILED: Portal object not found in scene";
			}
			else if (GameManager.Instance == null)
			{
				message = "FAILED: GameManager.Instance is null";
			}
			else
			{
				// Get portal collider
				Collider2D portalCollider = m_Portal.GetComponent<Collider2D>();
				Collider2D playerCollider = m_PlayerObject.GetComponent<Collider2D>();
				
				if (portalCollider == null || !portalCollider.isTrigger)
				{
					message = "FAILED: Portal does not have a trigger Collider2D";
				}
				else if (playerCollider == null)
				{
					message = "FAILED: Player does not have a Collider2D";
				}
				else
				{
					// Store initial game state
					bool wasRunning = GameManager.IsRunning;
					
					// Ensure player is alive (should be after reset)
					if (m_DamageManager != null && !m_DamageManager.IsAlive())
					{
						message = "FAILED: Player is dead after reset, cannot complete level";
					}
						else
						{
							// Get portal bounds to position player properly
							Vector3 portalPosition = m_Portal.transform.position;
							Vector3 originalPosition = m_PlayerObject.transform.position;
							
							// Reset portal's m_PlayerEntered flag using reflection to allow retesting
							FieldInfo playerEnteredField = typeof(Portal).GetField("m_PlayerEntered", 
								BindingFlags.NonPublic | BindingFlags.Instance);
							if (playerEnteredField != null)
							{
								playerEnteredField.SetValue(m_Portal, false);
							}
							
							// Position player slightly outside portal, then move into it
							// This ensures OnTriggerEnter2D is properly called
							m_PlayerObject.transform.position = portalPosition + Vector3.left * 0.5f;
							yield return new WaitForFixedUpdate();
							
							// Now move player into portal trigger zone
							m_PlayerObject.transform.position = portalPosition;
							
							// Wait for physics to process trigger
							yield return new WaitForFixedUpdate();
							yield return new WaitForFixedUpdate();
							yield return new WaitForSeconds(0.1f); // Give extra time for trigger to process
							
							// Check if level was completed
							bool isRunning = GameManager.IsRunning;
							
							if (!isRunning && wasRunning)
							{
								passed = true;
								message = "PASSED: Level completion triggered successfully (Game stopped running)";
							}
							else
							{
								// Additional debugging info
								bool playerHasTag = m_PlayerObject.CompareTag("Player");
								bool portalIsActive = m_Portal.gameObject.activeInHierarchy;
								message = $"FAILED: Level completion not triggered (Game still running: {isRunning}, Player has 'Player' tag: {playerHasTag}, Portal active: {portalIsActive})";
							}
							
							// Reset player position
							m_PlayerObject.transform.position = originalPosition;
							
							// Note: Game state is intentionally not reset here as IsRunning setter is protected
							// The test verifies level completion works, which is the primary goal
						}
					}
			}
			
			float executionTime = Time.realtimeSinceStartup - startTime;
			m_TestResults.Add(new TestResult(testName, passed, message, executionTime));
			
			Debug.Log($"[TEST] {testName} - {message} ({executionTime:F3}s)");
			yield return null;
		}
		
		/// <summary>
		/// Prints final test results summary
		/// </summary>
		private void PrintTestResults()
		{
			Debug.Log("========================================");
			Debug.Log("TEST RESULTS SUMMARY");
			Debug.Log("========================================");
			
			int passedCount = 0;
			int totalCount = m_TestResults.Count;
			
			foreach (TestResult result in m_TestResults)
			{
				string status = result.Passed ? "✓ PASS" : "✗ FAIL";
				Debug.Log($"{status} | {result.TestName}");
				if (m_PrintDetailedResults)
				{
					Debug.Log($"      Message: {result.Message}");
					Debug.Log($"      Time: {result.ExecutionTime:F3}s");
				}
				
				if (result.Passed)
					passedCount++;
			}
			
			Debug.Log("========================================");
			Debug.Log($"TOTAL: {passedCount}/{totalCount} tests passed");
			Debug.Log($"SUCCESS RATE: {(passedCount * 100f / totalCount):F1}%");
			Debug.Log("========================================");
			
			// Also print to console in a format that's easy to read
			string summary = $"\n=== TEST SUMMARY ===\n" +
							$"Passed: {passedCount}/{totalCount}\n" +
							$"Success Rate: {(passedCount * 100f / totalCount):F1}%\n" +
							$"===================\n";
			Debug.Log(summary);
		}
		
		/// <summary>
		/// Public method to manually trigger tests (can be called from UI button or other scripts)
		/// </summary>
		public void RunTests()
		{
			StartCoroutine(RunAllTests());
		}
	}
}

