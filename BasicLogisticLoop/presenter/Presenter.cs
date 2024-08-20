using BasicLogisticLoop.Model;
using BasicLogisticLoop.Presenter.Input;
using BasicLogisticLoop.Presenter.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BasicLogisticLoop.Presenter
{
    /// <summary>
    /// Holds and Controls the model and view and presents the models state by handing it to the view.
    /// Takes user input from the view and calls the models functions accordingly.
    /// </summary>
    internal class Presenter
    {
        /// <summary>
        /// Holds all the logistic loop model logic and state and is used as an interface to access them.
        /// </summary>
        private ILogisticModel Model;

        /// <summary>
        /// Handles all user interaction by showing the model data on screen and receiving user input through a GUI.
        /// </summary>
        private BasicLogisticLoopForm View;

        /// <summary>
        /// List of all the ViewNodes that are currently shown in the view. May be used to check which Nodes have changed after 
        /// </summary>
        private List<ViewNode> CurrentNodes;

        /// <summary>
        /// Constructor for the Presenter.
        /// </summary>
        /// <param name="view">Windows Form as the View Component of the MVP model.</param>
        public Presenter(Form view)
        {
            View = view as BasicLogisticLoopForm;
            if (View == null)
            {
                throw new ArgumentNullException("View needs to be a valid BasicLogisticLoopForm object!");
            }
            Model = new BasicLoopLogisticModel();
            CurrentNodes = Model.GetViewNodes();
            View.InitializeView(Model.GetViewNodes());
        }

        /// <summary>
        /// Method for the view to call when receiving input to let the presenter handle it. 
        /// Checks the type of the input and calls the corresponding method of the model.
        /// Updates the view after 
        /// </summary>
        /// <param name="input">Input object and its content that was given from the view.</param>
        /// <returns>ErrorMessage when operation on model fails or empty string if successful.</returns>
        public string ReceiveInput(IInput input)
        {
            string message = "";
            if (input is RetrievalInput retrieval)
            {
                try
                {
                    message = Model.RetrieveContainer(nodeID: retrieval.NodeID, containerTUN: retrieval.ContainerTUN, content: retrieval.Content);
                }
                catch (ArgumentException ex)
                {
                    message = ex.Message;
                }
            }
            else if (input is CommissionInput commission)
            {
                try
                {
                    message = Model.CommissionContainer(nodeID: commission.NodeID);
                }
                catch (ArgumentException ex)
                {
                    message = ex.Message;
                }
            }
            else if (input is StepInput)
            {
                message = Model.Step();
            }
            // No else{} because input cannot be of any other type, except if another gets added

            // handle view update
            List<ViewNode> changedNodes = FilterUnchangedNodes(Model.GetViewNodes());
            View.UpdateWithViewNodes(changedNodes);

            return message;
        }

        /// <summary>
        /// Takes a List of ViewNodes and filters out its ViewNodes that have not changed compared to the previous view state.
        /// Updates the currentNodes with all new nodes afterwards.
        /// </summary>
        /// <param name="newCurrentNodes">ViewNodes of the new state of the model.</param>
        /// <returns>Filtered List of ViewNodes.</returns>
        private List<ViewNode> FilterUnchangedNodes(List<ViewNode> newCurrentNodes)
        {
            // For every node of the newCurrentNodes:
            //      Add all nodes that match the following criteria to the return
            // Criteria:
            //      If the CurrentNodes does not contain the node, it is added
            // Contains():
            //      Equals() Method that is overridden in ViewNode will be taken as comparison for Contains()
            //      Equals() Method checks if the ViewNode has the same NodeType and NodeID and if it has the same Container on it
            newCurrentNodes = newCurrentNodes.FindAll(node => !CurrentNodes.Contains(node));

            // replace changed CurrentNodes
            CurrentNodes = newCurrentNodes;

            return newCurrentNodes;
        }
    }
}
