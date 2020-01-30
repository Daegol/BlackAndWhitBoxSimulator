using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace DiagramDesigner.Simulator
{
    public abstract class SystemComponent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<SystemComponent> nextComponents;

        public List<SystemComponent> prevComponents;

        public SystemComponent(Guid id, string name)
        {
            Id = id;
            Name = name;
            nextComponents = new List<SystemComponent>();
            prevComponents = new List<SystemComponent>();
        }

        public abstract double GetCorrectOutput(double x, double y);

        public abstract double Execute(double x, double y);

        public abstract int GetBrokeVariant(double r);

        public abstract void Broke(int i);
    }
}