using BasicLogisticLoop.Presenter.Output;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace BasicLogisticLoop
{
    internal static class BasicLogisticLoopHelper
    {

        public const string LabelBaseName = "Label";
        public static readonly Color LabelBackColor = Color.LightGray;
        public static readonly Color WindowBackColor = Color.White;

        /// <summary>
        /// Method to generate a label based on a ViewNode object.
        /// </summary>
        /// <param name="node">ViewNode object to represent as a label.</param>
        /// <param name="labelClick">labelClick: new EventHandler(OnLabelClick)</param>
        public static Label GenerateNodeLabel(ViewNode node, EventHandler labelClick)
        {
            // Convert node data to string representation
            string nodeType = NodeTypeToString(node.Type);
            string nodeID = node.NodeID.ToString();
            string containerTUN = "";
            if (node.Container != null)
            {
                containerTUN = node.Container.TransportUnitNumber.ToString();
            }

            string nodeName = GetNodeLabelName(node.Type, node.NodeID);
            string nodeText = nodeID + " " + nodeType + Environment.NewLine +
                                Environment.NewLine +
                                containerTUN;

            // Create the label
            Label label = new Label
            {
                Name = nodeName,
                Text = nodeText,
                AutoSize = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = LabelBackColor,
                Cursor = Cursors.Hand,
                Font = new Font(new FontFamily("Arial"), 20, FontStyle.Regular, GraphicsUnit.Pixel),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            // Add event handler
            label.Click += new EventHandler(labelClick);

            return label;
        }

        /// <summary>
        /// Method to create arrows that point between node labels to show to flow of containers in the model. 
        /// </summary>
        /// <param name="direction">One of the following strings: left, up, right, down, horizontal, vertical</param>
        /// <param name="index">Number of arrows added. (Start with 0)</param>
        public static Label GenerateArrowLabel(string direction, int index)
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
                case "horizontal":
                    arrow = "\u21D4"; break;
                case "vertical":
                    arrow = "\u21D5"; break;
                default:
                    throw new ArgumentException("Not a valid direction string.");
            }

            Label label = new Label
            {
                Name = GetArrowLabelName(direction, index),
                Text = arrow,
                AutoSize = true,
                BorderStyle = BorderStyle.None,
                Cursor = Cursors.Default,
                Font = new Font(new FontFamily("Arial"), 60, FontStyle.Regular, GraphicsUnit.Pixel),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            return label;
        }

        /// <summary>
        /// Method to generate the button that controls retrieving items from the warehouse onto the retrieval node.
        /// </summary>
        /// <param name="text">Text to display in the Button.</param>
        /// <param name="buttonClick">EventHandler of the View to be added on the ButtonClick event.</param>
        /// <returns>Generated Button.</returns>
        public static Button GenerateRetrievalButton(string text, EventHandler buttonClick)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method to generate a button for the lower panel of the model (Commission, Step).
        /// </summary>
        /// <param name="text">Text to display in the Button.</param>
        /// <param name="buttonClick">EventHandler of the View to be added on the ButtonClick event.</param>
        /// <returns>Generated Button.</returns>
        public static Button GeneratePanelButton(string text, EventHandler buttonClick)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method to determine the text a certain node should be displayed as in the view.
        /// </summary>
        /// <param name="type">NodeType to display as text.</param>
        /// <returns>String to display.</returns>
        public static string NodeTypeToString(NodeType type)
        {
            switch (type)
            {
                case NodeType.Conveyor:
                    return "Conveyor";
                case NodeType.Retrieval:
                    return "Retrieval";
                case NodeType.Storage:
                    return "Storage";
                case NodeType.Commissioning:
                    return "Commissioning";
                case NodeType.Warehouse:
                    return "Warehouse";
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
        public static string GetNodeLabelName(NodeType type, int nodeID)
        {
            return NodeTypeToString(type) + LabelBaseName + nodeID.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetArrowLabelName(string direction, int index)
        {
            return direction + "Arrow" + LabelBaseName + index.ToString();
        }
    }
}
