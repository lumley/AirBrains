using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyView : MonoBehaviour 
{
	[SerializeField]
	private CharacterVisualConfig Config;
	
	public LobbyModel Model;

	public void ApplyModel(LobbyModel model)
	{
		if (Model != null) 
		{
			Model.OnChanged -= OnModelChanged;
			Model = null;
		}

		Model = model;

		if (Model != null) 
		{
			Model.OnChanged += OnModelChanged;
			OnModelChanged ();
		}
	}

	private void OnModelChanged()
	{
		
	}
}
