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
	[SerializeField] protected float lifeSpanBonus = 0.25f;

	private float lifeTimer = 0;

	//Whether or not the projectile has left the launcher's circle collider
	private bool escaped;
	protected bool launched;
	private SpriteRenderer sprite;
	[HideInInspector] public float fixedSpeed;

	protected override void Awake()
	{
		base.Awake();
		if(gameObject.layer != 11) gameObject.layer = 10;

		sprite = GetComponent<SpriteRenderer>();
		sprite.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;


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
		rb.velocity = direction.normalized * LaunchSpeed(power);
		if (hasFixedSpeed)
		{
			fixedSpeed = LaunchSpeed(power);
		}

		sprite.maskInteraction = SpriteMaskInteraction.None;

		//Trigger VFX if available
		if(vfx) vfx.ActivateFX();
	}

	protected virtual void Update()
	{
		if (lifespanMode == LifespanMode.Timed)
		{
			if (launched)
				lifeTimer += Time.deltaTime;

			if (lifeTimer > lifespan * (1 + lifeSpanBonus * upgradeLevels[2]))
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

		if (gameObject.layer != 11 && !escaped && launcherCollider && col.Distance(launcherCollider).distance > 0)
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

	/*public float SpeedAtPower(float power)
	{

	}*/

	public override int[] UpgradeLevels
	{
		get
		{
			return upgradeLevels;
		}
		set
		{
			base.UpgradeLevels = value;
			rb.mass *= 1 + upgradeLevels[3] * massBonus;
			maxSpeed *= 1 + upgradeLevels[4] * maxSpeedBonus;
			minSpeed *= 1 + upgradeLevels[4] * minSpeedBonus;
			lifespan *= 1 + upgradeLevels[5] * lifeSpanBonus;
		}
	}

	public override float LaunchSpeed(float power)
	{
		power = Mathf.Clamp01(power);
		float vDiff = maxSpeed - minSpeed;
		return vDiff * power + minSpeed;

		//return SpeedAtPower(power);
	}

}

public enum LifespanMode {Timed, Other};
