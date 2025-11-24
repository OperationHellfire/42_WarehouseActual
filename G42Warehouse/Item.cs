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
        Waste= 4
    }
    [DataContract]
    [KnownType(typeof(PerishableItem))]
    public class Item
    {

        //EXTENT
        [DataMember]
        private static List<Item> _extent = new List<Item>();
        private static IReadOnlyList<Item> Extent
        {
            get => _extent.AsReadOnly();
        }
        //EXTENT_END
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
        private int _stock;

        public int Stock
        {
            get => _stock;
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Stock can't be negative.");
                }
                _stock = value;
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
        /*[DataMember]
        private int _shelftracker;
        public int ShelfTracker
        {
            get => _shelftracker;
            set => _shelftracker = value;
        }*/

        [DataMember(IsRequired = false)]
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
        }

        [DataMember(IsRequired = false)]
        private BiologicalHazardType? _biohazard;

        public BiologicalHazardType? BiologicalHazard
        {
            get => _biohazard;
            set => _biohazard = value;
        }



        public Item(string name,
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
            Name = name;
            Fragile = fragile;
            Category = category;
            HazardType = hazard;
            BiologicalHazard = biohazard;
            FlammabilityLevel = flamm;
            Stock = initialstock;
            Weight = weight;
            BuyingPrice = buyingprice;
            SellingPrice = sellingprice;
            //ShelfTracker = 0;
            addextent(this);
        }

        private static void addextent(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Item is null.");
            }
            _extent.Add(item);
        }

        //Extent helpers

        public static void Save(string path = "item_extent.xml")
        {
            var serializer = new DataContractSerializer(typeof(List<Item>));
            using var stream = File.Create(path);
            serializer.WriteObject(stream, _extent);
        }

        public static bool Load(string path = "item_extent.xml")
        {
            if (!File.Exists(path))
            {
                _extent.Clear();
                return false;
            }

            var serializer = new DataContractSerializer(typeof(List<Item>));
            using var stream = File.OpenRead(path);

            try
            {
                _extent = (List<Item>?)serializer.ReadObject(stream) ?? [];
                return true;
            }
            catch
            {
                _extent.Clear();
                return false;
            }
        }
    }


    //Extent helpers

    [DataContract]
    public class PerishableItem : Item
    {
        [DataMember]
        private DateTime _expiration;
        public DateTime Expiration
        {
            get => _expiration;
            set
            {
                if (value < DateTime.Now)
                {
                    throw new ArgumentOutOfRangeException("Expiration date can't be before " + DateTime.Now.ToString());
                }
                _expiration = value;
            }
        }

        [DataMember]
        private double _storageTemperature;

        public double StorageTemperature
        {
            get => _storageTemperature;
            set => _storageTemperature = value;
        }

        public PerishableItem(string name,
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
            double sellingprice = 0) : base(name, fragile, category, hazard, biohazard, flamm, initialstock, weight, buyingprice)
        {
            Expiration = expiration;
            StorageTemperature = storageTemp;
        }
    }

 }
