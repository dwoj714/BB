using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusBomb : BombController {

	public Buff effect;
	public VFXController FX;
	public PulseMode pulseMode = PulseMode.Sync;
	public float radius;
	public float delay = 1;
	private float timer;
	private int chargeID;
	private int alphaID;
	private bool timerEnabled = true;

	[Header("Detection Mode Values")]
	public LayerMask detectionMask;
	public float unchargedAlpha = .1f;
	public float chargedAlpha = .3f;


	private bool primed = false;

	public bool TimerEnabled
	{
		get
		{
			return timerEnabled;
		}
		set
		{
			timerEnabled = value;
		}
	}

	protected override void Start()
	{
		base.Start();
		FX.SetRadius(radius);

		chargeID = Shader.PropertyToID("_charge");
		alphaID = Shader.PropertyToID("_min");

		if (pulseMode == PulseMode.Sync)
		{
			timer = Time.time - delay * Mathf.Floor(Time.time / delay);
		}
		else
		{
			timer = 0;
		}
	}

	protected override void Update ()
	{
		base.Update();

		if(timerEnabled && hb.Health > 0)
		{
			timer += Time.deltaTime;
		}
		
		if(timer >= delay)
		{
			//if not in detection mode, apply the radial status. Otherwise, the status is primed, and awaiting a target
			if(pulseMode != PulseMode.Detection)
			{
				ApplyRadius();
				timer = 0;
			}
			else
			{
				primed = true;
				FX.mat.SetFloat(alphaID, chargedAlpha);
			}
		}

		FX.mat.SetFloat(chargeID, timer / delay);
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		//if the pulse is primed and there are colliders in layer 'detectionMask'...
		if (primed && Physics2D.OverlapCircle(transform.position, radius, detectionMask))
		{
			ApplyRadius();
			primed = false;
			timer = 0;
			FX.mat.SetFloat(alphaID, unchargedAlpha);
		}

	}

	private void ApplyRadius()
	{
		Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

		int count = 0;
		foreach (Collider2D hit in hits)
		{
			if (effect.ApplyEffect(hit.gameObject))
				count++;
		}

		FX.ActivateFX();
	}

}

public enum PulseMode { Sync, Self, Detection};
