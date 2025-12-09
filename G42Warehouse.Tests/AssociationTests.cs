using System;
using System.Collections.Generic;
using G42Warehouse;
using Xunit;

namespace G42Warehouse.Tests
{
    public class AssociationTests : IDisposable
    {
        public AssociationTests()
        {
            ExtentManager.Load("nonexistingpath.xml");
        }
    
        public void Dispose()
        {
            ExtentManager.Load("nonexistingpath.xml");
        }

        private RefrigeratedSection CreateSection(
            string name = "Sec1",
            string location = "Location1",
            int capacity = 10,
            bool isFreezer = false,
            double minTemp = -10,
            double maxTemp = 5)
        {
            return new RefrigeratedSection(name, location, capacity, isFreezer, minTemp, maxTemp);
        }

        private Shelf CreateShelf(ShelfType type, int capacity, RefrigeratedSection section)
        {
            return new Shelf(type, capacity, section);
        }

        private Item CreateItem(
            string name = "Item",
            ItemCategory category = ItemCategory.Food)
        {
            return new Item(name, false, category, null, null, null, 10, 5.0, 10.0, 15.0);
        }

        private Customer CreateCustomer(
            string name = "John Doe",
            string phone = "1234567890",
            string email = "john@example.com")
        {
            return new Customer(name, phone, email);
        }

        private WarehouseManager CreateWarehouseManager(
            string name = "Manager1",
            int yearsAgo = 5,
            double salary = 5000)
        {
            return new WarehouseManager(name, DateTime.Now.AddYears(-yearsAgo), salary);
        }

        private Worker CreateWorker(
            string name,
            HashSet<WarehouseManager> managers,
            int yearsAgo = 2,
            double salary = 3000)
        {
            return new Worker(name, DateTime.Now.AddYears(-yearsAgo), salary, managers);
        }

        private DeliveryDriver CreateDeliveryDriver(
            string name,
            HashSet<WarehouseManager> managers,
            DriverLicenceType licenceType)
        {
            return new DeliveryDriver(name, DateTime.Now.AddYears(-1), 3500, managers, licenceType);
        }

        private Transporter CreateTransporter()
        {
            return new Transporter(MachineStatus.Ready, DateTime.Now.AddDays(-30), 100, 10);
        }

        private Lifter CreateLifter()
        {
            return new Lifter(MachineStatus.Ready, DateTime.Now.AddDays(-30), 200);
        }

        private Address CreateAddress(
            string building = "Building1",
            string street = "Street1",
            string city = "City1",
            string postalCode = "12345",
            string country = "Country1")
        {
            return new Address(building, street, city, country, postalCode);
        }
        
        private Delivery CreateDeliveryWithSingleDriver(out DeliveryDriver driver)
        {
            var manager  = CreateWarehouseManager();
            var managers = new HashSet<WarehouseManager> { manager };
            driver       = CreateDeliveryDriver("Driver1", managers, DriverLicenceType.C);

            var customer     = CreateCustomer("Customer1", "111222333", "customer@example.com");
            var item         = CreateItem("Item1", ItemCategory.Food);
            var initialItems = new Dictionary<Item, int> { { item, 2 } };
            var order        = new Order(DateTime.Now, customer, initialItems);
            var address      = CreateAddress();

            var drivers = new HashSet<DeliveryDriver> { driver };
            return new Delivery(12345, DateTime.Now, DeliveryStatus.Pending,
                                address, null, DateTime.Now, order, drivers);
        }

        // Test 1: Shelf-Shelf Consisting Association - Add with Reverse Connection
        [Fact]
        public void Shelf_AddConsisting_CreatesReverseConnection()
        {
            var section = CreateSection();
            var shelf1 = CreateShelf(ShelfType.Pallet_Rack, 100, section);
            var shelf2 = CreateShelf(ShelfType.Solid_Rack, 150, section);

            shelf1.addConsisting(shelf2);

            Assert.Contains(shelf2, shelf1.ConsistingShelves);
            Assert.Contains(shelf1, shelf2.ConsistingShelves);
        }

        // Test 2: Shelf-Shelf Consisting Association - Remove with Reverse Connection
        [Fact]
        public void Shelf_RemoveConsisting_RemovesReverseConnection()
        {
            var section = CreateSection();
            var shelf1 = CreateShelf(ShelfType.Pallet_Rack, 100, section);
            var shelf2 = CreateShelf(ShelfType.Solid_Rack, 150, section);

            shelf1.addConsisting(shelf2);
            shelf1.removeConsisting(shelf2);

            Assert.DoesNotContain(shelf2, shelf1.ConsistingShelves);
            Assert.DoesNotContain(shelf1, shelf2.ConsistingShelves);
        }
        
        // Test 3: Shelf-Item Association - Add with Reverse Connection
        [Fact]
        public void Shelf_AddItem_CreatesReverseConnection()
        {
            var section = CreateSection();
            var shelf   = CreateShelf(ShelfType.Pallet_Rack, 100, section);
            var item    = CreateItem("TestItem", ItemCategory.Food);

            shelf.addItem(item, 2);

            Assert.Contains(item, shelf.Items);
            Assert.Equal(shelf, item.PlacementInf.Shelf);
            Assert.Equal(2, item.PlacementInf.ShelfLevel);
        }

        // Test 4: Shelf-Item Association - Remove with Reverse Connection
        [Fact]
        public void Shelf_RemoveItem_RemovesReverseConnection()
        {
            var section = CreateSection();
            var shelf   = CreateShelf(ShelfType.Pallet_Rack, 100, section);
            var item    = CreateItem("TestItem", ItemCategory.Food);

            shelf.addItem(item, 2);
            shelf.removeItem(item);

            Assert.DoesNotContain(item, shelf.Items);
            Assert.Null(item.PlacementInf.Shelf);
        }

        // Test 5: Shelf-Item Association - Error when Adding Item to Different Shelf
        [Fact]
        public void Shelf_AddItem_ThrowsWhenItemAlreadyOnDifferentShelf()
        {
            var section = CreateSection();
            var shelf1  = CreateShelf(ShelfType.Pallet_Rack, 100, section);
            var shelf2  = CreateShelf(ShelfType.Solid_Rack, 150, section);
            var item    = CreateItem("TestItem", ItemCategory.Food);

            shelf1.addItem(item, 2);

            Assert.Throws<ArgumentException>(() => shelf2.addItem(item, 3));
        }

        // Test 6: Machine-Item Association - Add with Reverse Connection
        [Fact]
        public void Machine_AddItem_CreatesReverseConnection()
        {
            var machine = CreateTransporter();
            var item    = CreateItem("TestItem", ItemCategory.Electronics);

            machine.addItem(item, 5);

            Assert.Contains(item, machine.Items.Keys);
            Assert.Equal(machine, item.CarryingMachine);
            Assert.Equal(6, machine.Items[item]);
        }

        // Test 7: Machine-Item Association - Remove with Reverse Connection
        [Fact]
        public void Machine_RemoveItem_RemovesReverseConnection()
        {
            var machine = CreateLifter();
            var item    = CreateItem("TestItem", ItemCategory.RawMaterial);

            machine.addItem(item, 3);
            machine.removeItem(item);

            Assert.DoesNotContain(item, machine.Items.Keys);
            Assert.Null(item.CarryingMachine);
        }

        // Test 8: Machine-Item Association - Error when Quantity is Non-Positive
        [Fact]
        public void Machine_AddItem_ThrowsWhenQuantityIsZeroOrNegative()
        {
            var machine = CreateTransporter();
            var item    = CreateItem("TestItem", ItemCategory.Chemical);

            Assert.Throws<ArgumentOutOfRangeException>(() => machine.addItem(item, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => machine.addItem(item, -5));
        }

        // Test 9: Order-Item Association - Add with Reverse Connection
        [Fact]
        public void Order_AddItem_CreatesReverseConnection()
        {
            var customer = CreateCustomer("John Doe", "1234567890", "john@example.com");
            
            var existingItem = CreateItem("ExistingItem", ItemCategory.Food);
            var initialItems = new Dictionary<Item, int> { { existingItem, 1 } };
            var order        = new Order(DateTime.Now, customer, initialItems);
            
            var newItem = CreateItem("NewItem", ItemCategory.Food);
            order.addItem(newItem, 2);

            Assert.Contains(newItem, order.Items.Keys);
            Assert.Equal(order, newItem.ItemOrder);
        }

        // Test 10: Order-Item Association - Error when Removing Last Item
        [Fact]
        public void Order_RemoveItem_ThrowsWhenRemovingLastItem()
        {
            var customer     = CreateCustomer("John Doe", "1234567890", "john@example.com");
            var item         = CreateItem("TestItem", ItemCategory.Medical);
            var initialItems = new Dictionary<Item, int> { { item, 2 } };
            var order        = new Order(DateTime.Now, customer, initialItems);

            Assert.Throws<ArgumentException>(() => order.removeItem(item));
        }

        // Test 11: Order-Item Association - Modify Item Quantity
        [Fact]
        public void Order_ModifyItemOrder_UpdatesQuantityCorrectly()
        {
            var customer     = CreateCustomer("Jane Doe", "0987654321", "jane@example.com");
            var item1        = CreateItem("Item1", ItemCategory.Food);
            var item2        = CreateItem("Item2", ItemCategory.Electronics);
            var initialItems = new Dictionary<Item, int> { { item1, 5 }, { item2, 5 } };
            var order        = new Order(DateTime.Now, customer, initialItems);
            
            order.modifyItemOrder(item1, -2);

            Assert.Equal(7, order.Items[item1]);
        }
        
        // Test 12: Worker-Manager Association - Add with Reverse Connection
        [Fact]
        public void Worker_AddManager_CreatesReverseConnection()
        {
            var manager  = CreateWarehouseManager("Manager1", yearsAgo: 5, salary: 5000);
            var managers = new HashSet<WarehouseManager> { manager };
            var worker   = CreateWorker("Worker1", managers);
            
            manager.addWorkerToManage(worker);

            Assert.Contains(manager, worker.Managers);
            Assert.Contains(worker, manager.Workers);
        }

        // Test 13: Worker-Manager Association - Remove with Reverse Connection
        [Fact]
        public void Worker_RemoveManager_RemovesReverseConnection()
        {
            var manager1 = CreateWarehouseManager("Manager1", yearsAgo: 5, salary: 5000);
            var manager2 = CreateWarehouseManager("Manager2", yearsAgo: 3, salary: 5500);
            var managers = new HashSet<WarehouseManager> { manager1, manager2 };
            var worker   = CreateWorker("Worker1", managers);

            worker.removeManager(manager1);

            Assert.DoesNotContain(manager1, worker.Managers);
            Assert.DoesNotContain(worker, manager1.Workers);
        }
        
        // Test 14: Delivery-DeliveryDriver Association - addDelivery Method Creates Reverse Connection
        [Fact]
        public void Delivery_AddDeliveryMethod_CreatesReverseConnection()
        {
            var delivery = CreateDeliveryWithSingleDriver(out var driver);
            
            Assert.DoesNotContain(delivery, driver.Deliveries);
            
            driver.addDelivery(delivery);
            
            Assert.Contains(driver, delivery.AssignedDrivers);
            Assert.Contains(delivery, driver.Deliveries);
        }

        // Test 15: Delivery-DeliveryDriver Association - Error when Removing Last Driver
        [Fact]
        public void Delivery_RemoveDriver_ThrowsWhenRemovingLastDriver()
        {
            var delivery = CreateDeliveryWithSingleDriver(out var driver);

            Assert.Throws<ArgumentException>(() => delivery.removeDeliveryDriver(driver));
        }
    }
}
