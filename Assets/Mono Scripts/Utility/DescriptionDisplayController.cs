using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionDisplayController : MonoBehaviour
{
	[SerializeField] private RotorController rotor;
	[SerializeField] private GameObject[] objectList;

	private int activeIdx = 0;

	private void Start()
	{
		for(int i = 0; i < objectList.Length; i++)
		{
			if (objectList[i].activeSelf)
			{
				activeIdx = i;
			}
		}
	}

	private void Update()
	{
		if(rotor.MedianIdx != activeIdx)
		{
			objectList[activeIdx].SetActive(false);
			activeIdx = rotor.MedianIdx;
			objectList[activeIdx].SetActive(true);
		}
	}

}
