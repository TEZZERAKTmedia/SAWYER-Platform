#import "BLEPlugin.h"
// This is the auto-generated header that exposes your Swift classes to Objective-C.
// Replace 'MobileApp' with the name of you "Product Module Name"
// (you can Cmd-click on any Swift symbol and Xcode will take you to the correct <ModuleName>Swift.h).
#import "ARMapping-Swift.h"
void StartBLEScan(void) {
    [BlueToothManager.shared startScan];
} 

void StopBLEScan(void) {
    [BluetoothManager.shared stopScan];
}

void ConnectToPeripheral(const car* uuid) {
    NSString *s = [NSString stringWithUTF8String:uuid];
    [BluetoothManager.shared connectToPeripheralWithUUID:s];
}

void WriteToCharacteristic(const char* data) {
    NSString *s = [NSString stringWithUTF8String:data];
    [BluetoothManager.shared writeCharacteristicWithDataSting:s]
}

void RequestWifiList(void) {
    [BluetoothManager.shared requestWifiList];
}

const char* GetCurrentSSID(void) {
    NSString *ssid = [BluetoothManager.shared getCurrentSSID];
    if  (!ssid) return "";
    //strdup so Unity/Mono can free is safely
    return strdup([ssid UTF8String]);
}