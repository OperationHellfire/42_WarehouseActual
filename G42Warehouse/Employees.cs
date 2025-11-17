using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace G42Warehouse
{
    public enum ExperienceLevelType
    {
        Junior = 1,
        Senior = 2
    }
    
    public enum DriverLicenceType
    {
        B,
        C,
        C1
    }

    
    [DataContract]
    [KnownType(typeof(WarehouseManager))]
    [KnownType(typeof(DeliveryDriver))]
    [KnownType(typeof(MachineOperator))]
    public abstract class Employee
    {
        private static List<Employee> _extent = new();

        [IgnoreDataMember]
        public static IReadOnlyList<Employee> Extent => _extent.AsReadOnly();

        protected static void AddToExtent(Employee e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            _extent.Add(e);
        }

        private string _name = string.Empty;

        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name cannot be empty.");
                _name = value;
            }
        }

        private DateTime _employmentDate;

        [DataMember]
        public DateTime EmploymentDate
        {
            get => _employmentDate;
            set
            {
                if (value > DateTime.Now)
                    throw new ArgumentException("Employment date cannot be in the future.");
                _employmentDate = value;
            }
        }

        public static double YearlyGrowth { get; set; } = 0.20;

        private double _baseSalary;

        [DataMember]
        public double BaseSalary
        {
            get => _baseSalary;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Base salary must be positive.");
                _baseSalary = value;
            }
        }

        [DataMember]
        public ExperienceLevelType ExperienceLevel { get; set; }

        [IgnoreDataMember]
        public int YearsSinceEmployment =>
            (int)((DateTime.Now - EmploymentDate).TotalDays / 365);

        [IgnoreDataMember]
        public double Salary => BaseSalary * (1 + YearsSinceEmployment * YearlyGrowth);

        protected Employee(
            string name,
            DateTime employmentDate,
            double baseSalary,
            ExperienceLevelType experienceLevel)
        {
            Name = name;
            EmploymentDate = employmentDate;
            BaseSalary = baseSalary;
            ExperienceLevel = experienceLevel;

            AddToExtent(this);
        }

        protected Employee()
        {
        }

        public static void Save(string path = "employee_extent.xml")
        {
            var serializer = new DataContractSerializer(typeof(List<Employee>));
            using var stream = File.Create(path);
            serializer.WriteObject(stream, _extent);
        }

        public static bool Load(string path = "employee_extent.xml")
        {
            if (!File.Exists(path))
            {
                _extent.Clear();
                return false;
            }

            var serializer = new DataContractSerializer(typeof(List<Employee>));
            using var stream = File.OpenRead(path);

            try
            {
                _extent = (List<Employee>?)serializer.ReadObject(stream) ?? new List<Employee>();
                return true;
            }
            catch
            {
                _extent.Clear();
                return false;
            }
        }
    }
    
    [DataContract]
    public class WarehouseManager : Employee
    {
        public WarehouseManager(
            string name,
            DateTime employmentDate,
            double baseSalary,
            ExperienceLevelType experienceLevel = ExperienceLevelType.Senior)
            : base(name, employmentDate, baseSalary, experienceLevel)
        {
        }

        public WarehouseManager()
        {
        }
    }

    [DataContract]
    [KnownType(typeof(DeliveryDriver))]
    [KnownType(typeof(MachineOperator))]
    public abstract class Worker : Employee
    {
        protected Worker(
            string name,
            DateTime employmentDate,
            double baseSalary,
            ExperienceLevelType experienceLevel = ExperienceLevelType.Junior)
            : base(name, employmentDate, baseSalary, experienceLevel)
        {
        }

        protected Worker()
        {
        }
        
        public virtual void ReportInventory()
        {
            Console.WriteLine($"{Name} reported inventory.");
        }
    }

    [DataContract]
    public class DeliveryDriver : Worker
    {
        [DataMember]
        public DriverLicenceType TypeOfDriversLicence { get; private set; }

        public DeliveryDriver(
            string name,
            DateTime employmentDate,
            double baseSalary,
            DriverLicenceType licenceType,
            ExperienceLevelType experienceLevel = ExperienceLevelType.Junior)
            : base(name, employmentDate, baseSalary, experienceLevel)
        {
            TypeOfDriversLicence = licenceType;
        }

        public DeliveryDriver()
        {
        }
    }


    [DataContract]
    public class MachineOperator : Worker
    {
        private string _serialCode = string.Empty;

        [DataMember]
        public string SerialCode
        {
            get => _serialCode;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("SerialCode cannot be empty.");
                _serialCode = value;
            }
        }

        public MachineOperator(
            string name,
            DateTime employmentDate,
            double baseSalary,
            string serialCode,
            ExperienceLevelType experienceLevel = ExperienceLevelType.Junior)
            : base(name, employmentDate, baseSalary, experienceLevel)
        {
            SerialCode = serialCode;
        }

        public MachineOperator()
        {
        }
    }
}
