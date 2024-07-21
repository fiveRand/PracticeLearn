using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ProceduralDungeonGeneration
{
    public class Edge : ScriptableObject
    {
        public Node start;
        public Node end;

        public Edge(Node start_, Node end_)
        {
            start = start_;
            end = end_;
        }
    }
}
