using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusBomb : BombController {

	public Buff effect;
	public VFXController FX;
	public float radius;
	public float delay = 1;
	private float timer;
	private int chargeID;
	private bool timerEnabled = true;

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
		timer = Time.time - delay * Mathf.Floor(Time.time / delay);
		Debug.Log(Time.time +" - " + delay + " * floor(" + Time.time + " / " + delay + ") = " + timer);
	}

	void Update ()
	{
		if(timerEnabled && hb.Health > 0)
		{
			timer += Time.deltaTime;
		}
		
		if(timer >= delay)
		{
			Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

			int count = 0;
			foreach(Collider2D hit in hits)
			{
				if (effect.ApplyEffect(hit.gameObject))
					count++;
			}

			FX.ActivateFX();

			timer = 0;

			//Debug.Log(gameObject.name + " applied " + effect.name + " to " + count + " objects");
		}
		FX.mat.SetFloat(chargeID, timer / delay);
	}

}
