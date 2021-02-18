using NUnit.Framework;
using JuanMartin.Utilities.Euler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuanMartin.Utilities.Euler.Tests
{
    [TestFixture()]
    public class RomanNumeralTests
    {
        [Test()]
        public void RomanNumeral_ABC_ShouldBeInValid()
        {
            var number = new RomanNumeral();

            number.Value = "ABC";
            Assert.IsFalse(number.IsValid(), $"{number.Value} is valid");

        }

        [Test()]
        public void RomanNumeral_IXX_ShouldBeInValid()
        {
            var number = new RomanNumeral();

            number.Value = "IXX";
            Assert.IsFalse(number.IsValid(), $"{number.Value} is valid");
        }
        [Test()]
        public void RomanNumeral_IL_ShouldBeInValid()
        {
            var number = new RomanNumeral();

            number.Value = "IL";
            Assert.IsFalse(number.IsValid(), $"{number.Value} is valid");
        }

        [Test()]
        public void RomanNumeral_VIV_ShouldBeInValid()
        {
            var number = new RomanNumeral();

            number.Value = "VIV";
            Assert.IsFalse(number.IsValid(), $"{number.Value} is valid");
        }


        [Test()]
        public void RomanNumeralShouldBeValid()
        {
            var number = new RomanNumeral();

            number.Value = "XLIX";
            Assert.IsTrue(number.IsValid(), $"{number.Value} is invalid");

            number.Value = "XVIIII";
            Assert.IsTrue(number.IsValid(), $"{number.Value} is invalid");

            number.Value = "XXXXVIIII";
            Assert.IsTrue(number.IsValid(), $"{number.Value} is invalid");

            number.Value = "XIX";
            Assert.IsTrue(number.IsValid(), $"{number.Value} is invalid");

            number.Value = "MCDVI";
            Assert.IsTrue(number.IsValid(), $"{number.Value} is invalid");

        }

        [Test()]
        public void RomanNumeralsShouldHaveTheSameArabicRepresentation()
        {
            var expected_arabic_value = 49;
            var number = new RomanNumeral();

            number.Value = "XXXXIIIIIIIII";
            Assert.AreEqual(expected_arabic_value, number.ToArabic(), $"{number.Value} is not equal to {number.ArabicValue}");

            number.Value = "XXXXIX";
            Assert.AreEqual(expected_arabic_value, number.ToArabic(), $"{number.Value} is not equal to {number.ArabicValue}");

            number.Value = "XXXXVIIII";
            Assert.AreEqual(expected_arabic_value, number.ToArabic(), $"{number.Value} is not equal to {number.ArabicValue}");

            number.Value = "XLIIIIIIIII";
            Assert.AreEqual(expected_arabic_value, number.ToArabic(), $"{number.Value} is not equal to {number.ArabicValue}");

            number.Value = "XLVIIII";
            Assert.AreEqual(expected_arabic_value, number.ToArabic(), $"{number.Value} is not equal to {number.ArabicValue}");

            number.Value = "XLIX";
            Assert.AreEqual(expected_arabic_value, number.ToArabic(), $"{number.Value} is not equal to {number.ArabicValue}");

        }

        [Test()]
        public void RomanNumeralGetMinimalForm_ShouldRetunrValidRomanFormOfArabicNumberSpecifiedInArguments()
        {
            var number = new RomanNumeral();

            var expected_roman_numeral = "XLIX";
            var actual_arabic_number = 49;

            Assert.AreEqual(expected_roman_numeral, number.GetMinimalForm(actual_arabic_number));

            expected_roman_numeral = "|VII|CCXXXI";
            actual_arabic_number = 7231;

            Assert.AreEqual(expected_roman_numeral, number.GetMinimalForm(actual_arabic_number));

            expected_roman_numeral = "|LXXVI|CCXXX";
            actual_arabic_number = 76230;

            Assert.AreEqual(expected_roman_numeral, number.GetMinimalForm(actual_arabic_number));
        }

        [Test()]
        public void RomanNumeralGetMinimalForm_ShouldRetunrShortestValidRomanRepresentationOfClassValue()
        {
            var number = new RomanNumeral();
            // Generally the Romans tried to use as few numerals as possible when displaying numbers.For this reason, 
            // XIX would be the preferred form of nineteen over other valid combinations, like XIIIIIIIII or XVIIII.
            var expected_roman_numeral = "XIX";
            var actual_roman_numeral = "XVIIII";
            number.Value = actual_roman_numeral;
            Assert.AreEqual(expected_roman_numeral, number.GetMinimalForm());

            actual_roman_numeral = "XIIIIIIIII";
            number.Value = actual_roman_numeral;
            Assert.AreEqual(expected_roman_numeral, number.GetMinimalForm());

            // In the church of Sant'Agnese fuori le Mura (St Agnes' outside 
            // the walls), found in Rome, the date, MCCCCCCVI (1606), is written on the gilded and coffered wooden 
            // ceiling; I am sure that many would argue that it should have been written MDCVI.
            expected_roman_numeral = "MDCVI";
            actual_roman_numeral = "MCCCCCCVI";
            number.Value = actual_roman_numeral;
            Assert.AreEqual(expected_roman_numeral, number.GetMinimalForm());


            expected_roman_numeral = "MCCLXXIX";
            actual_roman_numeral = "MCCLXXVIIII";
            number.Value = actual_roman_numeral;
            Assert.AreEqual(expected_roman_numeral, number.GetMinimalForm());
        }

        [Test()]
        public void RomanNumeral_MMCCLIV_IsItsOwnGetMinimalForm()
        {
            var number = new RomanNumeral();
            var actual_roman_numeral = "MMCCLIV";
            var expected_roman_numeral = actual_roman_numeral;
            number.Value = actual_roman_numeral;
            Assert.AreEqual(expected_roman_numeral, number.GetMinimalForm());
        }

        [Test()]
        public void RomanNumeral_D_IsItsOwnGetMinimalForm()
        {
            var number = new RomanNumeral();
            var actual_roman_numeral = "D";
            var expected_roman_numeral = actual_roman_numeral;
            number.Value = actual_roman_numeral;
            Assert.AreEqual(expected_roman_numeral, number.GetMinimalForm());
        }

        [Test()]
        public void RomanNumeral_IV_IsItsOwnGetMinimalForm()
        {
            var number = new RomanNumeral();
            var actual_roman_numeral = "IV";
            var expected_roman_numeral = actual_roman_numeral;
            number.Value = actual_roman_numeral;
            Assert.AreEqual(expected_roman_numeral, number.GetMinimalForm());
        }
    }
}