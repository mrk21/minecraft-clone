using UnityEngine;
using UniRx;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Terrain;

namespace MinecraftClone.Domain.Store
{
    public class Player
    {
        public ReactiveProperty<bool> isOperable;
        public ReactiveProperty<Vector3> position;
        public ReactiveProperty<Quaternion> rotation;
        public ReactiveProperty<Quaternion> headRotation;
        public ReactiveProperty<Ray> gaze;
        public ReactiveProperty<float> operationRange;
        public ReactiveProperty<Dimension> currentDimension;
        
        public Player()
        {
            isOperable = new ReactiveProperty<bool>(false);
            position = new ReactiveProperty<Vector3>();
            rotation = new ReactiveProperty<Quaternion>();
            headRotation = new ReactiveProperty<Quaternion>();
            gaze = new ReactiveProperty<Ray>();
            operationRange = new ReactiveProperty<float>(10f);
            currentDimension = new ReactiveProperty<Dimension>();
        }

        public void JoinDimension(Dimension dimension)
        {
            position.Value = new Vector3(60, ChunkFactory.MaxHeight, 60);
            isOperable.Value = true;
            currentDimension.Value = dimension;
        }

        public Chunk CurrentChunk()
        {
            if (!currentDimension.HasValue) throw new System.Exception("User does not join dimension!");
            return currentDimension.Value[position.Value];
        }

        public BaseBlock CurrentBlock()
        {
            if (!currentDimension.HasValue) throw new System.Exception("User does not join dimension!");
            return currentDimension.Value.Blocks[position.Value + 1.5f * Vector3.down];
        }
    }
}
