using UniRx;
using Zenject;

namespace Core.Services
{
	public class CoreFrameworkInstaller : MonoInstaller<CoreFrameworkInstaller>
	{
		private Subject<Unit> onGameStart = new Subject<Unit>();

		public override void InstallBindings()
		{
			Container.BindInstance(onGameStart).AsSingle();
		}

		public override void Start()
		{
			//Start game
			onGameStart.OnNext(new Unit());
			onGameStart.OnCompleted();
		}
	}
}