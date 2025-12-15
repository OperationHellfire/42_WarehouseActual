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

        [DataMember]
        private List<Shelf> _consistingshelves = new List<Shelf>();

        [DataMember]
        private HashSet<Item> _items = [];

        public HashSet<Item> Items { get { return _items; } }

        public IReadOnlyList<Shelf> ConsistingShelves
        {
            get => _consistingshelves.AsReadOnly();
        }

        private ShelfType _typeOfShelf;
        [DataMember]
        public ShelfType TypeOfShelf
        {
            get => _typeOfShelf;
            private set => _typeOfShelf = value;
        }

        private double _maxWeightCapacity;

        [DataMember]
        public double MaximumWeightCapacity
        {
            get => _maxWeightCapacity;
            private set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("Maximum weight must be a positive value.");
                }
                _maxWeightCapacity = value;
            }
        }

        [DataMember]
        private Section _containingSection;

        public Section ContainingSection
        {
            get => _containingSection;
            private set => _containingSection = value;
        }


        public Shelf(ShelfType type, double maximumweight, Section containingSection)
        {
            TypeOfShelf = type;
            MaximumWeightCapacity = maximumweight;
            ContainingSection = containingSection;
            addShelf(this);
        }


        public void addConsisting(Shelf shelf)
        {
            if (shelf == null)
            {
                throw new ArgumentNullException("Target shelf is null");
            }
            
            if(!ConsistingShelves.Contains(shelf))
            {
                _consistingshelves.Add(shelf);
                if (!shelf.ConsistingShelves.Contains(this)) shelf.addConsisting(this);
            }
        }

        public void removeConsisting(Shelf shelf)
        {
            if (shelf == null)
            {
                throw new ArgumentNullException("Target shelf is null");
            }

            if(ConsistingShelves.Contains(shelf))
            {
                _consistingshelves.Remove(shelf);
                if(shelf.ConsistingShelves.Contains(this)) shelf.removeConsisting(this);    
            }
        }
        private static void addShelf(Shelf shelf)
        {
            if (shelf == null)
            {
                throw new ArgumentNullException("Shelf is null, cannot add...");
            }
            ExtentManager.Instance.ExtentShelf.Add(shelf);
        }

        public void addItem(Item item, int placementLevel)
        {
            if(item == null)
            {
                throw new ArgumentNullException("Target item is null");
            }
            
            
            if(item.PlacementInf.Shelf != null && item.PlacementInf.Shelf != this)
            {
                throw new ArgumentException("Target item is already assigned to a different shelf.");
            }
            int itemid = item.ItemID;

            if (Item.StockTracker[itemid] <= 0 )
            {
                throw new ArgumentException($"Every instance of {itemid} is already in shelves. Amount(Stock) = {Item.StockTracker[itemid]} Amount(Shelf) = {Item.ShelfTracker[itemid]}. Take out existing items or add more.");
            }

            if(!Items.Contains(item)) {
                Items.Add(item);
                //Item.ShelfTracker[itemid]++;
                item.selectShelf(this,placementLevel);
            }
        }

        public void removeItem(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Target item is null");
            }

            if (Items.Contains(item))
            {
                int itemid = item.ItemID;
                Items.Remove(item);
                //Item.ShelfTracker[itemid]--;
                item.removeFromShelf();
            }
        }

        public override string ToString()
        {
            string a = string.Empty;
            foreach (var shelf in ConsistingShelves)
            {
                a += shelf.TypeOfShelf + " ";
            }

            return $"Type: {TypeOfShelf} Maximum Weight: {MaximumWeightCapacity} Consisting Shelves: {a}"; //" Current Inventory: {b}";
        }






    }
}
