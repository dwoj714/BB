using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyVisualizer : Visualizer
{
	public BaseController source;

	protected override void Update()
	{
		base.Update();
		transform.localScale = new Vector3(source.energy / source.maxEnergy * initialScaleX, transform.localScale.y, transform.localScale.z);
	}
}
