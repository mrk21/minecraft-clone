using UniRx;
using MinecraftClone.Domain.Terrain;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Store
{
    public class World : IEntity<ReactiveProperty<string>>
    {
        public ReactiveProperty<string> Id { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<string> Name { get; } = new ReactiveProperty<string>();
        public ReactiveProperty<Seed> Seed { get; } = new ReactiveProperty<Seed>();
        public IReadOnlyReactiveProperty<PlaySetting> PlaySetting { get; }
        public IReadOnlyReactiveProperty<Player> Player { get; }
        public IReadOnlyReactiveProperty<Dimension> CurrentDimension { get; }

        private ReactiveProperty<PlaySetting> PlaySetting_ { get; } = new ReactiveProperty<PlaySetting>();
        private ReactiveProperty<Player> Player_ { get; } = new ReactiveProperty<Player>();
        private ReactiveProperty<Dimension> CurrentDimension_ { get; } = new ReactiveProperty<Dimension>();

        public World(Seed seed, string name)
        {
            PlaySetting = PlaySetting_.ToReadOnlyReactiveProperty();
            Player = Player_.ToReadOnlyReactiveProperty();
            CurrentDimension = CurrentDimension_.ToReadOnlyReactiveProperty();

            Id.Value = GetHashCode().ToString();
            Name.Value = name;
            Seed.Value = seed;
        }

        public void Join()
        {
            if (PlaySetting_.Value == null) PlaySetting_.Value = new PlaySetting();
            if (Player_.Value == null) Player_.Value = new Player();
            if (CurrentDimension_.Value == null) CurrentDimension_.Value = new Dimension(Seed.Value);
        }
    }
}