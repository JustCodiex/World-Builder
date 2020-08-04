using System;
using WorldBuilder.Geography;

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

            World world = new WorldGenerator().SetSize(1200, 500).SetScale(1000).SetPointRange(300..470).SetContinentCount(7).Generate(4554);
            world.SaveMapToFile("Test.png");

        }

    }

}
