
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HealthBar))]
[RequireComponent(typeof(Detonator))]
public class BombController : PhysCircle, IHealable
{

	[Header("Scoring")]
	public int pointValue = 10;
	public int comboMult = 1;
	[HideInInspector]
	public ComboIncrement incrementer;

	public static float massMod = 1;
	public static float healthMod = 1;
	public static bool showHealth = false;

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

	private int healthID, timeID;

	//If not immediately sparked, combo multipliers will linger for a little while
	//If sparked while a combo timer is still active, it will hold that combo while detonating.
	//These lists keep track of the combos applied to it, to prevent things from being ignored/overwritten
	private List<int> comboValues = new List<int>();
	private List<float> comboTimers = new List<float>();

	//...Running UpdateHealthVisuals in Start() seems to do it after the values are properly set
	//Do it once during Update, while this is false. (Set to true after calling UpdateHealthVisuals once.
	private bool updatedHealth = false;

	public delegate void BombDetonationEventHandler(object source, EventArgs args);
	public static event BombDetonationEventHandler BombDetonated;

	protected override void Awake()
	{
		base.Awake();
		rb = GetComponent<Rigidbody2D>();

		detonator = GetComponent<Detonator>();
		spr = GetComponent<SpriteRenderer>();
		text = GetComponentInChildren<Text>();
		gravity = rb.gravityScale;
	}

	protected virtual void Start()
	{
		//instantiate the combo incrementer
		incrementer = ScriptableObject.CreateInstance<ComboIncrement>();

		healthID = Shader.PropertyToID("_health");
		timeID = Shader.PropertyToID("_startTime");

	}

	protected virtual void Update()
	{
		ProcessCombos();
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

		//see definition of private bool updatedHealth
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

		//set comboMult to the largest active combo value
		foreach(int i in comboValues)
		{
			if(i > comboMult)
			{
				comboMult = i;
			}
		}

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

	public virtual void UpdateHealthVisuals()
	{
		spr.material.SetFloat(healthID, hb.Health / hb.maxHealth);
		spr.material.SetFloat(timeID, Time.time);

		if (showHealth)
			text.text = Mathf.Ceil(hb.Health).ToString();
		else
			text.text = "";
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

		GameManager.main.AddScore(pointValue * comboMult);

		BombDetonated?.Invoke(this, EventArgs.Empty);

		Destroy(gameObject);
	}

	protected override void OnCollisionEnter2D(Collision2D hit)
	{
		BombController bomb = hit.gameObject.GetComponent<BombController>();

		if (bomb && (detonator.sparked || bomb.detonator.sparked))
		{
			bomb.PrimeCombo(this, 0.5f);
		}

		base.OnCollisionEnter2D(hit);

	}

	private IEnumerator HandleCollisionCombo(BombController bomb)
	{
		//wait a frame to make sure both collisions have been processed
		yield return null;
	}

	public void PrimeCombo(BombController primer, float duration)
	{
		comboValues.Add(primer.comboMult + 1);
		comboTimers.Add(duration);
	}

	public void PrimeCombo(int primer, float duration)
	{
		comboValues.Add(primer);
		comboTimers.Add(duration);
	}

	private void ProcessCombos()
	{
		if(!detonator.sparked)
			for (int i = 0; i < comboValues.Count; i++)
			{
				comboTimers[i] -= Time.deltaTime;
				if (comboTimers[i] <= 0)
				{
					comboTimers.RemoveAt(i);
					comboValues.RemoveAt(i);
					i--;
				}
			}
	}

	public void ApplyScaling()
	{
		rb.mass *= massMod;
		hb.maxHealth *= healthMod;
		hb.FullHeal();
	}

}