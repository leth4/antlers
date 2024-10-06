using UnityEngine;

namespace Foundation
{
    public static class NumberExtensions
    {
        public static float RoundTo(this float number, float roundTo)
        {
            return Mathf.Round(number / roundTo) * roundTo;
        }

        public static string ToWord(this int number)
        {
            return number switch
            {
                0 => "ZERO",
                1 => "ONE",
                2 => "TWO",
                3 => "THREE",
                4 => "FOUR",
                5 => "FIVE",
                6 => "SIX",
                7 => "SEVEN",
                8 => "EIGHT",
                9 => "NINE",
                10 => "TEN",
                11 => "ELEVEN",
                12 => "TWELVE",
                13 => "THIRTEEN",
                14 => "FOURTEEN",
                15 => "FIFTEEN",
                16 => "SIXTEEN",
                _ => "MANY"
            };
        }

        public static string GetEnd(this int number)
        {
            return number switch
            {
                0 => "TH",
                1 => "ST",
                2 => "ND",
                3 => "RD",
                4 => "TH",
                5 => "TH",
                6 => "TH",
                7 => "TH",
                8 => "TH",
                9 => "TH",
                _ => "TH"
            };
        }
    }
}