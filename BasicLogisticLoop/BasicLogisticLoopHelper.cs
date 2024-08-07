﻿using BasicLogisticLoop.Presenter.Output;
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
    public partial class BasicLogisticLoopForm
    {

        private const string LabelBaseName = "Label";
        private readonly Color LabelBackColor = Color.LightGray;
        private readonly Color WindowBackColor = Color.WhiteSmoke;

        /// <summary>
        /// Method to generate a label based on a ViewNode object.
        /// </summary>
        /// <param name="node">ViewNode object to represent as a label.</param>
        private Label GenerateNodeLabel(ViewNode node)
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
            label.Click += new EventHandler(OnLabelClick);

            return label;
        }

        /// <summary>
        /// Method to create arrows that point between node labels to show to flow of containers in the model. 
        /// </summary>
        /// <param name="direction">One of the following strings: left, up, right, down, horizontal, vertical</param>
        /// <param name="index">Number of arrows added. (Start with 0)</param>
        /// <exception cref="ArgumentException">When the direction string doesn't match the given pattern.</exception>
        private Label GenerateArrowLabel(string direction, int index)
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
        /// Method to generate a button for the view.
        /// </summary>
        /// <param name="buttonType">Type of button to generate: Match step, commission, retrieval</param>
        /// <returns>Generated Button.</returns>
        /// <exception cref="ArgumentException">When buttonType doesn't match the specified pattern.</exception>
        private Button GenerateButton(string buttonType)
        {
            // Apply Button attributes that are different per case
            DockStyle dockStyle = DockStyle.None;
            string buttonText = "";

            switch (buttonType)
            {
                case "step":
                    dockStyle = DockStyle.Right;
                    buttonText = "Forward cycle one step";
                    break;
                case "commission":
                    dockStyle = DockStyle.Right;
                    buttonText = "Commission container";
                    break;
                case "retrieval":
                    dockStyle = DockStyle.Fill;
                    buttonText = "Retrieve container";
                    break;
                default:
                    throw new ArgumentException("buttonType has to be either step, commission or retrieval!");
            }

            // Create Button
            Button button = new Button
            {
                Name = buttonType + "Button",
                Text = buttonText,
                AutoSize = true,
                BackColor = WindowBackColor,
                Font = new Font(new FontFamily("Arial"), 16, FontStyle.Bold, GraphicsUnit.Pixel),
                TextAlign = ContentAlignment.MiddleCenter
            };

            button.Click += new EventHandler(OnButtonClick);

            return button;
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
        /// Makes a string to use as a name in labels for nodes of the model.
        /// </summary>
        /// <param name="type">NodeType to show which kind of node it is.</param>
        /// <param name="nodeID">ID to represent which node in the model it belongs to.</param>
        /// <returns></returns>
        private string GetNodeLabelName(NodeType type, int nodeID)
        {
            return NodeTypeToString(type) + LabelBaseName + nodeID.ToString();
        }

        /// <summary>
        /// Makes a string to use as a name in labels for nodes of the arrows in the model.
        /// </summary>
        /// <param name="direction">Direction the arrow is pointing towards (left, up, right, down, horizontal, vertical).</param>
        /// <param name="index">>Number of arrows added. (Start with 0)</param>
        /// <returns>Name for arrow label.</returns>
        private string GetArrowLabelName(string direction, int index)
        {
            return direction + "Arrow" + LabelBaseName + index.ToString();
        }


    }
}
