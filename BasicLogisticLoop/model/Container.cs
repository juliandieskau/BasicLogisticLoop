using BasicLogisticLoop.Presenter.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLogisticLoop.Model
{
    internal class Container
    {
        /// <summary>
        /// Unique identifier for the container.
        /// </summary>
        public int TransportUnitNumber { get; private set; }

        /// <summary>
        /// String describing the content of the container.
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// NodeType enum to mark the type of destination node the container has.
        /// </summary>
        public NodeType DestinationType { get; set; }

        /// <summary>
        /// Constructor for the Container
        /// </summary>
        /// <param name="transportUnitNumber">Unique identifier for the container.</param>
        /// <param name="content">string describing the content of the container.</param>
        /// <param name="destinationType">NodeType of the destination node this container wants to get to.</param>
        public Container(int transportUnitNumber, string content, NodeType destinationType)
        {
            TransportUnitNumber = transportUnitNumber;
            Content = content;
            DestinationType = destinationType;
        }

        /// <summary>
        /// Constructor for the container. Automatically assigns commissioning as DestinationType
        /// since this is the goal of every Container that gets created in the model by being
        /// retrieved from the Warehouse onto the RetrievalNode.
        /// </summary>
        /// <param name="transportUnitNumber">Unique identifier for the container.</param>
        /// <param name="content">string describing the content of the container.</param>
        public Container(int transportUnitNumber, string content)
        {
            TransportUnitNumber = transportUnitNumber;
            Content = content;
            DestinationType = NodeType.Commissioning;
        }

        /// <summary>
        /// Set the content of the container to a new one.
        /// </summary>
        /// <param name="newContent">New content to change the content of the container to.</param>
        /// <returns>Old content the container had before changing to the new one.</returns>
        public string ChangeContent(string newContent)
        {
            string oldContent = Content;
            Content = newContent;
            return oldContent;
        }

        public override bool Equals(object obj)
        {
            return obj is Container container &&
                   TransportUnitNumber == container.TransportUnitNumber;
        }

        public override int GetHashCode()
        {
            return 623182445 + TransportUnitNumber.GetHashCode();
        }
    }
}
