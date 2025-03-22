using UnityEngine;
using UnityEngine.UI;
using MinecraftClone.Domain;
using MinecraftClone.Domain.Terrain;
using MinecraftClone.Domain.Block;

namespace MinecraftClone.Application.WorldScene
{
    public class DebugScreenView : MonoBehaviour
    {
        [SerializeField] private Text screen = null;

        public void SetEnabled(bool enabled)
        {
            gameObject.SetActive(enabled);
        }

        public void Display(
            Seed currentSeed,
            Chunk currentChunk,
            Block currentBlock,
            Vector3 currentPosition
        )
        {
            var value = "";
            value += $"CurrentSeed: {currentSeed.Base}\n";
            value += $"CurrentCunk: {currentChunk.Address}\n";
            value += $"CurrentBlock: {currentBlock}\n";
            value += $"CurrentPosition: {currentPosition}\n";
            screen.text = value;
        }
    }
}
