using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
	public class Checkpoint_Remove : MonoBehaviour
	{
		public ParticleSystem arch;
		public ParticleSystem archGlow;
		public ParticleSystem archSparksA;
		public ParticleSystem archSparksB;
		public ParticleSystem shrinkingArches;
		public ParticleSystem shrinkingSparks;
		public ParticleSystem archLight;

		public GameObject VFXStop;
		public GameObject portalEndVFX;

		void OnEnable()
		{
			portalEndVFX.SetActive(false);
		}

		public void OnHitEffect()
		{
			if (!gameObject.activeInHierarchy)
			{
				return;
			}
			arch.Stop();
			archGlow.Stop();
			archSparksA.Stop();
			archSparksB.Stop();
			shrinkingArches.Stop();
			shrinkingArches.Stop();
			shrinkingSparks.Stop();
			archLight.Stop();

			VFXStop.SetActive(false);
			portalEndVFX.SetActive(true);

			StartCoroutine(OnCreateEffect());
		}

		IEnumerator OnCreateEffect()
		{
			yield return new WaitForSeconds(0.5f);
			VFXStop.SetActive(true);
			portalEndVFX.SetActive(false);
		}
	}
}