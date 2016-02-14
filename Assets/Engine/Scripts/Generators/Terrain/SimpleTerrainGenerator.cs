using Assets.Engine.Plugins.CoherentNoise.Scripts.Generation;
using Assets.Engine.Scripts.Core.Blocks;
using Assets.Engine.Scripts.Core.Chunks;
using UnityEngine;

namespace Assets.Engine.Scripts.Generators.Terrain
{
    /// <summary>
    /// 	Produces a simple Minecraft-like terrain
    /// </summary>
    public class SimpleTerrainGenerator : AChunkGenerator
    {
        private readonly ValueNoise m_noise = new ValueNoise (0);
	
        #region IChunkGenerator implementation

        public override void Generate (Chunk chunk)
        {
            int index = 0;
			for (int y = 0; y < EngineSettings.ChunkConfig.SizeYTotal; y++)
			{
	            for (int z = 0; z < EngineSettings.ChunkConfig.Size; z++)
	            {
					int wz = z + (chunk.Pos.Z << EngineSettings.ChunkConfig.LogSize);

                    for (int x = 0; x < EngineSettings.ChunkConfig.Size; x++, index++)
                    {
						int wx = x + (chunk.Pos.X << EngineSettings.ChunkConfig.LogSize);

                        bool currentPoint = Eval(wx, y, wz);
                        bool up = Eval(wx, y + 1, wz);
					
                        if (currentPoint)
						{
                            if (!up)
							{
								chunk[index] = new BlockData(BlockType.Grass);
                                // Grass block was placed, lets place a tree blueprint here
                                //if (y < EngineSettings.ChunkConfig.SizeYTotal)
                                //{
                                //    //int height =
                                //}
                            }
							else
							{
                                if (y > 50)
									chunk[index] = new BlockData(BlockType.Dirt);
                                else
									chunk[index] = new BlockData(BlockType.Stone);
                            }
                        }
						else
						{
							chunk[index] = BlockData.Air;
                        }
                    }
                }
            }
        }
        public override void OnCalculateProperties(int x, int y, int z, ref BlockData data)
        {   
        }

        #endregion

        private const float Coef = 0.015f;

        private bool Eval (int x, int y, int z)
        {
            float density = m_noise.GetValue(new Vector3(x, y, z) * Coef);
            density -= ((y - 64) * Coef);
            return density > 0f;
        }
    }
}