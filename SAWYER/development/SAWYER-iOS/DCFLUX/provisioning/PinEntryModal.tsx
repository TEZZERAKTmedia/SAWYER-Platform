import React, {useState} from 'react';
import { View, Text, TextInput, Button, StyleSheet} from 'react-native';

export default function PinEntryModal({ onPinSubmit}: { onPinSubmit: (pin: string) => void}) {
    const [pin, setPin] = useState('');

    return (
        <View style={styles.container}>
            <Text style={styles.label}>Enter PIN (from robot label):</Text>
            <TextInput 
              style={styles.input}
              value={pin}
              onChangeText={setPin}
              maxLength={6}
              placeholder="123456"
            />
            <Button title="Submit Pin" onPress={() => onPinSubmit(pin)} disabled={pin.length < 4} />
        </View>
    )


}

const styles = StyleSheet.create({
    container: { flex: 1, padding: 20, justifyContent: 'center', alignItems: 'center'},
    label: {fontSize: 18, marginBottom: 10},
    input: {borderWidth: 1, padding: 10, marginBottom: 20, fontSize: 16}
})
