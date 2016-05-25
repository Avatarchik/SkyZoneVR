#include <Adafruit_NeoPixel.h>

//ticket pins
const int notchPin = 3;
const int motorPin = 4;
int ticketQueue = 2;
bool ticketInit = false;

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
bool lastNotchState = true; //false when the notch is detected
long lastNotchTime = 0; //when the last ticket went by
long notchTimeout = 100; //required time between tickets

//animation frame lengths
int flashLength = 15;
int attractIndex = 0;

/*
  0 : default
  1 : start game
  2 : hit
  3 : attract
*/

//Serial
String inString = "";    // string to hold input
int incomingByte = 0;   // for incoming serial data

//outgoing bills
bool billQueue = false; //if a dollar has been received but not acknowledged from computer yet
long billSent = 0; // time that $ was sent
long billSendTimeout = 50; //time to wait to get confirmation before trying again

//incoming bills
long lastReadBill = 0;
long incomingBillDebounce = 10;
long incomingBillTimeout = 300;
int billCount = 0;


/*
  serial messages:
  send 'x' for a hit
  send 's' for game begin
  send 'e' for game end

*/

//animation values
int idlePos = 0;

bool prevBillState = HIGH;
long lastBill = 0;
int billDebounce = 15;


void setup() {
  Serial.begin(9600);
  pinMode(enablePin, OUTPUT);
  digitalWrite(enablePin, HIGH); //disable bill acceptor
  pinMode(creditSig, INPUT_PULLUP);
  pinMode(creditCom, OUTPUT); //common line for bill switch closure, set to gnd
  digitalWrite(creditCom, LOW);
  pinMode(notchPin, INPUT_PULLUP);
  pinMode(motorPin, OUTPUT);
  //  pinMode(13, LOW);
  strip.begin();
  strip.show();
}

void loop() {
  cleanup();
  receiveSerial(); //listen for data
  checkBill();
  if (billCount > 0 && millis() - lastReadBill > incomingBillTimeout && millis() - billSent > billSendTimeout) {
    sendBill();
  }
  ledUpdate();
  //  Serial.print("tickets: ");
  //  Serial.println(ticketQueue);
  if (ticketQueue > 0) {
    spitTickets();
  }
}

void sendConf() {
  long startTime = millis();
  while (millis() - startTime < 1000) {
    Serial.println('!');
    //    delay(20);
  }
}

void spitTickets() {
  digitalWrite(motorPin, HIGH);    //turn on motor
  while (ticketQueue > 0) {
    int notchState = digitalRead(notchPin);
    if ((!notchState && lastNotchState == true && millis() - lastNotchTime > notchTimeout) || ticketInit == false) { //detect notch, rising edge, enough time has passed
      lastNotchState = false;
      lastNotchTime = millis();
      ticketQueue--;

      Serial.println(ticketQueue);
      if (ticketQueue == 0) { //turn off motor
        digitalWrite(motorPin, LOW);
      }
      if (!ticketInit) ticketInit = true;
    }
    else if (notchState) {
      lastNotchState = true;
    }
  }
}


void checkBill() {
  bool curState = digitalRead(creditSig);
  if (prevBillState && !curState && millis() - lastReadBill > incomingBillDebounce) { //last state was high, now it's low, and its happened some time after the last one
    lastReadBill = millis();
    prevBillState = false;
    billCount++;
  }
  else if (curState) {
    prevBillState = true;
  }
  //  curPattern = 2;
}

/*
  void checkBill() {
  //  bool curBill = false; // condition where bill has been accepted
  bool curBill = digitalRead(creditSig);

  //    for (int i = 0; i < 5; i++) {
  //      if (digitalRead(creditSig)) { //check pin multiple times, if reading disagrees
  //        curBill = true;
  //        return;
  //      }
  //    }

  if (curBill != billState && curBill == LOW && millis() - lastBill > billDebounce) {
    billCount++;
    Serial.println("incrementing");
    //    Serial.println("$");

    digitalWrite(enablePin, HIGH);
    //    billQueue = true;
    billSent = millis();
    curPattern = 2;
    lastBill = millis();
  }
  billState = curBill;
  }
*/


void sendBill() {
  Serial.print(billCount);
  Serial.println("$");
  billSent = millis();
  digitalWrite(enablePin, HIGH);
}

void ledUpdate() {
  if (millis() - lastLEDUpdate > LEDUpdateRate) {
    switch (curPattern) {
      case 0: // attract
        //        idleAnimation(20, 255, 65, 0);
        attract(250, 65, 0);
        break;
      case 1: //start game
        startUp(100, 0, 0);
        break;
      case 2: //hit
        hit(100, 100, 100);
        break;
      case 3:
        idleAnimation(20, 255, 65, 0);
        //        attract(100, 0, 0);
        break;
      case 4:
        billFlash(0, 150, 0);
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
        inString = "";
        break;
      case 's': // start game
        digitalWrite(enablePin, HIGH);
        delay(15);
        curPattern = 1;
        frameCount = 0;
        inString = "";
        break;
      case 'e': // end game
        digitalWrite(enablePin, LOW);
        delay(15);
        curPattern = 0;
        inString = "";
        break;
      case 'x': // hit animation
        curPattern = 2;
        inString = "";
        break;
      case '#':
        {
          int returnedValue = inString.toInt();
          billCount = billCount - returnedValue;
          //        billQueue = false;
          if (billCount == 0) {
            digitalWrite(enablePin, LOW);
          }
          delay(15);
          inString = "";
          curPattern = 4;
          break;
        }
      case 'a':
        frameCount = 0;
        curPattern = 3;
        inString = "";
        break;
      case 'b':
        curPattern = 4;
        inString = "";
        break;
      case 't':
        //        Serial.println("tickets");
        ticketQueue = inString.toInt();
        inString = "";
        break;
    }
  }
}



/*
  void checkBill() {
  bool curBill = digitalRead(creditSig);
  if (curBill != billState && curBill == LOW && millis() - lastBill > billDebounce) {
    count++;
    Serial.println("$");
    digitalWrite(enablePin, HIGH);

    billQueue = true;
    billSent = millis();
    curPattern = 4;
    lastBill = millis();
  }
  billState = curBill;
  }
*/

void cleanup() {
  if (billCount < 0) billCount = 0;
}

