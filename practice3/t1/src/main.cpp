#include <Arduino.h>
#include <WiFi.h>

void setup () {
    WiFi.begin("ASOIU", "kaf.asoiu.48");
    auto res = WiFi.waitForConnectResult();

    if (res == WL_CONNECTED) {
        auto ip = WiFi.localIP();
        Serial.print(ip.toString());
        Serial.print('\n');
    } else {
        Serial.print("error! can't connect to wi-fi!");
        while (true) { /* Hang up */ }
    }
}

void loop() {
    auto host = Serial.readStringUntil('\n');
    if (host.length() <= 0) 
        goto end;
    if (WiFi.ping(host) > 0) {
        Serial.printf("%s is available\n", host.c_str());
    } else {
        Serial.printf("%s: host is unreachable\n", host.c_str());
    }

    end:
        { }
}
