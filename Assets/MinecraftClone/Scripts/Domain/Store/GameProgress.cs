using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Store
{
    public class GameProgress
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            var _ = Singleton<GameProgress>.Instance;
        }

        public static GameProgress Get()
        {
            return Singleton<GameProgress>.Instance;
        }

        public ReactiveProperty<bool> worldIsActivated;
        public ReactiveProperty<World> currentWorld;
        public ReactiveDictionary<string, World> worldList;

        public GameProgress()
        {
            worldIsActivated = new ReactiveProperty<bool>(false);
            worldList = new ReactiveDictionary<string, World>();
            currentWorld = new ReactiveProperty<World>();

            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        public World CurrentWorld
        {
            get { return currentWorld.Value; }
        }

        public World MakeWorld(Seed seed, string name = null)
        {
            var world = new World(
                seed: seed,
                name: name ?? $"New World {(worldList.Count + 1).ToString()}"
            );
            worldList.Add(world.Id, world);
            return world;
        }

        public void JoinWorld(string id)
        {
            var world = worldList[id];
            world.Join();
            currentWorld.Value = world;
        }

        private void OnActiveSceneChanged(Scene prev, Scene next)
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
