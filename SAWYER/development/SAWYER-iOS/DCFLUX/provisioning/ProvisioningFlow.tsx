import React, { useState } from 'react';
import { View, Text, Button, Modal, StyleSheet } from 'react-native';
import BluetoothDeviceList from './BluetoothDeviceList.js';
import WifiNetworkListModal from './WifiNetworkListModal.js';
import WifiPasswordModal from './WifiPasswordModal.js';
import ProvisioningSuccessModal from './ProvisioningSuccessModal.js';
import  ProvisioningManager  from './ProvisioningManager.js';

export const ProvisioningFlow = () => {
  const [selectedDevice, setSelectedDevice] = useState<any>(null);
  const [selectedNetwork, setSelectedNetwork] = useState<string | null>(null);
  const [passwordModalVisible, setPasswordModalVisible] = useState(false);
  const [successModalVisible, setSuccessModalVisible] = useState(false);

  const handleDeviceSelected = async (device: any) => {
    setSelectedDevice(device);
    await ProvisioningManager.connect(device);
  };

  const handleNetworkSelected = (ssid: string) => {
    setSelectedNetwork(ssid);
    setPasswordModalVisible(true);
  };

  const handlePasswordEntered = async (password: string) => {
    setPasswordModalVisible(false);
    await ProvisioningManager.sendWifiCredentials(selectedNetwork!, password);
    setSuccessModalVisible(true);
  };

  const resetProvisioning = () => {
    setSelectedDevice(null);
    setSelectedNetwork(null);
    setSuccessModalVisible(false);
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Provisioning Flow</Text>

      {!selectedDevice ? (
        <BluetoothDeviceList onDeviceSelected={handleDeviceSelected} />
      ) : (
        <WifiNetworkListModal networks={[]} onSSIDSelect={handleNetworkSelected} />

      )}

    <WifiPasswordModal
    ssid={selectedNetwork!}
    visible={passwordModalVisible}
    onClose={() => setPasswordModalVisible(false)}
    onPasswordSubmit={handlePasswordEntered}
    />


    <ProvisioningSuccessModal
    visible={successModalVisible}
    onClose={resetProvisioning}
    onFinish={resetProvisioning}
    />

    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    paddingTop: 60,
    paddingHorizontal: 24,
    backgroundColor: '#fff',
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 20,
  },
});
