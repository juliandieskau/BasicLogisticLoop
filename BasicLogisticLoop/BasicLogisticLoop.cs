using BasicLogisticLoop.Model;
using BasicLogisticLoop.Presenter.Input;
using BasicLogisticLoop.Presenter.Output;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
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
            ClientSize = Size;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            ShowIcon = false;

            // generate form controls here
            Controls.Add(GenerateSplitContainer());
            Shown += new EventHandler(FormShown);
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
                int index = NodeData.FindIndex(n => n.NodeID == changedNode.NodeID);
                if (index > -1)
                {
                    NodeData[index] = changedNode;
                }

                // Update the corresponding label in the model view with the new node
                UpdateNode(changedNode);

                // Update NodeDetails if changedNode is currently displayed
                if (NodeShown != null)
                {
                    ShowContent(NodeShown);
                }
            }

            // Update the corresponding row in the table view with each new nodes container
            UpdateNodesTable();
        }

        /// <summary>
        /// Updates the table view of warehouse containers with given list of containers.
        /// </summary>
        /// <param name="containers">List of containers in the warehouse of the model.</param>
        public void UpdateWarehouseContainers(List<Container> containers)
        {
            // Get warehouse containers and order them by their TUN
            List<Container> orderedWarehouse = containers.OrderBy(n => n.TransportUnitNumber).ToList();

            // Get Table to put data inside
            TableLayoutPanel warehouseTable = Controls.Find("storageModelTable", true).First() as TableLayoutPanel;

            warehouseTable.SuspendLayout();

            // Dynamically adjust table size depending on amount of containers
            int rowsDifference = orderedWarehouse.Count - (warehouseTable.RowCount - 2);
            ResizeTable(rowsDifference, warehouseTable, "storage");

            // Update table with containers
            int w = 0;
            while (w < orderedWarehouse.Count)
            {
                // Put Containers into table rows
                int r = w + 1; // row (legend is on 0)
                string name = orderedWarehouse[w].TransportUnitNumber.ToString();

                warehouseTable.GetControlFromPosition(0, r).Text = name;
                warehouseTable.GetControlFromPosition(1, r).Text = NodeTypeToString(orderedWarehouse[w].DestinationType);
                warehouseTable.GetControlFromPosition(2, r).Text = orderedWarehouse[w].Content;

                // Change name of TUN Label to the TUN to get which container to retrieve on click
                warehouseTable.GetControlFromPosition(0, r).Name = name;

                // Make TUN Label clickable for retrieving its container
                warehouseTable.GetControlFromPosition(0, r).DoubleClick -= OnWarehouseTUNLabelClick;
                warehouseTable.GetControlFromPosition(0, r).DoubleClick += new EventHandler(OnWarehouseTUNLabelClick);

                w++;
            }

            warehouseTable.ResumeLayout();
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

        private void UpdateNodesTable()
        {
            // Get NodeData and filter by the nodes that have containers on them and order them by their containers TUN
            List<ViewNode> nodesWithContainer = NodeData.FindAll(n => n.Container != null)
                                                        .OrderBy(n => n.Container.TransportUnitNumber).ToList();

            // Get Table to put data inside
            TableLayoutPanel nodesTable = Controls.Find("nodesModelTable", true).First() as TableLayoutPanel;

            nodesTable.SuspendLayout();

            // Dynamically adjust table size depending on amount of containers
            int rowsDifference = nodesWithContainer.Count - (nodesTable.RowCount - 2);
            ResizeTable(rowsDifference, nodesTable, "nodes");

            // Update Table
            int w = 0;
            while (w < nodesWithContainer.Count)
            {
                // Put Containers into table rows
                int r = w + 1; // row (legend is on 0)

                nodesTable.GetControlFromPosition(0, r).Text
                    = nodesWithContainer[w].Container.TransportUnitNumber.ToString();
                nodesTable.GetControlFromPosition(1, r).Text
                    = NodeTypeToString(nodesWithContainer[w].Container.DestinationType);
                nodesTable.GetControlFromPosition(2, r).Text
                    = nodesWithContainer[w].Container.Content;
                nodesTable.GetControlFromPosition(3, r).Text
                    = nodesWithContainer[w].NodeID.ToString();
                nodesTable.GetControlFromPosition(4, r).Text
                    = NodeTypeToString(nodesWithContainer[w].Type);

                w++;
            }

            nodesTable.ResumeLayout();
        }

        /// <summary>
        /// Dynamically adjust table size depending on amount of containers that are in the model in comparison to the rows of the table.
        /// </summary>
        /// <param name="rowsDifference">greater 0 if more new containers, smaller 0 if more rows than new containers</param>
        /// <param name="table">table to adjust size (amount of rows) of</param>
        /// <param name="type">"nodes" or "storage"</param>
        private void ResizeTable(int rowsDifference, TableLayoutPanel table, string type)
        {
            int rowCount = table.RowCount;
            // Remove rows if less containers to place in rows than tableRows
            if (rowsDifference < 0)
            {
                // delete last rows (adapted from https://stackoverflow.com/a/19717178 by user Arm0geddon, accessed 27.08.2024 10:15)
                for (int i = 0; i > rowsDifference; i--)
                {
                    // remove labels in 2nd last row from back to front (otherwise previous operations would mess with the positions of the rest)
                    int secondLastRow = table.RowCount - 2;
                    for (int col = table.ColumnCount - 1; col >= 0; col--)
                    {
                        Control c = table.GetControlFromPosition(col, secondLastRow); // is null
                        if (c != null)
                        {
                            // remove DoubleClick event from Label if it has the event
                            c.DoubleClick -= OnWarehouseTUNLabelClick;

                            table.Controls.Remove(c);
                            c.Dispose();
                        }
                    }

                    // remove last row (which is empty and 2nd last is now also empty)
                    table.RowCount--;
                }
            }
            // Add rows if more containers to place in rows than tableRows
            else if (rowsDifference > 0)
            {
                // add needed rows at the end
                table.RowCount += rowsDifference;
                for (int r = rowCount; r < rowsDifference + rowCount; r++)
                {
                    // Add labels into table into new rows
                    for (int c = 0; c < table.ColumnCount; c++)
                    {
                        Label label = GenerateTableLabel(GetTableLabelName(c, r, type), c, r);
                        table.Controls.Add(label);
                    }
                }
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
            if (label.Name.Contains(NodeTypeToString(NodeType.Conveyor)) |
                label.Name.Contains(NodeTypeToString(NodeType.Retrieval)) |
                label.Name.Contains(NodeTypeToString(NodeType.Commissioning)) |
                label.Name.Contains(NodeTypeToString(NodeType.Storage)) |
                label.Name.Contains(NodeTypeToString(NodeType.Warehouse)))
            {
                NodeShown = label;
                ShowContent(label);
            }
        }

        /// <summary>
        /// Method as Event handler to be added to all Buttons that calls the Presenter by building a fitting IInput object.
        /// </summary>
        /// <param name="sender">Clicked Button.</param>
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
                TextBox tunTextBox = Controls.Find("tunTextBox", true).First() as TextBox;
                int containerTUN = 0;
                try
                {
                    containerTUN = Int32.Parse(tunTextBox.Text);
                }
                catch (Exception ex)
                {
                    containerTUN = 0;
                }

                // let presenter update model with input (update model in view is called from presenter)
                IInput input = new RetrievalInput(nodeID, containerTUN, content);
                errorMessage = Presenter.ReceiveInput(input);

                // Fill TextBox with new random items to be used if user is lazy
                if (errorMessage == "")
                {
                    retrievalTextBox.Text = GetRandomContainerContent();
                    tunTextBox.Text = "";
                }
            }
            else if (button.Name == "retrievalStepButton")
            {
                // RETRIEVE
                // TODO only works when only one retrieval node exists
                // Get the input data to give the model, no error checking since errors here should crash the program (wrong logic)
                int nodeID = NodeData.Find(node => node.Type == NodeType.Retrieval).NodeID;
                TextBox retrievalTextBox = Controls.Find("retrievalTextBox", true).First() as TextBox;
                string content = retrievalTextBox.Text;
                TextBox tunTextBox = Controls.Find("tunTextBox", true).First() as TextBox;
                int containerTUN = 0;
                try
                {
                    containerTUN = Int32.Parse(tunTextBox.Text);
                }
                catch (Exception)
                {
                    containerTUN = 0;
                }

                // let presenter update model with input (update model in view is called from presenter)
                IInput retrievalInput = new RetrievalInput(nodeID, containerTUN, content);
                errorMessage = Presenter.ReceiveInput(retrievalInput);

                // Fill TextBox with new random items to be used if user is lazy
                if (errorMessage == "")
                {
                    retrievalTextBox.Text = GetRandomContainerContent();
                    tunTextBox.Text = "";
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
        /// <param name="sender">TextBox object</param>
        private void OnTextBoxDoubleClick(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.SelectAll();
        }

        /// <summary>
        /// Method as Event handler to be added for TUN Labels in the Warehouse Table to Retrieve the Container which's TUN was clicked.
        /// </summary>
        /// <param name="sender">Warehouse TUN Label</param>
        private void OnWarehouseTUNLabelClick(object sender, EventArgs e)
        {
            Label label = sender as Label;
            string errorMessage = "";

            if (label != null)
            {
                // Get the TUN of the container to retrieve
                int containerTUN = Int32.Parse(label.Name.Trim());

                // Get the first retrieval node to retrieve to
                int nodeID = NodeData.Find(node => node.Type == NodeType.Retrieval).NodeID;

                // let presenter update model with input
                IInput retrievalInput = new RetrievalInput(nodeID, containerTUN, "");
                errorMessage = Presenter.ReceiveInput(retrievalInput);

                if (errorMessage != "")
                {
                    // output log
                    ShowErrorMessage(errorMessage);
                }

                // Removing the retrieved container from warehouse table is done on updating the view from presenter
            }
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
                        (label.Name.Contains(NodeTypeToString(NodeType.Conveyor)) |
                            label.Name.Contains(NodeTypeToString(NodeType.Retrieval)) |
                            label.Name.Contains(NodeTypeToString(NodeType.Commissioning)) |
                            label.Name.Contains(NodeTypeToString(NodeType.Storage)) |
                            label.Name.Contains(NodeTypeToString(NodeType.Warehouse))))
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
                if (nodeLabel.Name == GetNodeLabelName(node.Type, node.NodeID))
                {
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
