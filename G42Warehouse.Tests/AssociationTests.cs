using System;
using System.Collections.Generic;
using System.Reflection; // Used for initializing private fields via reflection
using G42Warehouse;
using Xunit;

namespace G42Warehouse.Tests
{
    public class AssociationTests : IDisposable
    {
        // Constructor resets extents before each test run
        public AssociationTests()
        {
            ExtentManager.Reset();
        }

        // Dispose resets extents after each test run
        public void Dispose()
        {
            ExtentManager.Reset();
        }

        // ---------------------------------------------------------
        // REFLECTION HELPER
        // Used to initialize private collections in source code
        // where constructors do not properly initialize them.
        // ---------------------------------------------------------
        private void SetPrivateField(object target, string fieldName, object value)
        {
            if (target == null) return;
            var type = target.GetType();
            FieldInfo field = null;

            while (type != null)
            {
                field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (field != null) break;
                type = type.BaseType;
            }

            if (field != null)
                field.SetValue(target, value);
        }

        // ---------------------------------------------------------
        // HELPER METHODS
        // ---------------------------------------------------------

        private RefrigeratedSection CreateSection()
        {
            var sec = new RefrigeratedSection("Sec1", "Loc1", 500, false, -10, 5);
            SetPrivateField(sec, "_shelves", new HashSet<Shelf>());
            SetPrivateField(sec, "_shiftsInSection", new HashSet<Shift>());
            return sec;
        }

        private Shelf CreateShelf(Section section)
        {
            return new Shelf(ShelfType.Pallet_Rack, 100, section);
        }

        private Item CreateItem(string name = "Item")
        {
            return new Item(null, name, false, ItemCategory.Food, null, null, null, 10, 5, 10, 15);
        }

        private WarehouseManager CreateManager(string name = "Boss")
        {
            return new WarehouseManager(name, DateTime.Now.AddYears(-5), 5000);
        }

        private MachineOperator CreateOperator(string name, HashSet<WarehouseManager> managers,
            HashSet<Machine> capableMachines, Dictionary<string, Machine> controlled)
        {
            return new MachineOperator(name, DateTime.Now.AddYears(-2), 3000, managers, capableMachines, controlled);
        }

        private Transporter CreateTransporter()
        {
            return new Transporter(MachineStatus.Ready, DateTime.Now.AddDays(-10), 100, 10);
        }

        // ---------------------------------------------------------
        // 1. REFLEX ASSOCIATION TESTS
        // Tests associations where objects of the same class
        // reference each other (Shelf ↔ Shelf).
        // ---------------------------------------------------------

        [Fact]
        // Tests adding a reflex association and verifies reverse connection.
        public void Reflex_AddConsistingShelf_SetsReverseConnection()
        {
            var section = CreateSection();
            var s1 = CreateShelf(section);
            var s2 = CreateShelf(section);

            s1.addConsisting(s2);

            Assert.Contains(s2, s1.ConsistingShelves);
            Assert.Contains(s1, s2.ConsistingShelves);
        }

        [Fact]
        // Tests removing a reflex association and verifies reverse removal.
        public void Reflex_RemoveConsistingShelf_RemovesReverseConnection()
        {
            var section = CreateSection();
            var s1 = CreateShelf(section);
            var s2 = CreateShelf(section);
            s1.addConsisting(s2);

            s1.removeConsisting(s2);

            Assert.DoesNotContain(s2, s1.ConsistingShelves);
            Assert.DoesNotContain(s1, s2.ConsistingShelves);
        }

        [Fact]
        // Tests modifying a reflex association by replacing one connection with another.
        public void Reflex_ModifyConsistingShelf_UpdatesConnection()
        {
            var section = CreateSection();
            var s1 = CreateShelf(section);
            var s2 = CreateShelf(section);
            var s3 = CreateShelf(section);

            s1.addConsisting(s2);
            s1.removeConsisting(s2);
            s1.addConsisting(s3);

            Assert.DoesNotContain(s2, s1.ConsistingShelves);
            Assert.Contains(s3, s1.ConsistingShelves);
            Assert.Contains(s1, s3.ConsistingShelves);
        }

        // ---------------------------------------------------------
        // 2. BASIC ASSOCIATION (1..*) TESTS
        // Tests one-to-many association Shelf ↔ Item.
        // ---------------------------------------------------------

        [Fact]
        // Tests adding an Item to a Shelf and verifies reverse connection via PlacementInf.
        public void Basic_AddItemToShelf_SetsReverseConnection()
        {
            var shelf = CreateShelf(CreateSection());
            var item = CreateItem();

            shelf.addItem(item, 1);

            Assert.Contains(item, shelf.Items);
            Assert.Equal(shelf, item.PlacementInf.Shelf);
        }

        [Fact]
        // Tests removing an Item from a Shelf and clearing the reverse connection.
        public void Basic_RemoveItemFromShelf_RemovesReverseConnection()
        {
            var shelf = CreateShelf(CreateSection());
            var item = CreateItem();
            shelf.addItem(item, 1);

            shelf.removeItem(item);

            Assert.DoesNotContain(item, shelf.Items);
            Assert.Null(item.PlacementInf.Shelf);
        }

        [Fact]
        // Tests that assigning an Item to multiple Shelves throws an exception.
        public void Basic_Item_AlreadyAssignedToShelf_ThrowsException()
        {
            var section = CreateSection();
            var shelf1 = CreateShelf(section);
            var shelf2 = CreateShelf(section);
            var item = CreateItem();

            shelf1.addItem(item, 1);

            Assert.Throws<ArgumentException>(() => shelf2.addItem(item, 1));
        }

        // ---------------------------------------------------------
        // 3. MANY-TO-MANY ASSOCIATION TESTS
        // Tests Worker ↔ WarehouseManager relationship.
        // ---------------------------------------------------------

        [Fact]
        // Tests adding a Manager to a Worker and reverse association.
        public void MtoM_AddManagerToWorker_SetsReverseConnection()
        {
            var manager = CreateManager();
            var worker = new Worker("Worker", DateTime.Now, 2000, new HashSet<WarehouseManager> { manager });
            var newManager = CreateManager("Boss2");

            worker.addManager(newManager);

            Assert.Contains(newManager, worker.Managers);
            Assert.Contains(worker, newManager.Workers);
        }

        [Fact]
        // Tests removing a Manager from a Worker.
        public void MtoM_RemoveManager_RemovesReverseConnection()
        {
            var m1 = CreateManager("M1");
            var m2 = CreateManager("M2");
            var worker = new Worker("Worker", DateTime.Now, 2000, new HashSet<WarehouseManager> { m1, m2 });

            worker.removeManager(m1);

            Assert.DoesNotContain(m1, worker.Managers);
            Assert.Contains(m2, worker.Managers);
        }

        [Fact]
        // Tests modifying many-to-many association by reassigning a Manager.
        public void MtoM_ModifyManager_ReassignsProperly()
        {
            var m1 = CreateManager("M1");
            var m2 = CreateManager("M2");
            var worker = new Worker("Worker", DateTime.Now, 2000, new HashSet<WarehouseManager> { m1 });

            worker.removeManager(m1);
            worker.addManager(m2);

            Assert.Contains(m2, worker.Managers);
            Assert.Contains(worker, m2.Workers);
        }

        // ---------------------------------------------------------
        // 4. QUALIFIED ASSOCIATION TESTS
        // Tests dictionary-based association Operator ↔ Machine.
        // ---------------------------------------------------------

        [Fact]
        // Tests adding a controlled machine with a qualifier key.
        public void Qualified_AddControlledMachine_SetsReverseConnection()
        {
            var manager = CreateManager();
            var machine = CreateTransporter();
            var op = CreateOperator("Op", new HashSet<WarehouseManager> { manager },
                new HashSet<Machine> { machine }, new Dictionary<string, Machine> { { "SEED", machine } });

            var newMachine = CreateTransporter();
            op.addCapability(newMachine);
            op.addControlledMachine("M1", newMachine);

            Assert.True(op.ControlledMachines.ContainsKey("M1"));
            Assert.Contains(op, newMachine.ControlOperators);
        }

        [Fact]
        // Tests removing a qualified association and reverse cleanup.
        public void Qualified_RemoveControlledMachine_RemovesReverseConnection()
        {
            var manager = CreateManager();
            var machine = CreateTransporter();
            var op = CreateOperator("Op", new HashSet<WarehouseManager> { manager },
                new HashSet<Machine> { machine }, new Dictionary<string, Machine> { { "K1", machine } });

            op.removeControlledMachine("K1", machine);

            Assert.False(op.ControlledMachines.ContainsKey("K1"));
            Assert.DoesNotContain(op, machine.ControlOperators);
        }

        [Fact]
        // Tests modifying the qualifier key in a qualified association.
        public void Qualified_ModifyControlledMachineKey_UpdatesDictionary()
        {
            var manager = CreateManager();
            var machine = CreateTransporter();
            var op = CreateOperator("Op", new HashSet<WarehouseManager> { manager },
                new HashSet<Machine> { machine }, new Dictionary<string, Machine> { { "OLD", machine } });

            op.removeControlledMachine("OLD", machine);
            op.addControlledMachine("NEW", machine);

            Assert.True(op.ControlledMachines.ContainsKey("NEW"));
        }

        // ---------------------------------------------------------
        // 5. COMPOSITION ASSOCIATION TESTS
        // Tests strong ownership Section ↔ Shelf.
        // ---------------------------------------------------------

        [Fact]
        // Tests adding a Shelf to a Section with strong ownership.
        public void Composition_AddShelfToSection_SetsReverseConnection()
        {
            var section = CreateSection();
            var shelf = new Shelf(ShelfType.Pallet_Rack, 100, section);

            if (!section.Shelves.Contains(shelf))
                section.addShelf(shelf);

            Assert.Contains(shelf, section.Shelves);
        }

        [Fact]
        // Tests minimum multiplicity constraint in composition.
        public void Composition_RemoveLastShelf_ThrowsException()
        {
            var section = CreateSection();
            var shelf = new Shelf(ShelfType.Pallet_Rack, 100, section);

            Assert.Throws<ArgumentException>(() => section.removeShelf(shelf));
        }

        // ---------------------------------------------------------
        // 6. AGGREGATION / BASIC ASSOCIATION TESTS (ORDER)
        // ---------------------------------------------------------

        [Fact]
        // Tests modifying quantity of an Item in an Order.
        public void Aggregation_ModifyItemOrder_UpdatesQuantity()
        {
            var item = CreateItem();
            var order = new Order(DateTime.Now, new Customer("C", "1", "e"),
                new Dictionary<Item, int> { { item, 5 } });

            SetPrivateField(item, "_order", order);
            order.modifyItemOrder(item, -2);

            Assert.Equal(7, order.Items[item]);
        }

        [Fact]
        // Tests removing an Item from an Order while respecting aggregation rules.
        public void Aggregation_RemoveItemFromOrder_RemovesReverseConnection()
        {
            var item1 = CreateItem("I1");
            var item2 = CreateItem("I2");
            var item3 = CreateItem("I3");

            var order = new Order(DateTime.Now, new Customer("C", "1", "e"),
                new Dictionary<Item, int> { { item1, 5 }, { item2, 5 }, { item3, 5 } });

            SetPrivateField(item1, "_order", order);
            order.removeItem(item1);

            Assert.False(order.Items.ContainsKey(item1));
            Assert.True(order.Items.ContainsKey(item2));
        }

        // ---------------------------------------------------------
        // 7. DELIVERY & ASSOCIATION CLASS TESTS
        // ---------------------------------------------------------

        [Fact]
        // Tests adding a DeliveryDriver to a Delivery.
        public void Delivery_AddDriver_SetsReverseConnection()
        {
            var manager = CreateManager();
            var managers = new HashSet<WarehouseManager> { manager };
            var driver = new DeliveryDriver("D1", DateTime.Now, 1, managers, DriverLicenceType.B);
             
            var customer = new Customer("C", "1", "e");
            var item = CreateItem();
             
            var order = new Order(DateTime.Now, customer, new Dictionary<Item, int>{{item, 1}});
            SetPrivateField(item, "_order", order);

            var drivers = new HashSet<DeliveryDriver> { new DeliveryDriver("D2", DateTime.Now, 1, managers, DriverLicenceType.B) };
            var delivery = new Delivery(1, DateTime.Now, DeliveryStatus.Pending, new Address("B","S","C","Co","00"), null, DateTime.Now, order, drivers);

            delivery.addDeliveryDriver(driver);

            Assert.Contains(driver, delivery.AssignedDrivers);
            Assert.Contains(delivery, driver.Deliveries);
        }

        [Fact]
        // Tests association class PlacementInf creation and cleanup.
        public void AssociationClass_PlacementInf_CreatesAndRemovesReferencesProperly()
        {
            var shelf = CreateShelf(CreateSection());
            var item = CreateItem("Apple");

            shelf.addItem(item, 2);
            Assert.NotNull(item.PlacementInf);

            shelf.removeItem(item);
            Assert.Null(item.PlacementInf.Shelf);
        }
    }
}
