import React, { useState, useEffect } from 'react';
import  { View, Text, FlatList, TouchableOpacity, StyleSheet} from 'react-native';
import ProvisioningManager from './ProvisioningManager.js';



export default function BluetoothDeviceList({onDeviceSelected}: {onDeviceSelected: (device: any) => void}) {
    const [devices, setDevices] = useState<any[]>([]);

    useEffect(() => {
        ProvisioningManager.scanForDevices((device: any) => {
            setDevices((prev) => (prev.find(d => d.id === device.id) ? prev : [...prev, device]));
        });
        return () => ProvisioningManager.stopScan();
    }, []);

    return (
        <View style={styles.container}>
            <Text style={styles.title}> Select Device</Text>
            <FlatList
              data={devices}
              keyExtractor={item => item.id}
              renderItem={({ item }) => (
                <TouchableOpacity onPress={() => onDeviceSelected(item)} style={styles.item}>
                    <Text>{item.name || item.id}</Text>
                </TouchableOpacity>
              )}
             />
        </View>
    );
} 


const styles = StyleSheet.create({
    container: { flex: 1, padding: 20},
    title: {fontSize: 22, marginBottom: 20},
    item: {padding: 12, borderBottomWidth: 1, borderBottomColor: '#ccc'},
})
