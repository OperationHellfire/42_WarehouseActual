using System;
using G42Warehouse;
using Xunit;

namespace G42Warehouse.Tests
{
    public class MachineTests
    {
        public MachineTests()
        {
            ExtentManager.Reset();
        }

        [Fact]
        public void Machine_Constructor_ValidData_AddsToExtentAndSetsProperties()
        {
            var machines = ExtentManager.Instance.ExtentMachine;
            int beforeCount = machines.Count;

            var lastMaintenance = DateTime.Now.AddDays(-5);

            // Transporter 4 parametreli ctor kullanıyor
            var transporter = new Transporter(
                status: MachineStatus.Ready,
                lastmain: lastMaintenance,
                load: 0,
                speed: 0);

            Assert.Equal(MachineStatus.Ready, transporter.Status);
            Assert.Equal(lastMaintenance, transporter.LastMaintanenceDate);

            Assert.Contains(transporter, machines);
            Assert.Equal(beforeCount + 1, machines.Count);
        }

        [Fact]
        public void Machine_Constructor_FutureMaintenanceDate_ThrowsArgumentException()
        {
            var futureDate = DateTime.Now.AddDays(1);

            Assert.Throws<ArgumentException>(() =>
                new Transporter(
                    status: MachineStatus.Ready,
                    lastmain: futureDate,
                    load: 0,
                    speed: 0));
        }

        [Fact]
        public void Machine_LastMaintenanceDate_SetToFuture_ThrowsArgumentException()
        {
            var transporter = new Transporter(
                status: MachineStatus.Ready,
                lastmain: DateTime.Now.AddDays(-1),
                load: 0,
                speed: 0);

            var futureDate = DateTime.Now.AddDays(1);

            Assert.Throws<ArgumentException>(() =>
            {
                transporter.LastMaintanenceDate = futureDate;
            });
        }

        [Fact]
        public void Machine_Status_CanBeChanged()
        {
            var transporter = new Transporter(
                status: MachineStatus.Ready,
                lastmain: DateTime.Now.AddDays(-2),
                load: 0,
                speed: 0);

            transporter.Status = MachineStatus.Busy;

            Assert.Equal(MachineStatus.Busy, transporter.Status);
        }

        [Fact]
        public void Transporter_Load_OutOfRange_ThrowsArgumentOutOfRangeException()
        {
            var transporter = new Transporter(
                status: MachineStatus.Ready,
                lastmain: DateTime.Now.AddDays(-2),
                load: 0,
                speed: 0);

            Assert.Throws<ArgumentOutOfRangeException>(() => transporter.Load = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => transporter.Load = 236);
        }

        [Fact]
        public void Transporter_Load_WithinRange_IsAccepted()
        {
            var transporter = new Transporter(
                status: MachineStatus.Ready,
                lastmain: DateTime.Now.AddDays(-2),
                load: 0,
                speed: 0);

            transporter.Load = 0;
            Assert.Equal(0, transporter.Load);

            transporter.Load = 235;
            Assert.Equal(235, transporter.Load);
        }

        [Fact]
        public void Transporter_Speed_OutOfRange_ThrowsArgumentOutOfRangeException()
        {
            var transporter = new Transporter(
                status: MachineStatus.Ready,
                lastmain: DateTime.Now.AddDays(-2),
                load: 0,
                speed: 0);

            Assert.Throws<ArgumentOutOfRangeException>(() => transporter.Speed = -0.1);
            Assert.Throws<ArgumentOutOfRangeException>(() => transporter.Speed = 12.6);
        }

        [Fact]
        public void Transporter_Speed_WithinRange_DoesNotThrow()
        {
            ExtentManager.Reset();

            var transporter = new Transporter(
                status: MachineStatus.Ready,
                lastmain: DateTime.Now.AddDays(-2),
                load: 0,
                speed: 0);

            Exception ex1 = Record.Exception(() => transporter.Speed = 0);
            Assert.Null(ex1);

            Exception ex2 = Record.Exception(() => transporter.Speed = 12.5);
            Assert.Null(ex2);
        }


        [Fact]
        public void Lifter_Constructor_ValidData_AddsToExtentAndSetsProperties()
        {
            var machines = ExtentManager.Instance.ExtentMachine;
            int beforeCount = machines.Count;

            var lastMaintenance = DateTime.Now.AddDays(-3);

            var lifter = new Lifter(
                status: MachineStatus.Ready,
                lastmain: lastMaintenance,
                load: 100);

            Assert.Equal(MachineStatus.Ready, lifter.Status);
            Assert.Equal(lastMaintenance, lifter.LastMaintanenceDate);
            Assert.Equal(100, lifter.Load);

            Assert.Contains(lifter, machines);
            Assert.Equal(beforeCount + 1, machines.Count);
        }

        [Fact]
        public void Lifter_Load_OutOfRange_ThrowsArgumentOutOfRangeException()
        {
            var lifter = new Lifter(
                status: MachineStatus.Ready,
                lastmain: DateTime.Now.AddDays(-2),
                load: 0);

            Assert.Throws<ArgumentOutOfRangeException>(() => lifter.Load = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => lifter.Load = 436);
        }

        [Fact]
        public void Lifter_Load_WithinRange_IsAccepted()
        {
            var lifter = new Lifter(
                status: MachineStatus.Ready,
                lastmain: DateTime.Now.AddDays(-2),
                load: 0);

            lifter.Load = 0;
            Assert.Equal(0, lifter.Load);

            lifter.Load = 435;
            Assert.Equal(435, lifter.Load);
        }
    }
}
