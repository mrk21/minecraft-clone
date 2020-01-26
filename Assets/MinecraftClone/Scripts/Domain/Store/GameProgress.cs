using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Store
{
    public class GameProgress
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static GameProgress Get()
        {
            return Singleton<GameProgress>.Instance;
        }

        public ReactiveDictionary<string, World> WorldList { get; } = new ReactiveDictionary<string, World>();
        public IReadOnlyReactiveProperty<World> CurrentWorld { get; }
        public IReadOnlyReactiveProperty<bool> WorldIsActivated { get; }

        private ReactiveProperty<World> CurrentWorld_ { get; } = new ReactiveProperty<World>();
        private ReactiveProperty<bool> WorldIsActivated_ { get; } = new ReactiveProperty<bool>(false);

        public GameProgress()
        {
            CurrentWorld = CurrentWorld_.ToReadOnlyReactiveProperty();
            WorldIsActivated = WorldIsActivated_.ToReadOnlyReactiveProperty();
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        public World MakeWorld(Seed seed, string name = null)
        {
            var world = new World(
                seed: seed,
                name: name ?? $"New World {(WorldList.Count + 1).ToString()}"
            );
            WorldList.Add(world.Id.Value, world);
            return world;
        }

        public void JoinWorld(string id)
        {
            var world = WorldList[id];
            world.Join();
            CurrentWorld_.Value = world;
        }

        private void OnActiveSceneChanged(Scene prev, Scene next)
        {
            if (prev.name == "World")
            {
                WorldIsActivated_.Value = false;
            }
            else if (next.name == "World")
            {
                WorldIsActivated_.Value = true;
            }
        }
    }
}
