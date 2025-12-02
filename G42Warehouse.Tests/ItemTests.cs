using System;
using G42Warehouse;
using Xunit;

namespace G42Warehouse.Tests
{
    public class ItemTests
    {
        public ItemTests()
        {
            ExtentManager.Reset();
        }

        [Fact]
        public void Item_Constructor_ValidData_AddsToExtentAndSetsProperties()
        {
            var items = ExtentManager.Instance.ExtentItemList;
            int beforeCount = items.Count;

            var item = new Item(
                name: "Pencil",
                fragile: false,
                category: ItemCategory.RawMaterial,
                hazard: null,
                biohazard: null,
                flamm: 2,
                initialstock: 10,
                weight: 0.5,
                buyingprice: 1.0,
                sellingprice: 2.0);

            Assert.Equal("Pencil", item.Name);
            Assert.False(item.Fragile);
            Assert.Equal(ItemCategory.RawMaterial, item.Category);
            Assert.Null(item.HazardType);

            Assert.Equal(10, item.StockQuantity);
            Assert.Equal(0.5, item.Weight);
            Assert.Equal(1.0, item.BuyingPrice);
            Assert.Equal(2.0, item.SellingPrice);
            Assert.Equal(2, item.FlammabilityLevel);

            Assert.Contains(item, items);
            Assert.Equal(beforeCount + 1, items.Count);
        }

        [Fact]
        public void Item_EmptyName_ThrowsArgumentException()
        {
            ExtentManager.Reset();

            Assert.Throws<ArgumentException>(() =>
                new Item(
                    name: "",
                    fragile: false,
                    category: ItemCategory.RawMaterial,
                    hazard: null,
                    biohazard: null,
                    flamm: null,
                    initialstock: 0,
                    weight: 1.0,
                    buyingprice: 0,
                    sellingprice: 0));
        }

        [Fact]
        public void Item_NegativeStock_ThrowsArgumentException()
        {
            ExtentManager.Reset();

            Assert.Throws<ArgumentException>(() =>
                new Item(
                    name: "BadStock",
                    fragile: false,
                    category: ItemCategory.RawMaterial,
                    hazard: null,
                    biohazard: null,
                    flamm: null,
                    initialstock: -1,
                    weight: 1.0,
                    buyingprice: 0,
                    sellingprice: 0));
        }

        [Fact]
        public void Item_NonPositiveWeight_ThrowsArgumentException()
        {
            ExtentManager.Reset();

            Assert.Throws<ArgumentException>(() =>
                new Item(
                    name: "ZeroWeight",
                    fragile: false,
                    category: ItemCategory.RawMaterial,
                    hazard: null,
                    biohazard: null,
                    flamm: null,
                    initialstock: 0,
                    weight: 0,
                    buyingprice: 0,
                    sellingprice: 0));
        }

        [Fact]
        public void Item_NegativeBuyingPrice_ThrowsArgumentException()
        {
            ExtentManager.Reset();

            Assert.Throws<ArgumentException>(() =>
                new Item(
                    name: "BadPriceBuy",
                    fragile: false,
                    category: ItemCategory.RawMaterial,
                    hazard: null,
                    biohazard: null,
                    flamm: null,
                    initialstock: 0,
                    weight: 1.0,
                    buyingprice: -1,
                    sellingprice: 0));
        }

        [Fact]
        public void Item_NegativeSellingPrice_ThrowsArgumentException()
        {
            ExtentManager.Reset();

            Assert.Throws<ArgumentException>(() =>
                new Item(
                    name: "BadPriceSell",
                    fragile: false,
                    category: ItemCategory.RawMaterial,
                    hazard: null,
                    biohazard: null,
                    flamm: null,
                    initialstock: 0,
                    weight: 1.0,
                    buyingprice: 0,
                    sellingprice: -1));
        }

        [Fact]
        public void Item_FlammabilityLevel_OutOfRange_ThrowsArgumentOutOfRangeException()
        {
            ExtentManager.Reset();

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new Item(
                    name: "Paint",
                    fragile: false,
                    category: ItemCategory.Chemical,
                    hazard: ItemHazardType.Toxins,
                    biohazard: null,
                    flamm: 5,
                    initialstock: 0,
                    weight: 1.0,
                    buyingprice: -1.0,
                    sellingprice: 2.0));
        }
    }
}
