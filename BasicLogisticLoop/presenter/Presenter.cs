using BasicLogisticLoop.Model;
using BasicLogisticLoop.Presenter.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BasicLogisticLoop.Presenter
{
    /// <summary>
    /// Holds and Controls the model and view and presents the models state by handing it to the view.
    /// Takes user input from the view and calls the models functions accordingly.
    /// </summary>
    internal class Presenter
    {
        /// <summary>
        /// Holds all the logistic loop model logic and state and is used as an interface to access them.
        /// </summary>
        private ILogisticModel Model;

        /// <summary>
        /// Handles all user interaction by showing the model data on screen and receiving user input through a GUI.
        /// </summary>
        private Form View;

        /// <summary>
        /// List of all the ViewNodes that are currently shown in the view. May be used to check which Nodes have changed after 
        /// </summary>
        private List<ViewNode> CurrentNodes;
    }
}
