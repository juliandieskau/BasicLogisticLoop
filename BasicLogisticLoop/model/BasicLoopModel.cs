using BasicLogisticLoop.Presenter.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLogisticLoop.Model
{
    /// <summary>
    /// Implementation of the <see cref="ILogisticModel"/> that creates a Layout based on the "Logistikloop Basic" assignment. 
    /// Contains the logic and model that the LogisticLoop Basic operates after.
    /// </summary>
    internal class BasicLoopModel : ILogisticModel
    {
        // Objects of the model
        /// <summary>
        /// Graph Object that holds the nodes by ID and manages their adjacency to other nodes.
        /// </summary>
        private IGraph Graph;

        /// <summary>
        /// List of GraphNodes that holds the state of the nodes and their data.
        /// </summary>
        private List<GraphNode> GraphNodes;

        /// <summary>
        /// List of ViewNodes that holds warehouse nodes which are needed for the LogisticLoop Basic but aren't included in the GraphNodes.
        /// </summary>
        private List<ViewNode> WarehouseNodes;

        // Values to use for constructing the model
        int GraphNodeNumber = 15;

        /// <summary>
        /// Constructor that sets up all the model data the BasicLogisticsLoop needs.
        /// Fills the Graph and GraphNodes with the Model-Layout that the BasicLoopModel.
        /// Nodes are ordered and linked accordingly by their assigned ID, IDs start at 0 and increment by 1 per new node added.
        /// </summary>
        public BasicLoopModel()
        {
            ConstructGraph();
            ConstructGraphNodes();
            ConstructWarehouseNodes();
        }
        public List<ViewNode> GetViewNodes()
        {
            throw new NotImplementedException();
        }

        public string CommissionContainer(int nodeID, Container container)
        {
            throw new NotImplementedException();
        }

        public string RetrieveContainer(int nodeID, Container container)
        {
            throw new NotImplementedException();
        }

        public string Step()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stores the container on given node into the warehouse, if the node is a storage node.
        /// </summary>
        /// <param name="nodeID"></param>
        /// <exception cref=""
        private void StoreContainer(int nodeID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the GraphNode corresponding to the given nodeID.
        /// </summary>
        /// <param name="nodeID">ID to search the not for</param>
        /// <returns>GraphNode if found, null if not.</returns>
        private GraphNode GetGraphNode(int nodeID)
        {
            // If no matching GraphNode is found Find() returns the default value of GraphNode
            // which is a class and thus the default is null.
            return GraphNodes.Find(node => node.NodeID == nodeID);
        }

        private void ConstructGraph()
        {
            Graph = new AdjacencyMatrixGraph(GraphNodeNumber);
            Graph.AddEdge
        }

        private void ConstructGraphNodes()
        {
            GraphNodes = new List<GraphNode>();
            GraphNodes.Add(new GraphNode());
        }

        private void ConstructWarehouseNodes()
        {
            WarehouseNodes = new List<ViewNode>();
        }
    }
}
