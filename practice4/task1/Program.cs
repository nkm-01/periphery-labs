using System.Device.Gpio;
using Iot.Device.RotaryEncoder;
using Raven.Iot.Device;
using Raven.Iot.Device.GpioExpander;
using UnitsNet;
using UnitsNet.Units;

const int motorPin = 9;
var pinA = DeviceHelper.WiringPiToBcm(0);
var pinB = DeviceHelper.WiringPiToBcm(1);

if (DeviceHelper.GetGpioExpanderDevices() is [var settings])
{
    var expander = new GpioExpander(settings);
    var encoder = new ScaledQuadratureEncoder(pinA, pinB, PinEventTypes.Falling, 20);
    encoder.PulseCountChanged += (_, args) => {
        var angle = new Angle(args.Value, AngleUnit.Arcminute);
        expander.WriteAngle(motorPin, angle);
        Console.WriteLine($"Angle: {angle.Degrees} degrees");
    };
}
else
{
    throw new IoTDeviceException("Expander not found!!!");
}