using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisableChildren : MonoBehaviour
{
	[SerializeField] private GameObject[] targets;

	private void OnDisable()
	{
		foreach(GameObject obj in targets)
		{
			obj.SetActive(false);
		}
	}
}
