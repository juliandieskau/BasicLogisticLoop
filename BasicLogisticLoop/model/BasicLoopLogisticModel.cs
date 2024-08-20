using BasicLogisticLoop.Presenter.Output;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BasicLogisticLoop.Model
{
    /// <summary>
    /// Implementation of the <see cref="ILogisticModel"/> that creates a Layout based on the "Logistikloop Basic" assignment. 
    /// Contains the logic and model that the LogisticLoop Basic operates after.
    /// </summary>
    internal class BasicLoopLogisticModel : ILogisticModel
    {
        // ########################################
        // MODEL OBJECTS

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

        // ########################################
        // MODEL VALUES

        /// <summary>
        /// Number of Nodes in the graph (excluding warehouse nodes, which are only present for the view).
        /// </summary>
        private int GraphNodeNumber = 15;

        /// <summary>
        /// Index to save the last used TransportUnit-Number and increase by 1 before assigning it to the next container.
        /// </summary>
        private int currentTUN = 10000;

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
        /// <exception cref="NullReferenceException">When Graph is empty.</exception>
        public List<ViewNode> GetViewNodes()
        {
            if (GraphNodes == null)
            {
                throw new NullReferenceException("Graph is empty!");
            }

            List<ViewNode> transformedNodes = new List<ViewNode>();
            // Add all the graph nodes
            foreach (var node in GraphNodes)
            {
                // Get array of nodes that the node has an edge towards
                int[] adjacentNodes = Graph.GetAdjacentNodes(node.NodeID).ToArray();

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

        // ########################################
        // MODEL LOGIC METHODS

        /// <summary>
        /// Steps a cycle forward in the logistic loop. Moves every container one node forward inside the loop or outside of the loop,
        /// when adjacent to a destination node. Gives priority to certain nodes to handle their step first, to avoid race conditions.
        /// Moves containers from the retrieval nodes into the loop if adjacent node is unoccupied.
        /// </summary>
        /// <returns>ErrorMessage when error occurs or empty string if successful.</returns>
        public string Step()
        {
            string message = "";

            // Save references to all graph nodes that have not been handled yet and make sure none are handled twice in one step.
            // Handling node: Moving the container on a node if it has one.
            List<int> unhandledNodeIDs = GraphNodes.Select(n => n.NodeID).ToList();
            if (unhandledNodeIDs == null)
            {
                return ErrorMessages.StepError;
            }
            if (!unhandledNodeIDs.Any()) // not in the same call, since error occurs when null
            {
                return ErrorMessages.StepError;
            }

            // 1.) Storage-Nodes: Store Container into Warehouse to take out of cycle.
            unhandledNodeIDs = StepStorageToWarehouse(unhandledNodeIDs);

            // 2.) Conveyor-Nodes: Move Container adjacent to Storage-Node onto it if it's their destination.
            unhandledNodeIDs = StepConveyorToStorage(unhandledNodeIDs);

            // 3.) Conveyor-Nodes: Move Container adjacent to Commissioning-Node onto it if it's their destination and it's empty.
            unhandledNodeIDs = StepConveyorToCommission(unhandledNodeIDs);

            // 4.) Retrieval-Nodes: Move Container on Retrieval-Node into the loop if adjacent conveyor node is empty.
            unhandledNodeIDs = StepRetrievalToConveyor(unhandledNodeIDs);

            // 5.) Conveyor-Nodes: Move Containers one node forward in the conveyor loop if target node of step is not adjacent to occupied Retrieval-Node.
            unhandledNodeIDs = StepConveyorToConveyor(unhandledNodeIDs, ref message);

            // 6.) Retrieval-Nodes: Move Container on Retrieval-Node into the loop if adjacent conveyor node is empty after loop has moved and hasn't been retrieved before.
            unhandledNodeIDs = StepRetrievalToConveyor(unhandledNodeIDs);

            // return successful completion of step
            return message;
        }

        // To show step by step how one step of a cycle is performed: (TODO if wanted)
        // Returns List(size 2?) of changed nodes in each atomic step of one whole step and empty List if whole step is completed
        //public List<ViewNode> AtomicStep() {}

        /// <summary>
        /// Commissions a container standing on the given Commissioning-Node (symbolically), gives it the destination "Storage"
        /// and moves it back into the loop, if adjacent node is empty.
        /// </summary>
        /// <param name="nodeID">ID of commission node to take the container of.</param>
        /// <returns>ErrorMessage when container cannot be moved back into the loop (node occupied) or empty string if successful.</returns>
        /// <exception cref="ArgumentException">When the given nodeID does not match a commission node.</exception>
        public string CommissionContainer(int nodeID)
        {
            GraphNode commissionNode = GetGraphNode(nodeID);
            // Check if node is commission node
            if (commissionNode == null || commissionNode.Type != NodeType.Commissioning)
            {
                throw new ArgumentException("The given node does not match a commission node.");
            }

            // Check if commission node has container on it
            if (commissionNode.IsEmpty())
            {
                // nothing to move so do nothing -> no error when spamming button
                return "";
            }

            // Get node to move container to, if all are empty returns null
            GraphNode followingNode = GetEmptyAdjacentGraphNode(commissionNode);
            if (followingNode == null)
            {
                return ErrorMessages.CommissionError;
            }

            // Commission container: Give destinationType: Storage (even if was already commissioned but not moved to loop)
            commissionNode.GetContainer().DestinationType = NodeType.Storage;

            // Move container from the commission node back into the loop
            try
            {
                MoveContainer(commissionNode, followingNode);
            }
            catch (ArgumentException e)
            {
                return ErrorMessages.CommissionError + " " + e.Message;
            }
            return "";
        }

        /// <summary>
        /// Takes a newly created container content (from user input) and retrieves it (symbolically) from the warehouse onto the given retrieval node - if empty.
        /// Creates a container with the given content, a generated TransportUnit-Number and gives it the destination "Commissioning".
        /// </summary>
        /// <param name="nodeID">ID of retrieval node to place the new container on.</param>
        /// <param name="content">Content of container to place onto the retrieval node.</param>
        /// <returns>ErrorMessage when adjacent node to commission node is occupied or empty string if successful.</returns>
        /// <exception cref="ArgumentException">When the given nodeID does not match a retrieval node.</exception>
        public string RetrieveContainer(int nodeID, string content)
        {
            GraphNode node = GetGraphNode(nodeID);
            // Check if node is retrieval node
            if (node == null || node.Type != NodeType.Retrieval)
            {
                throw new ArgumentException("The given node does not match a retrieval node.");
            }

            // Check if node is empty
            if (!node.IsEmpty())
            {
                return ErrorMessages.RetrievalError;
            }

            // Create container with input
            Container container = new Container(transportUnitNumber: ++currentTUN, content: content, destinationType: NodeType.Commissioning);
            node.ChangeContainer(container);
            return "";
        }

        // ########################################
        // STEP METHODS

        /// <summary>
        /// Stores the container on given node into the warehouse, if the node is a storage node.
        /// </summary>
        /// <param name="node">Storage node to take the container of.</param>
        /// <exception cref="ArgumentException">When the given nodeID does not match a storage node.</exception>
        private void StoreContainer(GraphNode node)
        {
            // Check if node is storage node
            if (node == null || node.Type != NodeType.Storage)
            {
                throw new ArgumentException("The given nodeID does not match a storage node.");
            }
            // If the model would need this, save Container into a database ("warehouse") here

            // Empty the storage node
            node.ChangeContainer(null);
        }

        /// <summary>
        /// Moves a container in GraphNodes from one given node to another given node.
        /// Checks if move is valid. Only able to move container onto empty nodes!
        /// </summary>
        /// <param name="fromID">node to move the container from.</param>
        /// <param name="toID">node to move the container to.</param>
        /// <exception cref="ArgumentException">When the given IDs do not match nodes in the graph or the move is not valid in the model.</exception>
        private void MoveContainer(GraphNode fromNode, GraphNode toNode)
        {
            // Check if the nodes are valid
            if (fromNode == null | toNode == null)
            {
                throw new ArgumentException("The given nodes are null.");
            }
            // Check if toNode is empty
            if (!toNode.IsEmpty())
            {
                throw new ArgumentException("The Node to move the container to is already occupied!");
            }

            // check valid nodes to move to or from
            // valid type pairs:
            //  Retrieval       (AB)    -> Conveyor         (Loop)
            //  Conveyor        (Loop)  -> Conveyor         (Loop)
            //  Conveyor        (Loop)  -> Commissioning    (K)
            //  Conveyor        (Loop)  -> Storage          (EB)
            //  Commissioning   (K)     -> Conveyor         (Loop)                      [CommissionContainer()]
            // explicitly not valid:
            //  Storage         (EB)                -> Warehouse    (nicht im System)   [StoreContainer()]
            //  Warehouse       (nicht im System)   -> Retrieval    (AB)                [RetrieveContainer()]

            if (    (   (fromNode.Type == NodeType.Retrieval)       && (toNode.Type == NodeType.Conveyor)       )|
                    (   (fromNode.Type == NodeType.Conveyor)        && (toNode.Type == NodeType.Conveyor)       )|
                    (   (fromNode.Type == NodeType.Conveyor)        && (toNode.Type == NodeType.Commissioning)  )|
                    (   (fromNode.Type == NodeType.Conveyor)        && (toNode.Type == NodeType.Storage)        )|
                    (   (fromNode.Type == NodeType.Commissioning)   && (toNode.Type == NodeType.Conveyor)       ))
            {
                // Set container on receiving node from sending node
                int toIndex = GraphNodes.FindIndex(n => n.NodeID == toNode.NodeID);
                if (toIndex > -1)
                {
                    GraphNodes[toIndex].ChangeContainer(fromNode.GetContainer());
                }
                // Remove container from sending node
                int fromIndex = GraphNodes.FindIndex(n => n.NodeID == fromNode.NodeID);
                if (fromIndex > -1)
                {
                    GraphNodes[fromIndex].ChangeContainer(null);
                }
            }
            else
            {
                throw new ArgumentException("The move between the two nodes is invalid.");
            }
        }

        /// <summary>
        /// STEP 1: Containers on Storage Node -> Warehouse Node
        /// </summary>
        /// <param name="unhandledNodeIDs">NodeIDs to check for moving containers from.</param>
        /// <returns>NodeIDs that havent moved the container after storage -> warehouse is completed.</returns>
        private List<int> StepStorageToWarehouse(List<int> unhandledNodeIDs) 
        {
            // Search all storage nodes
            List<int> storageNodeIDs = unhandledNodeIDs.FindAll(x => GetGraphNode(x).Type == NodeType.Storage);
            foreach (int nodeID in storageNodeIDs)
            {
                GraphNode storageNode = GetGraphNode(nodeID);
                if (storageNode != null)
                {
                    // Store container (to warehouse) on this node
                    StoreContainer(storageNode);
                }
                unhandledNodeIDs.Remove(nodeID);
            }
            return unhandledNodeIDs;
        }

        /// <summary>
        /// STEP 2: Containers on Conveyor Node -> Storage Node
        /// </summary>
        /// <param name="unhandledNodeIDs">NodeIDs to check for moving containers from.</param>
        /// <returns>NodeIDs that havent moved their container after conveyor -> storage is completed.</returns>
        private List<int> StepConveyorToStorage(List<int> unhandledNodeIDs) 
        {
            for (int i = 0; i < unhandledNodeIDs.Count; i++)
            {
                int nodeID = unhandledNodeIDs[i];

                // Find a conveyor node
                GraphNode conveyorNode = GraphNodes.Find(x => x.NodeID == nodeID);
                if (conveyorNode != null && conveyorNode.Type == NodeType.Conveyor)
                {
                    // Check if there's an adjacent storage node for the current node that is empty
                    GraphNode storageNode = GetAdjacentGraphNodeOfType(conveyorNode, NodeType.Storage);
                    if (storageNode != null && !storageNode.IsEmpty())
                    {
                        // Check if the destination of the nodes container is a storage node
                        if (conveyorNode.GetContainer() != null && conveyorNode.GetContainer().DestinationType == NodeType.Storage)
                        {
                            // Move Container onto storage node
                            MoveContainer(conveyorNode, storageNode);
                            unhandledNodeIDs.Remove(conveyorNode.NodeID);
                        }
                    }
                }
            }
            return unhandledNodeIDs;
        }

        /// <summary>
        /// STEP 3: Containers on Conveyor Node -> Commission Node
        /// </summary>
        /// <param name="unhandledNodeIDs">NodeIDs to check for moving containers from.</param>
        /// <returns>NodeIDs that havent moved the container after conveyor -> commission is completed.</returns>
        private List<int> StepConveyorToCommission(List<int> unhandledNodeIDs)
        {
            for (int i = 0; i < unhandledNodeIDs.Count; i++) {
                int nodeID = unhandledNodeIDs[i];

                // Find a conveyor node
                GraphNode conveyorNode = GraphNodes.Find(x => x.NodeID == nodeID && x.Type == NodeType.Conveyor);
                if (conveyorNode != null)
                {
                    // Check if there's an adjacent commissioning node for the current node
                    GraphNode commissioningNode = GetAdjacentGraphNodeOfType(conveyorNode, NodeType.Commissioning);
                    if (commissioningNode != null && commissioningNode.IsEmpty())
                    {
                        // reached
                        // Check if the destination of the nodes container is a commissioning node
                        if (conveyorNode.GetContainer() != null && conveyorNode.GetContainer().DestinationType == NodeType.Commissioning)
                        {
                            // Move Container onto commissioning node
                            MoveContainer(conveyorNode, commissioningNode);
                            unhandledNodeIDs.Remove(conveyorNode.NodeID);
                            unhandledNodeIDs.Remove(conveyorNode.NodeID);
                        }
                    }
                }
            }
            return unhandledNodeIDs;
        }

        /// <summary>
        /// STEP 4 (AND 6): Containers on Retrieval Node -> Conveyor Node
        /// </summary>
        /// <param name="unhandledNodeIDs">NodeIDs to check for moving containers from.</param>
        /// <returns>NodeIDs that havent moved the container after retrieval -> conveyor is completed.</returns>
        private List<int> StepRetrievalToConveyor(List<int> unhandledNodeIDs)
        {
            // Search all retrieval nodes
            List<int> retrievalNodeIDs = unhandledNodeIDs.FindAll(x => GetGraphNode(x).Type == NodeType.Retrieval);
            foreach (int nodeID in retrievalNodeIDs)
            {
                // Check if retrievalNode has a container on it (so it has to be moved)
                GraphNode retrievalNode = GetGraphNode(nodeID);
                if (retrievalNode != null && !retrievalNode.IsEmpty())
                {
                    // Get the node to move the container to and if its empty
                    GraphNode followingNode = GetAdjacentGraphNodeOfType(retrievalNode, NodeType.Conveyor);
                    if (followingNode != null && followingNode.IsEmpty())
                    {
                        // Check if the loop will be stuck after moving container from retrieval node into it 
                        if (IsRetrievalAllowed(followingNode))
                        {
                            // Move the container
                            MoveContainer(retrievalNode, followingNode);

                            // remove the loop conveyor and retrieval node from being handled again, container on it already moved
                            unhandledNodeIDs.Remove(retrievalNode.NodeID); 
                            unhandledNodeIDs.Remove(followingNode.NodeID);
                        }

                    }
                }
            }
            return unhandledNodeIDs;
        }

        /// <summary>
        /// STEP 5: Containers on Coneyor Node -> Conveyor Node
        /// </summary>
        /// <param name="unhandledNodeIDs">NodeIDs to check for moving containers from.</param>
        /// <returns>NodeIDs that havent moved the container after conveyor -> conveyor is completed.</returns>
        private List<int> StepConveyorToConveyor(List<int> unhandledNodeIDs, ref string message)
        {
            List<int> conveyorNodeIDs = GraphNodes.Select(n => n.NodeID).ToList().FindAll(x => GetGraphNode(x).Type == NodeType.Conveyor);

            // start with a commissioning nodes adjacent conveyorNode and save the container on the first node to give to the last
            GraphNode currentNode = GetAdjacentGraphNodeOfType(GraphNodes.Find(x => x.Type == NodeType.Commissioning), NodeType.Conveyor);
            GraphNode firstNode = currentNode;

            // remove container from starting node of the loop and save into temporary storage
            Container firstContainer = GetGraphNode(currentNode.NodeID).GetContainer();
            currentNode.ChangeContainer(null);

            // loop backwards over conveyor nodes except the starting one
            for (int i = 0; i < conveyorNodeIDs.Count - 1; i++)
            {
                // get CONVEYOR node current node is adjacent to
                GraphNode originNode = GraphNodes.FindAll(n => Graph.GetAdjacentNodes(n.NodeID)
                                                    .Contains(currentNode.NodeID))
                                                    .Find(n => n.Type == NodeType.Conveyor);

                // if node not handled before move the container from it onto the target
                if (unhandledNodeIDs.Contains(originNode.NodeID))
                {
                    // check that the origin conveyor has a container and that the target container is empty
                    if (!originNode.IsEmpty() && currentNode.IsEmpty())
                    {
                        MoveContainer(originNode, currentNode);
                    }
                    unhandledNodeIDs.Remove(originNode.NodeID);
                }
                currentNode = originNode;
            }

            // assign the last node the container of the first node to connect the loop (if first node wasnt handled yet)
            if (unhandledNodeIDs.Contains(firstNode.NodeID))
            {
                // only change container on tile if the first one had a container
                if (firstContainer != null) {
                    if (!currentNode.IsEmpty())
                    {
                        throw new InvalidOperationException(ErrorMessages.StepError);
                    }

                    // "move container" but the origin was already handled and overwritten so firstContainer was saved
                    currentNode.ChangeContainer(firstContainer);
                    unhandledNodeIDs.Remove(firstNode.NodeID);
                }
            }
            return unhandledNodeIDs;
        }

        // ########################################
        // GRAPH METHODS

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
        /// Finds the first occurence of a node that follows the given one if it is of the given type.
        /// </summary>
        /// <param name="node">Node to find a specific following node of.</param>
        /// <param name="type">Type of the following node to find.</param>
        /// <returns>Following node of type or null.</returns>
        private GraphNode GetAdjacentGraphNodeOfType(GraphNode node, NodeType type)
        {
            GraphNode followingNode = GraphNodes.Find(graphNode => Graph.GetAdjacentNodes(node.NodeID)
                                                                    .Contains(graphNode.NodeID)
                                                                    && graphNode.Type == type);
            return followingNode;
        }

        /// <summary>
        /// Finds the first occurence of a node that follows the given one if it is empty.
        /// </summary>
        /// <param name="node">Node to find an empty following node of.</param>
        /// <returns>Empty following node or null.</returns>
        private GraphNode GetEmptyAdjacentGraphNode(GraphNode node)
        {
            return GraphNodes.Find(graphNode => Graph.GetAdjacentNodes(node.NodeID)
                                                    .Contains(graphNode.NodeID)
                                                    && graphNode.IsEmpty());
        }

        /// <summary>
        /// Checks if the loop of conveyors will get stuck in an endless loop (that cannot be resolved automatically) after moving a new container onto it.
        /// This would occur, if the commissioning node is occupied and all conveyor nodes are occupied with containers that want to reach the commissioning node.
        /// Then the commissioning node cannot be emptied and no other container can me moved out of or into the loop.
        /// </summary>
        /// <param name="nodeToBeBlocked">Node that would be blocked if the retrieval would happen.</param>
        /// <returns><c>true</c> if retrieval is allowed, otherwise <c>false</c></returns>
        private bool IsRetrievalAllowed(GraphNode nodeToBeBlocked)
        {
            foreach (GraphNode node in GraphNodes)
            {
                // check the conveyor nodes but ignore the one that would be occupied if the retrieval would happen
                if (node.NodeID == nodeToBeBlocked.NodeID)
                {
                    continue;
                }
                // if there's an empty commissioning node, it can't get stuck
                if (node.Type == NodeType.Commissioning && node.IsEmpty())
                {
                    return true;
                }
                if (node.Type == NodeType.Conveyor)
                {
                    // if there's another empty conveyor node, it can't get stuck
                    if (node.IsEmpty())
                    {
                        return true;
                    }
                    // if there's a container that has a destination that's not commissioning, it will go out of the loop so it can't get stuck
                    if (node.GetContainer().DestinationType != NodeType.Commissioning)
                    {
                        return true;
                    }
                }
            }
            // if none of these conditions occur, all conveyor nodes besides the one where it would go onto
            // will be full of containers wanting to go to the commissioning node which is already full
            // thus don't allow the retrieval
            return false;
        }

        // ########################################
        // CONSTRUCTOR METHODS

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
