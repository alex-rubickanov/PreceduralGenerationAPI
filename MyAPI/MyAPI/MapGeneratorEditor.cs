using UnityEngine;
using UnityEditor;

namespace ProceduralTerrainAPI
{
    [CustomEditor(typeof(MapGenerator))]
    public class MapGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            MapGenerator mapGen = (MapGenerator)target;

            if(DrawDefaultInspector())
            {
                if(mapGen.autoUpdate)
                {
                    mapGen.GenerateMap();
                }
            }

            if(GUILayout.Button("GENERATE"))
            {
                mapGen.GenerateMap();
            }
            if(GUILayout.Button("GENERATE RANDOM"))
            {
                mapGen.GenerateRandomMap();
            }
        }
    }
}
