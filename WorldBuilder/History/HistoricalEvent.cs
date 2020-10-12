using System;
using WorldBuilder.Geography;

namespace WorldBuilder.History {
  
    public class HistoricalEvent {
    
        public HistoricalDate Date { get; }

        public HistoricalEventType Type { get; }

        public string Description { get; }

        public WorldProvince Province { get; }

        public HistoricalEvent(HistoricalDate date, HistoricalEventType type, string description, string title = "") {
            this.Date = date;
            this.Type = type;
            this.Description = description;
        }

        public HistoricalEvent(HistoricalDate date, HistoricalEventType type, string description, WorldProvince location, string title = "") {
            this.Date = date;
            this.Type = type;
            this.Province = location;
            this.Description = description;
        }

        public HistoricalEvent(HistoricalDate date, HistoricalEventType type, Enum secondaryType, string description, string title = "") {
            this.Date = date;
            this.Type = type;
            this.Description = description;
        }

    }

}
