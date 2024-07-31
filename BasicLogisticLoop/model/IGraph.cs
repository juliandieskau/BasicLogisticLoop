using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLogisticLoop.model
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
        public abstract void AddEdge(int fromNodeID, int toNodeID2, int weight);

        //public abstract void AddNode();

        /// <summary>
        /// Determines the adjacent nodes a node connects to.
        /// </summary>
        /// <param name="nodeID">Node the connection starts from.</param>
        /// <returns>List of nodes that are adjacent.</returns>
        public abstract IEnumerable<int> GetAdjacentNodes(int nodeID);

        /// <summary>
        /// Determines the weight of an edge.
        /// </summary>
        /// <param name="fromNodeID">ID of the node the edge starts from.</param>
        /// <param name="toNodeID">ID of the node the edge connects to.</param>
        /// <returns></returns>
        public abstract int GetEdgeWeight(int fromNodeID, int toNodeID);
    }
}
