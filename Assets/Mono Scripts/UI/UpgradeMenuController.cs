﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UpgradeMenuController : MonoBehaviour
{
	[SerializeField] private Button[] tabs = new Button[3];
	[SerializeField] private UpgradeNodeController[] rootNodePrefabs = new UpgradeNodeController[7];
	//private UpgradeNodeController[] loadedRootNodes = new UpgradeNodeController[3];
	[SerializeField] private RectTransform scrollContent;

	public void SelectTab(int tab)
	{
		ActiveTab = tab;
	}

	private List<UpgradeNodeController>[] nodeChains = new List<UpgradeNodeController>[3];

	private int activeTab = 1;
	public int ActiveTab
	{
		get
		{
			return activeTab;
		}
		set
		{
			activeTab = value;

			switch (activeTab)
			{
				case 0:
					tabs[0].interactable = false;
					tabs[1].interactable = true;
					tabs[2].interactable = true;

					GetRootNode(0).gameObject.SetActive(true);
					GetRootNode(1).gameObject.SetActive(false);
					GetRootNode(2).gameObject.SetActive(false);

					GetRootNode(0).CollapseDownward();
					
					break;
				case 1:
					tabs[0].interactable = true;
					tabs[1].interactable = false;
					tabs[2].interactable = true;

					GetRootNode(0).gameObject.SetActive(false);
					GetRootNode(1).gameObject.SetActive(true);
					GetRootNode(2).gameObject.SetActive(false);

					GetRootNode(1).CollapseDownward();
					
					break;
				case 2:
					tabs[0].interactable = true;
					tabs[1].interactable = true;
					tabs[2].interactable = false;

					GetRootNode(0).gameObject.SetActive(false);
					GetRootNode(1).gameObject.SetActive(false);
					GetRootNode(2).gameObject.SetActive(true);

					GetRootNode(2).CollapseDownward();

					break;
			}
		}
	}

	//Activate the upgrade menu for the middle launcher
	private void OnEnable()
	{
		ActiveTab = WeaponManager.MiddleIdx;
	}

	private void Update()
	{
		//resize the scroll view based on the height of all the upgrade nodes
		scrollContent.sizeDelta = Vector2.right * scrollContent.sizeDelta.x + Vector2.up * NodeChainHeight(activeTab);
	}

	public UpgradeMenuController()
	{
		//Instantiate the members of nodeChains
		nodeChains[0] = new List<UpgradeNodeController>();
		nodeChains[1] = new List<UpgradeNodeController>();
		nodeChains[2] = new List<UpgradeNodeController>();
	}

	private void Awake()
	{
		gameObject.SetActive(false);
		UpgradeNodeController.upgradeMenu = this;
	} 

	public void SetupMenus()
	{
		//destroy node chains that already exist before spawning new ones
		for(int i = 0; i < 3; i++)
		{
			Debug.Log("Delete Attempt " + i + ": " + GetRootNode(i));
			if (GetRootNode(i) != null)
			{
				Debug.Log("Destroying " + GetRootNode(i).gameObject);
				Destroy(GetRootNode(i).gameObject);
				nodeChains[i].Clear();
			}
		}

		for(int i = 0; i < WeaponManager.EquippedPrefabs.Length; i++)
		{
			UpgradeNodeController root = Instantiate(rootNodePrefabs[WeaponManager.EquippedPrefabs[i]], scrollContent).GetComponent<UpgradeNodeController>();

			Debug.Log("Instantiated root node from equipped prefab " + WeaponManager.EquippedPrefabs[i]);

			FillChainDownward(root, nodeChains[i]);

			foreach(UpgradeNodeController node in nodeChains[i])
			{
				node.targetLauncher = (LauncherController)WeaponManager.Weapons[i];
			}

			root.gameObject.SetActive(false);

			//spawn the root node corresponding to the equipped 
			//loadedRootNodes[i] = Instantiate<UpgradeNodeController>(rootNodePrefabs[WeaponManager.EquippedPrefabs[i]], scrollContent);
			//loadedRootNodes[i].SetLauncherDownward(WeaponManager.Weapons[i] as LauncherController);
		}

	}

	private void FillChainDownward(UpgradeNodeController root, List<UpgradeNodeController> list)
	{
		list.Add(root);
		UpgradeNodeController[] childNodes = root.GetComponentsInChildren<UpgradeNodeController>();
		foreach (UpgradeNodeController node in childNodes)
		{
			if (!list.Contains(node))
			{
				list.Add(node);
			}
		}
	}

	public void ToggleActive()
	{
		gameObject.SetActive(!gameObject.activeInHierarchy);
	}

	public void OnNodeExpanded(UpgradeNodeController node)
	{
		bool foundLastNode = false;
		for(int i = 0; i < nodeChains[activeTab].Count && !foundLastNode; i++)
		{

		}
	}

	private UpgradeNodeController GetRootNode(int i)
	{
		try
		{
			return nodeChains[i][0];
		}
		catch(Exception) { return null; }
	}

	private float NodeChainHeight(int i)
	{
		float total = 0;
		foreach(UpgradeNodeController node in nodeChains[i])
		{
			total += node.CurrentHeight;
		}
		return total;
	}

}
