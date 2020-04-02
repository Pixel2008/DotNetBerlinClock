using System;

namespace BerlinClock
{
    public class TimeConverter : ITimeConverter
    {
        //new line chars
        private readonly string NL = Environment.NewLine;
        //off light char
        private readonly char OFF = 'O';

        /// <summary>
        /// Parses time from given string in HH:mm:ss format
        /// </summary>
        public class TimeParser
        {
            /// <summary>
            /// Gets the hour of parsed time
            /// </summary>
            public int Hour { get; private set; }
            /// <summary>
            /// Gets the minutes of parsed time
            /// </summary>
            public int Minute { get; private set; }
            /// <summary>
            /// Gets the seconds of parsed time
            /// </summary>
            public int Second { get; private set; }
            /// <summary>
            /// Gets a value that indicates whether the time parsing succeeded
            /// </summary>
            public bool IsValid { get; private set; }
            /// <summary>
            /// Parses the specified string representation of time in HH:mm:ss format
            /// </summary>
            /// <param name="time"></param>
            public TimeParser(string time)
            {
                //DateTime doesn't know about 24th hour
                if (time.Equals("24:00:00") || time.Equals("24:00"))
                {
                    IsValid = true;
                    Hour = 24;
                }
                else
                {
                    IsValid = DateTime.TryParseExact(time, "HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out DateTime dt);
                    if (IsValid)
                    {
                        Hour = dt.Hour;
                        Minute = dt.Minute;
                        Second = dt.Second;
                    }
                }
            }
        }

        /// <summary>
        /// Converts the specified string representation of time to BerlinClock string
        /// </summary>
        /// <param name="aTime">A string containing a time to convert</param>
        /// <returns>Returns a string representation of time in 'BerlinClock' format</returns>
        public string ConvertTime(string aTime)
        {
            //check for time format
            var timeParser = new TimeParser(aTime);
            if (!timeParser.IsValid)
                throw new NotSupportedException($"'{aTime}' is not in an acceptable format.");

            return GetSecondLight(timeParser.Second) +
                   NL +
                   GetHourLights(timeParser.Hour) +
                   NL +
                   GetMinuteLights(timeParser.Minute);
        }
        /// <summary>
        /// Gets string representation of two row's hour lights
        /// </summary>
        /// <param name="hour">Hour to convert to two row's string</param>
        /// <returns>Returns two row's string of lights</returns>
        private string GetHourLights(int hour)
        {
            int topOn = (int)Math.Floor(hour / 5.0);    //how many lights is on in top row
            int bottomOn = hour - topOn * 5;            //how many lights is on in bottom row

            return 
                GetLights("RRRR", 4 - topOn) +
                NL +
                GetLights("RRRR", 4 - bottomOn);
        }
        /// <summary>
        /// Gets string representation of two row's minutes lights
        /// </summary>
        /// <param name="minute">Minutes to convert to two row's string</param>
        /// <returns>Returns two row's string of lights</returns>
        private string GetMinuteLights(int minute)
        {
            int topOn = (int)Math.Floor(minute / 5.0);  //how many lights is on in top row
            int bottomOn = minute - topOn * 5;          //how many lights is on in bottom row

            return
                GetLights("YYRYYRYYRYY", 11 - topOn) +
                NL +
                GetLights("YYYY", 4 - bottomOn);
        }
        /// <summary>
        /// Gets string representation of seconds light
        /// </summary>
        /// <param name="second">Seconds to convert to string</param>
        /// <returns>Returns one row string of light</returns>
        private string GetSecondLight(int second)
        {
            return second % 2 != 0 ? OFF.ToString() : "Y";  
        }
        /// <summary>
        /// Gets row of lights
        /// </summary>
        /// <param name="allLightsOn">Full row of on lights</param>
        /// <param name="offQuantity">How many lights is off starting from end</param>
        /// <returns>Returns one row string of lights</returns>
        private string GetLights(string allLightsOn, int offQuantity)
        {
            if (offQuantity == 0)
                return allLightsOn;

            var chars = allLightsOn.ToCharArray();
            for (int i = 1; i <= offQuantity; i++)
            {
                chars[chars.Length - i] = OFF;
            }
            return new string(chars);
        }
    }
}
