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
           /* Worker john = new Worker("John",DateTime.Now,1000.0);
            Shelf shelf = new Shelf(ShelfType.Pallet_Rack, 50.5);
            Item item = new Item("Pen", false, ItemCategory.RawMaterial, null, null, null,0,25,25,25);
            PerishableItem itemPerish = new PerishableItem("Frozen Pen", false, ItemCategory.RawMaterial, null, null, null, DateTime.Now.Add(new TimeSpan(36,0,0,0)),-25,0, 25, 25, 25);


            Console.WriteLine(john.ToString());
            Console.WriteLine(shelf.ToString());
            Console.WriteLine(item.ToString());
            ExtentManager.Save();*/

            ExtentManager.Load();

            Console.WriteLine(ExtentManager.Instance.ExtentShelf[0].ToString());
            Console.WriteLine(ExtentManager.Instance.ExtentItemList[0].ToString());
            Console.WriteLine(ExtentManager.Instance.ExtentEmployee[0].ToString());

            Console.WriteLine(ExtentManager.Instance.ExtentShelf.Count);
        }
    }
}
