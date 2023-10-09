#include <Arduino.h>

void setup() {
    pinMode(D25, OUTPUT);
    analogWriteRange(4095);
    analogWriteFreq(120);
}

void loop() {
    int voltage = analogRead(A0);
    analogWrite(D25, voltage);
}