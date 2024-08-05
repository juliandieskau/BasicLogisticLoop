using BasicLogisticLoop.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLogisticLoop.Presenter.Input
{
    /// <summary>
    /// Input Class to define the content of the input, when a button is clicked to call the RetrieveContainer method on the given node.
    /// </summary>
    internal class RetrievalInput : IInput
    {
        /// <summary>
        /// The unique ID of the RetrievalNode in the model that the input to retrieve was called from.
        /// </summary>
        public int NodeID { get; private set; }

        public Container RetrievedContainer { get; private set; }

        /// <summary>
        /// Type of Input to indicate that the model should try to retrieve a container from the warehouse to the RetrievalNode that called.
        /// </summary>
        /// <param name="nodeID">The unique ID of the RetrievalNode in the model that the input to retrieve was called from.</param>
        public RetrievalInput (int nodeID, Container retrievedContainer)
        {
            NodeID = nodeID;
            RetrievedContainer = retrievedContainer;
        }
    }
}
