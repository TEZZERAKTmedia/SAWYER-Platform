import React, { useState } from 'react';
import {
  View,
  Text,
  TextInput,
  Button,
  StyleSheet,
  Modal,
  TouchableWithoutFeedback,
  Keyboard,
} from 'react-native';

interface WifiPasswordModalProps {
  ssid: string;
  visible: boolean;
  onPasswordSubmit: (password: string) => void;
  onClose: () => void;
}

const WifiPasswordModal: React.FC<WifiPasswordModalProps> = ({
  ssid,
  visible,
  onPasswordSubmit,
  onClose,
}) => {
  const [password, setPassword] = useState('');

  const handleSubmit = () => {
    onPasswordSubmit(password);
    setPassword('');
  };

  return (
    <Modal visible={visible} animationType="slide" transparent={true} onRequestClose={onClose}>
      <TouchableWithoutFeedback onPress={Keyboard.dismiss}>
        <View style={styles.overlay}>
          <View style={styles.container}>
            <Text style={styles.title}>Enter password for: {ssid}</Text>
            <TextInput
              style={styles.input}
              value={password}
              onChangeText={setPassword}
              secureTextEntry
              placeholder="Wi-Fi Password"
            />
            <View style={styles.buttonRow}>
              <Button title="Cancel" onPress={onClose} />
              <Button
                title="Connect"
                onPress={handleSubmit}
                disabled={password.length < 4}
              />
            </View>
          </View>
        </View>
      </TouchableWithoutFeedback>
    </Modal>
  );
};

export default WifiPasswordModal;

const styles = StyleSheet.create({
  overlay: {
    flex: 1,
    justifyContent: 'center',
    backgroundColor: 'rgba(0,0,0,0.4)',
    padding: 20,
  },
  container: {
    backgroundColor: '#fff',
    borderRadius: 10,
    padding: 20,
    elevation: 4,
  },
  title: {
    fontSize: 18,
    marginBottom: 15,
    textAlign: 'center',
  },
  input: {
    borderWidth: 1,
    borderColor: '#ccc',
    padding: 10,
    borderRadius: 6,
    marginBottom: 20,
  },
  buttonRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
  },
});
