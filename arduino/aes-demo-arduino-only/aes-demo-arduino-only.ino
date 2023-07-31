// The AES Encryption Library
#include "AESLib.h"
#include "arduino_base64.hpp"

// declare a global AESLib object
AESLib aesLib;

void setup() {
    // put your setup code here, to run once:

    // 
    Serial.begin(115200);

    String encryptedText = "";
    String decryptedText = "";
    String url = "";

    // wait for the Serial port to successfully established connection
    // if there is no serial port, this line will be skipped
    while (!Serial)
        ;

    Serial.println();
    Serial.println();
    Serial.println("--- Device Started ---");
    Serial.println();

    String text1 = "Luke, I am your father";
    String text2 = encrypt(text1);
    String text3 = decrypt(text2);

    Serial.println("Original text: \"" + text1 + "\"");
    Serial.println("Encrypted text: \"" + text2 + "\"");
    Serial.println("Decrypted text: \"" + text3 + "\"");

    Serial.println();
    Serial.println("--- The End ---");
}

void loop() {
    // put your main code here, to run repeatedly:
}

// the text encryption function
String encrypt(String inputText) {

    // calculate the length of bytes of the input text
    // an extra of byte must be added for a null character
    // a null character will be filled as a text terminator
    // so that the process will not overflow to other parts of memory    
    int bytesInputLength = inputText.length() + 1;

    // declare an empty byte array (a memory storage)
    byte bytesInput[bytesInputLength];

    // convert the text into bytes, a null char is filled at the end
    inputText.getBytes(bytesInput, bytesInputLength);

    // calculate the length of bytes after encryption done
    int outputLength = aesLib.get_cipher_length(bytesInputLength);

    // declare an empty byte array (a memory storage)
    byte bytesEncrypted[outputLength];

    // declare the KEY and IV
    byte aesKey[] = { 23, 45, 56, 67, 67, 87, 98, 12, 32, 34, 45, 56, 67, 87, 65, 5 };
    byte aesIv[] = { 123, 43, 46, 89, 29, 187, 58, 213, 78, 50, 19, 106, 205, 1, 5, 7 };

    // set the padding mode to paddingMode.CMS
    aesLib.set_paddingmode((paddingMode)0);

    // encrypt the bytes in "bytesInput" and store the output at "bytesEncrypted"
    aesLib.encrypt(bytesInput, bytesInputLength, bytesEncrypted, aesKey, 16, aesIv);

    // declare a empty char array
    char base64EncodedOutput[base64::encodeLength(outputLength)];

    // convert the encrypted bytes into base64 string "base64EncodedOutput"
    base64::encode(bytesEncrypted, outputLength, base64EncodedOutput);

    // convert the encoded base64 char array into string
    return String(base64EncodedOutput);
}

// the decription function
String decrypt(String encryptedBase64Text) {

    // calculate the original length before it was coded into base64 string
    int originalBytesLength = base64::decodeLength(encryptedBase64Text.c_str());

    // declare empty byte array (a memory storage)
    byte encryptedBytes[originalBytesLength];
    byte decryptedBytes[originalBytesLength];

    // convert the base64 string into original bytes
    // which is the encryptedBytes
    base64::decode(encryptedBase64Text.c_str(), encryptedBytes);

    // declare the KEY and IV
    byte aesKey[] = { 23, 45, 56, 67, 67, 87, 98, 12, 32, 34, 45, 56, 67, 87, 65, 5 };
    byte aesIv[] = { 123, 43, 46, 89, 29, 187, 58, 213, 78, 50, 19, 106, 205, 1, 5, 7 };

    // set the padding mode to paddingMode.CMS
    aesLib.set_paddingmode((paddingMode)0);

    // decrypt bytes in "encryptedBytes" and save the output in "decryptedBytes"
    aesLib.decrypt(encryptedBytes, originalBytesLength, decryptedBytes, aesKey, 16, aesIv);

    // convert the decrypted bytes into original string
    String decryptedText = String((char*)decryptedBytes);

    return decryptedText;
}