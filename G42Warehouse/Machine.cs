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

        [DataMember]
        private Dictionary<Item, int> _items = [];

        public Dictionary<Item, int> Items
        {
            get => _items;
            private set => _items = value;
        }

        [DataMember]
        private HashSet<MachineOperator> _capableoperators = [];

        public HashSet<MachineOperator> CapableOperators
        {
            get => _capableoperators;
            private set => _capableoperators = value;
        }

        [DataMember]

        private HashSet<MachineOperator> _controlOperators = [];

        public HashSet<MachineOperator> ControlOperators
        {
            get => _controlOperators;
            private set => _controlOperators = value;
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

        public void addItem(Item item, int quantity = 1)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Target item is null.");
            }

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException("Quantity must be positive.");
            }

            if (item.CarryingMachine == null) {
                Items[item] = Items.GetValueOrDefault(item, 1) + quantity;
                item.setMachine(this);
            }
        }

        public void removeItem(Item item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Target item is null.");
            }

            if (!Items.ContainsKey(item))
            {
                return;
            }

            Items.Remove(item);
            item.removeMachine();
        }

        public void modifyItemOrder(Item item, int quantity)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Target item is null.");
            }

            if (!Items.ContainsKey(item))
            {
                throw new ArgumentException("This machine doesn't contain this item!");
            }

            if (quantity < 0)  //removal
            {
                if (quantity * -1 > Items[item])
                {
                    throw new ArgumentException("There's not enough items to remove!");
                }

                if (Items[item] - quantity <= 0)
                {
                    removeItem(item);
                }
                else
                {
                    Items[item] = Items[item] - quantity;
                }
            }
            else
            {
                Items[item] = Items[item] + quantity;
            }
        }

        public void addCapableOperator(MachineOperator Operator)
        {
            if (Operator == null) throw new ArgumentNullException("Target operator is null!");

            if(!CapableOperators.Contains(Operator))
            {
                CapableOperators.Add(Operator);
                Operator.addCapability(this);
            }
        }

        public void removeCapableOperator(MachineOperator Operator)
        {
            if (Operator == null) throw new ArgumentNullException("Target operator is null!");

            if (CapableOperators.Contains(Operator))
            {
                CapableOperators.Remove(Operator);
                Operator.removeCapability(this);
            }
        }

        public void addControlOperator(MachineOperator ControlOperator) //never ever run this on its own, ever.
        {
            if (ControlOperator == null) throw new ArgumentNullException("Target Operator is null!");
            
            if(!ControlOperators.Contains(ControlOperator))
            {
                ControlOperators.Add(ControlOperator);
            }
        }

        public void removeControlOperator(MachineOperator ControlOperator) //never ever run this on its own, ever.
        {
            if (ControlOperator == null) throw new ArgumentNullException("Target Operator is null!");

            if (ControlOperators.Contains(ControlOperator))
            {
                ControlOperators.Remove(ControlOperator);
            }
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
