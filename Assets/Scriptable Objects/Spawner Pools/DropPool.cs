using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
[CreateAssetMenu(menuName = "Drop Pools/Drop Pool")]
public class DropPool : ScriptableObject, IRandomList
{
	[SerializeField]
	private List<GameObject> objects = new List<GameObject>();
	[SerializeField]
	private List<int> chances = new List<int>();
	[SerializeField]
	private float timer;
	[SerializeField]
	private bool depleteChances = false;

	public GameObject GetObject()
	{
		if (IsDepleted)
		{
			Debug.Log("No object returned from depleted DropPool " + name);
			return null;
		}

		int chanceTotal = 0;

		//Add up the chance associated with each object
		foreach(int chance in Chances)
		{
			chanceTotal += chance;
		}

		//Generate a random value between 0 and chance total
		float selection = Random.value * chanceTotal;
		chanceTotal = 0;

		//Pretty sure the logic here results in unbiased randomness.
		//With each iteration, each PhysCircle/float pair has [float] chance
		//to surpass selection as chanceTotal increases
		for(int i = 0; i < Chances.Count; i++)
		{
			chanceTotal += Chances[i];
			if(chanceTotal > selection)
			{
				if (DepleteChances)
				{
					Chances[i]--;
				}

				return Objects[i];
			}
		}
		Debug.LogError(name + " erroniously returned null!!!");
		return null;
	}

	public bool IsDepleted
	{
		get
		{
			foreach (int chance in Chances)
			{
				if (chance > 0)
				{
					return false;
				}
			}
			return true;
		}
	}

	public GameObject SpawnRandom(Vector3 position)
	{
		GameObject newOBJ = GetObject();

		if (newOBJ)
		{
			return Instantiate(newOBJ, position, Quaternion.identity);
		}
		else return null;
	}

	public void CopyValues(DropPool pool)
	{
		name = pool.name + " (Copy)";
		for(int i = 0; i < pool.Objects.Count; i++)
		{
			objects.Add(pool.Objects[i]);
			chances.Add(pool.Chances[i]);
		}
		timer = pool.Timer;
		depleteChances = pool.depleteChances;
	}

	public float Timer
	{
		get
		{
			return timer;
		}

		set
		{
			timer = value;
		}
	}
	public bool DepleteChances
	{
		get
		{
			return depleteChances;
		}

		set
		{
			depleteChances = value;
		}
	}
	public List<GameObject> Objects
	{
		get
		{
			return objects;
		}

		set
		{
			objects = value;
		}
	}
	public List<int> Chances
	{
		get
		{
			return chances;
		}

		set
		{
			chances = value;
		}
	}

}
