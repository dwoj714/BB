using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptionDisplayController : MonoBehaviour
{
	[SerializeField] private RotorController rotorController;
	[SerializeField] private GameObject[] objectList;

	private int activeIdx = 0;

	private void Start()
	{
		activeIdx = rotorController.MedianIdx;
	}


	private void Update()
	{
		SetDisplay(rotorController.MedianIdx);
	}

	public void SetDisplay(int index)
	{
		objectList[activeIdx].SetActive(false);
		activeIdx = index;
		objectList[activeIdx].SetActive(true);
	}

}
