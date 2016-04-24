using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiO
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class RaspberryPi
    {
        private Dictionary<int, GpioPin> _pins = new Dictionary<int, GpioPin>();
        public RaspberryPi()
        {
            
        }

        public bool GetPinValue(int index)
        {
            return GetPin(index, GpioPinDirection.In).Value;
        }

        public GpioPin GetPin(int index, GpioPinDirection direction)
        {
            if (_pins.ContainsKey(index))
            {
                var pin = _pins[index];
                if (pin.Direction != direction)
                {
                    throw new InvalidOperationException("Pin " + index.ToString() +
                                                        " is already configured for a different direction");
                }

                return pin;
            }

            _pins[index] = new GpioPin(this, index, direction);
            return _pins[index];
        }
    }
}
