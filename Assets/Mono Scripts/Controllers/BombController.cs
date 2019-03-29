 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthBar))]
[RequireComponent(typeof(Detonator))]
public class BombController : PhysCircle
{
	public static GameManager manager;
	public int pointValue = 10;

	public static float massMult = 1.5f;

	public float fallSpeed;

	[HideInInspector]
	public float gravity;
	Detonator detonator;
	public DropPool loot;

	//Assign this in the inspector. This is used by the spawner to check if the space for this bomb is clear before spawning.
	//Intended for use with bombs that may have other parts attached as children.
	//This radius should account for how far out those parts reach.
	public float broadRadius;

	protected override void Awake()
	{
		base.Awake();
		rb = GetComponent<Rigidbody2D>();
		rb.mass *= massMult;
		detonator = GetComponent<Detonator>();
		gravity = rb.gravityScale;
	}

	protected virtual void Start()
	{
		//Set the game manager if it wasn't done in the inspector
		if (!manager)
		{
			manager = GameObject.Find("Game Manager").GetComponent<GameManager>();
			Debug.Log("Bombcontroller Game Manager assigned");
		}
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

	public void OnHealthDeplete()
	{
		fallSpeed = 0;
		detonator.sparked = true;
	}

	void OnExplosion()
	{
		//Drop an item from the loot pool if the gameObject has a loot pool attached
		if (loot)
		{
			GameObject drop = loot.SpawnRandom(transform.position);
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

		manager.AddScore(pointValue);

		Destroy(this.gameObject);
	}
}