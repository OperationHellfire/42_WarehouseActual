using System;
using System.IO;
using G42Warehouse;
using Xunit;

namespace G42Warehouse.Tests
{
    public class EmployeeTests
    {

        [Fact]
        public void DeliveryDriver_Constructor_ValidData_AddsToExtentAndSetsProperties()
        {
            ExtentManager.Load("non_existing_employees.xml");
            var employeearr = ExtentManager.Instance.ExtentEmployee;

            var employmentDate = DateTime.Now.AddYears(-2);
            int beforeCount = employeearr.Count;

            var emp = new DeliveryDriver(
                "John Doe",
                employmentDate,
                2000,
                DriverLicenceType.B);

            Assert.Equal("John Doe", emp.Name);
            Assert.Equal(2000, emp.BaseSalary);
            Assert.Equal(employmentDate, emp.EmploymentDate);
            Assert.Equal(ExperienceLevelType.Junior, emp.ExperienceLevel);
            Assert.Equal(DriverLicenceType.B, emp.TypeOfDriversLicence);

            Assert.True(employeearr.Contains(emp));
            Assert.Equal(beforeCount + 1, employeearr.Count);
            ExtentManager.Remove("non_existing_employees.xml");
        }

        [Fact]
        public void DeliveryDriver_ExperienceLevel_DefaultsToJunior_WhenNotSpecified()
        {
            var emp = new DeliveryDriver(
                "Test",
                DateTime.Now.AddYears(-1),
                1000,
                DriverLicenceType.C);

            Assert.Equal(ExperienceLevelType.Junior, emp.ExperienceLevel);
        }

        [Fact]
        public void WarehouseManager_DefaultsToSeniorExperience()
        {
            var mgr = new WarehouseManager(
                "Manager",
                DateTime.Now.AddYears(-5),
                3000);

            Assert.Equal(ExperienceLevelType.Senior, mgr.ExperienceLevel);
        }

        [Fact]
        public void DeliveryDriver_ExperienceLevel_CanBeSetToSenior()
        {
            var emp = new DeliveryDriver(
                "Test",
                DateTime.Now.AddYears(-1),
                1000,
                DriverLicenceType.C1,
                ExperienceLevelType.Senior);

            Assert.Equal(ExperienceLevelType.Senior, emp.ExperienceLevel);
        }

        [Fact]
        public void Constructor_EmptyName_ThrowsArgumentException()
        {
            var employmentDate = DateTime.Now.AddYears(-1);

            Assert.Throws<ArgumentException>(() =>
                new DeliveryDriver(
                    "",
                    employmentDate,
                    1500,
                    DriverLicenceType.B)
            );
        }

        [Fact]
        public void EmploymentDate_InFuture_ThrowsArgumentException()
        {
            var futureDate = DateTime.Now.AddDays(1);

            Assert.Throws<ArgumentException>(() =>
                new DeliveryDriver(
                    "Test",
                    futureDate,
                    1500,
                    DriverLicenceType.B)
            );
        }

        [Fact]
        public void BaseSalary_NotPositive_ThrowsArgumentException()
        {
            var employmentDate = DateTime.Now.AddYears(-1);

            Assert.Throws<ArgumentException>(() =>
                new DeliveryDriver(
                    "Test",
                    employmentDate,
                    0,
                    DriverLicenceType.B)
            );

            Assert.Throws<ArgumentException>(() =>
                new DeliveryDriver(
                    "Test",
                    employmentDate,
                    -10,
                    DriverLicenceType.B)
            );
        }

        [Fact]
        public void Salary_IsDerivedFromBaseSalaryAndGrowth()
        {
            Employee.YearlyGrowth = 0.20;
            var employmentDate = DateTime.Now.AddYears(-3);

            var emp = new DeliveryDriver(
                "Test",
                employmentDate,
                1000,
                DriverLicenceType.B);

            int years = emp.YearsSinceEmployment;
            double expectedSalary = 1000 * (1 + years * Employee.YearlyGrowth);

            Assert.Equal(expectedSalary, emp.Salary, 3);
        }

        [Fact]
        public void SaveAndLoad_PreservesExtent()
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + "_employees.xml");

            try
            {
                ExtentManager.Load(path);

                var e1 = new DeliveryDriver(
                    "Alice",
                    DateTime.Now.AddYears(-1),
                    1500,
                    DriverLicenceType.B);

                var e2 = new WarehouseManager(
                    "Bob",
                    DateTime.Now.AddYears(-2),
                    2000);

                ExtentManager.Save(path);

                bool loaded = ExtentManager.Load(path);
                var employeearr = ExtentManager.Instance.ExtentEmployee;

                Assert.True(loaded);
                Assert.Equal(2, employeearr.Count);
                Assert.Contains(employeearr, e => e.Name == "Alice");
                Assert.Contains(employeearr, e => e.Name == "Bob");
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
            ExtentManager.Load("non_existing_employees.xml");

            var e1 = new WarehouseManager(
                "Temp Emp",
                DateTime.Now.AddYears(-1),
                1000);

            var copy = new System.Collections.Generic.List<Employee>(ExtentManager.Instance.ExtentEmployee);
            copy.Clear();

            var employeearr = ExtentManager.Instance.ExtentEmployee;

            Assert.Equal(1, employeearr.Count);
            Assert.Contains(e1, employeearr);
        }
    }
}
