using System;
using G42Warehouse;
using Xunit;

namespace G42Warehouse.Tests
{
    public class CustomerTests
    {
        public CustomerTests()
        {
            // Make sure extents are clean before each test run
            ExtentManager.Reset();
        }

        [Fact]
        public void Customer_Constructor_ValidData_AddsToExtentAndSetsProperties()
        {
            var customers = ExtentManager.Instance.ExtentCustomer;
            int beforeCount = customers.Count;

            var customer = new Customer(
                name: "ACME Corp",
                telnumber: "+48 123 456 789",
                email: "contact@acme.test");

            Assert.Equal("ACME Corp", customer.Name);
            Assert.Equal("+48 123 456 789", customer.TelephoneNumber);
            Assert.Equal("contact@acme.test", customer.Email);

            Assert.Contains(customer, customers);
            Assert.Equal(beforeCount + 1, customers.Count);
        }

        [Fact]
        public void Customer_EmptyName_ThrowsArgumentException()
        {
            ExtentManager.Reset();

            Assert.Throws<ArgumentException>(() =>
                new Customer(
                    name: "",
                    telnumber: "+48 123 456 789",
                    email: "contact@acme.test"));
        }

        [Fact]
        public void Customer_EmptyTelephone_ThrowsArgumentException()
        {
            ExtentManager.Reset();

            Assert.Throws<ArgumentException>(() =>
                new Customer(
                    name: "ACME Corp",
                    telnumber: "",
                    email: "contact@acme.test"));
        }

        [Fact]
        public void Customer_EmptyEmail_ThrowsArgumentException()
        {
            ExtentManager.Reset();

            Assert.Throws<ArgumentException>(() =>
                new Customer(
                    name: "ACME Corp",
                    telnumber: "+48 123 456 789",
                    email: ""));
        }
    }
}
