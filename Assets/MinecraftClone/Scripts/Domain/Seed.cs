using UniRx;

namespace MinecraftClone.Domain
{
    public class Seed
    {
        public ReactiveProperty<int> Base { get; } = new ReactiveProperty<int>();
        public IReadOnlyReactiveProperty<int> Dimension { get; }
        public IReadOnlyReactiveProperty<int> Temperature { get; }
        public IReadOnlyReactiveProperty<int> Humidity { get; }

        private ReactiveProperty<int> Dimension_ { get; } = new ReactiveProperty<int>();
        private ReactiveProperty<int> Temperature_ { get; } = new ReactiveProperty<int>();
        private ReactiveProperty<int> Humidity_ { get; } = new ReactiveProperty<int>();

        public Seed()
        {
            Dimension = Dimension_.ToReadOnlyReactiveProperty();
            Temperature = Temperature_.ToReadOnlyReactiveProperty();
            Humidity = Humidity_.ToReadOnlyReactiveProperty();

            Base.Subscribe(Init);
            Base.Value = GetHashCode();
        }

        public Seed(int baseSeed)
        {
            Dimension = Dimension_.ToReadOnlyReactiveProperty();
            Temperature = Temperature_.ToReadOnlyReactiveProperty();
            Humidity = Humidity_.ToReadOnlyReactiveProperty();

            Base.Subscribe(Init);
            Base.Value = baseSeed;
        }

        private void Init(int baseSeed)
        {
            var rand = new System.Random(baseSeed);
            var max = (int)System.Math.Pow(2, 16) - 1;
            var seeds = new int[100];

            for (var i = 0; i < seeds.Length; i++)
            {
                seeds[i] = rand.Next(0, max);
            }
            Dimension_.Value = seeds[0];
            Temperature_.Value = seeds[10];
            Humidity_.Value = seeds[11];
        }
    }
}