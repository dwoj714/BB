 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthBar))]
[RequireComponent(typeof(Detonator))]
public class BombController : PhysCircle
{
	public float fallSpeed;

	[HideInInspector]
	public float gravity;
	Detonator detonator;
	DropPool loot;

	protected override void Awake()
	{
		base.Awake();
		rb = GetComponent<Rigidbody2D>();
		detonator = GetComponent<Detonator>();
		gravity = rb.gravityScale;
		loot = GetComponent<DropPool>();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		
		//This ensures the ball will always accelerate or decelerate toward
		//a certain fixed fall speed via velocity damping
		if(rb.velocity.y < -fallSpeed || (fallSpeed < 0 && rb.velocity.y > -fallSpeed ))
		{
			rb.gravityScale = 0;
		}
		else
		{
			rb.gravityScale = gravity;
		}
	}

	public void onHealthDeplete()
	{
		fallSpeed = 0;
		detonator.sparked = true;
	}

	void OnExplosion()
	{
		//Drop an item from the loot pool if the gameObject has a loot pool attached
		if (loot)
		{
			GameObject drop = loot.SpawnRandom(transform);
			Rigidbody2D dropRB = null;

			if (drop)
			{
				dropRB = drop.GetComponent<Rigidbody2D>();
			}

			if (dropRB)
			{
				dropRB.velocity = this.rb.velocity;
			}
		}
		Destroy(this.gameObject);
	}
}