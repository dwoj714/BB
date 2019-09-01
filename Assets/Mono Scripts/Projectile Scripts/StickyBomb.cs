using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RelativeJoint2D))]
public class StickyBomb : ExplosiveProjectile
{
	RelativeJoint2D joint;

	private bool autoDetonate = false;
	private BombController connection = null;

	protected override void Awake()
	{
		base.Awake();
		joint = GetComponent<RelativeJoint2D>();
	}

	void Start()
	{
		joint.enabled = false;
	}

	protected override void Update()
	{
		base.Update();

		if (connection && autoDetonate && connection.detonator.sparked)
		{
			detonator.Explode();
		}

		if (gameObject.layer == 14 && !joint.connectedBody)
		{
			gameObject.layer = 9;
		}
	}

	protected override void OnCollisionEnter2D(Collision2D hit)
	{
		base.OnCollisionEnter2D(hit);

		//if we're not connected to something already, and we just hit a bomb...
		if(!joint.connectedBody && hit.gameObject.layer == 12)
		{
			BombController bomb = hit.collider.GetComponent<BombController>();
			if (bomb)
			{
				joint.connectedBody = bomb.rb;
				joint.enabled = true;
				joint.linearOffset = joint.linearOffset.normalized * bomb.AdjustedRadius;
				gameObject.layer = 14;
				connection = bomb;
			}
		}
		detonator.sparked = true;
	}

	public override void SetUpgrades(int[] upgradeLevels)
	{
		base.SetUpgrades(upgradeLevels);
		if(upgradeLevels[3] != 0)
		{
			autoDetonate = true;
		}
	}

}