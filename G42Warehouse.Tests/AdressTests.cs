using System;
using G42Warehouse;
using Xunit;

namespace G42Warehouse.Tests
{
    public class AddressTests
    {
        [Fact]
        public void Address_Constructor_ValidData_SetsProperties()
        {
            var address = new Address(
                "B1",
                "Dluga 10",
                "Krakow",
                "Poland",
                "30-001");

            Assert.Equal("B1", address.Building);
            Assert.Equal("Dluga 10", address.Street);
            Assert.Equal("Krakow", address.City);
            Assert.Equal("Poland", address.Country);
            Assert.Equal("30-001", address.PostalCode);
        }

        [Fact]
        public void Address_EmptyBuilding_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Address(
                    "",
                    "Street",
                    "City",
                    "Country",
                    "00-001"));
        }

        [Fact]
        public void Address_EmptyStreet_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Address(
                    "B1",
                    "",
                    "City",
                    "Country",
                    "00-001"));
        }

        [Fact]
        public void Address_EmptyCity_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Address(
                    "B1",
                    "Street",
                    "",
                    "Country",
                    "00-001"));
        }

        [Fact]
        public void Address_EmptyCountry_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Address(
                    "B1",
                    "Street",
                    "City",
                    "",
                    "00-001"));
        }

        [Fact]
        public void Address_EmptyPostalCode_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Address(
                    "B1",
                    "Street",
                    "City",
                    "Country",
                    ""));
        }
    }
}
