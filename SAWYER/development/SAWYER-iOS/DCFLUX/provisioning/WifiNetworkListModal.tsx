import React from 'react';
import {
  View,
  Text,
  FlatList,
  TouchableOpacity,
  StyleSheet,
} from 'react-native';

interface WifiNetworkListModalProps {
  networks: string[];
  onSSIDSelect: (ssid: string) => void;
}

export default function WifiNetworkListModal({
  networks,
  onSSIDSelect,
}: WifiNetworkListModalProps) {
  return (
    <View style={styles.container}>
      <Text style={styles.title}>Available Wi-Fi Networks</Text>
      <FlatList
        data={networks}
        keyExtractor={(item) => item}
        renderItem={({ item }) => (
          <TouchableOpacity
            style={styles.item}
            onPress={() => onSSIDSelect(item)}
          >
            <Text>{item}</Text>
          </TouchableOpacity>
        )}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, padding: 20 },
  title: { fontSize: 22, fontWeight: 'bold', marginBottom: 20 },
  item: {
    padding: 12,
    borderBottomWidth: 1,
    borderBottomColor: '#ccc',
  },
});
