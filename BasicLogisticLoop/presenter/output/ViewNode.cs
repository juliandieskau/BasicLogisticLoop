using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLogisticLoop.Presenter.Output
{
    /// <summary>
    /// Value-Type representing the important data of a node at the current time without exposing the logic of the model.
    /// Initialize
    /// </summary>
    internal struct ViewNode
    {
        /// <summary>
        /// The type of the ViewNode.
        /// </summary>
        public NodeType Type { get; private set; }
        
        /// <summary>
        /// The unique ID of the node in the model to identify it.
        /// </summary>
        public int NodeID { get; private set; }

        /// <summary>
        /// The coordinates at which the ViewNode is located at in 2D-Space in a Grid. 
        /// </summary>
        public (int X, int Y) Coordinates { get; private set; }

        /// <summary>
        /// An array of NodeIDs that point to the Nodes, that this node can transport Containers to.
        /// </summary>
        public int[] FollowingNodes { get; private set; }

        /// <summary>
        /// The Container standing on the node. Can only hold one Container at a time. May be null if no Container on it.
        /// </summary>
        public Container Container { get; private set; }

        /// <summary>
        /// Constructor for the ViewNode.
        /// </summary>
        /// <param name="type">The type of the ViewNode.</param>
        /// <param name="coords">The coordinates at which the ViewNode is located at in 2D-Space in a Grid. </param>
        /// <param name="nodeID">The unique ID of the node in the model to identify it.</param>
        /// <param name="followingNodes">An array of NodeIDs that point to the Nodes, that this node can transport Containers to.</param>
        /// <param name="container">The Container standing on the node. Can only hold one Container at a time. May be null if no Container on it.</param>
        public ViewNode(NodeType type, (int x, int Y) coords, int nodeID, int[] followingNodes, Container container)
        {
            Type = type;
            Coordinates = coords;
            NodeID = nodeID;
            FollowingNodes = followingNodes;
            Container = container;
        }

        /// <summary>
        /// Constructor for the ViewNode. Sets Container to null.
        /// </summary>
        /// <param name="type">The type of the ViewNode.</param>
        /// <param name="coords">The coordinates at which the ViewNode is located at in 2D-Space in a Grid. </param>
        /// <param name="nodeID">The unique ID of the node in the model to identify it.</param>
        /// <param name="followingNodes">An array of NodeIDs that point to the Nodes, that this node can transport Containers to.</param>
        public ViewNode(NodeType type, (int x, int Y) coords, int nodeID, int[] followingNodes)
        {
            Type = type;
            Coordinates = coords;
            NodeID = nodeID;
            FollowingNodes = followingNodes;
            Container = null;
        }
    }
}
