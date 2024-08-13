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

            // Form properties
            Text = "Logisticloop Basic";
            BackColor = WindowBackColor;
            Name = "BasicLogisticLoop";
            AutoSize = false;
            Size = new Size(1600, 900);
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            ShowIcon = false;

            // generate form controls here
            Controls.Add(GenerateSplitContainer());
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
                // Find the matching ViewNode in the views NodeData
                ViewNode nodeToUpdate = NodeData.Find(oldNode => oldNode.NodeID == changedNode.NodeID);

                // Update the ViewNode in the views NodeData to the new data
                nodeToUpdate = changedNode;

                // Update the corresponding label in the view with the new data
                UpdateNode(nodeToUpdate);
            }
        }

        /// <summary>
        /// Updates the label in the view representing the given ViewNode with its data.
        /// Works even when viewNodes have multiple Labels representing them displayed in the view.
        /// </summary>
        /// <param name="node">ViewNode to update in the view.</param>
        private void UpdateNode(ViewNode node)
        {
            // Find Label matching the given nodes ID in the form
            string nodeLabelName = GetNodeLabelName(node.Type, node.NodeID);
            Control[] nodeLabels = Controls.Find(nodeLabelName, true);
            foreach (Control control in nodeLabels)
            {
                Label nodeLabel = control as Label;
                string newText = GenerateNodeLabelText(node.NodeID, node.Type, node.Container);
                nodeLabel.Text = newText;
            }
        }

        /// <summary>
        /// Method as Event handler to be added to all Labels that calls the ShowContent() Method when a Label is clicked.
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
        /// Method as Event handler to be added to all Buttons that calls the Presenter by building a fitting IInput object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButtonClick(object sender, EventArgs e)
        {
            Button button = sender as Button;
            // Step Button, Commissioning Button, Retrieval Button: let presenter update model with input (update model in view is called from presenter)
            // Retrieval: Clear TextBox (or fill new random text)
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method as Event handler to be added for TextBoxes to select all Text in the TextBox when clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextBoxDoubleClick(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.SelectAll();
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
    }
}
