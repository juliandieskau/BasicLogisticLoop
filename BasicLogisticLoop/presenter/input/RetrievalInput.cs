using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLogisticLoop.presenter.input
{
    internal class RetrievalInput : IInput
    {
        /// <summary>
        /// The unique ID of the RetrievalNode in the model that the input to retrieve was called from.
        /// </summary>
        public int NodeID { get; }

        /// <summary>
        /// Type of Input to indicate that the model should try to retrieve a container from the warehouse to the RetrievalNode that called.
        /// </summary>
        /// <param name="nodeID">The unique ID of the RetrievalNode in the model that the input to retrieve was called from.</param>
        public RetrievalInput (int nodeID)
        {
            NodeID = nodeID;
        }
    }
}
