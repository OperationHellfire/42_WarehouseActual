using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace G42Warehouse
{
    public enum MachineStatus
    {
        Ready = 0,
        InMaintanence = 1,
        Busy = 2
    }

    [DataContract]
    [KnownType(typeof(Transporter))]
    [KnownType(typeof(Lifter))]
    public abstract class Machine
    {
        [DataMember]
        private MachineStatus _status;
        public MachineStatus Status
        {
            get => _status;
            set => _status = value;
        }

        [DataMember]
        private DateTime _lastMaintanence;
        public DateTime LastMaintanenceDate
        {
            get => _lastMaintanence;
            set
            {
                if (value > DateTime.Now)
                {
                    throw new ArgumentException("Last Maintanence Date cannot be in the future!");
                }
                _lastMaintanence = value;
            }
        }

        [IgnoreDataMember]
        private TimeSpan _maintanenceSince => DateTime.Now.Subtract(_lastMaintanence);

        public Machine(MachineStatus status, DateTime lastmain)
        {
            Status = status;
            LastMaintanenceDate = lastmain;
            addExtent(this);
        }

        private static void addExtent(Machine machine)
        {
            if (machine == null)
            {
                throw new ArgumentNullException("Machine is null.");
            }
            ExtentManager.Instance.ExtentMachine.Add(machine);
        }

    }

    [DataContract]
    public class Transporter : Machine
    {
        private double _load;
        public double Load
        {
            get => _load;
            set
            {
                if(value < 0 || value > 235)
                {
                    throw new ArgumentOutOfRangeException("Load has to be between 0-235 KG!");
                }
                _load = value;
            }
        }

        private double _speed;
        public double Speed
        {
            get => _speed;
            set
            {
                if (value < 0 || value > 12.5)
                {
                    throw new ArgumentOutOfRangeException("Speed has to be between 0-12.5 KM/H!");
                }
            }
            }
        public Transporter(MachineStatus status, DateTime lastmain, double load, double speed) : base(status, lastmain)
        {
            Load = load;
            Speed = speed;
        }
    }

    [DataContract]
    public class Lifter : Machine
    {
        private double _load;
        public double Load
        {
            get => _load;
            set
            {
                if (value < 0 || value > 435)
                {
                    throw new ArgumentOutOfRangeException("Load has to be between 0-235 KG!");
                }
                _load = value;
            }
        }
        public Lifter(MachineStatus status, DateTime lastmain, double load) : base(status, lastmain)
        {
            Load = load;
        }
    }

}
