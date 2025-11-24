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
            Shelf.Load("nonexistingpath.xml");

            int curcount = Shelf.Extent.Count;

            Shelf testy = new Shelf(ShelfType.Pallet_Rack, 25.1);

            Assert.Equal(ShelfType.Pallet_Rack, testy.Type);
            Assert.Equal(25.1, testy.MaximumWeight);
            Assert.Contains(testy, Shelf.Extent);
            Assert.Single(Shelf.Extent);
        }

        [Fact]
        public void Shelf_AddConsisting_NullThrowException()
        {
            Shelf.Load("Snoremimimimi.xml");

            Shelf c2 = new Shelf(ShelfType.Pallet_Rack, 25.1);

            Assert.Throws<ArgumentNullException>(() =>
            {
                c2.addConsisting(null);
            });
        }

        [Fact]
        public void Shelf_RemoveConsisting_NullThrowException()
        {
            Shelf.Load("Snoremimimimi.xml");

            Shelf c2 = new Shelf(ShelfType.Pallet_Rack, 25.1);

            Assert.Throws<ArgumentNullException>(() =>
            {
                c2.removeConsisting(null);
            });
        }

        [Fact]
        public void Shelf_RemoveNonConsistingFromConsisting_DoesNothing()
        {
            Shelf.Load("Snoremimimimi.xml");

            Shelf c2 = new Shelf(ShelfType.Pallet_Rack, 25.1);
            Shelf c3 = new Shelf(ShelfType.Solid_Rack, 15.1);

            int curcount = Shelf.Extent.Count;

            c3.removeConsisting(c2);
            Assert.Equal(curcount, Shelf.Extent.Count);
        }

        [Fact]
        public void Shelf_NonPositiveWeight_Throws()
        {
            Shelf.Load("Ae.xml");

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
