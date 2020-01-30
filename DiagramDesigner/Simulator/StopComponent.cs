using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DiagramDesigner.Simulator
{
    class StopComponent : SystemComponent
    {
        public StopComponent(Guid id, string name) : base(id, name)
        {
        }

        public override double GetCorrectOutput(double x, double y)
        {
            return 1;
        }

        public override double Execute(double x, double y)
        {
            return 1;
        }

        public override int GetBrokeVariant(double r)
        {
            throw new NotImplementedException();
        }

        public override void Broke(int i)
        {
            throw new NotImplementedException();
        }
    }
}