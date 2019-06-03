using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : PhysCircle {

	[Header("Explosion Charges")]
	public int totalCharges = 3;
	public int charges;
	public bool trigger = false;

	SpriteRenderer spr;
	Detonator detonator;

	//EnergyUser main, wep1, wep2;

	// Use this for initialization
	void Start ()
	{
		charges = totalCharges;
		detonator = GetComponent<Detonator>();
		rb.isKinematic = true;
		spr = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		if (trigger)
		{
			trigger = false;
			spr.material.SetFloat("_startTime", Time.time);
		}
	}

	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		Detonator enemyDetonator = collision.collider.GetComponent<Detonator>();
																					//condition added to prevent sparked bombs from using a charge
		if (charges > 0 && !detonator.sparked && collision.collider.tag == "Enemy" && !enemyDetonator.sparkedLastFrame)
		{
			detonator.sparked = true;
			spr.material.SetFloat("_startTime", Time.time);
			spr.material.SetFloat("_angle", Vector2.Angle(collision.collider.transform.position, Vector2.right) * Mathf.Deg2Rad);
		}
	}

	private void OnExplosion()
	{
		charges--;
	}

	public void Restart()
	{
		charges = totalCharges;
	}
}
