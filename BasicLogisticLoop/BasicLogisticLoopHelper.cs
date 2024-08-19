using BasicLogisticLoop.Presenter.Output;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BasicLogisticLoop.Model;

namespace BasicLogisticLoop
{
    public partial class BasicLogisticLoopForm
    {
        // CONSTANTS

        private const string LabelBaseName = "Label";
        private readonly Color LabelBackColor = Color.LightGray;
        private readonly Color WindowBackColor = Color.WhiteSmoke;

        // VARIABLES

        private (int x, int y) minModelCords, maxModelCords;

        // #################################################
        // PANEL GENERATOR METHODS

        /// <summary>
        /// FORM
        /// Method that generates a container that splits the view in a left and right part.
        /// Calls all other generating methods, so only call this once. 
        /// </summary>
        /// <returns>Created SplitContainer.</returns>
        private SplitContainer GenerateSplitContainer()
        {
            SplitContainer splitContainer = new SplitContainer()
            {
                BorderStyle = BorderStyle.FixedSingle,
                Name = "splitContainer",
                Dock = DockStyle.Fill,
                IsSplitterFixed = true,
                SplitterDistance = 1200
            };

            // add left and right panel 
            splitContainer.Panel1.Controls.Add(GenerateLeftPanel());
            splitContainer.Panel2.Controls.Add(GenerateRightPanel());

            return splitContainer;
        }

        /// <summary>
        /// SPLIT CONTAINER Panel1
        /// Method that generates a panel to hold the model and the buttons above each other.
        /// </summary>
        /// <returns></returns>
        private TableLayoutPanel GenerateLeftPanel()
        {
            TableLayoutPanel panel = new TableLayoutPanel()
            {
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Name = "leftTableLayoutPanel",
                AutoSize = false,
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize
            };

            // add model and buttons above one another
            panel.Controls.Add(GenerateModelPanel());
            panel.Controls.Add(GenerateButtonsPanel());

            // set rows size
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 94.0f));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 6.0f));

            return panel;
        }

        /// <summary>
        /// LEFT PANEL
        /// Method that generates a panel that holds all model data in a grid.
        /// </summary>
        /// <returns>Created Panel.</returns>
        private TableLayoutPanel GenerateModelPanel()
        {
            // Get size of model (min and max coordinate(x, y) values of viewnodes)
            minModelCords = (Int32.MaxValue, Int32.MaxValue);
            maxModelCords = (Int32.MinValue, Int32.MinValue);
            foreach (ViewNode node in NodeData)
            {
                // if any coordinate in the model is outside of the current bounds, widen the bounds
                if (node.Coordinates.X < minModelCords.x)
                {
                    minModelCords.x = node.Coordinates.X;
                }
                if (node.Coordinates.Y < minModelCords.y)
                {
                    minModelCords.y = node.Coordinates.Y;
                }
                if (node.Coordinates.X > maxModelCords.x)
                {
                    maxModelCords.x = node.Coordinates.X;
                }
                if (node.Coordinates.Y > maxModelCords.y)
                {
                    maxModelCords.y = node.Coordinates.Y;
                }
            }

            // Transform into size of view, where between every coordinate of the model a coordinate for arrows gets inserted
            (int x, int y) modelSize = (Math.Abs(maxModelCords.x - minModelCords.x) + 1,
                                       Math.Abs(maxModelCords.y - minModelCords.y) + 1);
            (int x, int y) viewSize = ((modelSize.x * 2) - 1, (modelSize.y * 2) - 1);

            // Generate panel with columns and rows with size
            TableLayoutPanel panel = new TableLayoutPanel()
            {
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Name = "modelTableLayoutPanel",
                AutoSize = false,
                Dock = DockStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ColumnCount = viewSize.y,
                RowCount = viewSize.x,
            };

            // Calculate the column and row sizes so that a node- is double the size of an arrow-label
            int dividerX = (modelSize.x * 3) - 1;
            int dividerY = (modelSize.y * 3) - 1;
            float arrowSizeXPercent = 100.0f / dividerX;
            float arrowSizeYPercent = 100.0f / dividerY;
            float nodeSizeXPercent = arrowSizeXPercent * 2;
            float nodeSizeYPercent = arrowSizeYPercent * 2;

            // Set the size type and value of each row and column
            for (int x = 0; x < viewSize.x; x++)
            {
                // Columns
                if (x % 2 == 0)
                {
                    // Node Labels
                    panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, nodeSizeXPercent));
                } 
                else
                {
                    // Arrow Labels
                    panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, arrowSizeXPercent));
                }
            }
            for (int y = 0; y < viewSize.y; y++)
            {
                // Rows
                if (y % 2 == 0)
                {
                    // Node Labels
                    panel.RowStyles.Add(new RowStyle(SizeType.Percent, nodeSizeXPercent));
                }
                else
                {
                    // Arrow Labels
                    panel.RowStyles.Add(new RowStyle(SizeType.Percent, arrowSizeYPercent));
                }
            }

            // Place node labels and arrow labels 
            int arrowNumber = 0;
            foreach (ViewNode node in NodeData)
            {
                // Generate model labels and place them on the even numbered rows and columns (0, 2, 4, ...)
                // depending on the transformed ViewNode.Coordinates
                (int column, int row) nodePosition = TransformCoordinatesModelToView(node.Coordinates);
                Label nodeLabel = GenerateNodeLabel(node);

                panel.SetRow(nodeLabel, nodePosition.row);
                panel.SetColumn(nodeLabel, nodePosition.column);
                panel.Controls.Add(nodeLabel);

                // if node is storage node -> add all warehouse nodes to following nodes (not represented in graph!)
                // loop will check if the warehouse is the one that is in the correct position for the storage node
                List<int> followingNodes = node.FollowingNodes.ToList();
                if (node.Type == NodeType.Storage)
                {
                    followingNodes.AddRange(NodeData.FindAll(x => x.Type == NodeType.Warehouse)
                                                    .Select(n => n.NodeID).ToList()
                                           );
                }

                // Generate arrow labels by checking which nodes are adjacent to the current one
                // placing the arrow in the mean of the node and each following node (if distance is 2 rows/columns)
                foreach (int followingID in followingNodes)
                {
                    ViewNode followingNode = NodeData.Find(n => n.NodeID == followingID);
                    // check if arrow can be validly placed
                    if (ArrowIsPlacable(node.Coordinates, followingNode.Coordinates))
                    {
                        // calculate the arrows position in the panel
                        (int column, int row) followingPosition = TransformCoordinatesModelToView(followingNode.Coordinates);
                        int arrowColumnPos = (nodePosition.column + followingPosition.column) / 2;
                        int arrowRowPos = (nodePosition.row + followingPosition.row) / 2;

                        // calculate the arrow string by its direction
                        string direction = GetArrowDirection(node.Coordinates, followingNode.Coordinates);

                        // Check if panel already has an arrow (in other direction) on the given row and column
                        bool hasLabel = false;
                        foreach (Label existingLabel in panel.Controls.OfType<Label>())
                        {
                            if (panel.GetColumn(existingLabel) == arrowColumnPos && panel.GetRow(existingLabel) == arrowRowPos)
                            {
                                hasLabel = true;

                                // make the arrow of the existing label a double arrow
                                existingLabel.Text = DoubleArrow(existingLabel.Text);
                            }
                        }

                        if (!hasLabel)
                        {
                            // otherwise place arrow label on panel
                            Label arrowLabel = GenerateArrowLabel(direction, arrowNumber++);
                            panel.SetColumn(arrowLabel, arrowColumnPos);
                            panel.SetRow(arrowLabel, arrowRowPos);
                            panel.Controls.Add(arrowLabel);
                        }

                        // arrow label is now placed if valid
                    }
                    // if not valid, just skip the edge without throwing an error (-> might be warehouse nodes that arent adjacent to storage nodes)
                }
            }
            
            return panel;
        }

        /// <summary>
        /// LEFT PANEL
        /// Method that generates a panel that holds the step and commission button.
        /// </summary>
        /// <returns>Created Panel.</returns>
        private Panel GenerateButtonsPanel()
        {
            Panel panel = new Panel()
            {
                BorderStyle = BorderStyle.None,
                RightToLeft = RightToLeft.Yes,
                Name = "buttonsPanel",
                Dock = DockStyle.Fill,
                AutoSize = false
            };

            // create and add buttons from right to left
            panel.Controls.Add(GenerateButton("step"));
            panel.Controls.Add(GenerateButton("commission"));

            return panel;
        }

        /// <summary>
        /// SPLIT CONTAINER Panel2
        /// Method that generates a Panel that holds Labels, TextBoxes, Buttons and Panels for the Retrieval-Input and ShowContent.
        /// </summary>
        /// <returns>Created Panel.</returns>
        private TableLayoutPanel GenerateRightPanel()
        {
            TableLayoutPanel panel = new TableLayoutPanel()
            {
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Name = "rightTableLayoutPanel",
                AutoSize = true,
                Dock = DockStyle.Bottom,
                ColumnCount = 1,
                RowCount = 9
            };

            // create and add controls
            panel.Controls.AddRange(new Control[]
            {
                GenerateRightPanelLabel("retrieval"),
                GenerateRightPanelLabel("description"),
                GenerateTextBox(false),
                GenerateButton("retrieval"),
                GenerateRightPanelLabel("nodeDetails"),
                GenerateRightPanelLabel("nodeType"),
                GenerateRightPanelLabel("nodeID"),
                GenerateRightPanelLabel("container"),
                GenerateContainerPanel()
            });


            // set rows size
            for (int index = 0; index < 9; index++)
            {
                if (index == 2)
                {
                    panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 200));
                }
                else
                {
                    panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                }
            }

            return panel;
        }

        /// <summary>
        /// RIGHT PANEL
        /// Method that generates a Panel that holds Labels that shows the container of the node label that is being clicked on.
        /// </summary>
        /// <returns>Created Panel.</returns>
        private TableLayoutPanel GenerateContainerPanel()
        {
            TableLayoutPanel panel = new TableLayoutPanel
            {
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                Name = "containerTableLayoutPanel",
                AutoSize = true,
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4
            };

            // create controls and add to panel in order
            panel.Controls.AddRange(new Control[] { 
                GenerateRightPanelLabel("tun"), 
                GenerateRightPanelLabel("destination"),
                GenerateRightPanelLabel("content"),
                GenerateTextBox(true)
            });

            // set rows size
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 200)); // TextBox

            return panel;
        }

        // #################################################
        // CONTROLS GENERATOR METHODS

        /// <summary>
        /// Method to generate a label based on a ViewNode object.
        /// </summary>
        /// <param name="node">ViewNode object to represent as a label.</param>
        private Label GenerateNodeLabel(ViewNode node)
        {
            // Convert node data to string representation
            string nodeName = GetNodeLabelName(node.Type, node.NodeID);
            string nodeText = GenerateNodeLabelText(node.NodeID, node.Type, node.Container);

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
        /// <param name="direction">One of the following strings: left, up, right, down</param>
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
        /// Generates a label for the right panel of the view. Outputs labels with attributes depending on the given labelType.
        /// </summary>
        /// <param name="labelType">LabelType constant string</param>
        /// <returns>Label with corresponding attributes.</returns>
        private Label GenerateRightPanelLabel(string labelType) 
        {
            // default label attribute values
            BorderStyle border = BorderStyle.None;
            FontStyle fontStyle = FontStyle.Regular;
            string text = "";

            text = LabelType.GetText(labelType);
            // special values depending on label
            switch (labelType)
            {
                case LabelType.Retrieval:
                    border = BorderStyle.FixedSingle;
                    fontStyle = FontStyle.Bold;
                    break;
                case LabelType.NodeDetails:
                    border = BorderStyle.FixedSingle;
                    fontStyle = FontStyle.Bold;
                    break;
                case LabelType.Container:
                    fontStyle = FontStyle.Bold; 
                    break;
                case LabelType.Description:
                    break;
                case LabelType.NodeType:
                    text += "<select node>";
                    break;
                case LabelType.NodeID:
                    text += "<select node>";
                    break;
                case LabelType.TUN:
                    text += "<select node>";
                    break;
                case LabelType.Destination:
                    text += "<select node>";
                    break;
                case LabelType.Content:
                    break;
            }

            // create label
            Label label = new Label
            {
                Name = LabelType.GetName(labelType),
                Text = text,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(new FontFamily("Arial"), 16, fontStyle, GraphicsUnit.Pixel),
                AutoSize = true,
                Dock = DockStyle.Fill,
                BorderStyle = border,
                Margin = new Padding(3),
                Padding = new Padding(5)
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
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = dockStyle
            };

            button.Click += new EventHandler(OnButtonClick);

            return button;
        }

        /// <summary>
        /// Method to generate a textbox to enter or display text input by a user.
        /// </summary>
        /// <param name="readOnly">True if for displaying text, false for entering.</param>
        /// <returns>Generated TextBox.</returns>
        private TextBox GenerateTextBox(bool readOnly)
        {
            string exampleText = GetRandomContainerContent(); // generator for random text (supermarket products)

            TextBox textBox = new TextBox
            {
                Name = readOnly ? "containerTextBox" : "retrievalTextBox",
                ReadOnly = readOnly,
                MaxLength = 400,
                Multiline = true,
                BorderStyle = BorderStyle.Fixed3D,
                Text = exampleText,
                Font = new Font(new FontFamily("Arial"), 16, FontStyle.Regular, GraphicsUnit.Pixel),
                Dock = DockStyle.Fill,
                Margin = new Padding(3)
            };

            textBox.DoubleClick += new EventHandler(OnTextBoxDoubleClick);

            return textBox;
        }

        // #################################################
        // HELPER METHODS

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
        /// The string to use as a name in labels for nodes of the model.
        /// </summary>
        /// <param name="type">NodeType to show which kind of node it is.</param>
        /// <param name="nodeID">ID to represent which node in the model it belongs to.</param>
        /// <returns>string that should be set as the Label.Name property.</returns>
        private string GetNodeLabelName(NodeType type, int nodeID)
        {
            return NodeTypeToString(type) + LabelBaseName + nodeID.ToString();
        }

        /// <summary>
        /// The string to use as a name in labels for nodes of the arrows in the model.
        /// </summary>
        /// <param name="direction">Direction the arrow is pointing towards (left, up, right, down, horizontal, vertical).</param>
        /// <param name="index">>Number of arrows added. (Start with 0)</param>
        /// <returns>Name for arrow label.</returns>
        private string GetArrowLabelName(string direction, int index)
        {
            return direction + "Arrow" + LabelBaseName + index.ToString();
        }

        /// <summary>
        /// Generates the text to display on a label in the model part of the view.
        /// </summary>
        /// <param name="nodeID">ID to represent which node in the model it belongs to.</param>
        /// <param name="nodeType">Type of the node to display</param>
        /// <param name="containerTUN">TransportUnit Number of the container on the node.</param>
        /// <returns></returns>
        private string GenerateNodeLabelText(int nodeID, NodeType nodeType, Container container)
        {
            string containerTUN = "";
            if (container != null)
            {
                containerTUN = container.TransportUnitNumber.ToString();
            }
            return nodeID.ToString() + " " + NodeTypeToString(nodeType) + Environment.NewLine +
                                Environment.NewLine +
                                containerTUN;
        }

        /// <summary>
        /// Returns a string of random words to be used as the content for a retrieved container, if not input by the user.
        /// Uses an array of supermarket items to pick from.
        /// </summary>
        /// <returns>Generated string of words.</returns>
        private string GetRandomContainerContent()
        {
            Random random = new Random();
            string[] words = new string[]
            {
                "Mini Rispentomaten", "Passionsfrucht", "Nektarinen", "Mirabellen",
                "Mix Tafeltrauben", "Wassermelone", "Saftorangen", "Staudensellerie",
                "Romana Salatherzen", "Dunkle Bio Tafeltrauben", "Bananen", "Braune Champignons",
                "Helle Bio Tafeltrauben", "Roma Rispentomaten", "Bio Broccoli", "Broccoli",
                "Fleischtomaten", "Porree", "Heidelbeeren", "Paprika", "Bio Rispentomaten",
                "Frischkäse", "Schokoladen Pudding", "Käseaufschnitt", "Schafskäse",
                "Frucht Buttermilch", "Grillkäse", "Vegane Rostbratwürstchen", "High Protein Kirsch Pudding",
                "Erbsenproteindrink", "Vegane Cordon bleu", "Tofu natur", "Mozarella light",
                "Steinofen Ciabatta", "Bio Röstkaffee", "Erdbeer Konfitüre", "Bio Haferpops"
            };
            string content = "";
            for (int i = 0; i < random.Next(9); i++)
            {
                content += words[random.Next(words.Length)] + Environment.NewLine;
            }
            return content += words[random.Next(words.Length)];
        }

        // #################################################
        // Types

        /// <summary>
        /// Class to use to access right panel labels by their name and to get their text.
        /// </summary>
        private class LabelType
        {
            public const string Retrieval = "retrieval";
            public const string NodeDetails = "nodeDetails";
            public const string Container = "container";
            public const string Description = "description";
            public const string NodeType = "nodeType";
            public const string NodeID = "nodeID";
            public const string TUN = "tun";
            public const string Destination = "destination";
            public const string Content = "content";

            /// <summary>
            /// Returns the text to display for a Label of the given type.
            /// </summary>
            /// <param name="labelType">One of the LabelType strings.</param>
            /// <returns>Text to display in label.</returns>
            public static string GetText(string labelType)
            {
                if (labelType == Retrieval)
                {
                    return "Retrieval";
                }
                if (labelType == NodeDetails)
                {
                    return "Node Details";
                }
                if (labelType == Container)
                {
                    return "Container: ";
                }
                if (labelType == Description)
                {
                    return "Input content of next from" +
                            Environment.NewLine +
                            "warehouse retrieved container:";
                }
                if (labelType == NodeType)
                {
                    return "Node Type: ";
                }
                if (labelType == NodeID)
                {
                    return "Node ID: ";
                }
                if (labelType == TUN)
                {
                    return "TransportUnitNumber: ";
                }
                if (labelType == Destination)
                {
                    return "Destination: ";
                }
                if (labelType == Content)
                {
                    return "Content: ";
                }
                return "";
            }

            /// <summary>
            /// Returns the name of the label of the given type.
            /// </summary>
            /// <param name="labelType">One of the LabelType strings.</param>
            /// <returns>Text to use as name of label.</returns>
            public static string GetName(string labelType)
            {
                return labelType + "Label";
            }
        }

        // #################################################
        // MODEL METHODS

        /// <summary>
        /// Calculates the direction an arrow should be pointing towards depending on the coordinates of the two nodes.
        /// Diagonal direction not possible.
        /// </summary>
        /// <param name="fromCoords">x and y coordinates of node the arrow starts from.</param>
        /// <param name="toCoords">x and y coordinates of node the arrow points towards.</param>
        /// <returns>left, up, right, down</returns>
        /// <exception cref="ArgumentException">When coordinates not adjacent in a grid towards each other.</exception>
        private string GetArrowDirection((int x, int y) fromCoords, (int x, int y) toCoords)
        {
            if (fromCoords.x > toCoords.x && fromCoords.y == toCoords.y)
            {
                return "left";
            } 
            else if (fromCoords.x < toCoords.x && fromCoords.y == toCoords.y)
            {
                return "right";
            }
            else if (fromCoords.x == toCoords.x && fromCoords.y > toCoords.y)
            {
                return "down";
            }
            else if (fromCoords.x == toCoords.x && fromCoords.y < toCoords.y)
            {
                return "up";
            }
            else
            {
                string message = "Connecting model-nodes only possible when coordinates are adjacent in a grid: (" + 
                    fromCoords.x.ToString() + ", " + fromCoords.y.ToString() + ") -> (" +
                    toCoords.x.ToString() + ", " + toCoords.y.ToString() + ")";
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Calculates the row and column in the view where the label of the node with given coordinates will be placed.
        /// </summary>
        /// <param name="coordinates">Coordinates of the node in the model.</param>
        /// <returns>Integer tuple (column, row) of position in model table.</returns>
        private (int column, int row) TransformCoordinatesModelToView((int x, int y) coordinates)
        {
            // offset the model coordinates to begin at 0
            int offsetX = coordinates.x - minModelCords.x;
            int offsetY = coordinates.y - minModelCords.y;

            // expand the model coordinates over just the even columns/rows
            return (offsetX * 2, offsetY * 2);
        }

        /// <summary>
        /// Checks whether two given nodes are adjacent to each other in the models coordinate system.
        /// An edge towards a node that is further away than directly next to the node or on a diagonal is invalid.
        /// </summary>
        /// <param name="fromCoordinates">Coordinates to check edge from.</param>
        /// <param name="toCoordinates">Coordinates to check edge towards.</param>
        /// <returns><c>true</c> if edge is valid.</returns>
        private bool ArrowIsPlacable((int x, int y) fromCoordinates, (int x, int y) toCoordinates)
        {
            // either they are directly besides each other on the x axis or y axis
            // if both are true its diagonal and also not valid
            if ((Math.Abs(fromCoordinates.x - toCoordinates.x) == 1 && fromCoordinates.y == toCoordinates.y) ^ 
                (Math.Abs(fromCoordinates.y - toCoordinates.y) == 1 && fromCoordinates.x == toCoordinates.x))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Takes a given arrow string and makes a double arrow in the horizontal or vertical direction given the existing direction. TODO: make the arrow codes into an enum
        /// </summary>
        /// <param name="singleArrow">\u21D0, \u21D1, \u21D2 or \u21D3</param>
        /// <returns>\u21D4 or \u21D5</returns>
        /// <exception cref="ArgumentException">If singleArrow is not one of the given strings.</exception>
        private string DoubleArrow(string singleArrow)
        {
            switch (singleArrow)
            {
                case "\u21D0":      //left
                    return "\u21D4";    //horizontal
                case "\u21D1":      //up
                    return "\u21D5";    //vertical
                case "\u21D2":      //right
                    return "\u21D4";    //horizontal
                case "\u21D3":      //down
                    return "\u21D5";    //vertical
                default:
                    throw new ArgumentException("Not a valid arrow string.");
            }
        }
    }
}
