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
        private Form View;

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
            this.View = view;
            this.Model = new BasicLoopLogisticModel();
            this.CurrentNodes = this.Model.GetViewNodes();
            InitializeViewWithModel();
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
                    message = Model.RetrieveContainer(nodeID: retrieval.NodeID, container: retrieval.RetrievedContainer);
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
            this.View.UpdateWithViewNodes(changedNodes);

            return message;
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeViewWithModel()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newCurrentNodes"></param>
        /// <returns></returns>
        private List<ViewNode> FilterUnchangedNodes(List<ViewNode> newCurrentNodes)
        {
            throw new NotImplementedException();
        }
    }
}
