using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBomb : ExplosiveProjectile
{
	[SerializeField] private float detonationSpeedBonus = 0.12f;

	[Header("Time Bomb Behavior", order = 0)]
	[SerializeField] private int bombContactLimit = 3;
	[SerializeField] private Color[] contactColors = new Color[3];
	private SpriteRenderer spr;

	private bool contactDetonation = false;
	private int contactCount = 0;

	protected override void Awake()
	{
		base.Awake();
		spr = GetComponent<SpriteRenderer>();
	}

    public override void Launch(Vector2 direction, float power)
	{
		base.Launch(direction, power);
		detonator.sparked = true;
	}

	protected override void OnCollisionEnter2D(Collision2D other)
	{
		base.OnCollisionEnter2D(other);
		if(contactDetonation && other.collider.gameObject.layer == 12)
		{
			contactCount++;
			if (contactCount == bombContactLimit)
			{
				detonator.Explode();
			}
			else
			{
				spr.color = contactColors[contactCount];
			}
		}
	}

	public override int[] UpgradeLevels
	{
		get
		{
			return upgradeLevels;
		}
		set
		{
			int[] valCopy = new int[value.Length];
			value.CopyTo(valCopy, 0);
			valCopy[6] = valCopy[12];
			valCopy[7] = valCopy[12];
			valCopy[8] = valCopy[12];

			base.UpgradeLevels = valCopy;
			detonator.fuse *= 1 - detonationSpeedBonus * upgradeLevels[11];
			contactDetonation = value[13] != 0;

			if (contactDetonation)
			{
				spr.color = contactColors[0];
			}
		}
	}

}
