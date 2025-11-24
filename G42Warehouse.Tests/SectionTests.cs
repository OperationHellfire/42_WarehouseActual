using System;
using System.IO;
using System.Threading.Tasks;
using G42Warehouse;
using Xunit;

namespace G42Warehouse.Tests
{
    public class SectionTests
    {

        public SectionTests() {
            ExtentManager.Reset();
        }
        [Fact]
        public void HazardousSection_Constructor_ValidData_AddsToExtentAndSetsProperties()
        {
            ExtentManager.Load();
            var instance = ExtentManager.Instance;
            var sectionarr = instance.ExtentSection;
            int beforeCount = sectionarr.Count;

            var section = new HazardousMaterialsSection(
                "HazMat A",
                "North Wing",
                250.0,
                hasBackupGenerator: true,
                hazardTypes: new[] { HazardType.Flammable, HazardType.Corrosive },
                hasVentilationSystem: true,
                status: SectionStatus.Active
            );

            Assert.Equal("HazMat A", section.Name);
            Assert.Equal("North Wing", section.Location);
            Assert.Equal(250.0, section.Area);
            Assert.True(section.HasBackupGenerator);
            Assert.Equal(SectionStatus.Active, section.Status);
            Assert.Equal(2, section.HazardTypes.Count);
            Assert.Contains(HazardType.Flammable, section.HazardTypes);
            Assert.Contains(HazardType.Corrosive, section.HazardTypes);
            Assert.True(section.HasVentilationSystem);

            Assert.True(sectionarr.Contains(section));
            Assert.Equal(beforeCount + 1, sectionarr.Count);
        }

        [Fact]
        public void Name_Empty_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new RefrigeratedSection(
                    "",
                    "Loc",
                    10,
                    hasBackupGenerator: false,
                    minOperationalTemperature: -10,
                    maxOperationalTemperature: 5)
            );
        }

        [Fact]
        public void Location_Empty_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new RefrigeratedSection(
                    "Sec1",
                    "",
                    10,
                    hasBackupGenerator: false,
                    minOperationalTemperature: -10,
                    maxOperationalTemperature: 5)
            );
        }

        [Fact]
        public void Area_NotPositive_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new RefrigeratedSection(
                    "Sec1",
                    "Loc",
                    0,
                    hasBackupGenerator: false,
                    minOperationalTemperature: -10,
                    maxOperationalTemperature: 5)
            );

            Assert.Throws<ArgumentException>(() =>
                new RefrigeratedSection(
                    "Sec1",
                    "Loc",
                    -5,
                    hasBackupGenerator: false,
                    minOperationalTemperature: -10,
                    maxOperationalTemperature: 5)
            );
        }

        [Fact]
        public void Temperature_InvalidRange_ThrowsArgumentException()
        {
            var section = new RefrigeratedSection(
                "Cold Room",
                "B2",
                50,
                hasBackupGenerator: true,
                minOperationalTemperature: -10,
                maxOperationalTemperature: 5);

            Assert.Throws<ArgumentException>(() => section.Temperature = -100);
            Assert.Throws<ArgumentException>(() => section.Temperature = 100);
        }

        [Fact]
        public void Humidity_InvalidRange_ThrowsArgumentException()
        {
            var section = new RefrigeratedSection(
                "Ambient 1",
                "A1",
                100,
                hasBackupGenerator: false,
                minOperationalTemperature: -10,
                maxOperationalTemperature: 5);

            Assert.Throws<ArgumentException>(() => section.Humidity = -1);
            Assert.Throws<ArgumentException>(() => section.Humidity = 101);
        }

        [Fact]
        public void TemperatureAndHumidity_Optional_AndIsColdStorageWorks()
        {
            var section = new RefrigeratedSection(
                "Fridge",
                "C1",
                30,
                hasBackupGenerator: true,
                minOperationalTemperature: -10,
                maxOperationalTemperature: 5);

            section.Temperature = null;
            Assert.False(section.IsColdStorage);

            section.Temperature = 2;
            Assert.True(section.IsColdStorage);

            section.Humidity = null;
            section.Humidity = 40;
            Assert.Equal(40, section.Humidity);
        }

        [Fact]
        public void AddRestrictedMaterial_Valid_AddsToCollection()
        {
            var section = new HazardousMaterialsSection(
                "HazMat A",
                "D1",
                150,
                hasBackupGenerator: true,
                hazardTypes: new[] { HazardType.Flammable });

            section.AddRestrictedMaterial("Explosives");
            section.AddRestrictedMaterial("Flammable Liquids");

            Assert.Equal(2, section.RestrictedMaterials.Count);
            Assert.Contains("Explosives", section.RestrictedMaterials);
            Assert.Contains("Flammable Liquids", section.RestrictedMaterials);
        }

        [Fact]
        public void AddRestrictedMaterial_Empty_ThrowsArgumentException()
        {
            var section = new HazardousMaterialsSection(
                "HazMat A",
                "D1",
                150,
                hasBackupGenerator: true,
                hazardTypes: new[] { HazardType.Flammable });

            Assert.Throws<ArgumentException>(() => section.AddRestrictedMaterial(""));
        }

        [Fact]
        public async Task SaveAndLoad_PreservesExtent()
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + "_sections.xml");

            try
            {
                ExtentManager.Reset(); 

                var s1 = new HazardousMaterialsSection(
                    "HazMat A",
                    "A1",
                    100,
                    hasBackupGenerator: false,
                    hazardTypes: new[] { HazardType.Corrosive },
                    status: SectionStatus.Active);
                s1.Temperature = 20;

                var s2 = new RefrigeratedSection(
                    "Fridge 1",
                    "C1",
                    40,
                    hasBackupGenerator: true,
                    minOperationalTemperature: -10,
                    maxOperationalTemperature: 5,
                    status: SectionStatus.Maintenance);
                s2.Temperature = 2;
                s2.AddRestrictedMaterial("Biological Waste");

                ExtentManager.Save(path);

                bool loaded = ExtentManager.Load(path);
                 var sectionextent = ExtentManager.Instance.ExtentSection;

                Assert.True(loaded);
                Assert.Equal(2, sectionextent.Count);
                Assert.Contains(sectionextent, s => s.Name == "HazMat A");
                Assert.Contains(sectionextent, s => s.Name == "Fridge 1");
            }
            finally
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
        }

        [Fact]
        public void Extent_IsEncapsulated_ModifyingCopyDoesNotAffectExtent()
        {
            ExtentManager.Load("non_existing_sections.xml");

            var s = new RefrigeratedSection(
                "Test",
                "T1",
                10,
                hasBackupGenerator: false,
                minOperationalTemperature: -10,
                maxOperationalTemperature: 5);

            var sectionarr = ExtentManager.Instance.ExtentSection;

            var copy = new System.Collections.Generic.List<Section>(sectionarr);
            copy.Clear();

            Assert.Equal(1, sectionarr.Count);
            Assert.True(sectionarr.Contains(s));
        }
    }
}

