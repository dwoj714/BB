using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyVisualizer : Visualizer
{
	public EnergyUser source;
	public Image target;

	protected override void Update()
	{
		target.fillAmount = source.EnergyPercentage;
	}
}
