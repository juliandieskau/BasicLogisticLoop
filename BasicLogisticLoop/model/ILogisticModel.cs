using BasicLogisticLoop.Presenter.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLogisticLoop.Model
{
    /// <summary>
    /// Interface for logistic models with all the functionalities to the outside that user input can control.
    /// </summary>
    internal interface ILogisticModel
    {
        /// <summary>
        /// Transforms the inner state of the model into a list of value-type nodes that the view can use to display the model.
        /// </summary>
        /// <returns>List of transformed nodes.</returns>
        List<ViewNode> GetViewNodes();

        /// <summary>
        /// Steps one cycle forward in the model. This includes moving around containers on the nodes in the loop, 
        /// moving them onto their destination node if empty, moving a container onto the commissioning nodes 
        /// or idling them in the loop if the latter are full.
        /// </summary>
        /// <returns>ErrorMessage if something goes wrong.</returns>
        string Step();

        /// <summary>
        /// Moves container from a commissioning node back to the loop, if the adjacent node is empty.
        /// </summary>
        /// <param name="nodeID">ID of commissioning node from which the container should be moved.</param>
        /// <param name="container">Container that should be moved.</param>
        /// <returns>ErrorMessage if adjacent node is not empty.</returns>
        string CommissionContainer(int nodeID);

        /// <summary>
        /// Adds given container from the blackbox warehouse to the given retrieval node - if empty - with destination commissioning.
        /// </summary>
        /// <param name="nodeID">ID of retrieval node to which the container should be added.</param>
        /// <param name="content">Content that should be added in a container.</param>
        /// <returns>ErrorMessage if retrieval node is not empty.</returns>
        string RetrieveContainer(int nodeID, string content);
    }
}
