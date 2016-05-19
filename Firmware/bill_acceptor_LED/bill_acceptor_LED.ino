#include <Adafruit_NeoPixel.h>

//bill pins
const int enablePin = A0;
const int creditSig = A1;
const int creditCom = A2;

//LEDs
const int ledPin = 8;
//configurable stuff
int LEDUpdateRate = 30; //value in ms
const int ledsPerStrip = 180;
long lastLEDUpdate = 0;
Adafruit_NeoPixel strip = Adafruit_NeoPixel(ledsPerStrip, ledPin, NEO_GRB + NEO_KHZ800);
int curPattern = 0;
int frameCount = 0;
//animation frame lengths
int beginLength = 180;
int flashLength = 15;
/*
  0 : default
  1 : start game
  2 : hit
*/

//Serial
String inString = "";    // string to hold input
int incomingByte = 0;   // for incoming serial data

bool billQueue = false; //if a dollar has been received but not acknowledged from computer yet
long billSent = 0; // time that $ was sent
long billTimeout = 200; //time to wait to get confirmation before trying again


/*
  serial messages:
  send 'x' for a hit
  send 's' for game begin
  send 'e' for game end
*/

//animation values
int idlePos = 0;

bool billState = HIGH;
long lastBill = 0;
int billDebounce = 50;

int count = 0;

void setup() {
  Serial.begin(9600);
  pinMode(enablePin, OUTPUT);
  digitalWrite(enablePin, HIGH);
  pinMode(creditSig, INPUT_PULLUP);
  pinMode(creditCom, OUTPUT);
  digitalWrite(creditCom, LOW);

  pinMode(13, LOW);
  strip.begin();
  strip.show();
}

void loop() {
  receiveSerial(); //listen for data
  checkBill();
  verifyBill();
  ledUpdate();
}


void verifyBill() {
  if (billQueue && millis() - billSent > billTimeout) {
    Serial.println('$');
    billSent = millis();
  }

}

void ledUpdate() {
  if (millis() - lastLEDUpdate > LEDUpdateRate) {
    switch (curPattern) {
      case 0: // attract
        idleAnimation(5, 255, 65, 0);
        break;

      case 1: //start game
        startUp(100, 0, 0);
        break;

      case 2: //hit
        hit(100, 100, 100);
        break;

    }
    lastLEDUpdate = millis();
  }


}

void receiveSerial() {
  while (Serial.available() > 0) {
    int inChar = Serial.read();
    inString += (char)inChar;

    switch (inChar) {
      case '?': // used to identify serial port
        digitalWrite(13, HIGH);
        digitalWrite(enablePin, LOW);
        sendConf();
        break;
      case 's': // start game
        digitalWrite(enablePin, HIGH);
        delay(15);
        curPattern = 1;
        frameCount = 0;
        break;
      case 'e': // end game
        digitalWrite(enablePin, LOW);
        delay(15);
        break;
      case 'x': // hit animation
        curPattern = 2;
        break;
      case '#':
        billQueue = false;
        break;
    }
  }
  inString = "";
}

void checkBill() {
  bool curBill = digitalRead(creditSig);
  if (curBill != billState && curBill == LOW && millis() - lastBill > billDebounce) {
    count++;
    Serial.println("$");
    billQueue = true;
    billSent = millis();
    curPattern = 2;
    lastBill = millis();
  }
  billState = curBill;
}

void idleAnimation(int scanLength, uint8_t r, uint8_t g, uint8_t b) {
  for (int i = 0; i < ledsPerStrip; i++) {
    if (i < idlePos + scanLength && i > idlePos) {
      strip.setPixelColor(i, strip.Color(r, g, b));
    }
    else(strip.setPixelColor(i, 0));
  }
  strip.show();
  idlePos++;
  idlePos = idlePos % ledsPerStrip;
}

void startUp(uint8_t r, uint8_t g, uint8_t b) {
  if (frameCount >= beginLength) {
    fadeOut(r, g, b, 1);
    curPattern = 0;
    return;
  }
  //  int increment = ledsPerStrip / beginLength;
  int increment = 1;
  for (int i = 0; i < ledsPerStrip; i++) {
    if (i < frameCount * increment) {
      strip.setPixelColor(i, strip.Color(r, g, b));
    }
  }
  strip.show();
  frameCount ++;
}


void fadeOut(uint8_t r, uint8_t g, uint8_t b, int increment) {
  int f_increment = increment;
  int red = r;
  int green = g;
  int blue = b;
  for (int k = 0; k < (255 / f_increment); k++) {
    red = red - f_increment;
    if (red < 0) red = 0;
    green = green - f_increment;
    if (green < 0) green = 0;

    blue = blue - f_increment;
    if (blue < 0) blue = 0;

    for (int j = 0; j < 4; j++) {
      if (red < 5 && green < 5 && blue < 5) {
        return;
      }
      for (int i = 0; i < ledsPerStrip; i++) {
        strip.setPixelColor(i, strip.Color(red, green, blue));
      }
      strip.show();
    }
  }
}

//HIT
void hit(uint8_t r, uint8_t g, uint8_t b) {
  for (int i = 0; i < ledsPerStrip; i++) {
    //    for (int j = 0; j < 4; j++) {
    strip.setPixelColor(i, strip.Color(r, g, b));
    //    }
  }
  //  for (int j = 0; j < 4; j++) {
  strip.show();
  //  }

  fadeOut(r, g, b, 3);
  curPattern = 0;
}

void sendConf() {
  long startTime = millis();
  while (millis() - startTime < 1000) {
    Serial.println('!');
    //    delay(20);
  }
}
