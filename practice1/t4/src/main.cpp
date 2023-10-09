#include <Arduino.h>

int led = 0;
void on_button_pressed();
void setup() {
    pinMode(D0, OUTPUT);
    pinMode(D1, OUTPUT);
    pinMode(D2, OUTPUT);
    pinMode(D3, INPUT);

    digitalWrite(D0, LOW);
    digitalWrite(D1, LOW);
    digitalWrite(D2, LOW);

    attachInterrupt(D3, on_button_pressed, FALLING);
}

void loop() {        
}

void on_button_pressed() {
    led = (led + 1) % 3;
    switch (led)
    {
        case 0:
            digitalWrite(D0, HIGH);
            digitalWrite(D1, LOW);
            digitalWrite(D2, LOW);
            break;
        case 1:
            digitalWrite(D0, LOW);
            digitalWrite(D1, HIGH);
            digitalWrite(D2, LOW);
            break;
        case 2:
            digitalWrite(D0, LOW);
            digitalWrite(D1, LOW);
            digitalWrite(D2, HIGH);
            break;
        
        default:
            break;
    }
}
