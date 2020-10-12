using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WorldBuilder.History {
    
    public class TimeLine {

        public SortedDictionary<long, List<HistoricalEvent>> m_events;

        public TimeLine() {
            this.m_events = new SortedDictionary<long, List<HistoricalEvent>>();
        }

        public void AddEvent(HistoricalEvent historicalEvent) {
            if (!this.m_events.ContainsKey(historicalEvent.Date.Year)) {
                this.m_events.Add(historicalEvent.Date.Year, new List<HistoricalEvent>());
            }
            this.m_events[historicalEvent.Date.Year].Add(historicalEvent);
        }

        public void SaveToFile(string path) {

            using (StreamWriter sw = new StreamWriter(path)) {

                foreach (KeyValuePair<long, List<HistoricalEvent>> pair in this.m_events) {

                    sw.WriteLine($"{pair.Key}{((pair.Key < 0)?" BCE":" AD")}:");

                    foreach (HistoricalEvent e in pair.Value) {

                        sw.WriteLine($"\t{e.Date} - {e.Type.ToStr()}{(string.IsNullOrEmpty(e.Description)?"":":")} {e.Description}");

                    }

                }

            }

        }

    }

}
