using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlidingTilesPuzzelSimulation
{
    public struct TimeUnits
    {
        public TimeUnits(int resultingDays, int resultingHours, int resultingMinutes, int resultingSeconds, int resultingMilliseconds)
        {
            _resultingDays = resultingDays;
            _resultingHours = resultingHours;
            _resultingMinutes = resultingMinutes;
            _resultingSeconds = resultingSeconds;
            _resultingMilliseconds = resultingMilliseconds;
        }

        private int _resultingDays;
        public int ResultingDays
        {
            get { return _resultingDays; }
        }

        private int _resultingHours;
        public int ResultingHours
        {
            get { return _resultingHours; }
        }

        private int _resultingMinutes;
        public int ResultingMinutes
        {
            get { return _resultingMinutes; }
        }

        private int _resultingSeconds;
        public int ResultingSeconds
        {
            get { return _resultingSeconds; }
        }

        private int _resultingMilliseconds;
        public int ResultingMilliseconds
        {
            get { return _resultingMilliseconds; }
        }
    }
}
