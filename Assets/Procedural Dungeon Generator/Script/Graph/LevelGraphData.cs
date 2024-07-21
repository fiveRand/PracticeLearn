using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralDungeonGeneration
{
    [CreateAssetMenu(fileName = "LevelGraph", menuName = "Level graph")]
    public class LevelGraphData : ScriptableObject
    {
        public List<Node> nodes = new List<Node>();
        public List<Edge> edges = new List<Edge>();
        public Vector2 panOffset = Vector2.zero;
        public float zoom = 1;
        public static bool HasChanged { get; set; }



    }
}

