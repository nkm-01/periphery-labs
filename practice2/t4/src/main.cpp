#include <Arduino.h>
#include <TroykaMeteoSensor.h>
#include <TroykaIMU.h>

TroykaMeteoSensor *sensor;
Barometer *baro;

void setup () {
    sensor = new TroykaMeteoSensor();
    baro = new Barometer();
    sensor->begin();
    baro->begin();
}

void loop() {
  // считываем данные с датчика
    int stateSensor = sensor->read();
    switch (stateSensor) {
        case SHT_OK:
            Serial.println("Meteo sensor is OK");
            Serial.print("Temperature = ");
            Serial.print(sensor->getTemperatureC());
            Serial.println(" C \t");
            Serial.print(sensor->getHumidity());
            Serial.println(" %\r\n");
        break;
        case SHT_ERROR_DATA:
            Serial.println("Data error on meteo sensor or it is not connected");
        break; 
        case SHT_ERROR_CHECKSUM:
            Serial.println("Checksum error!");
        break;
    }
    Serial.print("Pressure: ");
    Serial.print(baro->readPressureMillimetersHg());
    Serial.println("mm Hg");
    Serial.print("Temp 2: ");
    Serial.print(baro->readTemperatureC());
    Serial.println("C");
    Serial.print("Height: ");
    Serial.println(baro->readAltitude());
    delay(1000);
}

