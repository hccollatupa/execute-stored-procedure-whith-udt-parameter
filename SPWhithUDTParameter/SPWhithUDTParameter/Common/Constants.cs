using System;

namespace SPWhithUDTParameter.Common
{
    public sealed class Constants
    {
        public static DateTime NullDateTime = DateTime.MinValue;

        public static decimal NullDecimal = 0;

        public static double NullDouble = double.MinValue;

        public static Guid NullGuid = Guid.Empty;

        public static int NullInt = 0;

        public static long NullLong = long.MinValue;

        public static float NullFloat = float.MinValue;

        public static string NullString = string.Empty;

        public static DateTime SqlMaxDate = new DateTime(9999, 1, 3, 23, 59, 59);

        public static DateTime SqlMinDate = new DateTime(1753, 1, 1, 00, 00, 00);

        public const string ZERO = "0";

        public const string UNO = "1";
    }
}
