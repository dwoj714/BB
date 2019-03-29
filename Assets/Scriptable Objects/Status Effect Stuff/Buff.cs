using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff : ScriptableObject
{
	//Should return true if the buff could be applied to the recieving object
	public abstract bool ApplyEffect(GameObject target);
}
