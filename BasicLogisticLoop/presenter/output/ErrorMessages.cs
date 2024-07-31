using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLogisticLoop.presenter.output
{
    /// <summary>
    /// Utility class containing error message strings to output from the Model to the View.
    /// </summary>
    internal static class ErrorMessages
    {
        public const String StepError = "Could not step a cycle forward.";
        public const String RetrievalError = "The retrieval node is not empty thus could not retrieve container from warehouse.";
        public const String CommissionError = "The node after picking station is not empty thus could not put container into loop.";
    }
}
