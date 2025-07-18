import { BleManager, Device, BleError } from 'react-native-ble-plx';

const manager = new BleManager();
let connectedDevice: Device | null = null;

const ProvisioningManager = {
  scanForDevices(callback: (device: Device) => void) {
    manager.startDeviceScan(null, null, (error: BleError | null, device: Device | null) => {
      if (device && !error) callback(device);
    });
  },

  stopScan() {
    manager.stopDeviceScan();
  },

  async connect(device: Device) {
    connectedDevice = await device.connect();
    await connectedDevice.discoverAllServicesAndCharacteristics();
  },

  async sendPin(pin: string) {
    console.log('Sending PIN to Pi:', pin);
    return true;
  },

  async getWifiNetworks() {
    return ['RobotNet', 'HomeBase', 'Guest_2G'];
  },

  async sendWifiCredentials(ssid: string, password: string) {
    console.log(`Sending SSID: ${ssid} + password`);
    return true;
  },
};

export default ProvisioningManager;
