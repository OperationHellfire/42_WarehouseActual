using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace G42Warehouse
{
    public enum DeliveryStatus
    {
        Pending = 0,
        InTransit = 1,
        Completed = 2,
        Incomplete =3,
        Cancelled = 4
    }
    [DataContract]
    public class Delivery
    {
        [DataMember]
        private int _trackingnumber;
        public int TrackingNumber
        {
            get => _trackingnumber;
            set
            {
                if(value < 0)
                {
                    throw new ArgumentException("Tracking Number can't be negative!");
                }
                _trackingnumber = value;
            }
        }

        [DataMember]
        private DateTime _date;

        public DateTime Date
        {
            get => _date;
            set
            {
                if (value > DateTime.Now)
                {
                    throw new ArgumentException("Date cannot be in the future");
                }
                _date = value;
            }
        }

        [DataMember]
        private DeliveryStatus _status;

        public DeliveryStatus Status
        {
            get => _status;
            set => _status = value;
        }

        [DataMember]
        private Address _address;
        public Address Address
        {
            get => _address;
            set => _address = value;
        }

        [DataMember(IsRequired = false)]
        private DateTime? _expectedarrival;

        public DateTime? Expectedarrival
        {
            get => _expectedarrival;
            set
            {
                if (value < DateTime.Now)
                {
                    throw new ArgumentException("Date can't be in the past.");
                }
                _expectedarrival = value;
            }
        }
        [DataMember]
        private DateTime _actualarrivaldate
        {
            get => _actualarrivaldate;
            set
            {
                if (value > DateTime.Now)
                {
                    throw new ArgumentException("Date can't be in the future.");
                }
            }
        }



        public Delivery(int trackingNumber,DateTime date,DeliveryStatus status,Address deliveryaddress, DateTime? expected, DateTime actual) {
            TrackingNumber = trackingNumber;
            Date = date;
            Status = status;
            Address = deliveryaddress;
            _expectedarrival = expected;
            _actualarrivaldate = actual;
            addExtent(this);
        }

        private static void addExtent(Delivery delivery)
        {
            if(delivery == null)
            {
                throw new ArgumentNullException("Delivery is null.");
            }
            ExtentManager.Instance.ExtentDelivery.Add(delivery);
        }

    }
}
