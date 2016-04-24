using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using PiO;

namespace PiButton.Controllers
{
    public class GpioController : Controller
    {
        private RaspberryPi _pi;

        public GpioController()
        {
            _pi = new RaspberryPi();
        }

        [HttpGet]
        [Route("gpio/pin/{pinId}")]
        public string Pin(int pinId)
        {
            return _pi.GetPinValue(pinId) ? "1" : "0";
        }

        [HttpPost]
        [HttpPut]
        [Route("gpio/pin/{pinId}/{value}")]
        public void Pin(int pinId, string value)
        {
            Console.WriteLine("HTTP PUT for value: " + (value ?? string.Empty));
            var pin = _pi.GetPin(pinId, GpioPinDirection.Out);
            switch ((value ?? string.Empty).ToUpper())
            {
                case "0":
                    pin.Write(false);
                    break;

                case "1":
                    pin.Write(true);
                    break;

                case "OFF":
                    pin.Write(false);
                    break;

                case "ON":
                    pin.Write(true);
                    break;

                case "FALSE":
                    pin.Write(false);
                    break;

                case "TRUE":
                    pin.Write(true);
                    break;
            }
        }
    }
}
