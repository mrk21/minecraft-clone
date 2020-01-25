using MinecraftClone.Domain.Terrain;
using MinecraftClone.Infrastructure;
using UniRx;

namespace MinecraftClone.Domain.Store
{
    public class World : IEntity<string>
    {
        public ReactiveProperty<string> id;
        public ReactiveProperty<string> name;
        public ReactiveProperty<Seed> seed;
        public ReactiveProperty<PlaySetting> playSetting;
        public ReactiveProperty<Player> player;
        public ReactiveProperty<Dimension> currentDimension;

        public string Id
        {
            get { return id.Value; }
        }

        public Player Player
        {
            get { return player.Value; }
        }

        public PlaySetting PlaySetting
        {
            get { return playSetting.Value; }
        }

        public World(Seed seed, string name)
        {
            id = new ReactiveProperty<string>(GetHashCode().ToString());
            this.name = new ReactiveProperty<string>(name);
            this.seed = new ReactiveProperty<Seed>(seed);

            playSetting = new ReactiveProperty<PlaySetting>();
            player = new ReactiveProperty<Player>();
            currentDimension = new ReactiveProperty<Dimension>();
        }

        public void Join()
        {
            if (playSetting.Value == null) playSetting.Value = new PlaySetting();
            if (player.Value == null) player.Value = new Player();
            if (currentDimension.Value == null) currentDimension.Value = new Dimension(seed.Value);
        }
    }
}