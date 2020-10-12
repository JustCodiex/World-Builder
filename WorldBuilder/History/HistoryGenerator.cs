using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldBuilder.Geography;
using WorldBuilder.Utility.Functional;

namespace WorldBuilder.History {
    
    /// <summary>
    /// Generates historical events and parameters without running a complete simulation
    /// </summary>
    public class HistoryGenerator {

        Random m_randomizer;

        public HistoryGenerator(Random random) {
            this.m_randomizer = random;
        }

        public void GenerateHistory(World world, int startYear, int endYear) {

            TimeLine timeLine = new TimeLine();

            int activeYear = startYear + this.m_randomizer.Next(0, 4096);

            WorldProvince originProvince = world.GetRandomWorldProvince(this.m_randomizer);
            timeLine.AddEvent(new HistoricalEvent(new HistoricalDate(activeYear, this.m_randomizer.Next(0, 12), this.m_randomizer.Next(1, 29)), HistoricalEventType.PopulationInfo, "The First Humans", originProvince));
            activeYear += m_randomizer.Next(1, 64);

            timeLine.AddEvent(new HistoricalEvent(new HistoricalDate(activeYear, this.m_randomizer.Next(0,12), this.m_randomizer.Next(1, 29)), HistoricalEventType.TechnologicalDiscovery, "Fire Discovered", originProvince));
            activeYear += m_randomizer.Next(1, 512);

            timeLine.AddEvent(new HistoricalEvent(new HistoricalDate(activeYear, this.m_randomizer.Next(0, 12), this.m_randomizer.Next(1, 29)), HistoricalEventType.TechnologicalDiscovery, "Primitive Tools", originProvince));
            activeYear += m_randomizer.Next(1, 1024);

            int humanCounter = m_randomizer.Next(512, 1768);
            timeLine.AddEvent(new HistoricalEvent(new HistoricalDate(activeYear, 0, 1), HistoricalEventType.PopulationInfo, $"Human Population: {humanCounter}"));

            List<WorldProvince> provincePool = new List<WorldProvince>() {
                originProvince
            };

            // Generate a primitive Tribal Phase
            this.PrimitiveTribalPhase(ref humanCounter, ref provincePool, timeLine, ref activeYear);

            // Save the timeline
            world.History = timeLine;

        }

        private void PrimitiveTribalPhase(ref int humanCounter, ref List<WorldProvince> pPool, TimeLine timeLine, ref int activeYear) {

            List<WorldProvince> provincePool = pPool;
            int tribesToFound = m_randomizer.Next(humanCounter / 128, humanCounter / 64);
            int tMonhcnt = 0;

            for (int i = 0; i < tribesToFound; i++) {

                tMonhcnt = m_randomizer.Next(tMonhcnt + 1, 12);

                if (i >= tribesToFound / 3) {
                    if (m_randomizer.NextDouble() >= 0.4) {
                        WorldProvince s = null;
                        while (s == null) {
                            if (!provincePool[0].NeighbourProvinces.All(x => provincePool.Contains(x)) && m_randomizer.NextDouble() >= 0.5) {
                                s = provincePool[0].NeighbourProvinces.Random(m_randomizer);

                            } else {
                                s = provincePool.Random(m_randomizer).NeighbourProvinces.Random(m_randomizer);
                            }
                            if (!provincePool.Contains(s)) {
                                provincePool.Add(s);
                                break;
                            } else {
                                s = null;
                            }
                        }
                    }
                }

                timeLine.AddEvent(new HistoricalEvent(new HistoricalDate(activeYear, tMonhcnt, this.m_randomizer.Next(1, 29)), HistoricalEventType.TribeFounded, "DA", provincePool.Random(m_randomizer)));
                humanCounter += m_randomizer.Next(-32, 128);

                if (m_randomizer.NextDouble() <= 0.45 || tMonhcnt >= 10) {
                    tMonhcnt = 0;
                    activeYear += m_randomizer.Next(1, 16);
                }

            }

            timeLine.AddEvent(new HistoricalEvent(new HistoricalDate(activeYear, 0, 1), HistoricalEventType.PopulationInfo, $"Human Population: {humanCounter}"));

        }

    }

}
