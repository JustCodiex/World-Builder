using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

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
        PopulationInfo,
        WarDeclared,
        WarEnded,
        ProvinceNamed,
        First,
        NewHistoricalPhase,
    }

    public static class HETEXT {
        public static string ToStr(this HistoricalEventType t) {
            return t switch
            {
                HistoricalEventType.TechnologicalDiscovery => "Technological Discovery",
                HistoricalEventType.TribeFounded => "Tribe Founded",
                HistoricalEventType.TribePerished => "Tribe Perished",
                HistoricalEventType.PopulationInfo => "Population Info",
                _ => t.ToString()
            };
        }
    }

}
