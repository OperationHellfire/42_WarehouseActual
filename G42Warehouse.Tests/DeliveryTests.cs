using System;
using G42Warehouse;
using Xunit;

namespace G42Warehouse.Tests
{
    public class DeliveryTests
    {
        public DeliveryTests()
        {
            ExtentManager.Reset();
        }

        private static Address CreateAddress()
        {
            return new Address(
                "B1",
                "Main Street",
                "Warsaw",
                "Poland",
                "00-001");
        }

        [Fact]
        public void Delivery_Constructor_ValidData_AddsToExtentAndSetsProperties()
        {
            var deliveries = ExtentManager.Instance.ExtentDelivery;
            int beforeCount = deliveries.Count;

            var address = CreateAddress();
            var date = DateTime.Now.AddHours(-1);
            var expected = DateTime.Now.AddHours(3);
            var actual = DateTime.Now; // <= Now inside setter
            Dictionary<Item, int> aee = new Dictionary<Item, int>();
            aee.Add(new Item("Pen", false, ItemCategory.RawMaterial, null, null, null, 0, 25, 25, 25), 2);

            var delivery = new Delivery(
                trackingNumber: 123,
                date: date,
                status: DeliveryStatus.Pending,
                deliveryaddress: address,
                expected: expected,
                actual: actual,
                                    new Order(DateTime.Now, new Customer("j", "j", "j"), aee),
                    new HashSet<DeliveryDriver> { new DeliveryDriver() });

            Assert.Equal(123, delivery.TrackingNumber);
            Assert.Equal(date, delivery.Date);
            Assert.Equal(DeliveryStatus.Pending, delivery.Status);
            Assert.Equal(address, delivery.Address);
            Assert.Equal(expected, delivery.Expectedarrival);

            Assert.Contains(delivery, deliveries);
            Assert.Equal(beforeCount + 1, deliveries.Count);
        }

        [Fact]
        public void Delivery_TrackingNumber_Negative_ThrowsArgumentException()
        {
            var address = CreateAddress();
            var now = DateTime.Now;

            Dictionary<Item, int> aee = new Dictionary<Item, int>();
            aee.Add(new Item("Pen", false, ItemCategory.RawMaterial, null, null, null, 0, 25, 25, 25), 2);

            Assert.Throws<ArgumentException>(() =>
                new Delivery(
                    trackingNumber: -1,
                    date: now,
                    status: DeliveryStatus.Pending,
                    deliveryaddress: address,
                    expected: null,
                    actual: now,
                    new Order(DateTime.Now, new Customer("j", "j", "j"), aee),
                    new HashSet<DeliveryDriver> { new DeliveryDriver()}));
        }

        [Fact]
        public void Delivery_Date_SetToFuture_ThrowsArgumentException()
        {
            var address = CreateAddress();

            Dictionary<Item, int> aee = new Dictionary<Item, int>();
            aee.Add(new Item("Pen", false, ItemCategory.RawMaterial, null, null, null, 0, 25, 25, 25), 2);

            var delivery = new Delivery(
                trackingNumber: 1,
                date: DateTime.Now,
                status: DeliveryStatus.Pending,
                deliveryaddress: address,
                expected: null,
                actual: DateTime.Now,
                    new Order(DateTime.Now, new Customer("j", "j", "j"), aee),
                    new HashSet<DeliveryDriver> { new DeliveryDriver() });

            Assert.Throws<ArgumentException>(() =>
            {
                delivery.Date = DateTime.Now.AddDays(1);
            });
        }

        [Fact]
        public void Delivery_ExpectedArrival_SetToPast_ThrowsArgumentException()
        {
            var address = CreateAddress();

            Dictionary<Item, int> aee = new Dictionary<Item, int>();
            aee.Add(new Item("Pen", false, ItemCategory.RawMaterial, null, null, null, 0, 25, 25, 25), 2);

            var delivery = new Delivery(
                trackingNumber: 1,
                date: DateTime.Now,
                status: DeliveryStatus.InTransit,
                deliveryaddress: address,
                expected: DateTime.Now.AddDays(1),
                actual: DateTime.Now,
                    new Order(DateTime.Now, new Customer("j", "j", "j"), aee),
                    new HashSet<DeliveryDriver> { new DeliveryDriver() });

            Assert.Throws<ArgumentException>(() =>
            {
                delivery.Expectedarrival = DateTime.Now.AddMinutes(-10);
            });
        }

        [Fact]
        public void Delivery_ActualArrival_InFutureInConstructor_ThrowsArgumentException()
        {
            var address = CreateAddress();
            var now = DateTime.Now;
            var futureActual = DateTime.Now.AddDays(1);

            Dictionary<Item, int> aee = new Dictionary<Item, int>();
            aee.Add(new Item("Pen", false, ItemCategory.RawMaterial, null, null, null, 0, 25, 25, 25), 2);

            Assert.Throws<ArgumentException>(() =>
                new Delivery(
                    trackingNumber: 1,
                    date: now,
                    status: DeliveryStatus.Pending,
                    deliveryaddress: address,
                    expected: null,
                    actual: futureActual,
                    new Order(DateTime.Now, new Customer("j", "j", "j"), aee),
                    new HashSet<DeliveryDriver> { new DeliveryDriver() }));
        }
    }
}
