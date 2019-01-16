using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class DropPool : MonoBehaviour
{
	//Use these until you find something more elegant...  :(
	public GameObject[] objects;
	public float[] chances;

	public GameObject GetObject()
	{
		float chanceTotal = 0;

		//Add up the chance associated with each PhysCircle
		foreach(float chance in chances)
		{
			chanceTotal += chance;
		}

		//Generate a random value between 0 and chance total
		float selection = Random.value * chanceTotal;
		chanceTotal = 0;

		//Pretty sure the logic here results in unbiased randomness
		//With each iteration, each PhysCircle/float pair has [float] chance to surpass selection as chanceTotal increases
		for(int i = 0; i < chances.Length; i++)
		{
			chanceTotal += chances[i];
			if(chanceTotal > selection)
			{
				return objects[i];
			}
		}

		Debug.Log("RandomObject returned null!!!");
		return null;
	}

	//Use getCircle to get a random PhysCircle, instantiate a clone of it (If it exists) and return a reference to it
	public GameObject SpawnRandom(Transform parent)
	{
		GameObject newCircle = GetObject();

		if (newCircle)
		{
			return Instantiate(newCircle, parent.position, Quaternion.identity);
		}
		else return null;
	}

}
