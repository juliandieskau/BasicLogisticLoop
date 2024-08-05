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
    internal class BasicLoopLogisticModel : ILogisticModel
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
        public BasicLoopLogisticModel()
        {
            ConstructGraph();
            ConstructGraphNodes();
            ConstructWarehouseNodes();
        }

        /// <summary>
        /// Transforms the models nodes into a List of value-type ViewNodes that can be used by the view the present the models state without having access to the logic of the model.
        /// Contains all the Conveyor, Retrieval, Storage, Commissioning and Warehouse nodes.
        /// </summary>
        /// <returns>List of ViewNodes that represent the current model state.</returns>
        public List<ViewNode> GetViewNodes()
        {
            List<ViewNode> transformedNodes = new List<ViewNode>();
            // Add all the graph nodes
            foreach (var node in GraphNodes)
            {
                // Get array of nodes that the node has an edge towards
                int[] adjacentNodes = new int[1];

                if (node.IsEmpty())
                {
                    // If node is occupied with container create a ViewNode without container
                    transformedNodes.Add(new ViewNode(type: node.Type, coords: node.Coordinates, nodeID: node.NodeID, followingNodes: adjacentNodes));
                }
                else
                {
                    // If node is occupied with container create a ViewNode that also has that information
                    transformedNodes.Add(new ViewNode(type: node.Type, coords: node.Coordinates, nodeID: node.NodeID, followingNodes: adjacentNodes, container: node.GetContainer()));
                }
            }

            // Add all warehouse nodes which are not represented in the graph
            WarehouseNodes.ForEach(x => transformedNodes.Add(x));
            
            return transformedNodes;
        }

        /// <summary>
        /// Steps a cycle forward in the logistic loop. Moves every container one node forward inside the loop or outside of the loop,
        /// when adjacent to a destination node. Gives priority to certain nodes to handle their step first, to avoid race conditions.
        /// Moves containers from the retrieval nodes into the loop if adjacent node is unoccupied.
        /// </summary>
        /// <returns>ErrorMessage when error occurs or empty string if successful.</returns>
        public string Step()
        {
            // Priorities:
            // 1.) Storage-Nodes: Store Container into Warehouse to take out of cycle and clear up space.
            // 2.) Conveyor-Nodes: Move Container adjacent to Storage-Node onto it if it's their destination.
            // 3.) Conveyor-Nodes: Move Container adjacent to Commissioning-Node onto it if it's their destination and it's empty.
            // 4.) Conveyor-Nodes: Move Containers one node forward in the conveyor loop, if target node of step is not adjacent to occupied Retrieval-Node. (Loop over conveyor nodes backwards from Commissioning Node)
            // 5.) Retrieval-Nodes: Move Container on Retrieval-Node into the loop, if adjacent node is empty.
            // Commissioning-Nodes have their own handling to move their Container into the loop on user input.
            // Warehouse-Nodes dont have an active state so they don't need handling.
            throw new NotImplementedException();
        }

        // To show step by step how one step of a cycle is performed: (TODO if wanted)
        // Returns List(size 2?) of changed nodes in each atomic step of one whole step and empty List if whole step is completed
        //public List<ViewNode> AtomicStep() {}

        /// <summary>
        /// Commissions a container standing on the given Commissioning-Node (symbolically), gives it the destination "Storage"
        /// and moves it back into the loop, if adjacent node is empty.
        /// </summary>
        /// <param name="nodeID">ID of the commission node to take the container of.</param>
        /// <returns>ErrorMessage when container cannot be moved back into the loop (node occupied) or empty string if successful.</returns>
        /// <exception cref="ArgumentException">When the given nodeID does not match a commission node.</exception>
        public string CommissionContainer(int nodeID)
        {
            throw new ArgumentException("The given nodeID does not match a commission node.");
        }

        /// <summary>
        /// Takes a newly created container (from user input) and retrieves it (symbolically) from the warehouse onto the given retrieval node.
        /// </summary>
        /// <param name="nodeID">ID of the retrieval node to place the new container on.</param>
        /// <param name="container">Container to place onto the retrieval node.</param>
        /// <returns>ErrorMessage when adjacent node to commission node is occupied or empty string if successful.</returns>
        /// <exception cref="ArgumentException">When the given nodeID does not match a retrieval node.</exception>
        public string RetrieveContainer(int nodeID, Container container)
        {
            throw new ArgumentException("The given nodeID does not match a retrieval node.");
        }

        /// <summary>
        /// Stores the container on given node into the warehouse, if the node is a storage node.
        /// </summary>
        /// <param name="nodeID">ID of the storage node to take the container of.</param>
        /// <exception cref="ArgumentException">When the given nodeID does not match a storage node.</exception>
        private void StoreContainer(int nodeID)
        {
            throw new ArgumentException();
        }

        /// <summary>
        /// Returns the GraphNode corresponding to the given nodeID.
        /// </summary>
        /// <param name="nodeID">ID of the node to search the graph for.</param>
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
