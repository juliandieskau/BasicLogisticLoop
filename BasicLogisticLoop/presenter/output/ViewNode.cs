using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLogisticLoop.presenter.output
{
    internal struct ViewNode
    {
        /// <summary>
        /// The type of the ViewNode, see enum <see cref="NodeType"/>.
        /// </summary>
        public NodeType Type { get; }

        /// <summary>
        /// The coordinates at which the ViewNode is located at in 2D-Space in a Grid. 
        /// </summary>
        public (int X, int Y) Coordinates { get; }
        
        /// <summary>
        /// The unique ID of the node in the model to identify it.
        /// </summary>
        public int NodeID { get; }

        /// <summary>
        /// An array of NodeIDs that point to the Nodes, that this node can transport Containers to.
        /// </summary>
        public int[] FollowingNodes { get; }

        /// <summary>
        /// The Container standing on the node. Can only hold one Container at a time. May be null if no Container on it.
        /// </summary>
        public Container Container { get; }

        /// <summary>
        /// Constructor for the ViewNode.
        /// </summary>
        /// <param name="type">The type of the ViewNode, see enum <see cref="NodeType"/>.</param>
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
    }
}
