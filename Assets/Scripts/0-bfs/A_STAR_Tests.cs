using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

    public class A_STAR_Tests
    {
       private MockTilemapGraph graph;

    [SetUp]
    public void Setup() {
        // Initialize a mock tilemap graph for testing
        graph = new MockTilemapGraph();
        graph.SetTile(new Vector3Int(0, 0, 0), 1);  // Grass
        graph.SetTile(new Vector3Int(1, 0, 0), 2);  // Hills
        graph.SetTile(new Vector3Int(2, 0, 0), 5);  // Swamp
        graph.SetTile(new Vector3Int(1, 1, 0), 1);  // Grass
        graph.SetImpassable(new Vector3Int(1, 2, 0)); // Blocked tile
    }

    [Test]
    public void TestShortestPath() {
        // Arrange
        var startNode = new Vector3Int(0, 0, 0);
        var endNode = new Vector3Int(2, 0, 0);

        // Act
        List<Vector3Int> path = A_STAR.GetPath(graph, startNode, endNode, 100);

        // Assert
        Assert.AreEqual(3, path.Count);  // Path length should include start, intermediate, and end
        Assert.AreEqual(new Vector3Int(0, 0, 0), path[0]);
        Assert.AreEqual(new Vector3Int(1, 0, 0), path[1]);
        Assert.AreEqual(new Vector3Int(2, 0, 0), path[2]);
    }

    [Test]
    public void TestNoPathExists() {
        // Arrange
        var startNode = new Vector3Int(0, 0, 0);
        var endNode = new Vector3Int(1, 2, 0); // Impassable

        // Act
        List<Vector3Int> path = A_STAR.GetPath(graph, startNode, endNode, 100);

        // Assert
        Assert.AreEqual(0, path.Count); // No path should exist
    }

    [Test]
    public void TestStartEqualsGoal() {
        // Arrange
        var startNode = new Vector3Int(0, 0, 0);
        var endNode = new Vector3Int(0, 0, 0);

        // Act
        List<Vector3Int> path = A_STAR.GetPath(graph, startNode, endNode, 100);

        // Assert
        Assert.AreEqual(1, path.Count); // Path should only contain the start node
        Assert.AreEqual(new Vector3Int(0, 0, 0), path[0]);
    }

    [Test]
    public void TestPathWithVaryingCosts() {
        // Arrange
        var startNode = new Vector3Int(0, 0, 0);
        var endNode = new Vector3Int(1, 1, 0);

        // Act
        List<Vector3Int> path = A_STAR.GetPath(graph, startNode, endNode, 100);

        // Assert
        Assert.AreEqual(3, path.Count); // Verify the correct path length
        Assert.AreEqual(new Vector3Int(0, 0, 0), path[0]);
        Assert.AreEqual(new Vector3Int(1, 0, 0), path[1]); // Go through lower-cost tile
        Assert.AreEqual(new Vector3Int(1, 1, 0), path[2]);
    }
    }

    public class MockTilemapGraph : IGraph<Vector3Int> {
        private Dictionary<Vector3Int, float> tileCosts = new Dictionary<Vector3Int, float>();
        private HashSet<Vector3Int> impassableTiles = new HashSet<Vector3Int>();

        public void SetTile(Vector3Int position, float cost) {
            tileCosts[position] = cost;
        }

        public void SetImpassable(Vector3Int position) {
            impassableTiles.Add(position);
        }

        public IEnumerable<Vector3Int> Neighbors(Vector3Int node) {
            Vector3Int[] directions = {
                new Vector3Int(0, 1, 0),
                new Vector3Int(0, -1, 0),
                new Vector3Int(1, 0, 0),
                new Vector3Int(-1, 0, 0)
            };

            foreach (var direction in directions) {
                var neighbor = node + direction;
                if (tileCosts.ContainsKey(neighbor) && !impassableTiles.Contains(neighbor)) {
                    yield return neighbor;
                }
            }
        }

        public float GetCost(Vector3Int fromNode, Vector3Int toNode) {
            return tileCosts.TryGetValue(toNode, out float cost) ? cost : float.MaxValue;
        }
    }
