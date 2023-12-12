using System;
using UnityEngine;

namespace ProceduralTerrainAPI
{
    public class MapGenerator : MonoBehaviour
    {
        private enum DrawMode
        {
            NoiseMap,
            ColorMap,
            Mesh
        }

        [SerializeField] private DrawMode drawMode;

        private const int mapChunkSize = 241;
        [Range(0, 6)]
        public int levelOfDetail;

        [SerializeField] private float noiseScale;

        [SerializeField] private int octaves;
        [Range(0, 1)]
        [SerializeField] private float persistance;
        [SerializeField] private float lacunarity;

        [SerializeField] private int seed;
        [SerializeField] private Vector2 offset;

        [SerializeField] private float meshHeightMultiplier;
        [SerializeField] private AnimationCurve meshHeightCurve;

        public bool autoUpdate;

        [SerializeField] private TerrainType[] regions;

        public void GenerateMap()
        {
            float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

            Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
            
            for(int y = 0; y < mapChunkSize; y++)
            {
                for(int x = 0; x < mapChunkSize; x++)
                {
                    float currentHeight = noiseMap[x, y];
                    for(int i = 0; i < regions.Length; i++)
                    {
                        if(currentHeight <= regions[i].height)
                        {
                            colorMap[y * mapChunkSize + x] = regions[i].color;
                            break;
                        }
                    }
                }
            }

            MapDisplay mapDisplay = GetComponent<MapDisplay>();
            switch(drawMode)
            {
                case DrawMode.NoiseMap:
                    mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
                    break;
                case DrawMode.ColorMap:
                    mapDisplay.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
                    break;
                case DrawMode.Mesh:
                    mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail),
                        TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void GenerateRandomMap()
        {
            seed = UnityEngine.Random.Range(0, 999999999);
            float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

            Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
            for(int y = 0; y < mapChunkSize; y++)
            {
                for(int x = 0; x < mapChunkSize; x++)
                {
                    float currentHeight = noiseMap[x, y];
                    for(int i = 0; i < regions.Length; i++)
                    {
                        if(currentHeight <= regions[i].height)
                        {
                            colorMap[y * mapChunkSize + x] = regions[i].color;
                            break;
                        }
                    }
                }
            }

            MapDisplay mapDisplay = GetComponent<MapDisplay>();
            if(drawMode == DrawMode.NoiseMap)
            {
                mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
            }
            else if(drawMode == DrawMode.ColorMap)
            {
                mapDisplay.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
            }
            else if(drawMode == DrawMode.Mesh)
            {
                mapDisplay.DrawMesh(
                    MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail),
                    TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
            }
        }

        private void OnValidate()
        {
            if(lacunarity < 1)
            {
                lacunarity = 1;
            }

            if(octaves < 0)
            {
                octaves = 0;
            }
        }
    }
}
