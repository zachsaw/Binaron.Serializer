/* Copyright 2016-present MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Binaron.Serializer.IeeeDecimal
{
    // Copied from MongoDB CSharp Driver: https://github.com/mongodb/mongo-csharp-driver
    internal struct Decimal128 : IComparable<Decimal128>, IEquatable<Decimal128>
    {
        // private constants
        private const short ExponentMax = 6111;
        private const short ExponentMin = -6176;

        // private static fields
        private static readonly UInt128 MaxSignificand = UInt128.Parse("9999999999999999999999999999999999"); // must be initialized before Decimal128.Parse is called
        private static readonly Decimal128 MaxDecimalValue = Parse("79228162514264337593543950335");
        private static readonly Decimal128 MinDecimalValue = Parse("-79228162514264337593543950335");

        /// <summary>
        /// Represents negative infinity.
        /// </summary>
        public static Decimal128 NegativeInfinity => new Decimal128(Flags.NegativeInfinity, 0);

        /// <summary>
        /// Represents one.
        /// </summary>
        public static Decimal128 One => new Decimal128(0, 1);

        /// <summary>
        /// Represents positive infinity.
        /// </summary>
        public static Decimal128 PositiveInfinity => new Decimal128(Flags.PositiveInfinity, 0);

        /// <summary>
        /// Represents a value that is not a number.
        /// </summary>
        public static Decimal128 QNaN => new Decimal128(Flags.QNaN, 0);

        /// <summary>
        /// Represents a value that is not a number and raises errors when used in calculations.
        /// </summary>
        public static Decimal128 SNaN => new Decimal128(Flags.SNaN, 0);

        /// <summary>
        /// Represents zero.
        /// </summary>
        public static Decimal128 Zero => new Decimal128(0, 0);

        /// <summary>
        /// Performs an explicit conversion from <see cref="Decimal128"/> to <see cref="System.Decimal"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static explicit operator decimal(Decimal128 value)
        {
            return ToDecimal(value);
        }

        // public static methods
        /// <summary>
        /// Compares two specified Decimal128 values and returns an integer that indicates whether the first value
        /// is greater than, less than, or equal to the second value.
        /// </summary>
        /// <param name="x">The first value.</param>
        /// <param name="y">The second value.</param>
        /// <returns>Less than zero if x &lt; y, zero if x == y, and greater than zero if x &gt; y.</returns>
        public static int Compare(Decimal128 x, Decimal128 y)
        {
            return Decimal128Comparer.Instance.Compare(x, y);
        }

        /// <summary>
        /// Determines whether the specified Decimal128 instances are considered equal.
        /// </summary>
        /// <param name="x">The first Decimal128 object to compare.</param>
        /// <param name="y">The second Decimal128 object to compare.</param>
        /// <returns>True if the objects are considered equal; otherwise false. If both x and y are null, the method returns true.</returns>
        public static bool Equals(Decimal128 x, Decimal128 y)
        {
            return Compare(x, y) == 0;
        }

        /// <summary>
        /// Creates a new Decimal128 value from the IEEE encoding bits.
        /// </summary>
        /// <param name="highBits">The high bits.</param>
        /// <param name="lowBits">The low bits.</param>
        /// <returns>A Decimal128 value.</returns>
        public static Decimal128 FromIeeeBits(ulong highBits, ulong lowBits)
        {
            return new Decimal128(MapIeeeHighBitsToDecimal128HighBits(highBits), lowBits);
        }

        /// <summary>
        /// Gets the exponent of a Decimal128 value.
        /// </summary>
        /// <param name="d">The Decimal128 value.</param>
        /// <returns>The exponent.</returns>
        public static short GetExponent(Decimal128 d)
        {
            if (Flags.IsFirstForm(d.highBits))
                return MapDecimal128BiasedExponentToExponent((short) ((d.highBits & Flags.FirstFormExponentBits) >> 49));

            if (Flags.IsSecondForm(d.highBits))
                return MapDecimal128BiasedExponentToExponent((short) ((d.highBits & Flags.SecondFormExponentBits) >> 47));

            throw new InvalidOperationException("GetExponent cannot be called for Infinity or NaN.");
        }

        /// <summary>
        /// Gets the high bits of the significand of a Decimal128 value.
        /// </summary>
        /// <param name="d">The Decimal128 value.</param>
        /// <returns>The high bits of the significand.</returns>
        public static ulong GetSignificandHighBits(Decimal128 d)
        {
            if (Flags.IsFirstForm(d.highBits))
                return d.highBits & Flags.FirstFormSignificandBits;

            if (Flags.IsSecondForm(d.highBits))
                return 0;

            throw new InvalidOperationException("GetSignificandHighBits cannot be called for Infinity or NaN.");
        }

        /// <summary>
        /// Gets the high bits of the significand of a Decimal128 value.
        /// </summary>
        /// <param name="d">The Decimal128 value.</param>
        /// <returns>The high bits of the significand.</returns>
        public static ulong GetSignificandLowBits(Decimal128 d)
        {
            if (Flags.IsFirstForm(d.highBits))
                return d.lowBits;

            return Flags.IsSecondForm(d.highBits) ? (ulong) 0 : throw new InvalidOperationException("GetSignificandLowBits cannot be called for Infinity or NaN.");
        }

        /// <summary>
        /// Returns a value indicating whether the specified number evaluates to negative or positive infinity.
        /// </summary>
        /// <param name="d">A 128-bit decimal.</param>
        /// <returns>true if <paramref name="d" /> evaluates to negative or positive infinity; otherwise, false.</returns>
        public static bool IsInfinity(Decimal128 d) => Flags.IsInfinity(d.highBits);

        /// <summary>
        /// Returns a value indicating whether the specified number is not a number.
        /// </summary>
        /// <param name="d">A 128-bit decimal.</param>
        /// <returns>true if <paramref name="d" /> is not a number; otherwise, false.</returns>
        public static bool IsNaN(Decimal128 d) => Flags.IsNaN(d.highBits);

        /// <summary>
        /// Returns a value indicating whether the specified number is negative.
        /// </summary>
        /// <param name="d">A 128-bit decimal.</param>
        /// <returns>true if <paramref name="d" /> is negative; otherwise, false.</returns>
        public static bool IsNegative(Decimal128 d) => Flags.IsNegative(d.highBits);

        /// <summary>
        /// Returns a value indicating whether the specified number evaluates to negative infinity.
        /// </summary>
        /// <param name="d">A 128-bit decimal.</param>
        /// <returns>true if <paramref name="d" /> evaluates to negative infinity; otherwise, false.</returns>
        public static bool IsNegativeInfinity(Decimal128 d) => Flags.IsNegativeInfinity(d.highBits);

        /// <summary>
        /// Returns a value indicating whether the specified number evaluates to positive infinity.
        /// </summary>
        /// <param name="d">A 128-bit decimal.</param>
        /// <returns>true if <paramref name="d" /> evaluates to positive infinity; otherwise, false.</returns>
        public static bool IsPositiveInfinity(Decimal128 d) => Flags.IsPositiveInfinity(d.highBits);

        /// <summary>
        /// Returns a value indicating whether the specified number is a quiet not a number.
        /// </summary>
        /// <param name="d">A 128-bit decimal.</param>
        /// <returns>true if <paramref name="d" /> is a quiet not a number; otherwise, false.</returns>
        public static bool IsQNaN(Decimal128 d) => Flags.IsQNaN(d.highBits);

        /// <summary>
        /// Returns a value indicating whether the specified number is a signaled not a number.
        /// </summary>
        /// <param name="d">A 128-bit decimal.</param>
        /// <returns>true if <paramref name="d" /> is a signaled not a number; otherwise, false.</returns>
        public static bool IsSNaN(Decimal128 d) => Flags.IsSNaN(d.highBits);

        /// <summary>
        /// Returns a value indicating whether the specified number is zero.
        /// </summary>
        /// <param name="d">A 128-bit decimal.</param>
        /// <returns>
        ///   <c>true</c> if the specified number is zero; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsZero(Decimal128 d)
        {
            if (Flags.IsFirstForm(d.highBits) && GetSignificand(d).Equals(UInt128.Zero))
                return true;

            if (Flags.IsSecondForm(d.highBits))
            {
                // all second form values are invalid representations and are interpreted as zero
                return true;
            }

            return false;
        }

        /// <summary>
        /// Negates the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>The result of multiplying the value by negative one.</returns>
        public static Decimal128 Negate(Decimal128 x)
        {
            return new Decimal128(x.highBits ^ Flags.SignBit, x.lowBits);
        }

        /// <summary>
        /// Converts the string representation of a number to its <see cref="Decimal128" /> equivalent.
        /// </summary>
        /// <param name="s">The string representation of the number to convert.</param>
        /// <returns>
        /// The equivalent to the number contained in <paramref name="s" />.
        /// </returns>
        public static Decimal128 Parse(string s)
        {
            return TryParse(s, out var value) ? value : throw new FormatException($"{s} is not a valid Decimal128.");
        }

        /// <summary>
        /// Converts the value of the specified <see cref="Decimal128"/> to the equivalent <see cref="decimal"/>.
        /// </summary>
        /// <param name="d">The number to convert.</param>
        /// <returns>A <see cref="decimal"/> equivalent to <paramref name="d" />.</returns>
        public static decimal ToDecimal(Decimal128 d)
        {
            if (Flags.IsFirstForm(d.highBits))
            {
                if (Compare(d, MinDecimalValue) < 0 || Compare(d, MaxDecimalValue) > 0)
                    throw new OverflowException("Value is too large or too small to be converted to a Decimal.");

                var isNegative = IsNegative(d);
                var exponent = GetExponent(d);
                var significand = GetSignificand(d);

                // decimal significand must fit in 96 bits
                while (significand.High >> 32 != 0)
                {
                    significand = UInt128.Divide(significand, 10, out _);
                    exponent += 1;
                }

                // decimal exponents must be between 0 and -28
                if (exponent > 0)
                {
                    // bring exponent within range
                    while (exponent > 0)
                    {
                        significand = UInt128.Multiply(significand, 10);
                        exponent -= 1;
                    }
                }
                else if (exponent < -28)
                {
                    // check if exponent is too far out of range to possibly be brought within range
                    if (exponent < -56)
                    {
                        return decimal.Zero;
                    }

                    // bring exponent within range
                    while (exponent < -28)
                    {
                        significand = UInt128.Divide(significand, 10, out _);
                        exponent += 1;
                    }

                    if (significand.Equals(UInt128.Zero))
                    {
                        return decimal.Zero;
                    }
                }

                var lo = (int) significand.Low;
                var mid = (int) (significand.Low >> 32);
                var hi = (int) significand.High;
                var scale = (byte) -exponent;

                return new decimal(lo, mid, hi, isNegative, scale);
            }

            return Flags.IsSecondForm(d.highBits) ? decimal.Zero : throw new OverflowException("Infinity or NaN cannot be converted to Decimal.");
        }

        /// <summary>
        /// Converts the value of the specified <see cref="Decimal128"/> to the equivalent <see cref="double"/>.
        /// </summary>
        /// <param name="d">The number to convert.</param>
        /// <returns>A <see cref="double"/> equivalent to <paramref name="d" />.</returns>
        public static double ToDouble(Decimal128 d)
        {
            if (Flags.IsFirstForm(d.highBits))
            {
                var stringValue = d.ToString();
                return double.Parse(stringValue);
            }

            if (Flags.IsSecondForm(d.highBits))
                return 0.0;

            if (Flags.IsPositiveInfinity(d.highBits))
                return double.PositiveInfinity;

            return Flags.IsNegativeInfinity(d.highBits) ? double.NegativeInfinity : double.NaN;
        }

        /// <summary>
        /// Converts the string representation of a number to its <see cref="Decimal128" /> equivalent. A return value indicates whether the conversion succeeded or failed.
        /// </summary>
        /// <param name="s">The string representation of the number to convert.</param>
        /// <param name="result">When this method returns, contains the <see cref="Decimal128" /> number that is equivalent to the numeric value contained in <paramref name="s" />, if the conversion succeeded, or is zero if the conversion failed. The conversion fails if the <paramref name="s" /> parameter is null, is not a number in a valid format, or represents a number less than the min value or greater than the max value. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if <paramref name="s" /> was converted successfully; otherwise, false.
        /// </returns>
        public static bool TryParse(string s, out Decimal128 result)
        {
            if (string.IsNullOrEmpty(s))
            {
                result = default;
                return false;
            }

            const string pattern =
                @"^(?<sign>[+-])?" +
                @"(?<significand>\d+([.]\d*)?|[.]\d+)" +
                @"(?<exponent>[eE](?<exponentSign>[+-])?(?<exponentDigits>\d+))?$";

            var match = Regex.Match(s, pattern);
            if (!match.Success)
            {
                if (s.Equals("Inf", StringComparison.OrdinalIgnoreCase) || s.Equals("Infinity", StringComparison.OrdinalIgnoreCase) ||
                    s.Equals("+Inf", StringComparison.OrdinalIgnoreCase) || s.Equals("+Infinity", StringComparison.OrdinalIgnoreCase))
                {
                    result = PositiveInfinity;
                    return true;
                }

                if (s.Equals("-Inf", StringComparison.OrdinalIgnoreCase) || s.Equals("-Infinity", StringComparison.OrdinalIgnoreCase))
                {
                    result = NegativeInfinity;
                    return true;
                }

                if (s.Equals("NaN", StringComparison.OrdinalIgnoreCase) || s.Equals("-NaN", StringComparison.OrdinalIgnoreCase))
                {
                    result = QNaN;
                    return true;
                }

                result = default;
                return false;
            }

            var isNegative = match.Groups["sign"].Value == "-";

            var exponent = 0;
            if (match.Groups["exponent"].Length != 0)
            {
                if (!int.TryParse(match.Groups["exponentDigits"].Value, out exponent))
                {
                    result = default;
                    return false;
                }

                if (match.Groups["exponentSign"].Value == "-") 
                    exponent = -exponent;
            }

            var significandString = match.Groups["significand"].Value;

            int decimalPointIndex;
            if ((decimalPointIndex = significandString.IndexOf('.')) != -1)
            {
                exponent -= significandString.Length - (decimalPointIndex + 1);
                significandString = significandString.Substring(0, decimalPointIndex) + significandString.Substring(decimalPointIndex + 1);
            }

            significandString = RemoveLeadingZeroes(significandString);
            significandString = ClampOrRound(ref exponent, significandString);

            if (exponent > ExponentMax || exponent < ExponentMin)
            {
                result = default;
                return false;
            }

            if (significandString.Length > 34)
            {
                result = default;
                return false;
            }

            var significand = UInt128.Parse(significandString);

            result = FromComponents(isNegative, (short) exponent, significand);
            return true;
        }

        private static string ClampOrRound(ref int exponent, string significandString)
        {
            if (exponent > ExponentMax)
            {
                if (significandString == "0")
                {
                    // since significand is zero simply use the largest possible exponent
                    exponent = ExponentMax;
                }
                else
                {
                    // use clamping to bring the exponent into range
                    var numberOfTrailingZeroesToAdd = exponent - ExponentMax;
                    var digitsAvailable = 34 - significandString.Length;
                    if (numberOfTrailingZeroesToAdd <= digitsAvailable)
                    {
                        exponent = ExponentMax;
                        significandString = significandString + new string('0', numberOfTrailingZeroesToAdd);
                    }
                }
            }
            else if (exponent < ExponentMin)
            {
                if (significandString == "0")
                {
                    // since significand is zero simply use the smallest possible exponent
                    exponent = ExponentMin;
                }
                else
                {
                    // use exact rounding to bring the exponent into range
                    var numberOfTrailingZeroesToRemove = ExponentMin - exponent;
                    if (numberOfTrailingZeroesToRemove < significandString.Length)
                    {
                        var trailingDigits = significandString.Substring(significandString.Length - numberOfTrailingZeroesToRemove);
                        if (Regex.IsMatch(trailingDigits, "^0+$"))
                        {
                            exponent = ExponentMin;
                            significandString = significandString.Substring(0, significandString.Length - numberOfTrailingZeroesToRemove);
                        }
                    }
                }
            }
            else if (significandString.Length > 34)
            {
                // use exact rounding to reduce significand to 34 digits
                var numberOfTrailingZeroesToRemove = significandString.Length - 34;
                if (exponent + numberOfTrailingZeroesToRemove <= ExponentMax)
                {
                    var trailingDigits = significandString.Substring(significandString.Length - numberOfTrailingZeroesToRemove);
                    if (Regex.IsMatch(trailingDigits, "^0+$"))
                    {
                        exponent += numberOfTrailingZeroesToRemove;
                        significandString = significandString.Substring(0, significandString.Length - numberOfTrailingZeroesToRemove);
                    }
                }
            }

            return significandString;
        }

        private static void TryDecreaseExponent(ref UInt128 significand, ref short exponent, short goal)
        {
            if (significand.Equals(UInt128.Zero))
            {
                exponent = goal;
                return;
            }

            while (exponent > goal)
            {
                var significandTimes10 = UInt128.Multiply(significand, 10);
                if (significandTimes10.CompareTo(MaxSignificand) > 0)
                    break;

                exponent -= 1;
                significand = significandTimes10;
            }
        }

        private static Decimal128 FromComponents(bool isNegative, short exponent, UInt128 significand)
        {
            if (exponent < ExponentMin || exponent > ExponentMax)
                throw new ArgumentOutOfRangeException(nameof(exponent));

            if (significand.CompareTo(MaxSignificand) > 0)
                throw new ArgumentOutOfRangeException(nameof(significand));

            var biasedExponent = MapExponentToDecimal128BiasedExponent(exponent);
            var highBits = ((ulong) biasedExponent << 49) | significand.High;
            if (isNegative) 
                highBits = Flags.SignBit | highBits;

            return new Decimal128(highBits, significand.Low);
        }

        private static UInt128 GetSignificand(Decimal128 d)
        {
            return new UInt128(GetSignificandHighBits(d), GetSignificandLowBits(d));
        }

        private static void TryIncreaseExponent(ref UInt128 significand, ref short exponent, short goal)
        {
            if (significand.Equals(UInt128.Zero))
            {
                exponent = goal;
                return;
            }

            while (exponent < goal)
            {
                var significandDividedBy10 = UInt128.Divide(significand, 10, out var remainder);
                if (remainder != 0)
                    break;

                exponent += 1;
                significand = significandDividedBy10;
            }
        }

        private static short MapDecimal128BiasedExponentToExponent(short biasedExponent)
        {
            if (biasedExponent <= 6111)
                return biasedExponent;

            return (short) (biasedExponent - 12288);
        }

        private static ulong MapDecimal128HighBitsToIeeeHighBits(ulong highBits)
        {
            // for Decimal128Bias from    0 to  6111: IEEEBias = Decimal128Bias + 6176
            // for Decimal128Bias from 6112 to 12287: IEEEBias = Decimal128Bias - 6112

            if (Flags.IsFirstForm(highBits))
            {
                var exponentBits = highBits & Flags.FirstFormExponentBits;
                if (exponentBits <= 6111L << 49)
                    return highBits + (6176L << 49);

                return highBits - (6112L << 49);
            }

            if (Flags.IsSecondForm(highBits))
            {
                var exponentBits = highBits & Flags.SecondFormExponentBits;
                if (exponentBits <= 6111L << 47)
                    return highBits + (6176L << 47);

                return highBits - (6112L << 47);
            }

            return highBits;
        }

        private static short MapExponentToDecimal128BiasedExponent(short exponent)
        {
            // internally we use a different bias than IEEE so that a Decimal128 struct filled with zero bytes is a true Decimal128 zero
            // Decimal128Bias is defined as:
            // exponents from     0 to 6111: biasedExponent = exponent
            // exponents from -6176 to   -1: biasedExponent = exponent + 12288

            if (exponent >= 0)
                return exponent;

            return (short) (exponent + 12288);
        }

        private static ulong MapIeeeHighBitsToDecimal128HighBits(ulong highBits)
        {
            // for IEEEBias from    0 to  6175: Decimal128Bias = IEEEBias + 6112
            // for IEEEBias from 6176 to 12287: Decimal128Bias = IEEEBias - 6176

            if (Flags.IsFirstForm(highBits))
            {
                var exponentBits = highBits & Flags.FirstFormExponentBits;
                if (exponentBits <= 6175L << 49)
                    return highBits + (6112L << 49);

                return highBits - (6176L << 49);
            }

            if (Flags.IsSecondForm(highBits))
            {
                var exponentBits = highBits & Flags.SecondFormExponentBits;
                if (exponentBits <= 6175L << 47)
                    return highBits + (6112L << 47);

                return highBits - (6176L << 47);
            }

            return highBits;
        }

        private static string RemoveLeadingZeroes(string significandString)
        {
            if (significandString[0] == '0' && significandString.Length > 1)
            {
                significandString = Regex.Replace(significandString, "^0+", "");
                return significandString.Length == 0 ? "0" : significandString;
            }

            return significandString;
        }

        private readonly ulong highBits;
        private readonly ulong lowBits;

        private Decimal128(ulong highBits, ulong lowBits)
        {
            this.highBits = highBits;
            this.lowBits = lowBits;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Decimal128"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Decimal128(decimal value)
        {
            var bits = decimal.GetBits(value);
            var isNegative = (bits[3] & 0x80000000) != 0;
            var scale = (short) ((bits[3] & 0x00FF0000) >> 16);
            var exponent = (short) -scale;
            var significandHigh = (ulong) (uint) bits[2];
            var significandLow = ((ulong) (uint) bits[1] << 32) | (uint) bits[0];

            highBits = (isNegative ? Flags.SignBit : 0) | ((ulong) MapExponentToDecimal128BiasedExponent(exponent) << 49) | significandHigh;
            lowBits = significandLow;
        }

        // public methods
        /// <inheritdoc />
        public int CompareTo(Decimal128 other)
        {
            return Compare(this, other);
        }

        /// <inheritdoc />
        public bool Equals(Decimal128 other)
        {
            return Equals(this, other);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Decimal128))
                return false;

            return Equals((Decimal128) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hash = 17;
            hash = 37 * hash + highBits.GetHashCode();
            hash = 37 * hash + lowBits.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Gets the high order 64 bits of the binary representation of this instance.
        /// </summary>
        /// <returns>The high order 64 bits of the binary representation of this instance.</returns>
        public ulong GetIeeeHighBits()
        {
            return MapDecimal128HighBitsToIeeeHighBits(highBits);
        }

        /// <summary>
        /// Gets the low order 64 bits of the binary representation of this instance.
        /// </summary>
        /// <returns>The low order 64 bits of the binary representation of this instance.</returns>
        public ulong GetIeeeLowBits()
        {
            return lowBits;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (Flags.IsFirstForm(highBits))
            {
                var exponent = GetExponent(this);
                var significand = GetSignificand(this);
                var coefficientString = significand.ToString();
                var adjustedExponent = exponent + coefficientString.Length - 1;

                string result;
                if (exponent > 0 || adjustedExponent < -6)
                    result = ToStringWithExponentialNotation(coefficientString, adjustedExponent);
                else
                    result = ToStringWithoutExponentialNotation(coefficientString, exponent);

                if (Flags.IsNegative(highBits)) result = "-" + result;

                return result;
            }

            if (Flags.IsSecondForm(highBits))
            {
                // invalid representation treated as zero
                var exponent = GetExponent(this);
                if (exponent == 0)
                    return Flags.IsNegative(highBits) ? "-0" : "0";

                var exponentString = exponent.ToString(NumberFormatInfo.InvariantInfo);
                if (exponent > 0) 
                    exponentString = "+" + exponentString;

                return (Flags.IsNegative(highBits) ? "-0E" : "0E") + exponentString;
            }

            if (Flags.IsNegativeInfinity(highBits))
                return "-Infinity";

            return Flags.IsPositiveInfinity(highBits) ? "Infinity" : "NaN";
        }

        private static string ToStringWithExponentialNotation(string coefficientString, int adjustedExponent)
        {
            if (coefficientString.Length > 1) 
                coefficientString = coefficientString.Substring(0, 1) + "." + coefficientString.Substring(1);

            var exponentString = adjustedExponent.ToString(NumberFormatInfo.InvariantInfo);
            if (adjustedExponent >= 0) 
                exponentString = "+" + exponentString;

            return coefficientString + "E" + exponentString;
        }

        private static string ToStringWithoutExponentialNotation(string coefficientString, int exponent)
        {
            if (exponent == 0)
                return coefficientString;

            var exponentAbsoluteValue = Math.Abs(exponent);
            var minimumCoefficientStringLength = exponentAbsoluteValue + 1;
            if (coefficientString.Length < minimumCoefficientStringLength) 
                coefficientString = coefficientString.PadLeft(minimumCoefficientStringLength, '0');

            var decimalPointIndex = coefficientString.Length - exponentAbsoluteValue;
            return coefficientString.Substring(0, decimalPointIndex) + "." + coefficientString.Substring(decimalPointIndex);
        }

        private static bool TryTruncateToUInt64(Decimal128 d, ulong maxNegativeValue, ulong maxPositiveValue, out ulong value)
        {
            if (IsZero(d))
            {
                value = 0;
                return true;
            }

            var exponent = GetExponent(d);
            var significand = GetSignificand(d);

            if (exponent < 0)
            {
                while (exponent < 0)
                {
                    significand = UInt128.Divide(significand, 10, out _);
                    if (significand.Equals(UInt128.Zero))
                    {
                        value = 0;
                        return true;
                    }

                    exponent += 1;
                }
            }
            else if (exponent > 0)
            {
                while (exponent > 0)
                {
                    significand = UInt128.Multiply(significand, 10);
                    if (significand.CompareTo(MaxSignificand) > 0)
                    {
                        value = 0;
                        return false;
                    }

                    exponent -= 1;
                }
            }

            if (exponent != 0)
            {
                value = 0;
                return false;
            }

            if (significand.High != 0 || significand.Low > (IsNegative(d) ? maxNegativeValue : maxPositiveValue))
            {
                value = 0;
                return false;
            }

            value = significand.Low;
            return true;
        }

        // nested types
        private class Decimal128Comparer : IComparer<Decimal128>
        {
            public static Decimal128Comparer Instance { get; } = new Decimal128Comparer();

            public int Compare(Decimal128 x, Decimal128 y)
            {
                var xType = GetDecimal128Type(x);
                var yType = GetDecimal128Type(y);
                var result = xType.CompareTo(yType);
                if (result == 0 && xType == Decimal128Type.Number)
                {
                    return CompareNumbers(x, y);
                }

                return result;
            }

            private static Decimal128Type GetDecimal128Type(Decimal128 x)
            {
                if (IsNaN(x))
                    return Decimal128Type.NaN;

                if (IsNegativeInfinity(x))
                    return Decimal128Type.NegativeInfinity;

                if (IsPositiveInfinity(x))
                    return Decimal128Type.PositiveInfity;

                return Decimal128Type.Number;
            }

            private int CompareNumbers(Decimal128 x, Decimal128 y)
            {
                var xClass = GetNumberClass(x);
                var yClass = GetNumberClass(y);
                var result = xClass.CompareTo(yClass);
                if (result == 0)
                {
                    if (xClass == NumberClass.Negative)
                        return CompareNegativeNumbers(x, y);

                    if (xClass == NumberClass.Positive)
                        return ComparePositiveNumbers(x, y);

                    return 0; // else all Zeroes compare equal
                }

                return result;
            }

            private static NumberClass GetNumberClass(Decimal128 x)
            {
                if (IsZero(x)) // must test for Zero first
                    return NumberClass.Zero;

                return IsNegative(x) ? NumberClass.Negative : NumberClass.Positive;
            }

            private static int CompareNegativeNumbers(Decimal128 x, Decimal128 y)
            {
                return -ComparePositiveNumbers(Negate(x), Negate(y));
            }

            private static int ComparePositiveNumbers(Decimal128 x, Decimal128 y)
            {
                var xExponent = GetExponent(x);
                var xSignificand = GetSignificand(x);
                var yExponent = GetExponent(y);
                var ySignificand = GetSignificand(y);

                var exponentDifference = Math.Abs(xExponent - yExponent);
                if (exponentDifference <= 66)
                {
                    // we may or may not be able to make the exponents equal but we won't know until we try
                    // but we do know we can't eliminate an exponent difference larger than 66
                    if (xExponent < yExponent)
                    {
                        TryIncreaseExponent(ref xSignificand, ref xExponent, yExponent);
                        TryDecreaseExponent(ref ySignificand, ref yExponent, xExponent);
                    }
                    else if (xExponent > yExponent)
                    {
                        TryDecreaseExponent(ref xSignificand, ref xExponent, yExponent);
                        TryIncreaseExponent(ref ySignificand, ref yExponent, xExponent);
                    }
                }

                if (xExponent == yExponent)
                    return xSignificand.CompareTo(ySignificand);

                return xExponent.CompareTo(yExponent);
            }

            private enum Decimal128Type
            {
                NaN,
                NegativeInfinity,
                Number,
                PositiveInfity
            }; // the order matters

            private enum NumberClass
            {
                Negative,
                Zero,
                Positive
            }; // the order matters
        }

        private static class Flags
        {
            public const ulong SignBit = 0x8000000000000000;
            public const ulong FirstFormLeadingBits = 0x6000000000000000;
            public const ulong FirstFormLeadingBitsMax = 0x4000000000000000;
            public const ulong FirstFormExponentBits = 0x7FFE000000000000;
            public const ulong FirstFormSignificandBits = 0x0001FFFFFFFFFFFF;
            public const ulong SecondFormLeadingBits = 0x7800000000000000;
            public const ulong SecondFormLeadingBitsMin = 0x6000000000000000;
            public const ulong SecondFormLeadingBitsMax = 0x7000000000000000;
            public const ulong SecondFormExponentBits = 0x1FFF800000000000;
            public const ulong InfinityBits = 0x7C00000000000000;
            public const ulong Infinity = 0x7800000000000000;
            public const ulong SignedInfinityBits = 0xFC00000000000000;
            public const ulong PositiveInfinity = 0x7800000000000000;
            public const ulong NegativeInfinity = 0xF800000000000000;
            public const ulong PartialNaNBits = 0x7C00000000000000;
            public const ulong PartialNaN = 0x7C00000000000000;
            public const ulong NaNBits = 0x7E00000000000000;
            public const ulong QNaN = 0x7C00000000000000;
            public const ulong SNaN = 0x7E00000000000000;

            public static bool IsFirstForm(ulong highBits)
            {
                return (highBits & FirstFormLeadingBits) <= FirstFormLeadingBitsMax;
            }

            public static bool IsInfinity(ulong highBits)
            {
                return (highBits & InfinityBits) == Infinity;
            }

            public static bool IsNaN(ulong highBits)
            {
                return (highBits & PartialNaNBits) == PartialNaN;
            }

            public static bool IsNegative(ulong highBits)
            {
                return (highBits & SignBit) != 0;
            }

            public static bool IsNegativeInfinity(ulong highBits)
            {
                return (highBits & SignedInfinityBits) == NegativeInfinity;
            }

            public static bool IsPositiveInfinity(ulong highBits)
            {
                return (highBits & SignedInfinityBits) == PositiveInfinity;
            }

            public static bool IsQNaN(ulong highBits)
            {
                return (highBits & NaNBits) == QNaN;
            }

            public static bool IsSecondForm(ulong highBits)
            {
                var secondFormLeadingBits = highBits & SecondFormLeadingBits;
                return secondFormLeadingBits >= SecondFormLeadingBitsMin & secondFormLeadingBits <= SecondFormLeadingBitsMax;
            }

            public static bool IsSNaN(ulong highBits)
            {
                return (highBits & NaNBits) == SNaN;
            }
        }
    }
}