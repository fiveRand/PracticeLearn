using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralDungeonGeneration
{
    [CreateAssetMenu(fileName = "LevelGraph", menuName = "Level graph")]
    public class LevelGraphData : ScriptableObject
    {
        public Node spawnNode;
        public Node exitNode;
        public List<Node> nodes = new List<Node>();
        public Vector2 panOffset = Vector2.zero;
        public float zoom = 1;
        public static bool HasChanged { get; set; }


        public void AddDirectedEdge(Node from,Node to)
        {
            from.edges.Add(to);
        }

        public void AddUndirectedEdge(Node from, Node to)
        {
            from.edges.Add(to);
            to.edges.Add(from);
        }


        public void Clear()
        {
            spawnNode = null;
            exitNode = null;
            nodes.Clear();
        }
    }
}

