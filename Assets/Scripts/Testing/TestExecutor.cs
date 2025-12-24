using UnityEngine;
using QAAPlatformer.Testing;

namespace QAAPlatformer.Testing
{
	/// <summary>
	/// Simple executor script to trigger tests via keyboard shortcut or button
	/// Press 'T' key to run tests, or attach to a UI button
	/// </summary>
	public class TestExecutor : MonoBehaviour
	{
		[Header("Test Runner Reference")]
		[SerializeField] private GameTestRunner m_TestRunner;
		
		[Header("Keyboard Shortcut")]
		[SerializeField] private KeyCode m_TestKey = KeyCode.T;
		
		private void Start()
		{
			// Auto-find test runner if not assigned
			if (m_TestRunner == null)
			{
				m_TestRunner = FindObjectOfType<GameTestRunner>();
			}
		}
		
		private void Update()
		{
			// Press 'T' to run tests
			if (Input.GetKeyDown(m_TestKey))
			{
				RunTests();
			}
		}
		
		/// <summary>
		/// Public method to run tests (can be called from UI button)
		/// </summary>
		public void RunTests()
		{
			if (m_TestRunner != null)
			{
				Debug.Log("=== MANUALLY TRIGGERING TESTS ===");
				m_TestRunner.RunTests();
			}
			else
			{
				Debug.LogError("TestExecutor: GameTestRunner not found! Please assign it in the inspector or ensure it exists in the scene.");
			}
		}
	}
}

