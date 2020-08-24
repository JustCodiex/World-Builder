using WorldBuilder.Formatting;

namespace WorldBuilder.History {
    
    public struct HistoricalDate {

        int _year, _month, _day;

        public int Year => _year;

        public int Month => _month;

        public int Day => _day;

        public bool BeforeCommonEra => Year < 0;

        public HistoricalDate(int year, int month, int day) {
            this._year = year;
            this._month = month;
            this._day = day;
        }

        public override string ToString() => $"{this._year}{((this.BeforeCommonEra)?" BC":"")}, {Database.Month.Names[this._month]} {this._day.ToFormattedString()}";

    }

}
