using System.Device.Gpio;
using System.Net.WebSockets;
using Iot.Device.RotaryEncoder;
using Raven.Iot.Device;
using Raven.Iot.Device.GpioExpander;
using UnitsNet;
using UnitsNet.Units;

if (Iot.Device.OneWire.OneWireThermometerDevice.EnumerateDevices().Take(1).ToList() is [var settings])
{
    while (true)
    {
        var temperature = settings.ReadTemperature(); 
        Console.WriteLine($"Temperature in °C: {temperature.DegreesCelsius}");
        Thread.Sleep(1000);
    }
}