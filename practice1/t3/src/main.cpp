#include <Arduino.h>

int last_state = 0;
int led = 0;

void setup() {
    pinMode(D0, OUTPUT);
    pinMode(D1, OUTPUT);
    pinMode(D2, OUTPUT);
    pinMode(D3, INPUT);

    digitalWrite(D0, LOW);
    digitalWrite(D1, LOW);
    digitalWrite(D2, LOW);
}

void loop() {
    int current_state = digitalRead(D3);
    if (current_state != last_state) {
        if (current_state == LOW) {
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
    }
    last_state = current_state;
}