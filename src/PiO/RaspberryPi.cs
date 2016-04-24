using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PiO
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class RaspberryPi
    {
        private static Dictionary<int, GpioPin> _pins = new Dictionary<int, GpioPin>();
        private static Thread _pollThread = null;
        public RaspberryPi()
        {
            if (_pollThread == null)
            {
                _pollThread = new Thread(new ParameterizedThreadStart(PollThreadLoop));
                _pollThread.IsBackground = true;
                _pollThread.Start();
            }
        }

        private void PollThreadLoop(object state)
        {
            // change to interrupts
            while (true)
            {
                try
                {
                    PollPins(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception polling pins: " + ex.Message);
                    System.Threading.Thread.Sleep(1000);
                }
                System.Threading.Thread.Sleep(1);
            }
        }

        private void PollPins(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                var pins = _pins.Values.ToList();

                foreach (var pin in pins)
                {
                    pin.Poll();
                }
                System.Threading.Thread.Sleep(1);
            }
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
