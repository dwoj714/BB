using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//most of this is copied from the StatusBomb class. This is intended 
//to control a separate object which can be  attached to any bomb
public class StatusFieldController : MonoBehaviour
{
	[Header("Behavior")]
	public Buff effect;
	public PulseMode pulseMode = PulseMode.Sync;
	public float radius;
	public float delay = 1;

	[Header("Imagery")]
	public Sprite icon;
	public Color iconColor;
	public Vector3 iconLocalScale = Vector3.up + Vector3.right;

	[Header("Detection Mode Values")]
	public LayerMask detectionMask;
	public float unchargedAlpha = .1f;
	public float chargedAlpha = .3f;

	private float timer;
	private int chargeID = Shader.PropertyToID("_charge");
	private int alphaID = Shader.PropertyToID("_min");
	private BombController bomb;
	private bool timerEnabled = true;
	private bool primed = false;

	private VFXController FX;

	// Start is called before the first frame update
	void Start()
    {
		FX = GetComponent<VFXController>();
		bomb = transform.parent.GetComponent<BombController>();

		FX.SetRadius(radius);

		if (pulseMode == PulseMode.Sync)
		{
			timer = Time.time - delay * Mathf.Floor(Time.time / delay);
		}
		else
		{
			timer = 0;
		}

		if (bomb)
		{
			SpriteRenderer iconSprite = new GameObject().AddComponent<SpriteRenderer>();
			iconSprite.transform.parent = transform.parent;
			iconSprite.transform.localPosition = Vector3.zero;
			iconSprite.sprite = icon;
			iconSprite.color = iconColor;
			iconSprite.sortingOrder = -4;
			iconSprite.transform.localScale = iconLocalScale;
		}
		else
		{
			Debug.LogError("StatusField: No bombcontroller found on parent!");
		}

	}

	private void Update()
	{
		//count up to recharge the pulse, if the parent bomb isn't dead
		if (timerEnabled && bomb.hb.Health > 0)
		{
			timer += Time.deltaTime;
		}

		//either prime the pulse or set it off at full charge
		if (timer >= delay)
		{
			//if not in detection mode, apply the radial status. Otherwise, the status is primed, and awaiting a target
			if (pulseMode != PulseMode.Detection)
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

		//update the visuals
		FX.mat.SetFloat(chargeID, timer / delay);
	}

	void FixedUpdate()
    {
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
