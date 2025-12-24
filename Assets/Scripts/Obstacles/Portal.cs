using QAAPlatformer.Damage;
using UnityEngine;

namespace QAAPlatformer.Obstacles
{
	/// <summary>
	/// Portal entity that can proccess player enter/leave events. 
	/// </summary>
	public class Portal : MonoBehaviour
	{
		protected bool m_PlayerEntered = false;

		/// <summary>
		/// Called when player enters the portal.
		/// If player is alive, calls OnLevelComplete to GameManager, finishing the level effectively
		/// </summary>
		/// <param name="playerObject">The game object player is controlling.</param>
		private void OnPlayerEnter(GameObject playerObject)
		{
			CharacterDamageManager damageManager = playerObject.GetComponent<CharacterDamageManager>();
			if (damageManager && damageManager.IsAlive())
			{
				GameManager.OnLevelComplete(playerObject);
			}
		}

		/// <summary>
		/// Called when player leaves the portal.
		/// </summary>
		/// <param name="playerObject">The game object player is controlling.</param>
		private void OnPlayerLeft(GameObject playerObject)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collision"></param>
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.gameObject.CompareTag("Player"))
			{
				if (!m_PlayerEntered)
				{
					m_PlayerEntered = true;
					OnPlayerEnter(collision.gameObject);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="collision"></param>
		private void OnTriggerExit2D(Collider2D collision)
		{
			if (collision.gameObject.CompareTag("Player"))
			{
				m_PlayerEntered = false;
				OnPlayerLeft(collision.gameObject);
			}
		}
	}

}