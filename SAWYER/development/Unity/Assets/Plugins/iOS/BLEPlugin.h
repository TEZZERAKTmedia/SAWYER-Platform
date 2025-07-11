#ifdef __cplusplus
extern "C" {
#endif

//These signatures must match the C# [DllImport("__Internal")] declarations exactly
void StartBLEScan(void);
void StopBLEScan(void);
void ConnectToPerioheral(const char* uuid);
void WriteToCharacteristic(const char* data);
void RequestWifiList(void);
const char* GetCurrentSSID(void);

#ifdef __cplusplus

}
#endif