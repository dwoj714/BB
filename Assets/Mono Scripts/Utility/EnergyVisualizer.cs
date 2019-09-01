using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyVisualizer : Visualizer
{
	[SerializeField] private EnergyUser source;
	private SpriteRenderer spr;
	private int chargeID;
	[SerializeField] private int segments = 3;

	protected override void Start()
	{
		base.Start();
		spr = GetComponent<SpriteRenderer>();
		chargeID = Shader.PropertyToID("_charge");

		spr.material.SetFloat("_seg", segments);

	}

	protected override void Update()
	{
		spr.material.SetFloat(chargeID, source.EnergyPercentage);
	}
}
