using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace DiagramDesigner.Simulator
{
    public class TestManeger
    {
        private List<SystemComponent> allComponents;
        private List<TestPair> blackBoxTests;
        private List<List<TestPair>> whiteBoxTests;
        private SystemComponent mainComponent;
        private int numberOfTests;
        private int blackBoxErrors;
        private List<int> findedBlackBoxErrorsTime;
        private List<int> findedblackBoxErrors;
        private int whiteBoxErrors;
        private List<int> findedWhiteBoxErrorsTime;
        private List<int> findedwhiteBoxErrors;
        private static TestManeger instance;

        public static TestManeger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TestManeger();
                }

                return instance;
            }
        }

        public TestManeger()
        {
            allComponents = new List<SystemComponent>();
        }

        public void AddComponent(Guid id, string name)
        {
            if (name == "Add")
            {
                allComponents.Add(new AddComponent(id, name));
            }
            else if (name == "Multi")
            {
                allComponents.Add(new MultiComponent(id, name));
            }
            else if (name == "Power")
            {
                allComponents.Add(new PowerComponent(id, name));
            }
            else if (name == "Start")
            {
                allComponents.Add(new StartComponent(id, name));
            }
            else if (name == "Stop")
            {
                allComponents.Add(new StopComponent(id, name));
            }
            else if (name == "Sum")
            {
                allComponents.Add(new SumComponent(id, name));
            }
        }

        public void RemoveComponent(Guid id)
        {
            foreach (SystemComponent systemComponent in allComponents)
            {
                if (systemComponent.Id == id)
                {
                    foreach (SystemComponent systemComponentNextComponent in systemComponent.nextComponents)
                    {
                        systemComponentNextComponent.prevComponents.Remove(systemComponent);
                    }

                    foreach (SystemComponent systemComponentPrevComponent in systemComponent.prevComponents)
                    {
                        systemComponentPrevComponent.nextComponents.Remove(systemComponent);
                    }
                    allComponents.Remove(systemComponent);
                    break;
                }
            }
        }

        public void SetConnection(Guid parentId, Guid id)
        {
            SystemComponent childComponent = allComponents.FirstOrDefault(x => x.Id == id);
            SystemComponent parentComponent = allComponents.FirstOrDefault(x => x.Id == parentId);
            if (childComponent != null)
            {
                childComponent.prevComponents.Add(parentComponent);
                if (parentComponent != null) parentComponent.nextComponents.Add(childComponent);
            }
        }

        public void RemoveConnection(Guid parentId, Guid childId)
        {
            allComponents.FirstOrDefault(x => x.Id == parentId).nextComponents
                .Remove(allComponents.FirstOrDefault(x => x.Id == childId));
            allComponents.FirstOrDefault(x => x.Id == childId).prevComponents
                .Remove(allComponents.FirstOrDefault(x => x.Id == parentId));
        }

        public void StartSim()
        {
            try
            {
                GenerateModel();
            }
            catch (Exception)
            {
                MessageBox.Show("Błędny diagram");
            }
            GenerateBlackBoxTests();
            StartBlackBoxTest();
            GenerateWhiteBoxTests();
            StartWhiteBoxTest();
            Window window = new Window
            {
                Title = "Liczba błędow na metodę",
                Content = new NumberOfErrors(blackBoxErrors,whiteBoxErrors),
                Height = 400,
                Width = 500
            };
            window.Show();
            Window window2 = new Window
            {
                Title = "Liczba błędow po liczbie prób",
                Content = new ErrorsPerTest(numberOfTests,findedWhiteBoxErrorsTime,findedBlackBoxErrorsTime),
                Height = 400,
                Width = 500
            };

            window2.Show();
        }

        private void GenerateModel()
        {
            SystemComponent currentComponent = allComponents.FirstOrDefault(x => x.Name == "Start");
            mainComponent = new PipeLineComponent(Guid.NewGuid(), "Main");
            mainComponent.nextComponents.Add(CreateComponent(currentComponent.Name));
            while (currentComponent != null && currentComponent.Name != "Stop")
            {
                if (currentComponent.nextComponents.Count > 1)
                {
                    currentComponent = SumComponent(currentComponent, mainComponent);
                    mainComponent.nextComponents.Add(currentComponent);
                }
                else
                {
                    currentComponent = allComponents.FirstOrDefault(
                        x => x.Id == currentComponent.nextComponents[0].Id);
                    mainComponent.nextComponents.Add(CreateComponent(currentComponent.Name));
                }
            }
        }

        private SystemComponent SumComponent(SystemComponent currentComponent, SystemComponent addComponent)
        {
            SummaryComponent summaryComponent = new SummaryComponent(Guid.NewGuid(), "Summary");
            foreach (SystemComponent systemComponent in currentComponent.nextComponents)
            {
                if (systemComponent.nextComponents.Count > 1)
                {
                    currentComponent=LineComponent(systemComponent, summaryComponent);
                }
                else if (systemComponent.nextComponents.Count == 1 &&
                         systemComponent.nextComponents[0].prevComponents.Count > 1)
                {
                    summaryComponent.nextComponents.Add(CreateComponent(systemComponent.Name));
                    currentComponent = systemComponent.nextComponents[0];
                }
                else
                {
                    currentComponent=LineComponent(systemComponent, summaryComponent);
                }
            }
            addComponent.nextComponents.Add(summaryComponent);
            return currentComponent;
        }

        private SystemComponent LineComponent(SystemComponent currnetComponent, SystemComponent addComponent)
        {
            PipeLineComponent pipeLineComponent = new PipeLineComponent(Guid.NewGuid(), "PipeLine");
            while (true)
            {
                if (currnetComponent.prevComponents.Count > 1)
                {
                    addComponent.nextComponents.Add(pipeLineComponent);
                    return currnetComponent;
                }
                else if (currnetComponent.nextComponents.Count > 1)
                {
                    pipeLineComponent.nextComponents.Add(CreateComponent(currnetComponent.Name));
                    currnetComponent=SumComponent(currnetComponent, pipeLineComponent);
                }
                else
                {
                    pipeLineComponent.nextComponents.Add(CreateComponent(currnetComponent.Name));
                    currnetComponent = currnetComponent.nextComponents[0];
                }
            }
        }

        private SystemComponent CreateComponent(string name)
        {

            if (name == "Add")
            {
                return new AddComponent(Guid.NewGuid(), name);
            }
            else if (name == "Multi")
            {
                return new MultiComponent(Guid.NewGuid(), name);
            }
            else if (name == "Power")
            {
                return new PowerComponent(Guid.NewGuid(), name);
            }
            else if (name == "Start")
            {
                return new StartComponent(Guid.NewGuid(), name);
            }
            else if (name == "Stop")
            {
                return new StopComponent(Guid.NewGuid(), name);
            }
            else
            {
                return null;
            }
        }

        public void SetNumberOfTests(int a)
        {
            numberOfTests = a;
        }

        private void GenerateBlackBoxTests()
        {
            blackBoxTests = new List<TestPair>();
            Random random = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < numberOfTests; i++)
            {
                var pair = new TestPair(
                    random.Next(-100,100)+random.NextDouble(), random.Next(-100, 100) + random.NextDouble());
                if (blackBoxTests.Exists(x => x.X.Equals(pair.X) && x.Y.Equals(pair.Y)))
                {
                    i--;
                    continue;
                }
                blackBoxTests.Add(pair);
            }
        }

        private void GenerateWhiteBoxTests()
        {
            whiteBoxTests = new List<List<TestPair>>();
            Random random = new Random(Guid.NewGuid().GetHashCode());
            var testsPerComponent = numberOfTests / (allComponents.Count - 2);
            foreach (var systemComponent in allComponents)
            {
                if (systemComponent.Name != "Start" && systemComponent.Name != "Stop")
                {
                    var pairList = new List<TestPair>();
                    for (int i = 0; i < testsPerComponent; i++)
                    {
                        var pair = new TestPair(
                            random.Next(-100, 100) + random.NextDouble(), random.Next(-100, 100) + random.NextDouble());
                        if (pairList.Exists(x => x.X.Equals(pair.X) && x.Y.Equals(pair.Y)))
                        {
                            i--;
                            continue;
                        }
                        pairList.Add(pair);
                    }
                    whiteBoxTests.Add(pairList);
                }
            }
        }

        private void StartBlackBoxTest()
        {
            findedBlackBoxErrorsTime = new List<int>();
            blackBoxErrors = 0;
            for (int i = 0; i < numberOfTests; i++)
            {
                var correctOutput = mainComponent.GetCorrectOutput(blackBoxTests[i].X, blackBoxTests[i].Y);
                var executedOutput = mainComponent.Execute(blackBoxTests[i].X, blackBoxTests[i].Y);
                if (correctOutput != executedOutput)
                {
                    blackBoxErrors++;
                    findedBlackBoxErrorsTime.Add(i);
                }
            }
        }

        private void StartWhiteBoxTest()
        {
            findedWhiteBoxErrorsTime = new List<int>();
            whiteBoxErrors = 0;
            int testedComponent = 0;
            int generalNumberOfTest = 0;
            foreach (var systemComponent in allComponents)
            {
                if (systemComponent.Name != "Start" && systemComponent.Name != "Stop")
                {
                    for (int i = 0; i < whiteBoxTests[testedComponent].Count; i++)
                    {
                        var correctOutput = systemComponent.GetCorrectOutput(whiteBoxTests[testedComponent][i].X,
                            whiteBoxTests[testedComponent][i].X);
                        var executedOutput = systemComponent.Execute(whiteBoxTests[testedComponent][i].X,
                            whiteBoxTests[testedComponent][i].X);
                        if (correctOutput != executedOutput)
                        {
                            whiteBoxErrors++;
                            findedWhiteBoxErrorsTime.Add(generalNumberOfTest);
                        }

                        generalNumberOfTest++;
                    }

                    testedComponent++;
                }
            }
        }
    }
}