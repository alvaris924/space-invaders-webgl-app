using UnityEngine;
using System.Collections;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

	public const string InstancePropertyName = "Instance";
	
	private static T instance;
	
	// Gets the spawned instance of this singleton
	// Spawned instance of this class, or null if it hasn't been spawned or has been destroyed.
	
	public static T Instance
	{
		get
		{
			if(instance == null || instance.gameObject == null)
			{
				T existingInstance = FindObjectOfType<T>();
				
				if(existingInstance != null)
				{
					instance = existingInstance;
				}
			}
			return instance;
		}
	}

}
