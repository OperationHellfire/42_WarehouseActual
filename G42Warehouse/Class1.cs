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
            /*Worker john = new Worker("John",DateTime.Now,1000.0,new HashSet<WarehouseManager> { new WarehouseManager("asdasd",DateTime.Now,25)});
            Section sec = new RefrigeratedSection(
                    "Sec1",
                    "asd",
                    10,
                    hasBackupGenerator: false,
                    minOperationalTemperature: -10,
                    maxOperationalTemperature: 5);
            Shelf shelf = new Shelf(ShelfType.Pallet_Rack, 50.5,sec);
            Item item = new Item(null, "Pen", false, ItemCategory.RawMaterial, null, null, null, 0, 25, 25, 25);
            
            PerishableItem itemPerish = new PerishableItem(null, "Frozen Pen", false, ItemCategory.RawMaterial, null, null, null, DateTime.Now.Add(new TimeSpan(36, 0, 0, 0)), -25, 0, 25, 25, 25);

            shelf.addItem(itemPerish,2);
            sec.addShelf(shelf);

            Console.WriteLine("Before: ");

            foreach(Shelf shf in sec.Shelves)
            {
                Console.WriteLine(shf.ToString());
            }

            ExtentManager.Save("Ex1.xml");*/

            /*sec.removeSection(sec);
            Console.WriteLine("After: ");
                Console.WriteLine(shelf.ToString());

                if (itemPerish.PlacementInf.Shelf == null) {
                    Console.WriteLine("Success");
                } else
                {
                    Console.WriteLine(itemPerish.PlacementInf.Shelf.ToString());    
                }

            ExtentManager.Save("Ex2.xml");*/

            ExtentManager.Load("Ex1.xml");

            foreach (Shelf shf in ExtentManager.Instance.ExtentShelf)
            {
                Console.WriteLine(shf.ToString());
            }

            foreach (Item shf in ExtentManager.Instance.ExtentItemList)
            {
                Console.WriteLine(shf.ToString());
            }




        }
    }
}
