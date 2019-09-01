using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/Projectile Slow")]
public class ProjectileSlow : Buff
{
	public float speedMult = 0.5f;

	public override bool ApplyEffect(GameObject target)
	{
		ProjectileController projectile = target.GetComponent<ProjectileController>();

		if (projectile && projectile.gameObject.layer == 9)
		{
			if (projectile.hasFixedSpeed)
			{
				projectile.fixedSpeed *= speedMult;
			}
			else
			{
				projectile.rb.velocity *= speedMult;
				projectile.rb.gravityScale *= speedMult;
			}
			return true;
		}
		return false;
	}
}
