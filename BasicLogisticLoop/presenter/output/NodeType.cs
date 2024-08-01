using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLogisticLoop.Presenter.Output
{
    /// <summary>
    /// Defines the types of Nodes in the Model by their function.
    /// </summary>
    internal enum NodeType
    {
        Conveyor,
        Retrieval,
        Storage,
        Commissioning,
        Warehouse
    }
}
