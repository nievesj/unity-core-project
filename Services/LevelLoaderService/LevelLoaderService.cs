using System.Threading.Tasks;
using Core.Services.Assets;
using Core.Services.Factory;
using Core.Services.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace Core.Services.Levels
{
    public class LevelLoaderService : Service
    {
        [Inject]
        private AssetService _assetService;

        [Inject]
        private UIService _uiService;

        [Inject]
        private FactoryService _factoryService;

#pragma warning disable 0414    // suppress value not used warning
        private LevelLoaderServiceConfiguration _configuration;
#pragma warning restore 0414    // restore value not used warning

        private Level _currentLevel;

        public Level CurrentLevel => _currentLevel;

        public LevelLoaderService(ServiceConfiguration config)
        {
            _configuration = config as LevelLoaderServiceConfiguration;
        }

        /// <summary>
        /// Attemps to load a level. First the screen is faded
        /// </summary>
        /// <param name="name"> bundle name </param>
        /// <returns> Observable </returns>
        public async Task<Level> LoadLevel(string name)
        {
            //Fade screen before loading level
            _uiService.DarkenScreen(true).Subscribe();
            return await DoLoadLevel(name);
        }

        /// <summary>
        /// Once the screen has been blocked, load the level
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private async Task<Level> DoLoadLevel(string name)
        {
            if (_currentLevel)
                await UnloadLevel(_currentLevel);

            var level = await _assetService.GetAndLoadAsset<Level>(AssetCategoryRoot.Levels, name);

            await Resources.UnloadUnusedAssets();
            Debug.Log(("LevelLoaderService: Loaded level - " + level.name).Colored(Colors.LightBlue));

            _currentLevel = _factoryService.Instantiate<Level>(level);
            _currentLevel.name = level.name;

            //Level loaded, return screen to normal.
            _uiService.DarkenScreen(false).Subscribe();

            return _currentLevel;
        }

        /// <summary>
        /// Unloads level.
        /// </summary>
        /// <param name="level"> level name </param>
        /// <returns> Observable </returns>
        public async Task UnloadLevel(Level level)
        {
            if (!level)
                return;

            Debug.Log(("LevelLoaderService: Unloading level  - " + _currentLevel.name).Colored(Colors.LightBlue));
            Object.Destroy(level.gameObject);
            await _assetService.UnloadAsset(level.name, true);

            await Resources.UnloadUnusedAssets();
        }
    }
}