using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace G42Warehouse
{
    [DataContract]
    public class Shift
    {
        [DataMember]
        private DateTime _shiftStart;

        public DateTime ShiftStart
        {
            get => _shiftStart;
            set
            {
                if (value < DateTime.Now)
                {
                    throw new ArgumentOutOfRangeException("Shift start time can't be in the past.");
                }
                _shiftStart = value;
            }
        }

        [DataMember]
        private DateTime _shiftEnd;

        public DateTime ShiftEnd
        {
            get => _shiftEnd;
            set
            {
                if (value < DateTime.Now)
                {
                    throw new ArgumentOutOfRangeException("Shift end time can't be in the past.");
                }
                _shiftEnd = value;
            }
        }

        [DataMember]
        private Employee _employee;

        public Employee Employee
        {
            get => _employee;
            private set => _employee = value;
        }

        [DataMember]
        private Section _assignedSection;

        public Section AssignedSection
        {
            get => _assignedSection;
            private set => _assignedSection = value;
        }

        public Shift(DateTime start, DateTime end)
        {
            ShiftStart = start;
            ShiftEnd = end;
            addExtent(this);
        }


        public void connectShift(Employee emp, Section sec)
        {
            checkCollision(emp, this.ShiftStart,this.ShiftEnd);
            Employee = emp;
            emp.addShift(this);
            AssignedSection = sec;
            sec.addShift(this);
        }

        public void checkCollision(Employee emp,DateTime new1, DateTime new2)
        {
            foreach (Shift shift in emp.AssignedShifts)
            {
                if(new1 >= shift.ShiftStart && new2 <= shift.ShiftEnd)
                {
                    throw new ArgumentException("This employee already has a shift assigned at this time period.");
                }
            }
        }

        private void addExtent(Shift shift)
        {
            if(shift == null)
            {
                throw new ArgumentNullException("Shift is null");
            }

            ExtentManager.Instance.ExtentShift.Add(shift);
        }


    }
}
