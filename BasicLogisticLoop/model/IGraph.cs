using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLogisticLoop.Model
{
    /// <summary>
    /// Abstract Class to define the methods needed to form a Graph.
    /// </summary>
    internal abstract class IGraph
    {
        /// <summary>
        /// Number of Nodes in the Graph.
        /// </summary>
        protected readonly int NodeNumber;

        /// <summary>
        /// Adds a directional edge with weight 1 to the graph by connecting two nodes.
        /// </summary>
        /// <param name="fromNodeID">Node the directional edge starts from.</param>
        /// <param name="toNodeID2">Node the directional edge connects to.</param>
        /// <exception cref="ArgumentOutOfRangeException">When NodeIDs are smaller 0 or greater the number of nodes.</exception>
        public void AddEdge(int fromNodeID, int toNodeID2)
        {
            AddEdge(fromNodeID, toNodeID2, 1);
        }

        /// <summary>
        /// Adds a directional edge with a weight to the graph by connecting two nodes.
        /// </summary>
        /// <param name="fromNodeID">Node the directional edge starts from.</param>
        /// <param name="toNodeID2">Node the directional edge connects to.</param>
        /// <param name="weight">Weight of the Edge.</param>
        /// <exception cref="ArgumentOutOfRangeException">When NodeIDs are smaller 0 or greater the number of nodes.</exception>
        /// <exception cref="ArgumentException">When the weight is smaller than 1.</exception>
        public abstract void AddEdge(int fromNodeID, int toNodeID2, int weight);

        //public abstract void AddNode();

        /// <summary>
        /// Calculates a List of NodeIDs that the given node has an edge towards.
        /// </summary>
        /// <param name="nodeID">ID of the node the adjacent ones are to be calculated.</param>
        /// <returns>List of adjacent nodes.</returns>
        /// <exception cref="ArgumentOutOfRangeException">When the nodeID is smaller 0 or greater the number of nodes.</exception>
        public abstract IEnumerable<int> GetAdjacentNodes(int nodeID);

        /// <summary>
        /// Determines the weight of an edge.
        /// </summary>
        /// <param name="fromNodeID">ID of the node the edge starts from.</param>
        /// <param name="toNodeID">ID of the node the edge connects to.</param>
        /// <returns>Weight of the edge.</returns>
        /// <exception cref="ArgumentOutOfRangeException">When NodeIDs are smaller 0 or greater the number of nodes.</exception>
        public abstract int GetEdgeWeight(int fromNodeID, int toNodeID);
    }
}
