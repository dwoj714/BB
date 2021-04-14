using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEndMenuController : MonoBehaviour
{

	private void Start()
	{
		GameplayController.waveEvent += OnWaveEvent;
		gameObject.SetActive(false);
	}

	void OnWaveEvent(object source, WaveEventArgs args)
	{
		gameObject.SetActive(!args.waveStart);
	}

	public void SignalWaveStart()
	{
		GameplayController.main.BeginWave();
	}
}
