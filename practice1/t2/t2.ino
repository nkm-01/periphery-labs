void setup() {
  pinMode(D0, OUTPUT);
  pinMode(D1, OUTPUT);
  pinMode(D2, OUTPUT);
  pinMode(D3, INPUT_PULLUP);
}

static int led = 0;
static bool pressed = false;

void loop() {
  if (digitalRead(D3) == LOW) {
    pressed = true;
  } else if (pressed){
    pressed = false;
    switch (led) {
      case 0:
        digitalWrite(D2, LOW);
        digitalWrite(D0, HIGH);
      case 1:
        digitalWrite(D0, LOW);
        digitalWrite(D1, HIGH);
      case 2:
        digitalWrite(D1, LOW);
        digitalWrite(D2, HIGH);
    }
    led++;
    if (led >= 3)
      led = 0;
  }
}
