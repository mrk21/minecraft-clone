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
        public ReactiveProperty<World> currentWorld;
        
        public Player()
        {
            isOperable = new ReactiveProperty<bool>(false);
            position = new ReactiveProperty<Vector3>();
            rotation = new ReactiveProperty<Quaternion>();
            headRotation = new ReactiveProperty<Quaternion>();
            gaze = new ReactiveProperty<Ray>();
            operationRange = new ReactiveProperty<float>(10f);
            currentWorld = new ReactiveProperty<World>();
        }

        public void JoinWorld(World world)
        {
            position.Value = new Vector3(60, ChunkFactory.MaxHeight, 60);
            isOperable.Value = true;
            currentWorld.Value = world;
        }

        public Chunk CurrentChunk()
        {
            if (!currentWorld.HasValue) throw new System.Exception("User does not join world!");
            return currentWorld.Value[position.Value];
        }

        public BaseBlock CurrentBlock()
        {
            if (!currentWorld.HasValue) throw new System.Exception("User does not join world!");
            return currentWorld.Value.Blocks[position.Value + 1.5f * Vector3.down];
        }
    }
}
