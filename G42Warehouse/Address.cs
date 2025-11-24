using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace G42Warehouse
{
    [DataContract]
    public struct Address
    {
        private string _building;
        private string _street;
        private string _city;
        private string _country;
        private string _postalcode;


        public Address(string f1, string f2, string f3, string f4, string f5) {
            Building = f1;
            Street = f2;
            City = f3;
            Country = f4;
            PostalCode = f5;
        }

        public string Building
        {
            get => _building;
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("String can't be empty!");
                }
                _building = value;
            }
        }

        public string Street
        {
            get => _street;
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("String can't be empty!");
                }
                _street = value;
            }
        }

        public string City
        {
            get => _city;
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("String can't be empty!");
                }
                _city = value;
            }
        }

        public string Country
        {
            get => _country;
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("String can't be empty!");
                }
                _country = value;
            }
        }

        public string PostalCode
        {
            get => _postalcode;
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("String can't be empty!");
                }
                _postalcode = value;
            }
        }


    }
}
