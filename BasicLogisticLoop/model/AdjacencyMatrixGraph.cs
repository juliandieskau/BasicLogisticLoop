using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLogisticLoop.Model
{
    /// <summary>
    /// Representation of a graph by using a adjacency matrix to keep track of edges and their weights.
    /// 
    /// Adapted from:
    /// https://dev.to/russianguycoding/how-to-represent-a-graph-in-c-4cmo (31.07.2024 13:00)
    /// </summary>
    internal class AdjacencyMatrixGraph : IGraph
    {
        int[,] Matrix;

        /// <summary>
        /// Constructs the AdjacencyMatrixGraph.
        /// </summary>
        /// <param name="nodeNumber">Number of nodes the graph will consist of.</param>
        public AdjacencyMatrixGraph(int nodeNumber)
        {
            try
            {
                GenerateEmptyMatrix(nodeNumber);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw;
            }
        }

        public override void AddEdge(int fromNodeID, int toNodeID, int weight)
        {
            // Test validity of arguments.
            if (fromNodeID < 0 || toNodeID < 0 || fromNodeID >= this.NodeNumber || toNodeID >= this.NodeNumber)
            {
                throw new ArgumentOutOfRangeException("NodeIDs are out of bounds.");
            } 

            if (weight < 1)
            {
                throw new ArgumentException("Weight of Edge cannot be less than 1.");
            } 

            // Add edge.
            this.Matrix[fromNodeID, toNodeID] = weight;
        }

        public override IEnumerable<int> GetAdjacentNodes(int nodeID)
        {
            if (nodeID < 0 || nodeID >= this.NodeNumber)
            {
                throw new ArgumentOutOfRangeException("This nodeID is not in the valid range.");
            }
                

            List<int> adjacentNodes = new List<int>();
            for (int i = 0; i < this.NodeNumber; i++)
            {
                if (this.Matrix[nodeID, i] > 0)
                {
                    adjacentNodes.Add(i);
                }
            }
            return adjacentNodes;
        }

        public override int GetEdgeWeight(int fromNodeID, int toNodeID)
        {
            // Test validity of arguments.
            if (fromNodeID < 0 || toNodeID < 0 || fromNodeID >= this.NodeNumber || toNodeID >= this.NodeNumber)
            {
                throw new ArgumentOutOfRangeException("NodeIDs are out of bounds.");
            }

            return this.Matrix[fromNodeID, toNodeID];
        }

        /// <summary>
        /// Generates a matrix of given size filled with zeros.
        /// </summary>
        /// <param name="nodeNumber">Size n of the n x n Matrix to generate.</param>
        /// <exception cref="ArgumentOutOfRangeException">When nodeNumber is smaller 0.</exception>
        private void GenerateEmptyMatrix(int nodeNumber)
        {
            if (nodeNumber < 0)
            {
                throw new ArgumentOutOfRangeException("Cannot generate Matrix with negative sizes.");
            }

            this.Matrix = new int[nodeNumber, nodeNumber];
            for (int i = 0; i < nodeNumber; i++)
            {
                for (int j = 0; j < nodeNumber; j++)
                {
                    Matrix[i, j] = 0;
                }
            }
        }
    }
}
