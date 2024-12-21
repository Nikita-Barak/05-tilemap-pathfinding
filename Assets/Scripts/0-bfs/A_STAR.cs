using System.Collections.Generic;
using UnityEngine;

    public class A_STAR
    {
        // Function to compute the heuristic cost between two nodes
        public static float Heuristic<NodeType>(NodeType fromNode, NodeType toNode) {
            if (fromNode is Vector3Int from && toNode is Vector3Int to) {
                return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
            }
            throw new System.InvalidCastException("NodeType must be Vector3Int for this heuristic.");
        }

        // Main pathfinding function
        public static void FindPath<NodeType>(
        IGraph<NodeType> graph,
        NodeType startNode,
        NodeType endNode,
        List<NodeType> outputPath,
        int maxiterations = 1000
    ) {
        // Open set (nodes to explore), prioritized by f score
        var openQueue = new PriorityQueue<NodeType, float>();
        var openSet = new HashSet<NodeType>(); // Track nodes currently in the open queue
        openQueue.Enqueue(startNode, 0f);
        openSet.Add(startNode);

        // Closed set (nodes already processed)
        var closedSet = new HashSet<NodeType>();

        // Cost from start to each node
        var gScore = new Dictionary<NodeType, float> { [startNode] = 0f };

        // Estimated total cost (g + h) for each node
        var fScore = new Dictionary<NodeType, float> {
            [startNode] = Heuristic((Vector3Int)(object)startNode, (Vector3Int)(object)endNode)
        };

        // Path reconstruction map
        var previous = new Dictionary<NodeType, NodeType>();

        int iterations = 0;

        while (openQueue.Count > 0 && iterations < maxiterations) {
            iterations++;

            // Get the node with the lowest f score
            var current = openQueue.Dequeue();
            openSet.Remove(current); // Remove from the open set

            // If we reached the goal, reconstruct the path
            if (current.Equals(endNode)) {
                ReconstructPath(previous, current, outputPath);
                return;
            }

            closedSet.Add(current);

            // Explore neighbors
            foreach (var neighbor in graph.Neighbors(current)) {
                if (closedSet.Contains(neighbor)) continue;

                // Tentative g score
                float tentativeGScore = gScore[current] + graph.GetCost(current, neighbor);

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor]) {
                    // Update g and f scores
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + Heuristic(
                        (Vector3Int)(object)neighbor,
                        (Vector3Int)(object)endNode
                    );

                    // Update path reconstruction
                    previous[neighbor] = current;

                    // Add to open queue if not already in it
                    if (!openSet.Contains(neighbor)) {
                        openQueue.Enqueue(neighbor, fScore[neighbor]);
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        // If we exit the loop without finding a path, the outputPath remains empty
    }

        // Helper function to return the path directly
        public static List<NodeType> GetPath<NodeType>(
            IGraph<NodeType> graph,
            NodeType startNode,
            NodeType endNode,
            int maxiterations = 1000
        ) {
            List<NodeType> path = new List<NodeType>();
            FindPath(graph, startNode, endNode, path, maxiterations);
            return path;
        }
        
        // Reconstruct the path from the previous map
        private static void ReconstructPath<NodeType>(
            Dictionary<NodeType, NodeType> previous,
            NodeType current,
            List<NodeType> path
        ) {
            path.Add(current);
            while (previous.ContainsKey(current)) {
                current = previous[current];
                path.Add(current);
            }
            path.Reverse();
        }
        
    }