using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace G42Warehouse
{
    [DataContract]
    public class Customer
    {
        [DataMember]
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("String can't be empty.");
                }
                _name = value;
            }
        }

        [DataMember]
        private string _telnumber;

        public string TelephoneNumber
        {
            get => _telnumber;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("String can't be empty.");
                }
                _telnumber = value;
            }
        }
        [DataMember]

        private string _email;

        public string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("String can't be empty.");
                }
                _email = value;
            }
        }

        [DataMember]
        private HashSet<Order> _orders = [];

        public HashSet<Order> Orders
        {
            get => _orders;
            private set => _orders = value;
        }


        public Customer(string name, string telnumber, string email)
        {
            Name = name;
            TelephoneNumber = telnumber;
            Email = email;
            addExtent(this);
        }

        private static void addExtent(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException("Customer is null.");
            }
            ExtentManager.Instance.ExtentCustomer.Add(customer);
        }

        public void addOrder(Order order)
        {
            if(order == null)
            {
                throw new ArgumentNullException("Target order is null.");
            }

            if (!Orders.Contains(order))
            {
                Orders.Add(order);
                //The caller is going to be the Customer anyways, an order can't be created without a customer. Reverse connection is set like this
            }
        }
    }
}
