using System;
using WorldBuilder.Geography;
using WorldBuilder.Utility.Maths;

namespace WorldBuilder {
    class Program {
        static void Main(string[] args) {

            Console.WriteLine($"World-Builder V0.0.1");
            Console.WriteLine();

            // Open program and decide what to do
            if (args.Length == 0) {
                TestCurrent();
            } else {
                // TODO: Process commands
            }

            Console.WriteLine("Simulation over - Press any key to exit...");
            Console.ReadLine();

        }

        static void TestCurrent() {

            Console.WriteLine("-- Running test instance --");
            Console.WriteLine();

            RunMapTest();

        }

        static void RunMapTest() {

            World world = new WorldGenerator().SetSize(1000, 400).SetScale(1000)
                .SetPointRange(360..768)
                .SetContinentCount(7)
                .SetDistanceMethod(VoroniDiagram.DistanceMethod.Euclidean)
                .Generate(4554);
            
            world.SaveMapToFile("Test.png");

        }

    }

}
