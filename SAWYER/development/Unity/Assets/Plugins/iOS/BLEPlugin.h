#ifdef __cplusplus
extern "C" {
#endif

// These must exactly match your C# [DllImport("__Internal")]
void StartBLEScan(void);
void StopBLEScan(void);
void ConnectToPeripheral(const char* uuid);
void WriteToCharacteristic(const char* data);
void RequestWiFiList(void);
const char* GetCurrentSSID(void);

#ifdef __cplusplus
}
#endif
