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

        /// <summary>
        /// The Transport Unit number of the retrieved container, 0 if not set
        /// </summary>
        public int ContainerTUN { get; private set; }

        /// <summary>
        /// The content as a string of the retrieved container.
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Type of Input to indicate that the model should try to retrieve a container from the warehouse to the RetrievalNode that called.
        /// </summary>
        /// <param name="nodeID">The unique ID of the RetrievalNode in the model that the input to retrieve was called from.</param>
        /// <param name="retrievedContent">The content of the container that should be retrieved from the warehouse.</param>
        public RetrievalInput(int nodeID, string retrievedContent)
        {
            NodeID = nodeID;
            ContainerTUN = 0;
            Content = retrievedContent;
        }

        /// <summary>
        /// Type of Input to indicate that the model should try to retrieve a container from the warehouse to the RetrievalNode that called.
        /// </summary>
        /// <param name="nodeID">The unique ID of the RetrievalNode in the model that the input to retrieve was called from.</param>
        /// <param name="containerTUN">The Transport Unit number of the retrieved container, 0 if not set</param>
        /// <param name="content">The content of the container that should be retrieved from the warehouse.</param>
        public RetrievalInput(int nodeID, int containerTUN, string content)
        {
            NodeID = nodeID;
            ContainerTUN = containerTUN;
            Content = content;
        }
    }
}
