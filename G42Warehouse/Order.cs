using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace G42Warehouse
{

    public enum PaymentStatus
    {
        Non_Paid = 0,
        In_Progress=1,
        Cancelled = 2,
        Paid = 3
    }
    public enum PreparationStatus
    {
        In_Progress = 0,
        Ready = 1,
        Cancelled = 2
    }

    public enum PaymentMethod
    {
        Cash = 0,
        Checks = 1,
        CreditCard = 2,
        DebitCard = 3
    }
    [DataContract]
    public class Order
    {
        [DataMember]
        private DateTime _orderdate;
        public DateTime OrderDate
        {
            get => _orderdate;
            set
            {
                if(value > DateTime.Now)
                {
                    throw new ArgumentException("Order Date cant be in the future!");
                }
                _orderdate = value;
            }
        }

        [DataMember]
        private PaymentStatus _paymentStatus;

        public PaymentStatus PaymentStatus
        {
            get => _paymentStatus;
            set => _paymentStatus = value;
        }

        [DataMember]
        private PaymentMethod _paymentMethod;
        public PaymentMethod PaymentMethod
        {
            get => _paymentMethod;
            set => _paymentMethod = value;
        }

        [DataMember]
        private PreparationStatus _preparationStatus;
        public PreparationStatus PreparationStatus
        {
            get => _preparationStatus;
            set => _preparationStatus = value;
        }

        //HERE WILL BE TOTALAMOUNT IMPLEMENTATION DURING NEXT PHASE!

        [DataMember]
        private Dictionary<Item,int> _items;

        public Dictionary<Item,int> Items
        {
            get => _items;
            private set
            {
                if( value.Count == 0)
                {
                    throw new ArgumentException("There must be one item present in the dictionary!");
                }

                foreach ( var item in value )
                {
                    if(item.Value < 0) 
                    {
                        throw new ArgumentException("Quantity of item must be positive!");
                    }
                }
                _items = value;
            }
        }

        [DataMember]
        private Customer _customer;

        public Customer Customer
        {
            get => _customer;
            private set => _customer = value;
        }

        [DataMember]
        private HashSet<Delivery> _deliveries = [];

        public HashSet<Delivery> Deliveries
        {
            get => _deliveries;
            private set => _deliveries = value;
        }

        public Order(DateTime time, Customer customer, Dictionary<Item,int> nitems)
        {
            _orderdate = time;
            PaymentStatus = PaymentStatus.Non_Paid;
            PreparationStatus = PreparationStatus.In_Progress;
            PaymentMethod = PaymentMethod.Cash; //Placeholder
            Customer = customer;
            Items = nitems;
            addExtent(this);
        }


        private static void addExtent(Order order)
        {
            if(order == null)
            {
                throw new ArgumentNullException("Order is null!");
            }
            ExtentManager.Instance.ExtentOrder.Add(order);
        }

        public void addItem(Item item,int quantity = 1)
        {
            if (item == null) { throw new ArgumentNullException("Target item is null!"); }

            if (quantity < 1)
            {
                throw new ArgumentException("Quantity must be positive.");
            }

            if (item.ItemOrder != null && item.ItemOrder != this)
            {
                throw new ArgumentException("This item is already assigned to a different order.");
            }

            if(item.ItemOrder == null) 
            {
                Items[item] = Items.GetValueOrDefault(item, 1) + quantity;
                item.selectOrder(this);
            }
        }

        public void removeItem(Item item) //TEST THESE!
        {
            if(item == null)
            {
                throw new ArgumentNullException("Target item is null!");
            }

            if(Items.Count <= 1)
            {
                throw new ArgumentException("An order must contain an item at all times!");
            }


            if (!Items.ContainsKey(item))
            {
                return;
            }

            Items.Remove(item);
            item.removeOrder();

        }

        public void modifyItemOrder(Item item, int quantity)
        {
            if(item == null)
            {
                throw new ArgumentNullException("Target item is null.");
            }

            if (!Items.ContainsKey(item))
            {
                throw new ArgumentException("This order doesn't contain this item!");
            }

            if (quantity < 0)  //removal
            {
                if (quantity * -1 > Items[item])
                {
                    throw new ArgumentException("There's not enough items to remove!");
                }

                if (Items[item]-quantity <= 0)
                {
                    removeItem(item);
                } else
                {
                    Items[item] = Items[item]-quantity;
                }
            } else
                {
                Items[item] = Items[item]+quantity;
                }
        }



        public void addDelivery(Delivery delivery)
        {
            if (delivery == null)
            {
                throw new ArgumentNullException("Target delivery is null.");
            }

            if (!Deliveries.Contains(delivery))
            {
                Deliveries.Add(delivery);
                //Same with customer, reverse through constructor access
            }
        }

        public void removeDelivery(Delivery delivery)
        {
            if (delivery == null)
            {
                throw new ArgumentNullException("Target delivery is null.");
            }

            if(Deliveries.Contains(delivery))
            {
                Deliveries.Remove(delivery);
                ExtentManager.Instance.ExtentDelivery.Remove(delivery); //A delivery can't live without a parent order
            }
        }
    }
}
