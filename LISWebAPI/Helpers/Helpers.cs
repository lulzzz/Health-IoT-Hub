using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace LISWebAPI
{
    public static class Helpers
    {
        public static class Constants
        {
            public static string ReceivingAppName = "Health_IoT_Hub";
            public static string ReceivingAppFacility = "Health_IoT_Facility";

            public static string HL7 = "HL7";
            public static string ASTM = "ASTM";
            public static string POCT = "POCT";
        }

        public static class Converters
        {
            public static DateTime? ConvertStringToDate(string Value, string Format)
            {
                DateTime? dtr = null;
                try
                {
                    DateTime dt;
                    if (DateTime.TryParseExact(Value, Format,
                                              CultureInfo.InvariantCulture,
                                              DateTimeStyles.None, out dt))
                    {
                        dtr = (DateTime?)dt;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
                return dtr;
            }
        }
    }
}
