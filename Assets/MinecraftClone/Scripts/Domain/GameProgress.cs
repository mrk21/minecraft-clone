using UniRx;
using UnityEngine.SceneManagement;
using MinecraftClone.Infrastructure;
using UnityEngine;
using MinecraftClone.Domain.Terrain;

namespace MinecraftClone.Domain
{
    public class GameProgress
    {
        public ReactiveProperty<bool> worldIsActivated;
        public ReactiveProperty<World> currentWorld;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            var _ = Singleton<GameProgress>.Instance;
        }

        public GameProgress()
        {
            worldIsActivated = new ReactiveProperty<bool>(false);
            currentWorld = new ReactiveProperty<World>();

            SceneManager.activeSceneChanged += OnActiveSceneChanged_;
        }

        public void MakeNewWorld(Seed seed)
        {
            currentWorld.Value = new World(seed);
        }

        private void OnActiveSceneChanged_(Scene prev, Scene next)
        {
            if (prev.name == "World")
            {
                worldIsActivated.Value = false;
            }
            else if (next.name == "World")
            {
                worldIsActivated.Value = true;
            }
        }
    }
}
