using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace G42Warehouse
{
    [DataContract]
    public class ExtentManager
    {
        private static ExtentManager _singleton = Init();

        public static ExtentManager Instance { get { return _singleton; } }

        [DataMember]
        private List<Shelf> _extentshelf = [];
        public List<Shelf> ExtentShelf
        {
            get => _extentshelf;
        }

        [DataMember]
        private List<Item> _extentitem = [];
        public List<Item> ExtentItemList
        {
            get => _extentitem;
        }

        [DataMember]
        private List<Employee> _extentemployee = [];
        public List<Employee> ExtentEmployee
        {
            get => _extentemployee;
        }

        [DataMember]
        private List<Section> _extentsection = [];
        public List<Section> ExtentSection
        {
            get => _extentsection;
        }

        [DataMember]
        private List<Machine> _extentmachine = [];
        public List<Machine> ExtentMachine
        {
            get => _extentmachine;
        }

        [DataMember]
        private List<Order> _extentorder = [];
        public List<Order> ExtentOrder
        {
            get => _extentorder;
        }

        [DataMember]
        private List<Delivery> _extentDelivery = [];
        public List<Delivery> ExtentDelivery
        {
            get => _extentDelivery;
        }

        [DataMember]
        private List<Customer> _extentcustomer = [];
        public List<Customer> ExtentCustomer
        {
            get => _extentcustomer;
        }

        [DataMember]
        private List<Shift> _extentShift = [];
        public List<Shift> ExtentShift
        {
            get => _extentShift;
        }


        public static void Save(string path = "Extent.xml")
        {
            var serializer = new DataContractSerializer(typeof(ExtentManager));
            using var stream = File.Create(path);
            serializer.WriteObject(stream, _singleton);
        }

        public static bool Load(string path = "Extent.xml")
        {
            if (!File.Exists(path))
            {
                _singleton = new ExtentManager();
                return false;
            }

            var serializer = new DataContractSerializer(typeof(ExtentManager));
            using var stream = File.OpenRead(path);

            try
            {
                var mainobj = (ExtentManager)serializer.ReadObject(stream);
                _singleton = mainobj;
                return true;
            }
            catch
            {
                _singleton = new ExtentManager();
                return false;
            }
        }

        private static ExtentManager Init(string path = "Extent.xml")
        {
            if (!File.Exists(path))
            {
                return new ExtentManager();
            }

            var serializer = new DataContractSerializer(typeof(ExtentManager));
            using var stream = File.OpenRead(path);

            try
            {
                var mainobj = (ExtentManager)serializer.ReadObject(stream);
                return mainobj;
            }
            catch
            {
                return new ExtentManager();
            }
        }

        public static bool Remove(string path = "ExtentTest.xml")
        {
            if (!path.Contains(".xml")) { return false;  }; 
            if (!File.Exists(path)) { return false; }

            _singleton = new ExtentManager();

            File.Delete(path);
            return true;

        } //This is only meant for test cases, should not be used in any other way!

        public static void Reset()
        {
            _singleton = new ExtentManager();
        }


    }
}
