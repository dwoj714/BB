using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarVisualizer : Visualizer {

	HealthBar source;
	public bool shieldBar;

	protected override void Start()
	{
		base.Start();
		source = transform.parent.GetComponent<HealthBar>();
	}

	protected override void Update()
	{
		base.Update();

		//alter the scale of the transform based on health/shield percentages
		if (shieldBar && source.maxShield > 0)
		{
			transform.localScale = new Vector3(source.getShield() / source.maxShield * initialScaleX, transform.localScale.y, transform.localScale.z);
		}
		else
		{
			transform.localScale = new Vector3(source.getHealth() / source.maxHealth * initialScaleX, transform.localScale.y, transform.localScale.z);
		}
	}
}
