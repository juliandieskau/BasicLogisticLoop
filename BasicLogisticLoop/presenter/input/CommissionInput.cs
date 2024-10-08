﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLogisticLoop.Presenter.Input
{
    /// <summary>
    /// Input Class to define the content of the input, when a button is clicked to call the CommissionContainer method on the given node.
    /// </summary>
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
