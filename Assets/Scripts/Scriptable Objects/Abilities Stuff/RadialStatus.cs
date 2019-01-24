using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Abilites/RadiaStatus"))]
public class RadialStatus : PointAbility
{
	public float radius = 4;
	public StatusEffect effect;


	public override void Activate(Vector2 position)
	{
		Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius);
		
		foreach(Collider2D col in hits)
		{
			effect.ApplyEffect(col.gameObject);
		}
	}
}
