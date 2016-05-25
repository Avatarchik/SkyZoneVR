

void idleAnimation(int scanLength, uint8_t r, uint8_t g, uint8_t b) {
  if (frameCount == 0) {
    off();
  }

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

  if (frameCount == 0) {
    off();
  }

  for (int i = 0; i < ledsPerStrip; i++) {
    strip.setPixelColor(i, 0);

  }


  if (frameCount >= ledsPerStrip) {
    fadeOut(r, g, b, 1);
    curPattern = 3;
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


//HIT and return to game animation
void hit(uint8_t r, uint8_t g, uint8_t b) {
  for (int i = 0; i < ledsPerStrip; i++) {
    strip.setPixelColor(i, 0);
  }

  for (int i = 0; i < ledsPerStrip; i++) {
    //    for (int j = 0; j < 4; j++) {
    strip.setPixelColor(i, strip.Color(r, g, b));
    //    }
  }
  //  for (int j = 0; j < 4; j++) {
  strip.show();
  //  }

  fadeOut(r, g, b, 3);
  curPattern = 3;
}

//HIT and return to idle animation
void billFlash(uint8_t r, uint8_t g, uint8_t b) {
  for (int i = 0; i < ledsPerStrip; i++) {
    strip.setPixelColor(i, 0);
  }

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


void attract(uint8_t r, uint8_t g, uint8_t b) {

  if (frameCount == 0) {
    off();
  }

  if (frameCount < ledsPerStrip) { //populate every other pixel on the way down
    for (int i = 0; i <= frameCount; i++) {
      if (!(i % 2)) {
        strip.setPixelColor(i, strip.Color(r, g, b));
      }
      else {
        strip.setPixelColor(i, 0);
      }
    }
  }
  else if (frameCount >= ledsPerStrip && frameCount < 2 * ledsPerStrip) { //populate every other pixel on the way back up
    for (int i = ledsPerStrip; i <= frameCount; i++) {
      int transId = map(i, ledsPerStrip, ledsPerStrip * 2, ledsPerStrip * 2, ledsPerStrip) - ledsPerStrip - 1;
      if (transId % 2 == 1) {
        strip.setPixelColor(transId, strip.Color(r, g, b));
      }
    }
  }

  else if (frameCount > 2 * ledsPerStrip - 1) { //flash 3 times
    if (frameCount < 2 * ledsPerStrip + flashLength) {
      for (int i = 0; i < ledsPerStrip; i++) {
        strip.setPixelColor(i, 0);
      }
    }

    else if (frameCount < (ledsPerStrip * 2) + 2 * flashLength) {
      for (int i = 0; i < ledsPerStrip; i++) {
        strip.setPixelColor(i, strip.Color(r, g, b));
      }
    }

    else if (frameCount > (ledsPerStrip * 2) + flashLength * 2 && frameCount < (ledsPerStrip * 2) + 3 * flashLength) {
      for (int i = 0; i < ledsPerStrip; i++) {
        strip.setPixelColor(i, 0);
      }
    }
    else if (frameCount < (ledsPerStrip * 2) + 4 * flashLength) {
      for (int i = 0; i < ledsPerStrip; i++) {
        strip.setPixelColor(i, strip.Color(r, g, b));
      }
    }

    else if (frameCount > (ledsPerStrip * 2) + (flashLength * 4) && frameCount < (ledsPerStrip * 2) + 5 * flashLength) {
      for (int i = 0; i < ledsPerStrip; i++) {
        strip.setPixelColor(i, 0);
      }
    }

  }
  strip.show();
  frameCount++;
  if (frameCount >= (2 * ledsPerStrip) + flashLength * 5) {
    frameCount = 0;
  }
}

void off() {
  for (int i = 0; i < ledsPerStrip; i++) {
    strip.setPixelColor(i, 0);
  }
  strip.show();
}
