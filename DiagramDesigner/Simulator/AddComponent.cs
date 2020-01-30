using System;
using NotImplementedException = System.NotImplementedException;

namespace DiagramDesigner.Simulator
{
    public class AddComponent : SystemComponent
    {
        private double correctOutput;

        private double output;

        public override double GetCorrectOutput(double x, double y)
        {
            correctOutput = x + y;
            output = correctOutput;
            return correctOutput;
        }

        public override double Execute(double x, double y)
        {
            Broke(GetBrokeVariant(correctOutput));
            return output;
        }

        public override int GetBrokeVariant(double r)
        {
            int number = (int) r;
            switch (number)
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

        public AddComponent(Guid id, string name) : base(id,name)
        {
        }
    }
}