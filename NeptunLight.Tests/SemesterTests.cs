using System;
using NeptunLight.Models;
using Xunit;

namespace NeptunLight.Tests
{
    public class SemesterTests
    {
        [Theory]
        [InlineData(2017, 1)]
        [InlineData(1998, 2)]
        public void Costructor_Correct(int year, int semester)
        {
            Semester x = new Semester(year, semester);
            Assert.Equal(year, x.PrimaryYear);
            Assert.Equal(year+1, x.SecondaryYear);
            Assert.Equal(semester, x.SemesterNumber);
        }

        [Theory]
        [InlineData(2017, 0)]
        [InlineData(1998, 3)]
        public void Costructor_OutofRange(int year, int semester)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Semester(year, semester));
        }

        [Fact]
        public void Name()
        {
            Semester a = new Semester(2017, 2);
            Assert.Equal("2017/18/2", a.Name);
        }

        [Fact]
        public void Parse_Correct()
        {
            Semester s = Semester.Parse("2017/18/2");
            Assert.Equal(2017, s.PrimaryYear);
            Assert.Equal(2, s.SemesterNumber);
        }
    }
}