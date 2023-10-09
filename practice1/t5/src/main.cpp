#include <Arduino.h>
#include <pico.h>
#include <hardware/timer.h>

int last_state = 0;
int led = 0;
repeating_timer_t timer;

bool on_timer_tick(repeating_timer_t *_);

void setup() {
    pinMode(D0, OUTPUT);
    pinMode(D1, OUTPUT);
    pinMode(D2, OUTPUT);
    pinMode(D3, INPUT);

    digitalWrite(D0, LOW);
    digitalWrite(D1, LOW);
    digitalWrite(D2, LOW);

    add_repeating_timer_ms(5000, on_timer_tick, NULL, &timer);
}

void loop() {
    
}

bool on_timer_tick(repeating_timer_t *_) {
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

    return true;
}
