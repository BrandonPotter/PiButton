using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Raspberry.IO.GeneralPurpose;

namespace PiButton.Controllers
{
    public class GpioController : Controller
    {
        private static Dictionary<ushort, OutputPinConfiguration> pins;
        private GpioConnection connection;

        public GpioController()
        {
            pins = new Dictionary<ushort, OutputPinConfiguration> {
                { 23, ProcessorPin.Pin23.Output() },
                { 24, ProcessorPin.Pin24.Output() }
            };

            connection = new GpioConnection(pins[23], pins[24]);
        }

        // POST api/leds/23
        [HttpPost("{id}")]
        public void Put(ushort id)
        {
            connection.Blink(pins[id], TimeSpan.FromMilliseconds(500));
            Thread.Sleep(500);
            connection.Blink(pins[id], TimeSpan.FromMilliseconds(500));
        }
    }
}
