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
                Dock = DockStyle.None,
                ColumnCount = 1,
                RowCount = 2,
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize
            };

            // add model and buttons above one another
            panel.Controls.Add(GenerateModelPanel());
            panel.Controls.Add(GenerateButtonsPanel());

            // set rows size
            panel.RowStyles[0] = new RowStyle(SizeType.Percent, 6.0f);
            panel.RowStyles[1] = new RowStyle(SizeType.Percent, 6.0f);

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
            (int x, int y) modelSize = (Math.Abs(maxModelCords.x - minModelCords.x),
                                       Math.Abs(maxModelCords.y - minModelCords.y));
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
                    panel.ColumnStyles[x] = new ColumnStyle(SizeType.Percent, nodeSizeXPercent);
                } 
                else
                {
                    // Arrow Labels
                    panel.ColumnStyles[x] = new ColumnStyle(SizeType.Percent, arrowSizeXPercent);
                }
            }
            for (int y = 0; y < viewSize.y; y++)
            {
                // Rows
                if (y % 2 == 0)
                {
                    // Node Labels
                    panel.RowStyles[y] = new RowStyle(SizeType.Percent, nodeSizeYPercent);
                }
                else
                {
                    // Arrow Labels
                    panel.RowStyles[y] = new RowStyle(SizeType.Percent, arrowSizeYPercent);
                }
            }

            foreach (ViewNode node in NodeData)
            {
                // Generate model labels and place them on the even numbered rows and columns (0, 2, 4, ...)
                // depending on the transformed ViewNode.Coordinates
                (int column, int row) panelPosition = TransformCoordinatesModelToView(node.Coordinates);
                Label label = GenerateNodeLabel(node);

                panel.SetRow(label, panelPosition.row);
                panel.SetColumn(label, panelPosition.column);

                // Generate arrow labels by checking ViewNode.FollowingNodes
                // placing the arrow in the mean of the node and each following node (if distance is 2 rows/columns)
                foreach (int followingID in node.FollowingNodes)
                {
                    ViewNode followingNode = NodeData.Find(n => n.NodeID == followingID);

                    // calculate the arrows direction 
                    string direction = GetArrowDirection(node.Coordinates, followingNode.Coordinates);

                    // If label already has an arrow (in other direction), make it a double arrow
                }
            }
            
            throw new NotImplementedException();
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
                RowCount = 9,
            };

            // create and add controls in backwards order (DockStyle.Bottom -> first is at the bottom and then stack on top)
            panel.Controls.AddRange(new Control[]
            {
                GenerateContainerPanel(),
                GenerateRightPanelLabel("container"),
                GenerateRightPanelLabel("nodeID"),
                GenerateRightPanelLabel("nodeType"),
                GenerateRightPanelLabel("nodeDetails"),
                GenerateButton("retrieval"),
                GenerateTextBox(false),
                GenerateRightPanelLabel("description"),
                GenerateRightPanelLabel("retrieval")
            });

            // set rows size
            for (int index = 0; index < 9; index++)
            {
                if (index == 2)
                {
                    panel.RowStyles[index] = new RowStyle(SizeType.Absolute, 200); // TextBox
                }
                else
                {
                    panel.RowStyles[index] = new RowStyle(SizeType.AutoSize);
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
            panel.RowStyles[0] = new RowStyle(SizeType.AutoSize);
            panel.RowStyles[1] = new RowStyle(SizeType.AutoSize);
            panel.RowStyles[2] = new RowStyle(SizeType.AutoSize);
            panel.RowStyles[3] = new RowStyle(SizeType.Absolute, 200); // TextBox

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
        /// Generates a label for the right panel of the view. Outputs labels with attributes depending on the given labelType.
        /// </summary>
        /// <param name="labelType">retrieval, nodeDetails, container, description, nodeType, nodeID, tun, destination, content</param>
        /// <returns>Label with corresponding attributes.</returns>
        private Label GenerateRightPanelLabel(string labelType) 
        {
            // default label attribute values
            BorderStyle border = BorderStyle.None;
            FontStyle fontStyle = FontStyle.Regular;
            string text = "";

            // special values depending on label
            switch (labelType)
            {
                case "retrieval":
                    text = "Retrieval";
                    border = BorderStyle.FixedSingle;
                    fontStyle = FontStyle.Bold;
                    break;
                case "nodeDetails":
                    text = "Node Details";
                    border = BorderStyle.FixedSingle;
                    fontStyle = FontStyle.Bold;
                    break;
                case "container":
                    text = "Container:";
                    fontStyle = FontStyle.Bold; 
                    break;
                case "description":
                    text = "Input content of next from" + Environment.NewLine + "warehouse retrieved container:";
                    break;
                case "nodeType":
                    text = "Node Type: <select node>";
                    break;
                case "nodeID":
                    text = "Node ID: <select node>";
                    break;
                case "tun":
                    text = "TransportUnitNumber: <select node>";
                    break;
                case "destination":
                    text = "Destination: <select node>";
                    break;
                case "content":
                    text = "Content:";
                    break;
            }

            // create label
            Label label = new Label
            {
                Name = labelType + "Label",
                Text = text,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(new FontFamily("Arial"), 16, fontStyle, GraphicsUnit.Pixel),
                AutoSize = true,
                Dock = DockStyle.Fill,
                BorderStyle = border
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
                Text = exampleText,
                Font = new Font(new FontFamily("Arial"), 16, FontStyle.Regular, GraphicsUnit.Pixel),
                Dock = DockStyle.Fill
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
    }
}
