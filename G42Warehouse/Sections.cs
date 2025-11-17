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

    public enum SectionType
    {
        HazardousMaterials,
        AmbientStorage,
        RefrigeratedStorage,
        Other
    }

    [DataContract]
    public class Section
    {
        private static List<Section> _extent = new();

        [IgnoreDataMember]
        public static IReadOnlyList<Section> Extent => _extent.AsReadOnly();

        private static void AddToExtent(Section s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            _extent.Add(s);
        }

        private string _name;

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

        private string _location;

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
        private List<string> _forbiddenMaterials = new();

        [IgnoreDataMember]
        public IReadOnlyList<string> ForbiddenMaterials
            => (_forbiddenMaterials ??= new List<string>()).AsReadOnly();

        public void AddForbiddenMaterial(string material)
        {
            if (string.IsNullOrWhiteSpace(material))
                throw new ArgumentException("Material cannot be empty.");

            (_forbiddenMaterials ??= new List<string>()).Add(material);
        }

        public static int MaxSections { get; set; } = 200;

        [IgnoreDataMember]
        public bool IsColdStorage => Temperature != null && Temperature < 5;

        [DataMember]
        public SectionStatus Status { get; set; }

        [DataMember]
        public SectionType Type { get; set; }

        public Section(
            string name,
            string location,
            double area,
            bool hasBackupGenerator,
            SectionType type = SectionType.AmbientStorage,
            SectionStatus status = SectionStatus.Active)
        {
            Name = name;
            Location = location;
            Area = area;
            HasBackupGenerator = hasBackupGenerator;
            Type = type;
            Status = status;

            AddToExtent(this);
        }

        public Section()
        {
        }

        public static void Save(string path = "section_extent.xml")
        {
            var serializer = new DataContractSerializer(typeof(List<Section>));
            using var stream = File.Create(path);
            serializer.WriteObject(stream, _extent);
        }

        public static bool Load(string path = "section_extent.xml")
        {
            if (!File.Exists(path))
            {
                _extent.Clear();
                return false;
            }

            var serializer = new DataContractSerializer(typeof(List<Section>));
            using var stream = File.OpenRead(path);

            try
            {
                _extent = (List<Section>?)serializer.ReadObject(stream) ?? new List<Section>();
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
