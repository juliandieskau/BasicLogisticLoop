using BasicLogisticLoop.Presenter.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLogisticLoop.Model
{
    /// <summary>
    /// Represents a node and its data in the Graph Model.
    /// Equals() on an object of this class does not depend on its Container.
    /// </summary>
    internal class GraphNode
    {
        /// <summary>
        /// The type of the Node.
        /// </summary>
        public NodeType Type { get; private set; }

        /// <summary>
        /// The unique ID of the node to identify it.
        /// </summary>
        public int NodeID { get; private set; }

        /// <summary>
        /// The coordinates at which the GraphNode is located at in 2D-Space in a Grid.
        /// </summary>
        public (int X, int Y) Coordinates { get; private set; }

        /// <summary>
        /// The Container standing on the node. Can only hold one Container at a time. May be null if no Container on it.
        /// </summary>
        private Container Container;

        /// <summary>
        /// Constructor for the GraphNode. Parameters input here may never be changed after initializing once.
        /// Sets Container to null since when starting the LogisticLoop it will be empty.
        /// </summary>
        /// <param name="type">The type of the ViewNode.</param>
        /// <param name="id">The unique ID of the node to identify it.</param>
        /// <param name="coords">The coordinates at which the GraphNode is located at in 2D-Space in a Grid.</param>
        public GraphNode(NodeType type, int id, (int X, int Y) coords)
        {
            Type = type;
            NodeID = id;
            Coordinates = coords;
            Container = null;
        }

        /// <summary>
        /// Evaluates whether a container is on the node or not. 
        /// Check this method to make sure to not try to get the container which is not there!
        /// </summary>
        /// <returns>Returns <c>false</c> if a container is on the node, <c>true</c> if not. </returns>
        public bool IsEmpty()
        {
            if (Container == null) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Getter for container on node. Always make sure to check if IsEmpty() is false first before calling GetContainer()!
        /// </summary>
        /// <returns>Container on node or null.</returns>
        public Container GetContainer() {
            return Container;
        }

        /// <summary>
        /// Setter for container on node. Use <c>null</c> as argument to set the node to be empty.
        /// </summary>
        /// <param name="newContainer">New Container or <c>null</c> to empty the node.</param>
        public void ChangeContainer(Container newContainer) 
        {
            Container = newContainer;
        }


        public override bool Equals(object obj)
        {
            return obj is GraphNode node &&
                   Type == node.Type &&
                   NodeID == node.NodeID &&
                   Coordinates.Equals(node.Coordinates);
        }

        public override int GetHashCode()
        {
            int hashCode = 1165973981;
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + NodeID.GetHashCode();
            hashCode = hashCode * -1521134295 + Coordinates.GetHashCode();
            return hashCode;
        }
    }
}
