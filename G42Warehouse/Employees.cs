using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace G42Warehouse
{
    public enum EmployeeCategory
    {
        WarehouseManager,
        Worker
    }

    public enum WorkerSpecialization
    {
        None,
        DeliveryDriver,
        MachineOperator
    }

    public enum ExperienceLevelType
    {
        Junior = 1,
        Senior = 2
    }

    [DataContract]
    public class Employee
    {
        private static List<Employee> _extent = new();

        [IgnoreDataMember]
        public static IReadOnlyList<Employee> Extent => _extent.AsReadOnly();

        private static void AddToExtent(Employee e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            _extent.Add(e);
        }

        private string _name;

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

        [IgnoreDataMember]
        public int YearsSinceEmployment =>
            (int)((DateTime.Now - EmploymentDate).TotalDays / 365);

        [IgnoreDataMember]
        public double Salary => BaseSalary * (1 + YearsSinceEmployment * YearlyGrowth);

        [DataMember]
        public ExperienceLevelType ExperienceLevel { get; set; }

        [DataMember]
        public EmployeeCategory Category { get; private set; }

        [DataMember]
        public WorkerSpecialization Specialization { get; private set; }

        public Employee(
            string name,
            DateTime employmentDate,
            double baseSalary,
            EmployeeCategory category = EmployeeCategory.Worker,
            WorkerSpecialization specialization = WorkerSpecialization.None,
            ExperienceLevelType experienceLevel = ExperienceLevelType.Junior)
        {
            Name = name;
            EmploymentDate = employmentDate;
            BaseSalary = baseSalary;
            Category = category;
            Specialization = specialization;
            ExperienceLevel = experienceLevel;

            AddToExtent(this);
        }

        public Employee()
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
}
