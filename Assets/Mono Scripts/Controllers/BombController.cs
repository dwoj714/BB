 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HealthBar))]
[RequireComponent(typeof(Detonator))]
public class BombController : PhysCircle, IHealable
{
	public static GameManager manager;

	[Header("Scoring")]
	public int pointValue = 10;
	public int comboMult = 1;

	public static float massMult = 1.5f;

	[HideInInspector]
	public float gravity;
	[Header("References")]
	public Detonator detonator;
	public DropPool loot;

	[Header ("Visuals")]
	public Text text;
	public SpriteRenderer spr;

	[Header("Behavior")]
	public float fallSpeed;

	//Assign this in the inspector. This is used by the spawner to check if the space for this bomb is clear before spawning.
	//Intended for use with bombs that may have other parts attached as children.
	//This radius should account for how far out those parts reach.
	public float broadRadius;

	//...Running UpdateHealthVisuals in Start() seems to do it after the values are properly set
	//Do it once during Update, while this is false. (Set to true after calling UpdateHealthVisuals once.
	private bool updatedHealth = false;

	protected override void Awake()
	{
		base.Awake();
		rb = GetComponent<Rigidbody2D>();
		rb.mass *= massMult;
		detonator = GetComponent<Detonator>();
		spr = GetComponent<SpriteRenderer>();
		text = GetComponentInChildren<Text>();
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

		if (!updatedHealth)
		{
			UpdateHealthVisuals();
			updatedHealth = true;
		}
	}

	public void OnHealthDeplete()
	{
		fallSpeed = 0;
		detonator.sparked = true;
		spr.material.SetFloat("_health", hb.Health / hb.maxHealth);
		text.text = "x" + comboMult;
		text.color = Color.white;
	}

	public void OnHeal(float healAmount)
	{
		UpdateHealthVisuals();
	}

	public void OnTakeDamage(float damage)
	{
		UpdateHealthVisuals();
	}

	void UpdateHealthVisuals()
	{
		spr.material.SetFloat("_health", hb.Health / hb.maxHealth);
		text.text = "" + Mathf.Ceil(hb.Health);
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

		manager.AddScore(pointValue * comboMult);
		//Debug.Log("Adding 10 * " + comboMult + "To score.");

		Destroy(this.gameObject);
	}
}