using System;

namespace WorldBuilder.History {
  
    public enum HistoricalEventType {
        TechnologicalDiscovery,
        TribeFounded,
        TribePerished,
        CultureEstablished,
        CulturePerished,
        ReligionFounded,
        ReligionPerished,
        NaturalDisaster,
    }

    public class HistoricalEvent {
    
        public HistoricalEvent(HistoricalDate date, HistoricalEventType type, string description, string title = "") {

        }

        public HistoricalEvent(HistoricalDate date, HistoricalEventType type, Enum secondaryType, string description, string title = "") {

        }

    }

}
