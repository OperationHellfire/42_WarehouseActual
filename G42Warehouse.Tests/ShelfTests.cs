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

            Shelf testy = new Shelf(ShelfType.Pallet_Rack, 25.1);

            Assert.Equal(ShelfType.Pallet_Rack, testy.TypeOfShelf);
            Assert.Equal(25.1, testy.MaximumWeightCapacity);
            Assert.Contains(testy, arr);
            Assert.Single(arr);
        }

        [Fact]
        public void Shelf_AddConsisting_NullThrowException()
        {
            ExtentManager.Load("nonexistingpath.xml");

            Shelf c2 = new Shelf(ShelfType.Pallet_Rack, 25.1);

            Assert.Throws<ArgumentNullException>(() =>
            {
                c2.addConsisting(null);
            });
        }

        [Fact]
        public void Shelf_RemoveConsisting_NullThrowException()
        {
            ExtentManager.Load("nonexistingpath.xml");

            Shelf c2 = new Shelf(ShelfType.Pallet_Rack, 25.1);

            Assert.Throws<ArgumentNullException>(() =>
            {
                c2.removeConsisting(null);
            });
        }

        [Fact]
        public void Shelf_RemoveNonConsistingFromConsisting_DoesNothing()
        {
            ExtentManager.Load("nonexistingpath.xml");

            Shelf c2 = new Shelf(ShelfType.Pallet_Rack, 25.1);
            Shelf c3 = new Shelf(ShelfType.Solid_Rack, 15.1);

            var arr = ExtentManager.Instance.ExtentShelf;
            int curcount = arr.Count;

            c3.removeConsisting(c2);
            Assert.Equal(curcount, arr.Count);
        }

        [Fact]
        public void Shelf_NonPositiveWeight_Throws()
        {
            ExtentManager.Load("nonexistingpath.xml");

            Assert.Throws<ArgumentOutOfRangeException>(() => { new Shelf(ShelfType.Pallet_Rack, -1); });
        }

        /*[Fact]
        public void Shelf_AddingItemBeyondStockRange_Throws()
        {
            Shelf.Load("ae.png");
            Item.Load("fsdk.pnnng");
            Item pencil = new Item("Pencil", false, ItemCategory.RawMaterial, 5, 1.5, 20.0, 20.0);
            Shelf c1 = new Shelf(ShelfType.Pallet_Rack, 25.4);
            Assert.Throws<ArgumentException>(() => { c1.addItem(pencil, 265); });
        }

        [Fact]
        public void Shelf_AddingItemOneByOne_Throws()
        {
            Shelf.Load("ae.png");
            Item.Load("fsdk.pnnng");
            Item pencil = new Item("Pencil", false, ItemCategory.RawMaterial, 5, 1.5, 20.0, 20.0);
            Shelf c1 = new Shelf(ShelfType.Pallet_Rack, 25.4);
            Assert.Throws<ArgumentException>(() =>
            {
                for (int i = 0; i < 7; i++)
                {
                    c1.addItem(pencil);
                }
            });
        }

        [Fact]
        public void Shelf_RemovingItemBeyondStockRange_Throws()
        {
            Shelf.Load("ae.png");
            Item.Load("fsdk.pnnng");
            Item pencil = new Item("Pencil", false, ItemCategory.RawMaterial, 5, 1.5, 20.0, 20.0);
            Shelf c1 = new Shelf(ShelfType.Pallet_Rack, 25.4);
            c1.addItem(pencil, 5);
            Assert.Throws<ArgumentException>(() => { c1.removeItem(pencil, 265); });
        }

        [Fact]
        public void Shelf_RemovingItemOneByOne_Throws()
        {
            Shelf.Load("ae.png");
            Item.Load("fsdk.pnnng");
            Item pencil = new Item("Pencil", false, ItemCategory.RawMaterial, 5, 1.5, 20.0, 20.0);
            Shelf c1 = new Shelf(ShelfType.Pallet_Rack, 25.4);
            c1.addItem(pencil, 5);
            Assert.Throws<ArgumentException>(() => {
                for (int i = 0; i < 7; i++)
                {
                    c1.removeItem(pencil);
                }
            });
        }*/
    }
}
