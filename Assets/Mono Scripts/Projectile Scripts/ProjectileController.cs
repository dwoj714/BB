using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : Launchable
{
	//The launcher uses a percentage to determine actual speed. 
	//0% results in minSpeed, 100% results in maxSpeed
	public float maxSpeed = 5;
	public float minSpeed = 0;

	[SerializeField]
	private VFXController vfx;

	public float lifespan = Mathf.Infinity;
	private float lifeTimer;

	public int bounces = 3;
	private int bounceCount = 0;

	public bool hasFixedSpeed = false;

	//Whether or not the projectile has left the launcher's circle collider
	private bool escaped;
	private bool launched = false;
	protected float fixedSpeed;

	protected override void Awake()
	{
		base.Awake();
		if(gameObject.layer != 11) gameObject.layer = 10;
		lifeTimer = lifespan;
	}

	public override void Launch(Vector2 direction, float power)
	{
		launched = true;

		float vDiff = maxSpeed - minSpeed;
		rb.isKinematic = false;
		rb.velocity = direction.normalized * (vDiff * power + minSpeed);
		if (hasFixedSpeed)
		{
			fixedSpeed = Mathf.Abs(vDiff * power + minSpeed);
		}

		//Trigger VFX if available
		if(vfx) vfx.ActivateFX();

	}

	protected virtual void Update()
	{
		if(launched)
			lifeTimer -= Time.deltaTime;

		if(lifeTimer <= 0)
		{
			Destroy(gameObject);
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (hasFixedSpeed)
		{
			rb.velocity = rb.velocity.normalized * fixedSpeed;
		}

		if (gameObject.layer != 11 && !escaped && col.Distance(launcherCollider).distance > 0)
		{
			escaped = true;
			gameObject.layer = 9;
		}

	}

	protected override void OnCollisionEnter2D(Collision2D collision)
	{
		base.OnCollisionEnter2D(collision);
		bounceCount++;
		if (bounceCount > bounces)
			Destroy(gameObject);
	}
}
