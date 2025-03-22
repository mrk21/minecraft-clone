using UnityEngine;
using UniRx;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Terrain;

namespace MinecraftClone.Domain.Store
{
    public class Player
    {
        public ReactiveProperty<bool> IsOperable { get; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<Vector3> Position { get; } = new ReactiveProperty<Vector3>();
        public ReactiveProperty<Quaternion> Rotation { get; } = new ReactiveProperty<Quaternion>();
        public ReactiveProperty<Quaternion> HeadRotation { get; } = new ReactiveProperty<Quaternion>();
        public ReactiveProperty<Ray> Gaze { get; } = new ReactiveProperty<Ray>();
        public ReactiveProperty<float> OperationRange { get; } = new ReactiveProperty<float>(10f);
        public IReadOnlyReactiveProperty<Dimension> CurrentDimension { get; }
        public IReadOnlyReactiveProperty<Chunk> CurrentChunk { get; }
        public IReadOnlyReactiveProperty<Block.Block> CurrentBlock { get; }

        public Subject<Unit> OnMoveGaze { get; } = new Subject<Unit>();
        public Subject<Unit> OnMoveToForward { get; } = new Subject<Unit>();
        public Subject<Unit> OnMoveToBack { get; } = new Subject<Unit>();
        public Subject<Unit> OnMoveToLeft { get; } = new Subject<Unit>();
        public Subject<Unit> OnMoveToRight { get; } = new Subject<Unit>();
        public Subject<Unit> OnJump { get; } = new Subject<Unit>();
        public Subject<Unit> OnPut { get; } = new Subject<Unit>();
        public Subject<Unit> OnRemove { get; } = new Subject<Unit>();

        private ReactiveProperty<Dimension> CurrentDimension_ { get; } = new ReactiveProperty<Dimension>();
        private ReactiveProperty<Chunk> CurrentChunk_ { get; } = new ReactiveProperty<Chunk>();
        private ReactiveProperty<Block.Block> CurrentBlock_ { get; } = new ReactiveProperty<Block.Block>();

        public Player()
        {
            CurrentDimension = CurrentDimension_.ToReadOnlyReactiveProperty();
            CurrentChunk = CurrentChunk_.ToReadOnlyReactiveProperty();
            CurrentBlock = CurrentBlock_.ToReadOnlyReactiveProperty();

            Observable.Merge(
                Position.AsUnitObservable(),
                CurrentDimension.AsUnitObservable()
            )
                .Where(_ => Position.Value != null && CurrentDimension.Value != null)
                .Subscribe(_ =>
                {
                    CurrentChunk_.Value = CurrentDimension.Value[Position.Value];
                    CurrentBlock_.Value = CurrentDimension.Value.Blocks[Position.Value + 1.5f * Vector3.down];
                });
        }

        public void JoinDimension(Dimension dimension)
        {
            Position.Value = new Vector3(60, ChunkFactory.MaxHeight, 60);
            Rotation.Value = Quaternion.identity;
            HeadRotation.Value = Quaternion.AngleAxis(90.0f, Vector3.up);
            IsOperable.Value = true;
            CurrentDimension_.Value = dimension;
        }
    }
}
