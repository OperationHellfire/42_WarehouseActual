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
    [KnownType(typeof(Worker))]
    [KnownType(typeof(WarehouseManager))]
    [KnownType(typeof(DeliveryDriver))]
    [KnownType(typeof(MachineOperator))]
    public abstract class Employee
    {

        protected static void AddToExtent(Employee e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            ExtentManager.Instance.ExtentEmployee.Add(e);
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

        [DataMember]
        private HashSet<Shift> _assignedShifts = [];

        public HashSet<Shift> AssignedShifts
        {
            get => _assignedShifts;
        }



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

        public void addShift(Shift shift)
        {
            if (shift == null) throw new ArgumentNullException("Shift is null");
            AssignedShifts.Add(shift);
        }
    }
    
    [DataContract]
    public class WarehouseManager : Employee
    {

        [DataMember]
        private HashSet<Worker> _workers = [];

        public HashSet<Worker> Workers
        {
            get => _workers;
            private set => _workers = value;
        }

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

        public void addWorkerToManage(Worker work)
        {
            if (work == null) throw new ArgumentNullException("Target employee is null!");

            if(!Workers.Contains(work))
            {
                Workers.Add(work);
                work.addManager(this);
            }
        }

        public void removeWorkerToManage(Worker work)
        {
            if (work == null) throw new ArgumentNullException("Target employee is null!");

            if (Workers.Contains(work))
            {
                Workers.Remove(work);
                work.removeManager(this);
            }
        }
    }

    [DataContract]
    public class Worker : Employee
    {
        [DataMember]
        private HashSet<WarehouseManager> _managers;

        public HashSet<WarehouseManager> Managers
        {
            get => _managers;
            private set
            {
                if (value.Count == 0) throw new ArgumentException("The set must contain at least one manager!");
                _managers = value;
            }
        }
        public Worker(
            string name,
            DateTime employmentDate,
            double baseSalary,
            HashSet<WarehouseManager> managers,
            ExperienceLevelType experienceLevel = ExperienceLevelType.Junior)
            : base(name, employmentDate, baseSalary, experienceLevel)
        {
            Managers = managers;
        }

        protected Worker()
        {
        }
        
        public virtual void ReportInventory()
        {
            Console.WriteLine($"{Name} reported inventory.");
        }

        public void addManager(WarehouseManager manager)
        {
            if (manager == null) throw new ArgumentNullException("Target manager is null!");

            if(!Managers.Contains(manager))
            {
                Managers.Add(manager);
                manager.addWorkerToManage(this);
            }
        }

        public void removeManager(WarehouseManager manager)
        {
            if (manager == null) throw new ArgumentNullException("Target manager is null!");

            if (Managers.Count == 0) throw new ArgumentException("There must be at least one manager present!");

            if (Managers.Contains(manager))
            {
                Managers.Remove(manager);
                manager.removeWorkerToManage(this);
            }
        }
    }

    [DataContract]
    public class DeliveryDriver : Worker
    {
        [DataMember]
        public DriverLicenceType TypeOfDriversLicence { get; private set; }

        [DataMember]
        private HashSet<Delivery> _deliveries = [];

        public HashSet<Delivery> Deliveries
        {
            get => _deliveries;
        }

        public DeliveryDriver(
            string name,
            DateTime employmentDate,
            double baseSalary,
            HashSet<WarehouseManager> managers,
            DriverLicenceType licenceType,
            ExperienceLevelType experienceLevel = ExperienceLevelType.Junior)
            : base(name, employmentDate, baseSalary, managers,experienceLevel)
        {
            TypeOfDriversLicence = licenceType;
        }

        public DeliveryDriver()
        {
        }

        public void addDelivery(Delivery delivery)
        {
            if (delivery == null) throw new ArgumentNullException("Target delivery is null.");

            if(!Deliveries.Contains(delivery))
            {
                Deliveries.Add(delivery);
                delivery.addDeliveryDriver(this);
            }
        }

        public void removeDelivery(Delivery delivery)
        {
            if (delivery == null) throw new ArgumentNullException("Target delivery is null.");

            if (Deliveries.Contains(delivery))
            {
                Deliveries.Remove(delivery);
                delivery.removeDeliveryDriver(this);
            }
        }
    }


    [DataContract]
    public class MachineOperator : Worker
    {
        [DataMember]
        private HashSet<Machine> _capableOfOperating = [];

        public HashSet<Machine> CapableOfOperating
        {
            get => _capableOfOperating;
            private set
            {
                if(value.Count == 0)
                {
                    throw new ArgumentException("The set must contain at least one machine!!");
                }
                _capableOfOperating = value;
            }
        }

        [DataMember]
        private Dictionary<String, Machine> _controlledMachines;

        public Dictionary<String, Machine> ControlledMachines
        {
            get => _controlledMachines;
            private set
            {
                if (value.Count == 0) throw new ArgumentException("This Dictionary must contain at minimum one element!");
                _controlledMachines = value;
;           }
        }
        public MachineOperator(
            string name,
            DateTime employmentDate,
            double baseSalary,
            HashSet<WarehouseManager> managers,
            HashSet<Machine> capability,
            Dictionary<String, Machine> Controlled,
            ExperienceLevelType experienceLevel = ExperienceLevelType.Junior)
            : base(name, employmentDate, baseSalary, managers, experienceLevel)
        {
            CapableOfOperating = capability;
            ControlledMachines = Controlled;
        }

        public void addCapability(Machine capability)
        {
            if (capability == null) throw new ArgumentNullException("Target machine is null!");
            
            if(!CapableOfOperating.Contains(capability))
            {
                CapableOfOperating.Add(capability);
                capability.addCapableOperator(this);
            }
        }

        public void removeCapability(Machine capability) {
            if (capability == null) throw new ArgumentNullException("Target machine is null!");

            if(CapableOfOperating.Count == 1) throw new ArgumentException("There must be at least one machine present in this set!");

            if(CapableOfOperating.Contains(capability))
            {
                CapableOfOperating.Remove(capability);
                capability.removeCapableOperator(this);
            }
        }

        public void addControlledMachine(String serial_code, Machine mach)
        {
            if (mach == null) throw new ArgumentNullException("Target machine is null!");
            if (!CapableOfOperating.Contains(mach)) throw new ArgumentException("This operator is incapable of operating this machinery!");

            if(!ControlledMachines.ContainsKey(serial_code))
            {
                ControlledMachines.Add(serial_code, mach);
                mach.addControlOperator(this);
            }
        }

        public void removeControlledMachine(String serial_code, Machine mach)
        {
            if (mach == null) throw new ArgumentNullException("Target machine is null!");

            if (ControlledMachines.ContainsKey(serial_code))
            {
                ControlledMachines.Remove(serial_code);
                mach.removeControlOperator(this);
            }
        }

    }
}
