using System.Device.Gpio;
using Iot.Device.RotaryEncoder;
using OfficeOpenXml.Core.ExcelPackage;
using Raven.Iot.Device;
using Raven.Iot.Device.GpioExpander;
using Raven.Iot.Device.Ina219;
using UnitsNet;
using UnitsNet.Units;


var pinA = DeviceHelper.BcmToWiringPi(0);
var pinB = DeviceHelper.BcmToWiringPi(1);

if (DeviceHelper.GetIna219Devices() is [var settings])
{
    var calibrator = Ina219Calibrator.Default with
    {
        VMax = ElectricPotential.FromVolts(5.0),
        IMax = ElectricCurrent.FromAmperes(0.6)
    };

    var ina219 = calibrator.CreateCalibratedDevice(settings);
    var package = new ExcelPackage();

}