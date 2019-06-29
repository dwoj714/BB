using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Drop Pools/Drop Chain")]
public class DropChain : ScriptableObject, IRandomList
{
	[SerializeField]
	private List<DropPool> links = new List<DropPool>();

	public GameObject GetObject()
	{
		foreach(DropPool link in links)
		{
			if (!link.IsDepleted)
			{
				return link.GetObject();
			}
		}
		Debug.Log("No object returned from depleted DropChain " + name);
		return null;
	}

	public bool IsDepleted
	{
		get
		{
			foreach (DropPool link in links)
			{
				if (!link.IsDepleted)
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

	public void CopyValues(DropChain chain)
	{
		name = chain.name + " (Copy)";

		links.Clear();
		foreach(DropPool link in chain.Links)
		{
			DropPool copy = ScriptableObject.CreateInstance<DropPool>();
			copy.CopyValues(link);
			links.Add(copy);
		}
	}

	public List<DropPool> Links
	{
		get
		{
			return links;
		}

		set
		{
			links = value;
		}
	}
}
