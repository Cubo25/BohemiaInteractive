using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static QAAPlatformer.GameManager;

namespace QAAPlatformer.Damage
{
	/// <summary>
	/// Manages receiving of damage and the total health of the player.
	/// </summary>
	public class CharacterDamageManager : MonoBehaviour
	{
		[SerializeField] protected int m_HealthPoints = 100;
		protected bool m_IsDead = false;

		/// <summary>
		/// Returns true if player's health is below or equal to 0, therefore marked as dead.
		/// </summary>
		/// <returns></returns>
		public bool IsDead()
		{
			return (m_IsDead);
		}

		/// <summary>
		/// Returns true if player's health is above 0, therefore marked as alive.
		/// </summary>
		/// <returns></returns>
		public bool IsAlive()
		{
			return !IsDead();
		}

		/// <summary>
		/// Receives damage, damage is substracted from the player's health pool. 
		/// If damage received takes the player's health below 0, it's marked as death
		/// and OnDeath is called
		/// </summary>
		/// <param name="damage">Amount of damage received</param>
		public void ReceiveDamage(int damage)
		{
			m_HealthPoints -= damage;
			if (m_HealthPoints <= 0 && !m_IsDead)
				OnDeath();
		}

		/// <summary>
		/// Called when player's health drops to or below 0
		/// </summary>
		protected void OnDeath()
		{
			m_IsDead = true;
			GameManager.OnPlayerDeath(gameObject);	
		}
	}
}