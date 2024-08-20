using BasicLogisticLoop.Model;
using BasicLogisticLoop.Presenter;
using BasicLogisticLoop.Presenter.Input;
using BasicLogisticLoop.Presenter.Output;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

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
            ClientSize = Size;
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
                // Update the ViewNode in the views NodeData to the new node
                int index = NodeData.FindIndex(n =>  n.NodeID == changedNode.NodeID);
                if (index > -1)
                {
                    NodeData[index] = changedNode;
                }

                // Update the corresponding label in the view with the new node
                UpdateNode(changedNode);

                // Update NodeDetails if changedNode is currently displayed
                if (NodeShown != null)
                {
                    ShowContent(NodeShown);
                }
            }
        }

        /// <summary>
        /// Updates the labels in the view representing the given ViewNode with its data.
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
        /// <param name="e">Event Arguments.</param>
        private void OnLabelClick(object sender, EventArgs e)
        {
            Label label = sender as Label;
            // For Node Labels: 
            //      When clicked display its content in label on the right side panel
            if (label.Name.Contains(NodeTypeToString(NodeType.Conveyor))        |
                label.Name.Contains(NodeTypeToString(NodeType.Retrieval))       |
                label.Name.Contains(NodeTypeToString(NodeType.Commissioning))   |
                label.Name.Contains(NodeTypeToString(NodeType.Storage))         |
                label.Name.Contains(NodeTypeToString(NodeType.Warehouse))) {
                NodeShown = label;
                ShowContent(label);
            }
        }

        /// <summary>
        /// Method as Event handler to be added to all Buttons that calls the Presenter by building a fitting IInput object.
        /// </summary>
        /// <param name="sender">Clicked Button.</param>
        /// <param name="e">Event Arguments.</param>
        private void OnButtonClick(object sender, EventArgs e)
        {
            // Step Button, Commissioning Button, Retrieval Button:
            //      let presenter update model with input (update model in view is called from presenter)
            // Retrieval:
            //      Clear TextBox (or fill new random text)
            Button button = sender as Button;
            string errorMessage = "";
            if (button.Name == "stepButton")
            {
                // let presenter update model with input (update model in view is called from presenter)
                IInput input = new StepInput();
                errorMessage = Presenter.ReceiveInput(input);
            }
            else if (button.Name == "commissionButton")
            {
                // search ViewNodes for (first) Commission-Node and get its NodeID
                // For more than one commission node change here to get specific labels ID instead
                // and at the commissionButton to specify which node to commission
                int nodeID = NodeData.Find(node => node.Type == NodeType.Commissioning).NodeID;

                // let presenter update model with input (update model in view is called from presenter)
                IInput input = new CommissionInput(nodeID);
                errorMessage = Presenter.ReceiveInput(input);
            }
            else if (button.Name == "retrievalButton")
            {
                // Get the input data to give the model, no error checking since errors here should crash the program (wrong logic)
                int nodeID = NodeData.Find(node => node.Type == NodeType.Retrieval).NodeID;
                TextBox retrievalTextBox = Controls.Find("retrievalTextBox", true).First() as TextBox;
                string content = retrievalTextBox.Text;

                // let presenter update model with input (update model in view is called from presenter)
                IInput input = new RetrievalInput(nodeID, content);
                errorMessage = Presenter.ReceiveInput(input);

                // Fill TextBox with new random items to be used if user is lazy
                if (errorMessage == "")
                {
                    retrievalTextBox.Text = GetRandomContainerContent();
                }
            }
            else if (button.Name == "retrievalStepButton")
            {
                // RETRIEVE
                // Get the input data to give the model, no error checking since errors here should crash the program (wrong logic)
                int nodeID = NodeData.Find(node => node.Type == NodeType.Retrieval).NodeID;
                TextBox retrievalTextBox = Controls.Find("retrievalTextBox", true).First() as TextBox;
                string content = retrievalTextBox.Text;

                // let presenter update model with input (update model in view is called from presenter)
                IInput retrievalInput = new RetrievalInput(nodeID, content);
                errorMessage = Presenter.ReceiveInput(retrievalInput);

                // Fill TextBox with new random items to be used if user is lazy
                if (errorMessage == "")
                {
                    retrievalTextBox.Text = GetRandomContainerContent();
                }

                // STEP
                // let presenter update model with input (update model in view is called from presenter)
                IInput stepInput = new StepInput();
                errorMessage = Presenter.ReceiveInput(stepInput);
            }
            
            // Output error message if receiving input failed
            if (errorMessage != "")
            {
                ShowErrorMessage(errorMessage);
            }
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
        /// <param name="nodeLabel">Label to show the content of.</param>
        private void ShowContent(Label nodeLabel)
        {
            // unhighlight all labels (reset to normal color)
            Control modelPanel = Controls.Find("modelTableLayoutPanel", true).FirstOrDefault();
            if (modelPanel != null)
            {
                // search all labels in model panel
                foreach (Control control in modelPanel.Controls)
                {
                    Label label = control as Label;
                    // only for node labels
                    if (label != null &&
                        (   label.Name.Contains(NodeTypeToString(NodeType.Conveyor))        |
                            label.Name.Contains(NodeTypeToString(NodeType.Retrieval))       |
                            label.Name.Contains(NodeTypeToString(NodeType.Commissioning))   |
                            label.Name.Contains(NodeTypeToString(NodeType.Storage))         |
                            label.Name.Contains(NodeTypeToString(NodeType.Warehouse))       ))
                    {
                        // unhighlight label: set background color
                        label.BackColor = LabelBackColor;
                    }
                }
            }

            // search the NodeData for the corresponding ViewNode of the clicked node label
            foreach (ViewNode node in NodeData)
            {
                // compare the node labels name with the view nodes name
                if (nodeLabel.Name == GetNodeLabelName(node.Type, node.NodeID)) {
                    // Get the Controls of the Node Details part of the side bar
                    Label nodeTypeLabel = Controls.Find(LabelType.GetName(LabelType.NodeType), true).First() as Label;
                    Label nodeIDLabel = Controls.Find(LabelType.GetName(LabelType.NodeID), true).First() as Label;
                    Label tunLabel = Controls.Find(LabelType.GetName(LabelType.TUN), true).First() as Label;
                    Label destinationLabel = Controls.Find(LabelType.GetName(LabelType.Destination), true).First() as Label;
                    TextBox containerTextBox = Controls.Find("containerTextBox", true).First() as TextBox;

                    // check if node has container
                    string tunText = LabelType.GetText(LabelType.TUN);
                    string destinationText = LabelType.GetText(LabelType.Destination);
                    string containerText = "<empty>";
                    tunText = node.Container == null 
                        ? tunText + "<empty>" 
                        : tunText + node.Container.TransportUnitNumber.ToString();
                    destinationText = node.Container == null
                        ? destinationText + "<empty>"
                        : destinationText + NodeTypeToString(node.Container.DestinationType);
                    if (node.Container != null) { containerText = node.Container.Content; }

                    // Put the content of the  into the Controls
                    nodeTypeLabel.Text = LabelType.GetText(LabelType.NodeType) + NodeTypeToString(node.Type);
                    nodeIDLabel.Text = LabelType.GetText(LabelType.NodeID) + node.NodeID.ToString();
                    tunLabel.Text = tunText;
                    destinationLabel.Text = destinationText;
                    containerTextBox.Text = containerText;

                    // If label not found program should crash (wrong logic), so no error handling here

                    // hightlight label: set background color
                    nodeLabel.BackColor = LabelBackColorHighlighted;
                }
            }
            return;
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
