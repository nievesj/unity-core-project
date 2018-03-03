using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ModestTree;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Assert=ModestTree.Assert;

namespace Zenject.Tests.SceneDecorators
{
    public class TestSceneDecorators
    {
        const string CommonFolderPath = "Assets/Plugins/Zenject/OptionalExtras/IntegrationTests/SceneLoading/Decorators";
        const string DecoratorScenePath = CommonFolderPath + "/DecoratorScene.unity";
        const string MainScenePath = CommonFolderPath + "/MainScene.unity";

        Scene _mainScene;
        Scene _decoratorScene;

        [UnityTest]
        public IEnumerator TestValidationPass()
        {
            ZenUnityEditorUtil.ValidateCurrentSceneSetup();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestSuccess()
        {
            ZenUnityEditorUtil.RunCurrentSceneSetup();
            yield break;
        }

        [UnityTest]
        public IEnumerator TestFail()
        {
            _decoratorScene.GetRootGameObjects().Where(x => x.name == "Foo")
                .Single().GetComponent<ZenjectBinding>().enabled = false;

            Assert.Throws(() => ZenUnityEditorUtil.RunCurrentSceneSetup());
            yield break;
        }

        [SetUp]
        public void SetUp()
        {
            ClearScene();

            _decoratorScene = EditorSceneManager.OpenScene(DecoratorScenePath, OpenSceneMode.Additive);
            _mainScene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Additive);
        }

        [TearDown]
        public void TearDown()
        {
            EditorSceneManager.CloseScene(_mainScene, true);
            EditorSceneManager.CloseScene(_decoratorScene, true);

            ClearScene();
        }

        void ClearScene()
        {
            var scene = EditorSceneManager.GetActiveScene();

            // This is the temp scene that unity creates for EditorTestRunner
            Assert.IsEqual(scene.name, "");

            foreach (var gameObject in scene.GetRootGameObjects())
            {
                GameObject.DestroyImmediate(gameObject);
            }
        }
    }
}
