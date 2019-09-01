using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : Launchable
{
	//The launcher uses a percentage to determine actual speed. 
	//0% results in minSpeed, 100% results in maxSpeed
	[Header("Launch Speed/Behavior")]
	public float maxSpeed = 5;
	public float minSpeed = 0;
	public bool hasFixedSpeed = false;
	[SerializeField] private LifespanMode lifespanMode = LifespanMode.Timed;
	public float lifespan = Mathf.Infinity;

	[Header ("Visual References")]
	[SerializeField]
	private VFXController vfx;
	[SerializeField] protected ParticleSystem deathFX;
	private TrailRenderer trail;

	[Header("Upgrades Per Level")]
	[SerializeField] protected float massBonus = 0.15f;
	[SerializeField] protected float maxSpeedBonus = 0.25f;
	[SerializeField] protected float minSpeedBonus = 0.25f;

	private float lifeTimer;

	//Whether or not the projectile has left the launcher's circle collider
	private bool escaped;
	protected bool launched = false;
	public float fixedSpeed;

	protected override void Awake()
	{
		base.Awake();
		if(gameObject.layer != 11) gameObject.layer = 10;
		lifeTimer = lifespan;
		trail = GetComponentInChildren<TrailRenderer>();
		if (deathFX)
		{
			deathFX = Instantiate(deathFX, transform.position, Quaternion.identity);
			deathFX.gameObject.SetActive(false);
		}
	}

	public override void Launch(Vector2 direction, float power)
	{
		launched = true;
		rb.isKinematic = false;
		rb.velocity = direction.normalized * SpeedAtPower(power);
		if (hasFixedSpeed)
		{
			fixedSpeed = SpeedAtPower(power);
		}

		//Trigger VFX if available
		if(vfx) vfx.ActivateFX();
	}

	protected virtual void Update()
	{
		if (lifespanMode == LifespanMode.Timed)
		{
			if (launched)
				lifeTimer -= Time.deltaTime;

			if (lifeTimer <= 0)
			{
				Destroy(gameObject);
			}
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

	protected virtual void OnDestroy()
	{
		if (deathFX && launched)
		{
			deathFX.transform.position = transform.position;
			deathFX.gameObject.SetActive(true);

			if (trail)
			{
				trail.transform.parent = deathFX.transform;
			}

		}
	}

	public float SpeedAtPower(float power)
	{
		power = Mathf.Clamp01(power);
		float vDiff = maxSpeed - minSpeed;
		return vDiff * power + minSpeed;
	}

	public override void SetUpgrades(int[] upgradeLevels)
	{
		rb.mass *= 1 + upgradeLevels[0] * massBonus;
		maxSpeed *= 1 + upgradeLevels[1] * maxSpeedBonus;
		minSpeed *= 1 + upgradeLevels[2] * minSpeedBonus;
	}

}

public enum LifespanMode {Timed, Other};
