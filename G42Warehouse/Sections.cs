using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace G42Warehouse
{
    public enum SectionStatus
    {
        Active,
        Maintenance,
        Closed
    }

    public enum HazardType
    {
        Flammable,
        Corrosive,
        Toxic
    }

    [DataContract]
    [KnownType(typeof(HazardousMaterialsSection))]
    [KnownType(typeof(RefrigeratedSection))]
    public abstract class Section
    {

        protected static void AddToExtent(Section s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            ExtentManager.Instance.ExtentSection.Add(s);
        }

        private string _name = string.Empty;

        [DataMember]
        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Section name cannot be empty.");
                _name = value;
            }
        }

        private string _location = string.Empty;

        [DataMember]
        public string Location
        {
            get => _location;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Location cannot be empty.");
                _location = value;
            }
        }

        private double _area;

        [DataMember]
        public double Area
        {
            get => _area;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Area must be positive.");
                _area = value;
            }
        }

        [DataMember]
        public bool HasBackupGenerator { get; set; }

        private double? _temperature;

        [DataMember]
        public double? Temperature
        {
            get => _temperature;
            set
            {
                if (value != null && (value < -60 || value > 70))
                    throw new ArgumentException("Temperature out of valid range.");
                _temperature = value;
            }
        }

        private double? _humidity;

        [DataMember]
        public double? Humidity
        {
            get => _humidity;
            set
            {
                if (value != null && (value < 0 || value > 100))
                    throw new ArgumentException("Humidity must be between 0 and 100.");
                _humidity = value;
            }
        }

        [DataMember]
        private List<string> _restrictedMaterials = new();

        [IgnoreDataMember]
        public IReadOnlyList<string> RestrictedMaterials =>
            (_restrictedMaterials ??= new List<string>()).AsReadOnly();

        public void AddRestrictedMaterial(string material)
        {
            if (string.IsNullOrWhiteSpace(material))
                throw new ArgumentException("Material cannot be empty.");

            (_restrictedMaterials ??= new List<string>()).Add(material);
        }

        public static int MaxSections { get; set; } = 200;

        [IgnoreDataMember]
        public bool IsColdStorage => Temperature != null && Temperature < 5;

        [DataMember]
        public SectionStatus Status { get; set; }

        protected Section(
            string name,
            string location,
            double area,
            bool hasBackupGenerator,
            SectionStatus status = SectionStatus.Active)
        {
            Name = name;
            Location = location;
            Area = area;
            HasBackupGenerator = hasBackupGenerator;
            Status = status;

            AddToExtent(this);
        }

        protected Section()
        {
        }
    }

    [DataContract]
    public class HazardousMaterialsSection : Section
    {
        [DataMember]
        private List<HazardType> _hazardTypes = new();

        [IgnoreDataMember]
        public IReadOnlyList<HazardType> HazardTypes => _hazardTypes.AsReadOnly();

        [DataMember]
        public bool? HasVentilationSystem { get; set; }

        public HazardousMaterialsSection(
            string name,
            string location,
            double area,
            bool hasBackupGenerator,
            IEnumerable<HazardType> hazardTypes,
            bool? hasVentilationSystem = null,
            SectionStatus status = SectionStatus.Active)
            : base(name, location, area, hasBackupGenerator, status)
        {
            if (hazardTypes == null)
                throw new ArgumentNullException(nameof(hazardTypes));

            _hazardTypes = new List<HazardType>(hazardTypes);
            if (_hazardTypes.Count == 0)
                throw new ArgumentException("At least one hazard type is required.");

            HasVentilationSystem = hasVentilationSystem;
        }

        public HazardousMaterialsSection()
        {
        }
    }

    [DataContract]
    public class RefrigeratedSection : Section
    {
        private double _minOperationalTemperature;
        private double _maxOperationalTemperature;
        
        [DataMember]
        public double MinOperationalTemperature
        {
            get => _minOperationalTemperature;
            set
            {
                if (value < -80 || value > 30)
                    throw new ArgumentException("MinOperationalTemperature out of range.");
                _minOperationalTemperature = value;
            }
        }

        [DataMember]
        public double MaxOperationalTemperature
        {
            get => _maxOperationalTemperature;
            set
            {
                if (value < -80 || value > 30)
                    throw new ArgumentException("MaxOperationalTemperature out of range.");
                if (value < MinOperationalTemperature)
                    throw new ArgumentException("MaxOperationalTemperature cannot be lower than minimum.");
                _maxOperationalTemperature = value;
            }
        }
        
        public RefrigeratedSection(
            string name,
            string location,
            double area,
            bool hasBackupGenerator,
            double minOperationalTemperature,
            double maxOperationalTemperature,
            SectionStatus status = SectionStatus.Active)
            : base(name, location, area, hasBackupGenerator, status)
        {
            MinOperationalTemperature = minOperationalTemperature;
            MaxOperationalTemperature = maxOperationalTemperature;
        }

        public RefrigeratedSection()
        {
        }
    }
}
