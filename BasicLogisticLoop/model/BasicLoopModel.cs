using BasicLogisticLoop.Presenter.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicLogisticLoop.Model
{
    internal class BasicLoopModel : ILogisticModel
    {
        public string CommissionContainer(int nodeID, Container container)
        {
            throw new NotImplementedException();
        }

        public List<ViewNode> GetViewNodes()
        {
            throw new NotImplementedException();
        }

        public string RetrieveContainer(int nodeID)
        {
            throw new NotImplementedException();
        }

        public string Step()
        {
            throw new NotImplementedException();
        }
    }
}
