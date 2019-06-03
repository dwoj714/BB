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

		Rigidbody2D hitRb = hit.collider.GetComponent<Rigidbody2D>();
		if (hitRb)
		{
			joint.connectedBody = hitRb;
		}

		joint.enabled = true;
		detonator.sparked = true;
	}

	void OnExplosion()
	{
		GameObject.Destroy(this.gameObject);
	}

}