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
    public partial class BasicLogisticLoopForm
    {
        // CONSTANTS

        private const string LabelBaseName = "Label";
        private readonly Color LabelBackColor = Color.LightGray;
        private readonly Color WindowBackColor = Color.WhiteSmoke;

        // #################################################
        // PANEL GENERATOR METHODS

        /// <summary>
        /// FORM
        /// Method that generates a container that splits the view in a left and right part.
        /// </summary>
        /// <returns>Created SplitContainer.</returns>
        private SplitContainer GenerateSplitContainer()
        {
            throw new NotImplementedException();
        }

            /// <summary>
            /// SPLIT CONTAINER Panel1
            /// Method that generates a panel to hold the model and the buttons above each other.
            /// </summary>
            /// <returns></returns>
            private TableLayoutPanel GenerateLeftPanel()
            {
                throw new NotImplementedException();
            }

                /// <summary>
                /// LEFT PANEL
                /// Method that generates a panel that holds all model data in a grid.
                /// </summary>
                /// <returns>Created Panel.</returns>
                private TableLayoutPanel GenerateModelPanel()
                {
                    throw new NotImplementedException();
                }

                /// <summary>
                /// LEFT PANEL
                /// Method that generates a panel that holds the step and commission button.
                /// </summary>
                /// <returns>Created Panel.</returns>
                private Panel GenerateButtonsPanel()
                {
                    throw new NotImplementedException();
                }

            /// <summary>
            /// SPLIT CONTAINER Panel2
            /// Method that generates a Panel that holds Labels, TextBoxes, Buttons and Panels for the Retrieval-Input and ShowContent.
            /// </summary>
            /// <returns>Created Panel.</returns>
            private TableLayoutPanel GenerateRightPanel()
            {
                throw new NotImplementedException();
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

                    };
                    
                    return panel;
                    throw new NotImplementedException();
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
        /// Method to generate a textbox to enter or display user input text.
        /// </summary>
        /// <param name="readOnly">True if for displaying text, false for entering.</param>
        /// <returns>Generated TextBox.</returns>
        private TextBox GenerateTextBox(bool readOnly)
        {
            string exampleText = GetRandomContainerContent(); // TODO: generator for random text (lorem ipsum)

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

        /// <summary>
        /// Returns a string of random words to be used as the content for a retrieved container, if not input by the user.
        /// Uses an array of supermarket items to pick from.
        /// </summary>
        /// <returns>Generated word.</returns>
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
    }
}
