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
                    throw new ArgumentException("Order Date can't be in the future!");
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

        public Order(DateTime time)
        {
            _orderdate = time;
            PaymentStatus = PaymentStatus.Non_Paid;
            PreparationStatus = PreparationStatus.In_Progress;
            PaymentMethod = PaymentMethod.Cash; //Placeholder
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



    }
}
