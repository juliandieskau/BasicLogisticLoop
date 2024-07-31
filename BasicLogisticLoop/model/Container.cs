using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLogisticLoop.model
{
    internal class Container
    {
        /// <summary>
        /// Unique Identifier for the Container.
        /// </summary>
        public int TransportUnitNumber { get; private set; }

        public String Content { get; private set; }
    }
}
