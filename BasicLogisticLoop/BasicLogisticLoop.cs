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

        private const string LabelBaseName = "Label";
        private readonly Color LabelBackColor = Color.White;
        private readonly Color WindowBackColor = Color.White;

        /// <summary>
        /// Constructor for the Form. Initializes the Presenter and gives it this Form as an argument.
        /// </summary>
        public BasicLogisticLoopForm()
        {
            Presenter = new Presenter.Presenter(this);
            InitializeComponent();
        }

        /// <summary>
        /// Takes a list of ViewNodes and constructs the view on the basis of it. Needs to be called once for the view to work.
        /// </summary>
        /// <param name="initialViewNodes">List of ViewNodes to use as a basis to build the View out of.</param>
        public void InitializeView(List<ViewNode> initialViewNodes)
        {
            NodeData = initialViewNodes;

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
        /// Event handler to be added to all Labels that calls the Presenter by building a fitting IInput object.
        /// </summary>
        /// <param name="sender">Label that was clicked.</param>
        /// <param name="e"></param>
        private void OnLabelClick(object sender, EventArgs e)
        {
            Label label = sender as Label;
            // For Step Label
            // For all Node Labels: 
            //      When clicked display its content in label on the side (ShowContent)
            // For Commissioning, Retrieval:
            //      When clicked also call 
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
        /// Method to generate a label based on a ViewNode object.
        /// </summary>
        /// <param name="node">ViewNode object to represent as a label.</param>
        private void GenerateNodeLabel(ViewNode node)
        {
            // Convert node data to string representation
            string nodeType = NodeTypeToString(node.Type);
            string nodeID = node.NodeID.ToString();
            string containerTUI = "empty";
            if (node.Container != null)
            {
               containerTUI = node.Container.TransportUnitNumber.ToString();
            }

            string nodeName = GetNodeLabelName(node.Type, node.NodeID);
            string nodeText = nodeType + nodeID + "\n" + "Content: " + containerTUI;

            // Create the label
            CreateLabel(name: nodeName, text: nodeText);
        }

        /// <summary>
        /// Method to generate a label to be used as a button to forward the model one step.
        /// </summary>
        private void GenerateStepLabel()
        {
            // Create the label
            CreateLabel(name: "StepLabel", text: "One step forward..");
        }

        /// <summary>
        /// Adapter-Method to create label objects in a specified format. Adds the label to the Form's Controls.
        /// Always use this method to create labels!
        /// </summary>
        /// <param name="name">Name of the label to access it by.</param>
        /// <param name="text">Text of th label to be displayed.</param>
        /// <returns>Created label.</returns>
        private void CreateLabel(string name, string text)
        {
            Label label = new Label
            {
                Name = name,
                Text = text,
                AutoSize = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = LabelBackColor,
                Cursor = Cursors.Hand,
                Font = new Font(new FontFamily("Arial"), 16, FontStyle.Regular, GraphicsUnit.Pixel)
            };

            label.Click += new EventHandler(OnLabelClick);

            Controls.Add(label);
        }

        /// <summary>
        /// Method to create arrows that point between node labels to show to flow of containers in the model. 
        /// </summary>
        /// <param name="direction">One of the following strings: left, up, right, down, doubleHorizontal, doubleVertical</param>
        /// <param name="index">Number of arrows added. (Start with 0)</param>
        private void CreateArrowLabel(string direction, int index)
        {
            string arrow = "";
            switch (direction)
            {
                case "left":
                    arrow = "\u21D0"; break;
                case "up":
                    arrow = "\u21D1"; break;
                case "right":
                    arrow = "\u21D2"; break;
                case "down":
                    arrow = "\u21D3"; break;
                case "doubleHorizontal":
                    arrow = "\u21D4"; break;
                case "doubleVertical":
                    arrow = "\u21D5"; break;
                default:
                    throw new ArgumentException("Not a valid direction string.");
            }

            Label label = new Label
            {
                Name = direction + index.ToString(),
                Text = arrow,
                AutoSize = true,
                BorderStyle = BorderStyle.None,
                Cursor = Cursors.Default,
                Font = new Font(new FontFamily("Arial"), 20, FontStyle.Regular, GraphicsUnit.Pixel)
            };

            Controls.Add(label);
        }

        /// <summary>
        /// Method to determine the text a certain node should be displayed as in the view.
        /// </summary>
        /// <param name="type">NodeType to display as text.</param>
        /// <returns>String to display.</returns>
        private string NodeTypeToString(NodeType type)
        {
            switch (type)
            {
                case NodeType.Conveyor:
                    return "";
                case NodeType.Retrieval:
                    return "R";
                case NodeType.Storage:
                    return "S";
                case NodeType.Commissioning:
                    return "C";
                case NodeType.Warehouse:
                    return "W";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Makes a string to use as names in labels for nodes of the model.
        /// </summary>
        /// <param name="type">NodeType to show which kind of node it is.</param>
        /// <param name="nodeID">ID to represent which node in the model it belongs to.</param>
        /// <returns></returns>
        private string GetNodeLabelName(NodeType type, int nodeID)
        {
            return NodeTypeToString(type) + LabelBaseName + nodeID.ToString();
        }
    }
}
