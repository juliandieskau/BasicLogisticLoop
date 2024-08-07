using BasicLogisticLoop.Presenter;
using BasicLogisticLoop.Presenter.Output;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BasicLogisticLoop
{
    /// <summary>
    /// View of the BasicLogisticLoop. Initializes the Presenter which then calls the view to updates it with the model.
    /// Handles user input and output in a GUI. InitializeView() needs to be called once.
    /// </summary>
    public partial class BasicLogisticLoopForm : Form
    {
        /// <summary>
        /// Controls the model and calls the form back with updates of the model. 
        /// Needs to be called when receiving user input.
        /// </summary>
        private Presenter.Presenter Presenter;

        /// <summary>
        /// List of ViewNodes representing the current state of the model given from the presenter.
        /// </summary>
        private List<ViewNode> NodeData;

        /// <summary>
        /// Constructor for the Form. Initializes the Presenter and gives it this Form as an argument.
        /// </summary>
        public BasicLogisticLoopForm()
        {
            Presenter = new Presenter.Presenter(this);
            // Presenter calls InitializeView() in its constructor
            InitializeComponent();
        }

        /// <summary>
        /// Takes a list of ViewNodes and constructs the view on the basis of it. Needs to be called once for the view to work.
        /// </summary>
        /// <param name="initialViewNodes">List of ViewNodes to use as a basis to build the View out of.</param>
        public void InitializeView(List<ViewNode> initialViewNodes)
        {
            NodeData = initialViewNodes;

            // Form settings
            Text = "Logisticloop Basic";
            BackColor = WindowBackColor;


            // TODO generate Labels and InputWindows here
            throw new NotImplementedException();
        }

        /// <summary>
        /// Takes a list of nodes that contains nodes that have already been initialized in the view and updates their state to the given one.
        /// Updates the GUI Label accordingly.
        /// </summary>
        /// <param name="changedNodes">List of changedNodes to be updated.</param>
        public void UpdateWithViewNodes(List<ViewNode> changedNodes)
        {
            foreach (ViewNode changedNode in changedNodes)
            {
                ViewNode nodeToUpdate = NodeData.Find(oldNode => oldNode.NodeID == changedNode.NodeID);
                nodeToUpdate = changedNode;
                UpdateNode(nodeToUpdate);
            }
        }

        /// <summary>
        /// Updates the label in the view representing the given ViewNode with its data.
        /// </summary>
        /// <param name="node">ViewNode to update in the view.</param>
        private void UpdateNode(ViewNode node)
        {
            // TODO find Label matching the given nodes ID

            // update the labels content with the data of the node
            throw new NotImplementedException();
        }

        /// <summary>
        /// Event handler to be added to all Labels that calls the ShowContent() Method when a Label is clicked.
        /// </summary>
        /// <param name="sender">Label that was clicked.</param>
        /// <param name="e"></param>
        private void OnLabelClick(object sender, EventArgs e)
        {
            Label label = sender as Label;
            // For all Node Labels: 
            //      When clicked display its content in label on the side (ShowContent)
            throw new NotImplementedException();
        }

        /// <summary>
        /// Event handler to be added to all Buttons that calls the Presenter by building a fitting IInput object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonClick(object sender, EventArgs e)
        {
            Button button = sender as Button;
            // Step Button
            // Commissioning, Retrieval Button
            throw new NotImplementedException();
        }

        /// <summary>
        /// Display the content of a given node label in the GUIs sidebar
        /// </summary>
        /// <param name="label">Label to show the content of.</param>
        private void ShowContent(Label label)
        {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Opens a MessageBox to show an error when performing an action that the model does not allow.
        /// Has to be called when receiving a ErrorMessages string back from the Presenter.
        /// </summary>
        /// <param name="message">Error message to display.</param>
        private void ShowErrorMessage(string message)
        {
            string subMessage = Environment.NewLine + "Try again when valid.";
            MessageBox.Show(text: message + subMessage, caption: "Invalid action");
        }

        /// <summary>
        /// Method that generates a Panel that holds Labels that shows the content of the node label that is being clicked on.
        /// </summary>
        /// <returns>Created Panel.</returns>
        private FlowLayoutPanel GenerateContentPanel()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method that generates a Panel that holds Labels and TextBoxes to input the content of containers retrieved from the warehouse.
        /// </summary>
        /// <returns>Created Panel.</returns>
        private FlowLayoutPanel GenerateRetrievalPanel()
        {
            throw new NotImplementedException();
        }
    }
}
