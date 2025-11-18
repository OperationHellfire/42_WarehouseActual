using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace G42Warehouse
{
    public enum ShelfType
    {
        Pallet_Rack = 0,
        Solid_Rack = 1,
    }
    [DataContract]
    public class Shelf
    {

        // EXTENT
        [DataMember]
        private static List<Shelf> _extent = new List<Shelf>();

        public static IReadOnlyList<Shelf> Extent => _extent.AsReadOnly();

        //EXTENT_END

        [DataMember]
        private List<Shelf> _consistingshelves = new List<Shelf>();

        public IReadOnlyList<Shelf> ConsistingShelves
        {
           get => _consistingshelves.AsReadOnly();
        }

        private ShelfType _type;
        [DataMember]
        public ShelfType Type
        {
            get => _type;
            private set => _type = value;
        }

        private double _maxweight;

        [DataMember]
        public double MaximumWeight
        {
            get => _maxweight;
            private set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("Maximum weight must be a positive value.");
                }
                _maxweight = value;
            }
        }

        [DataMember]
        private Dictionary<Item, int> _inventory = new();

        public Dictionary<Item,int> Inventory { get => _inventory; }

        public Shelf(ShelfType type, double maximumweight)
        {
            Type = type;
            MaximumWeight = maximumweight;
            addShelf(this);
        }


        public void addConsisting(Shelf shelf)
        {
            if(shelf == null)
            {
                throw new ArgumentNullException("Target shelf is null");
            }
            _consistingshelves.Add(shelf);
        }

        public void removeConsisting(Shelf shelf)
        {
            if (shelf == null)
            {
                throw new ArgumentNullException("Target shelf is null");
            }
            _consistingshelves.Remove(shelf);
        }
        private static void addShelf(Shelf shelf) 
        {
            if(shelf == null)
            {
                throw new ArgumentNullException("Shelf is null, cannot add...");
            }
            _extent.Add(shelf);
        }

        public void addItem(Item item, int amount = 1)
        {
            if (item == null)
            {
                throw new ArgumentException("Item is null.");
            }

            if(amount < 0)
            {
                throw new ArgumentException("Amount added can't be negative.");
            }

            if (item.ShelfTracker == item.Stock)
            {
                throw new ArgumentException("All of the items are already located in shelves. Please increase stock of item or relocate existing items.");
            }

            if (amount < 1 || amount > item.Stock)
            {
                throw new ArgumentException("There's not enough stock of this item to add in that amount: " + " Item stock: " + item.Stock);
            }

            if (item.ShelfTracker + amount > item.Stock)
            {
                throw new ArgumentException("Amount of items currently present in all shelves exceed current stock, please increase stock of item or relocate existing items.");
            }

            item.ShelfTracker += amount;

            Inventory[item] = Inventory.GetValueOrDefault(item, 0) + amount;
        }

        public void removeItem(Item item, int amount = 1)
        {
            if (item == null)
            {
                throw new ArgumentException("Item is null.");
            }

            if (amount < 0)
            {
                throw new ArgumentException("Amount removed can't be negative.");
            }

            if(amount < item.Stock)
            {
                throw new ArgumentException("There's not enough stock of this item to remove in that amount: " + " Item stock: " + item.Stock);
            }

            if (item.ShelfTracker == 0)
            {
                throw new ArgumentException("This item is currently not present on any shelves, cannot remove.");
            }

            if (item.ShelfTracker - amount < 0)
            {
                throw new ArgumentException("The amount given to remove exceeds the amount present on all shelves, cannot remove.");
            }

            if (!Inventory.ContainsKey(item) || Inventory[item] <= 0)
            {
                throw new ArgumentException("You can't remove an item that is not in this shelf.");
            }

            Inventory[item] -= amount;
        }




        public static void Save(string path = "shelf_extent.xml")
        {
            var serializer = new DataContractSerializer(typeof(List<Shelf>));
            using var stream = File.Create(path);
            serializer.WriteObject(stream, _extent);
        }

        public static bool Load(string path = "shelf_extent.xml")
        {
            if (!File.Exists(path))
            {
                _extent.Clear();
                return false;
            }

            var serializer = new DataContractSerializer(typeof(List<Shelf>));
            using var stream = File.OpenRead(path);

            try
            {
                _extent = (List<Shelf>?)serializer.ReadObject(stream) ?? [];
                return true;
            }
            catch
            {
                _extent.Clear();
                return false;
            }
        }

        public override string ToString()
        {
            string a = string.Empty;    
            foreach(var shelf in ConsistingShelves)
            {
                 a += shelf.Type + " ";
            }

            string b = string.Empty;
            foreach(var item in Inventory)
            {
                b += item.Key.Name + " amount: " + item.Value + " | ";
            }
            return $"Type: {Type} Maximum Weight: {MaximumWeight} Consisting Shelves: {a} Current Inventory: {b}";
        }






    }
}
