#include <Arduino.h>
#include <WiFi.h>
#include <TroykaMeteoSensor.h>
#include <TroykaIMU.h>

WiFiServer server = WiFiServer(80);
WiFiClient client;
TroykaMeteoSensor sensor;
Barometer baro;

void setup () {
    WiFi.begin("ASOIU", "kaf.asoiu.48");
    Serial.print("Connecting...\n");
    auto res = WiFi.waitForConnectResult();

    if (res == WL_CONNECTED) {
        auto ip = WiFi.localIP();
        Serial.print(ip.toString());
        Serial.print('\n');
    } else {
        Serial.print("error! can't connect to wi-fi!");
        while (true) { /* Hang up */ }
    }

    sensor.begin();
    baro.begin();
    server.begin();
}

void loop() {
    String meteo_temp;
    String humidity;
    String baro_temp;
    String pressure;
    String altitude;

    client = server.available();
    if (client) {
        Serial.print("some client\n");
        auto state = sensor.read();
        switch (state) {
            case SHT_OK:
                meteo_temp = String(sensor.getTemperatureC());
                humidity = String(sensor.getHumidity());
            break;
            default:
                meteo_temp = String("Error!");
                humidity = String("Error!");
            break;
        }

        baro_temp = String(baro.readTemperatureC());
        pressure = String(baro.readPressureMillimetersHg());
        altitude = String(baro.readAltitude());

        client.write("HTTP/1.0 200 OK\n", 16);
        client.write("Content-Type: text/html\n\n", 25);

        client.write("<html><body><h2>SHT31</h2><p>Temp: ", 35);
        client.write(meteo_temp.c_str(), meteo_temp.length());
        client.write("&deg;C</p><p>Humid: ", 20);
        client.write(humidity.c_str(), humidity.length());
        client.write("%</p><h2>LPS25HB</h2><p>Temp: ", 30);
        client.write(baro_temp.c_str(), baro_temp.length());
        client.write("&deg;C</p><p>Press: ", 20);
        client.write(pressure.c_str(), pressure.length());
        client.write("mmHg</p><p>Alt: ", 16);
        client.write(altitude.c_str(), altitude.length());
        client.write("m</p></body></html>", 19);
        client.stop();
    }
}
