using System;
using System.IO;
using G42Warehouse;
using Xunit;

namespace G42Warehouse.Tests
{
    public class ShelfTests
    {
        [Fact]
        public void Shelf_Constructor_ValidData_ExtentProperty()
        {
            ExtentManager.Load("nonexistingpath.xml");

            var arr = ExtentManager.Instance.ExtentShelf;
            int curcount = arr.Count;

            Shelf testy = new Shelf(ShelfType.Pallet_Rack, 25.1, new RefrigeratedSection(
                    "Sec1",
                    "sad",
                    10,
                    hasBackupGenerator: false,
                    minOperationalTemperature: -10,
                    maxOperationalTemperature: 5)
            );

            Assert.Equal(ShelfType.Pallet_Rack, testy.TypeOfShelf);
            Assert.Equal(25.1, testy.MaximumWeightCapacity);
            Assert.Contains(testy, arr);
            Assert.Single(arr);
        }

        [Fact]
        public void Shelf_AddConsisting_NullThrowException()
        {
            ExtentManager.Load("nonexistingpath.xml");

            Shelf c2 = new Shelf(ShelfType.Pallet_Rack, 25.1, new RefrigeratedSection(
                    "Sec1",
                    "sad",
                    10,
                    hasBackupGenerator: false,
                    minOperationalTemperature: -10,
                    maxOperationalTemperature: 20)
            );

            Assert.Throws<ArgumentNullException>(() =>
            {
                c2.addConsisting(null);
            });
        }

        [Fact]
        public void Shelf_RemoveConsisting_NullThrowException()
        {
            ExtentManager.Load("nonexistingpath.xml");

            Shelf c2 = new Shelf(ShelfType.Pallet_Rack, 25.1, new RefrigeratedSection(
                    "Sec1",
                    "sad",
                    10,
                    hasBackupGenerator: false,
                    minOperationalTemperature: -10,
                    maxOperationalTemperature: 5)
            );

            Assert.Throws<ArgumentNullException>(() =>
            {
                c2.removeConsisting(null);
            });
        }

        [Fact]
        public void Shelf_RemoveNonConsistingFromConsisting_DoesNothing()
        {
            ExtentManager.Load("nonexistingpath.xml");

            Shelf c2 = new Shelf(ShelfType.Pallet_Rack, 25.1, new RefrigeratedSection(
                    "Sec1",
                    "sad",
                    10,
                    hasBackupGenerator: false,
                    minOperationalTemperature: -10,
                    maxOperationalTemperature: 20)
            );
            Shelf c3 = new Shelf(ShelfType.Solid_Rack, 15.1, new RefrigeratedSection(
                    "Sec1",
                    "sad",
                    10,
                    hasBackupGenerator: false,
                    minOperationalTemperature: -10,
                    maxOperationalTemperature: 5)
            );

            var arr = ExtentManager.Instance.ExtentShelf;
            int curcount = arr.Count;

            c3.removeConsisting(c2);
            Assert.Equal(curcount, arr.Count);
        }

        [Fact]
        public void Shelf_NonPositiveWeight_Throws()
        {
            ExtentManager.Load("nonexistingpath.xml");

            Assert.Throws<ArgumentOutOfRangeException>(() => { new Shelf(ShelfType.Pallet_Rack, -1, new RefrigeratedSection(
                    "Sec1",
                    "sad",
                    10,
                    hasBackupGenerator: false,
                    minOperationalTemperature: -10,
                    maxOperationalTemperature: 5)
            );
            });
        }
    }
}
