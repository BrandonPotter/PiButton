using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiO
{
    public enum GpioPinDirection
    {
        In,
        Out
    }

    public class GpioPin
    {
        private RaspberryPi _parentPi;
        public int Index { get; private set; }
        public GpioPinDirection Direction { get; private set; }
        private bool _lastWriteValue = false;
        private bool _lastReadValue = false;

        internal GpioPin(RaspberryPi parentPi, int index, GpioPinDirection direction)
        {
            _parentPi = parentPi;
            Index = index;
            Direction = direction;

            SetupPin();
        }

        public string PinPath
        {
            get { return $"/sys/class/gpio/gpio{Index}"; }
        }

        private void SetupPin()
        {
            Log("Setup Pin");
            string directionFilePath = $"{PinPath}/direction";

            if (!System.IO.File.Exists("/sys/class/gpio/unexport"))
            {
                Log("Unexport file not found");
                throw new InvalidOperationException("Unexport path does not exist. Is service running on Pi?");
            }

            if (System.IO.File.Exists(directionFilePath))
            {
                Log("Unexporting");
                System.IO.File.WriteAllText("/sys/class/gpio/unexport", Index.ToString());
            }

            Log("Exporting");
            System.IO.File.WriteAllText("/sys/class/gpio/export", Index.ToString());

            string directionContent = "out";
            if (Direction == GpioPinDirection.In)
            {
                directionContent = "in";
            }
            Log("Setting direction");

            
            for (int i = 0; i < 10; i++)
            {
                if (!System.IO.File.Exists(directionFilePath))
                {
                    Log("Waiting for " + directionFilePath + " to exist");
                }
                else
                {
                    break;
                }
                System.Threading.Thread.Sleep(250);
            }

            System.IO.File.WriteAllText(directionFilePath, directionContent);
            Log("Setup finished");
        }

        public void Write(bool value)
        {
            Log("Setting " + value.ToString());
            System.IO.File.WriteAllText($"{PinPath}/value", value ? "1" : "0");
            _lastWriteValue = value;
        }

        public bool Value
        {
            get
            {
                if (Direction == GpioPinDirection.In)
                {
                    return Read();
                }

                return _lastWriteValue;
            }
        }

        public bool Read()
        {
            string fileName = $"{PinPath}/value";
            if (System.IO.File.Exists($"{PinPath}/value"))
            {
                string result = System.IO.File.ReadAllText($"{PinPath}/value");
                bool currentValue = result.Trim() == "1";

                if (currentValue != _lastReadValue)
                {
                    Log("Value changed to " + currentValue.ToString());
                }

                _lastReadValue = currentValue;
                return currentValue;
            }

            throw new InvalidOperationException("Could not read pin " + Index.ToString() + ", value file does not exist");
        }

        private void Log(string msg)
        {
            System.Console.WriteLine($"GPIO Pin {Index}: {msg}");
        }

        internal void Poll()
        {
            if (Direction == GpioPinDirection.In)
            {
                Read();
            }
        }
    }
}
