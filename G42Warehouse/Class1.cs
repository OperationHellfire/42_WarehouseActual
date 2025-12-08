using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G42Warehouse
{
    internal class Class1
    {
        public static void Main(string[] args)
        {
            Worker john = new Worker("John",DateTime.Now,1000.0,new HashSet<WarehouseManager> { new WarehouseManager("asdasd",DateTime.Now,25)});
            Shelf shelf = new Shelf(ShelfType.Pallet_Rack, 50.5, new RefrigeratedSection(
                    "Sec1",
                    "asd",
                    10,
                    hasBackupGenerator: false,
                    minOperationalTemperature: -10,
                    maxOperationalTemperature: 5)
            );
            Item item = new Item("Pen", false, ItemCategory.RawMaterial, null, null, null,0,25,25,25);
            PerishableItem itemPerish = new PerishableItem("Frozen Pen", false, ItemCategory.RawMaterial, null, null, null, DateTime.Now.Add(new TimeSpan(36,0,0,0)),-25,0, 25, 25, 25);

            item.selectShelf(shelf,0);
            shelf.addItem(itemPerish,0);
            Console.WriteLine("Shelf: " + shelf.Items.ToString());
            Console.WriteLine("Item #1: " + item.PlacementInf.Shelf.ToString());
            Console.WriteLine("Item #2: " + itemPerish.PlacementInf.Shelf.ToString());
            shelf.removeItem(itemPerish);   
            shelf.removeItem(item);
        }
    }
}
