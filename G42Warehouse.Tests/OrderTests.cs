using System;
using G42Warehouse;
using Xunit;

namespace G42Warehouse.Tests
{
    public class OrderTests
    {
        public OrderTests()
        {
            ExtentManager.Reset();
        }

        [Fact]
        public void Order_Constructor_ValidData_AddsToExtentAndSetsDefaultValues()
        {
            var orders = ExtentManager.Instance.ExtentOrder;
            int beforeCount = orders.Count;

            var orderDate = DateTime.Now.AddMinutes(-5);
            Dictionary<Item,int> aee = new Dictionary<Item,int>();
            aee.Add(new Item("Pen", false, ItemCategory.RawMaterial, null, null, null, 0, 25, 25, 25), 2);

            var order = new Order(orderDate,new Customer("J","j","j"), aee);

            Assert.Equal(orderDate, order.OrderDate);
            Assert.Equal(PaymentStatus.Non_Paid, order.PaymentStatus);
            Assert.Equal(PreparationStatus.In_Progress, order.PreparationStatus);
            Assert.Equal(PaymentMethod.Cash, order.PaymentMethod);

            Assert.Contains(order, orders);
            Assert.Equal(beforeCount + 1, orders.Count);
        }

        [Fact]
        public void Order_OrderDate_SetToFuture_ThrowsArgumentException()
        {
            Dictionary<Item, int> aee = new Dictionary<Item, int>();
            aee.Add(new Item("Pen", false, ItemCategory.RawMaterial, null, null, null, 0, 25, 25, 25), 2);

            var order = new Order(DateTime.Now, new Customer("J", "j", "j"), aee);


            Assert.Throws<ArgumentException>(() =>
            {
                order.OrderDate = DateTime.Now.AddDays(1);
            });
        }

        [Fact]
        public void Order_StatusAndPayment_CanBeUpdated()
        {

            Dictionary<Item, int> aee = new Dictionary<Item, int>();
            aee.Add(new Item("Pen", false, ItemCategory.RawMaterial, null, null, null, 0, 25, 25, 25), 2);

            var order = new Order(DateTime.Now.AddMinutes(-10), new Customer("J", "j", "j"), aee);
            order.PaymentStatus = PaymentStatus.Paid;
            order.PreparationStatus = PreparationStatus.Ready;
            order.PaymentMethod = PaymentMethod.CreditCard;
            Assert.Equal(PaymentStatus.Paid, order.PaymentStatus);
            Assert.Equal(PreparationStatus.Ready, order.PreparationStatus);
            Assert.Equal(PaymentMethod.CreditCard, order.PaymentMethod);
        }
    }
}