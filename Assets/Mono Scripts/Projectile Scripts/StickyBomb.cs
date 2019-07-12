using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RelativeJoint2D))]
public class StickyBomb : ExplosiveProjectile
{
	RelativeJoint2D joint;

	protected override void Awake()
	{
		base.Awake();
		joint = GetComponent<RelativeJoint2D>();
	}

	void Start()
	{
		joint.enabled = false;
		joint.connectedBody = GameObject.Find("World Rigidbody").GetComponent<Rigidbody2D>();
	}

	protected override void OnCollisionEnter2D(Collision2D hit)
	{
		base.OnCollisionEnter2D(hit);

		if(joint.connectedBody != null && hit.gameObject.layer == 12)
		{
			BombController bomb = hit.collider.GetComponent<BombController>();
			if (bomb)
			{
				joint.connectedBody = bomb.rb;
				joint.enabled = true;
			}
			joint.linearOffset = joint.linearOffset.normalized * bomb.AdjustedRadius;
		}

		detonator.sparked = true;
	}

	private void OnDestroy()
	{
		Debug.Log(name + " what");
	}

	void OnExplosion()
	{
		GameObject.Destroy(this.gameObject);
	}

}