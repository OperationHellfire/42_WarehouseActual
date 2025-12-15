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
            ExtentManager.Remove();
            Worker john = new Worker("John",DateTime.Now,1000.0,new HashSet<WarehouseManager> { new WarehouseManager("asdasd",DateTime.Now,25)});
            Shelf shelf = new Shelf(ShelfType.Pallet_Rack, 50.5, new RefrigeratedSection(
                    "Sec1",
                    "asd",
                    10,
                    hasBackupGenerator: false,
                    minOperationalTemperature: -10,
                    maxOperationalTemperature: 5)
            );
            Item item = new Item(null, "Pen", false, ItemCategory.RawMaterial, ItemHazardType.Toxins, null, 3, 0, 25, 25, 25);
            
            PerishableItem itemPerish = new PerishableItem(null, "Frozen Pen", false, ItemCategory.RawMaterial, null, null, 3, DateTime.Now.Add(new TimeSpan(36, 0, 0, 0)), -25, 0, 25, 25, 25);
            ExtentManager.Save();
        }
    }
}
