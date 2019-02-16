using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "Scene Configuration", menuName = "Installers/Core Framework Settings/Scene Configuration")]
public class SceneConfiguration : ScriptableObjectInstaller<SceneConfiguration>
{
    public override void InstallBindings()
    {
        //nothing
    }
}
