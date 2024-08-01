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
        /// Needs to be added to GetViewNodes after GraphNodes are transformed into ViewNodes to complete information needed in view.
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

        /// <summary>
        /// Initializes Graph variable with an AdjacencyMatrixGraph filled with the edges defined in the LogisticLoop Basic.
        /// </summary>
        private void ConstructGraph()
        {
            // initialize edges as array of 3-tuples with {fromID, toID, weight}
            int[,] edges = new int[,] {
                {1,0,1},
                {0,1,1},
                {1,2,1},
                {2,3,1},
                {3,4,1},
                {4,5,1},
                {5,6,1},
                {6,7,1},
                {7,8,1},
                {8,9,1},
                {9,10,1},
                {10,11,1},
                {11,12,1},
                {12,1,1},
                {6,13,1},
                {14,8,1}
            };
            Graph = new AdjacencyMatrixGraph(GraphNodeNumber, edges);
        }

        /// <summary>
        /// Initializes GraphNodes variable with a List of GraphNodes filled with the Nodes defined in the LogisticLoop Basic.
        /// Adds their NodeType, nodeID and raster coordinates.
        /// </summary>
        private void ConstructGraphNodes()
        {
            GraphNodes = new List<GraphNode>
            {
                new GraphNode(type: NodeType.Commissioning, id: 0,  coords: (2, 4)),
                new GraphNode(type: NodeType.Conveyor,      id: 1,  coords: (2, 3)),
                new GraphNode(type: NodeType.Conveyor,      id: 2,  coords: (1, 3)),
                new GraphNode(type: NodeType.Conveyor,      id: 3,  coords: (0, 3)),
                new GraphNode(type: NodeType.Conveyor,      id: 4,  coords: (0, 2)),
                new GraphNode(type: NodeType.Conveyor,      id: 5,  coords: (0, 1)),
                new GraphNode(type: NodeType.Conveyor,      id: 6,  coords: (1, 1)),
                new GraphNode(type: NodeType.Conveyor,      id: 7,  coords: (2, 1)),
                new GraphNode(type: NodeType.Conveyor,      id: 8,  coords: (3, 1)),
                new GraphNode(type: NodeType.Conveyor,      id: 9,  coords: (4, 1)),
                new GraphNode(type: NodeType.Conveyor,      id: 10, coords: (4, 2)),
                new GraphNode(type: NodeType.Conveyor,      id: 11, coords: (4, 3)),
                new GraphNode(type: NodeType.Conveyor,      id: 12, coords: (3, 3)),
                new GraphNode(type: NodeType.Storage,       id: 13, coords: (1, 0)),
                new GraphNode(type: NodeType.Retrieval,     id: 14, coords: (3, 0)),
            };
        }

        /// <summary>
        /// Initializes WarehouseNodes variable with a List of ViewNodes filled with the Nodes defined in the LogisticLoop Basic.
        /// </summary>
        private void ConstructWarehouseNodes()
        {
            WarehouseNodes = new List<ViewNode>
            {
                new ViewNode(type: NodeType.Warehouse, coords: (2, 0),
                    nodeID: 15, followingNodes: new[] { 14 })
            };
        }
    }
}
