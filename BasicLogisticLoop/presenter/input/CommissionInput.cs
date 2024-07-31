using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLogisticLoop.presenter.input
{
    internal class CommissionInput : IInput
    {
        /// <summary>
        /// The unique ID of the ComissionNode in the model that the input to retrieve was called from.
        /// </summary>
        public int NodeID { get; private set; }

        /// <summary>
        /// Type of Input to indicate that the model should try to retrieve a container from the CommissionNode that called back to the adjacent loop node.
        /// </summary>
        /// <param name="nodeID">The unique ID of the RetrievalNode in the model that the input to retrieve was called from.</param>
        public CommissionInput(int nodeID)
        {
            NodeID = nodeID;
        }
    }
}
