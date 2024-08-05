using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLogisticLoop.Presenter.Output
{
    /// <summary>
    /// Utility class containing error message strings to output from the Model to the View.
    /// </summary>
    internal static class ErrorMessages
    {
        public const string StepError = "Could not step a cycle forward.";
        public const string RetrievalError = "The retrieval node is not empty thus could not retrieve container from warehouse.";
        public const string CommissionError = "The node after commission node is occupied thus could not move container into conveyor loop.";
    }
}
