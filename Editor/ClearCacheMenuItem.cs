using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ClearCacheMenuItem
{
	[MenuItem("_Core / Clear Persistent Data Path Directory")]
	private static void CreateRedBlueGameObject()
	{
		Directory.Delete(Application.persistentDataPath, true);
		Debug.Log(("Directory " + Application.persistentDataPath + " deleted.").Colored(Colors.yellow));
	}
}