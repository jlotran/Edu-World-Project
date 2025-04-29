using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace SimpleFPS
{
	/// <summary>
	/// Main UI script that stores references to other elements (views).
	/// </summary>
	public class GameUI : MonoBehaviour
	{
		public Gameplay       Gameplay;
		[HideInInspector]
		public NetworkRunner  Runner;

	}
}
