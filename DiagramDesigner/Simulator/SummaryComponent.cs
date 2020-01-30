using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DiagramDesigner.Simulator
{
    public class SummaryComponent : SystemComponent
    {
        public List<SystemComponent> summaryComponents { get; set; }

        private double correctOutput;

        private double output;

        public override double GetCorrectOutput(double x, double y)
        {
            correctOutput = 0;

            foreach (var summaryComponent in nextComponents)
            {
                correctOutput += summaryComponent.GetCorrectOutput(x, y);
            }
            output = correctOutput;

            return correctOutput;
        }

        public override double Execute(double x, double y)
        {
            output = 0;
            foreach (var systemComponent in nextComponents)
            {
                output += systemComponent.Execute(x,y);
            }

            return output;
        }

        public override int GetBrokeVariant(double r)
        {
            switch (r)
            {
                case 10:
                    return 1;
                case 18:
                    return 2;
                case 22:
                    return 3;
                case 58:
                    return 4;
                default:
                    return 0;
            }
        }

        public override void Broke(int i)
        {
            switch (i)
            {
                case 1:
                    output = 23;
                    break;
                case 2:
                    output = 3231;
                    break;
                case 3:
                    output = 432;
                    break;
                case 4:
                    output = 3245;
                    break;
            }
        }

        public SummaryComponent(Guid id, string name) : base(id, name)
        {
        }
    }
}