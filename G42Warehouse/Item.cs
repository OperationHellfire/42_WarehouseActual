using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace G42Warehouse
{
    public enum ItemCategory
    {
        Food = 0,
        Chemical = 1,
        Electronics = 2,
        Medical = 3,
        RawMaterial = 4,
        Volatile = 5,
    }

    public enum ItemHazardType
    {
        Toxins = 0,
        Irritants = 1,
        Sensitizers = 2,
        Asphyxiants = 3
    }

    public enum BiologicalHazardType
    {
        Bacteria = 0,
        Virus = 1,
        Parasite = 2,
        BiologicalFluid = 3,
        Waste = 4
    }

    public interface ChemicallyHazardousItem
    {
        public ItemHazardType? ChemItemHazardType { get; set; }
        public int? FlammLevel { get; set; }
    }

    public interface BiologicallyHazardousItem
    {
        public BiologicalHazardType? BioHazardType { get; set; }
    }

    [DataContract]
    [KnownType(typeof(PerishableItem))]
    public class Item : ChemicallyHazardousItem, BiologicallyHazardousItem
    {
        [DataMember]
        private static int IDTracker = 0;

        [DataMember]
        private int _id;

        public int ItemID { get => _id; private set => value = _id; }
        [DataMember]
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("String cannot be empty.");
                }
                _name = value;
            }
        }
        [DataMember]
        private bool _isfragile;
        public bool Fragile
        {
            get => _isfragile;
            private set => _isfragile = value;
        }

        [DataMember]
        private ItemCategory _category;
        public ItemCategory Category
        {
            get => _category;
            private set => _category = value;
        }

        [DataMember]
        private int _stockQuantity;

        public int StockQuantity
        {
            get => _stockQuantity;
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Stock can't be negative.");
                }
                _stockQuantity = value;
            }
        }

        [DataMember]
        private double _weight;
        public double Weight
        {
            get => _weight;
            private set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Weight can't be less or equal than 0!");
                }
                _weight = value;
            }
        }
        [DataMember]
        private double _buyingprice;
        public double BuyingPrice
        {
            get => _buyingprice;
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Price cannot be less than 0!");
                }
                _buyingprice = value;
            }
        }

        [DataMember]
        private double _sellingprice;
        public double SellingPrice
        {
            get => _sellingprice;
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Price cannot be less than 0!");
                }
                _sellingprice = value;
            }
        }

        [DataMember]
        private Placement _placement;

        public Placement PlacementInf
        {
            get => _placement;
            private set => _placement = value;
        }

        [DataMember]

        private Order? _order;

        public Order? ItemOrder
        {
            get => _order;
            private set => _order = value;
        }

        [DataMember]

        private Machine? _machine;

        public Machine? CarryingMachine
        {
            get => _machine;
            private set => _machine = value;
        }

        /*[DataMember(IsRequired = false)]
        private ItemHazardType? _hazardtype;

        public ItemHazardType? HazardType
        {
            get => _hazardtype;
            set => _hazardtype = value;
        }

        [DataMember(IsRequired = false)]
        private int? _flammabilityLevel;
        public int? FlammabilityLevel
        {
            get => _flammabilityLevel;
            set
            {
                if (value < 0 || value > 4)
                {
                    throw new ArgumentOutOfRangeException("Flammability level out of range");
                }
                _flammabilityLevel = value;
            }
        }*/

        [DataMember]
        private static Dictionary<int, int> _stockTracker = [];

        public static Dictionary<int, int> StockTracker { get => _stockTracker; }

        //interfaces

        [DataMember(IsRequired = false)]
        public ItemHazardType? ChemItemHazardType { get; set; }

        [DataMember(IsRequired = false)]
        private int? _flammLevel { get; set; }

        public int? FlammLevel
        {
            get => _flammLevel;
            set
            {
                if (value < 0 || value > 4)
                {
                    throw new ArgumentOutOfRangeException("Flammability level out of range");
                }
                _flammLevel = value;
            }
        }

        [DataMember(IsRequired = false)]
        public BiologicalHazardType? BioHazardType { get; set; }

        /*[DataMember]
        private static Dictionary<int, int> _shelfTracker = [];

        public static Dictionary<int, int> ShelfTracker { get => _shelfTracker; }*/


        public Item(int? Assign_Id,
            string name,
            bool fragile,
            ItemCategory category,
            ItemHazardType? hazard,
            BiologicalHazardType? biohazard,
            int? flamm,
            int initialstock = 0,
            double weight = 1.0,
            double buyingprice = 0,
            double sellingprice = 0)
        {
            ItemID = Assign_Id == null ? IDTracker++ : Assign_Id.Value;
            Name = name;
            Fragile = fragile;
            Category = category;
            ChemItemHazardType = hazard;
            BioHazardType = biohazard;
            FlammLevel = flamm;
            StockQuantity = initialstock;
            Weight = weight;
            BuyingPrice = buyingprice;
            SellingPrice = sellingprice;
            PlacementInf = new Placement();
            ItemOrder = null;
            CarryingMachine = null;
            addToStockTrack(ItemID);
            addextent(this);
        }

        private static void addToStockTrack(int id, int quantity = 1)
        {
            StockTracker[id] = StockTracker.GetValueOrDefault(id,0)+quantity;
        }

        private static void addextent(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Item is null.");
            }
            ExtentManager.Instance.ExtentItemList.Add(item);
        }
        public override string ToString()
        {
            return $"[\n Name: {Name}\n Fragile: {Fragile}\n Category: {Category}\n Hazard Type:{ChemItemHazardType}\n Biological Hazard:{BioHazardType} \n Flamm Level: {FlammLevel}\n Weight: {Weight}\n Buying Price: {BuyingPrice}\n Selling Price: {SellingPrice}\n]";

        }

        //SHELF RELATION

        public void selectShelf(Shelf shelf, int placementLevel)
        {
            if (shelf == null)
            {
                throw new ArgumentNullException("Target shelf is null");
            }

            if (PlacementInf.Shelf == shelf) return;


            if (PlacementInf.Shelf != null)
            {
                throw new ArgumentException("This item is already assigned to a shelf.");
            }

            PlacementInf.setShelf(shelf);
            PlacementInf.ShelfLevel = placementLevel;
            shelf.addItem(this, placementLevel);
        }

        public void removeFromShelf() //remove from the shelf, not from the item. In the case you want to use this individually, you'll have to deal with the exception!
        {
            if(PlacementInf.Shelf == null)
            {
                throw new ArgumentNullException("This item doesn't have a shelf assigned to it.");
            }
            Shelf shlf = PlacementInf.Shelf;
            PlacementInf = new Placement();
            shlf.removeItem(this);
        }

        //ORDER RELATION

        public void selectOrder(Order order) //This should never be called individually, only Order addItem should be called if an order needs an item added.
        {
            if (order == null)
            {
                throw new ArgumentNullException("Target order is null!");
            }

            if(ItemOrder != null)
            {
                throw new ArgumentException("This item is already assigned to a different order.");
            }
            ItemOrder = order;
            order.addItem(this);
        }

        public void removeOrder() //same with this
        {
            if (ItemOrder == null) throw new ArgumentNullException("There is no assigned order.");
            ItemOrder.removeItem(this);
            ItemOrder = null;
        }

        public void setMachine(Machine mach) //same with this
        {
            if(mach == null)
            {
                throw new ArgumentNullException("Target machine is null.");
            }

            if(CarryingMachine != null)
            {
                throw new ArgumentException("This item is already assigned to a different order.");
            }

            CarryingMachine = mach;
            mach.addItem(this);
        }

        public void removeMachine()
        {
            CarryingMachine = null;
        }


        //PLACEMENT INFO
        [DataContract]
        public class Placement
        {
            private Shelf? _shelf;
            public Shelf? Shelf
            {
                get => _shelf;
            }

            private int? _shelfLevel;
            public int? ShelfLevel
            {
                get => _shelfLevel;
                set
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException("Value can't be smaller than 0");
                    }
                    _shelfLevel = value;
                }
            }

            public Placement()
            {
                ShelfLevel = null;
                _shelf = null;
            }

            public Placement(Shelf? shelf,int placement)
            {
                _shelf = shelf;
                ShelfLevel = placement;
            }

            internal void setShelf(Shelf shelf)
            {
                if (shelf == null)
                {
                    throw new ArgumentNullException("Shelf is nulL!");
                }
                _shelf = shelf;
            }
        }
    }


    //Extent helpers

    [DataContract]
    public class PerishableItem : Item
    {
        [DataMember]
        private DateTime _expirationDate;
        public DateTime ExpirationDate
        {
            get => _expirationDate;
            set
            {
                if (value < DateTime.Now)
                {
                    throw new ArgumentOutOfRangeException("Expiration date can't be before " + DateTime.Now.ToString());
                }
                _expirationDate = value;
            }
        }

        [DataMember]
        private double _storageTemperature;

        public double StorageTemperature
        {
            get => _storageTemperature;
            set => _storageTemperature = value;
        }

        public PerishableItem(
            int? Assign_Id, 
            string name,
            bool fragile,
            ItemCategory category,
            ItemHazardType? hazard,
            BiologicalHazardType? biohazard,
            int? flamm,
            DateTime expiration,
            double storageTemp,
            int initialstock = 0,
            double weight = 1.0,
            double buyingprice = 0,
            double sellingprice = 0) 
            : base(Assign_Id, name, fragile, category, hazard, biohazard, flamm, initialstock, weight, buyingprice, sellingprice)
        {
            ExpirationDate = expiration;
            StorageTemperature = storageTemp;
        }

        public override string ToString()
        {
            return base.ToString().TrimEnd() + $" Expiration: {ExpirationDate}\n Storage Temperature: {StorageTemperature}\n]";
        }
    }

}
