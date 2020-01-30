using System;

namespace DiagramDesigner.Simulator
{
    public class MultiComponent : SystemComponent
    {
        private double correctOutput;

        private double output;

        public override double GetCorrectOutput(double x, double y)
        {
            correctOutput = 2 * x * y;
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
            int number = (int)r;
            switch (number)
            {
                case 28:
                    return 1;
                case 42:
                    return 2;
                case 56:
                    return 3;
                case 100:
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

        public MultiComponent(Guid id, string name) : base(id, name)
        {
        }
    }
}