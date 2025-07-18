import React from 'react';
import {
  View,
  Text,
  Button,
  Modal,
  StyleSheet,
  TouchableWithoutFeedback,
  Keyboard,
} from 'react-native';

interface Props {
  visible: boolean;
  onClose: () => void;
  onFinish: () => void;
}

const ProvisioningSuccessModal: React.FC<Props> = ({ visible, onClose, onFinish }) => {
  return (
    <Modal visible={visible} animationType="fade" transparent={true} onRequestClose={onClose}>
      <TouchableWithoutFeedback onPress={Keyboard.dismiss}>
        <View style={styles.overlay}>
          <View style={styles.container}>
            <Text style={styles.title}>🎉 Provisioning Successful!</Text>
            <Button title="Finish" onPress={onFinish} />
          </View>
        </View>
      </TouchableWithoutFeedback>
    </Modal>
  );
};

export default ProvisioningSuccessModal;

const styles = StyleSheet.create({
  overlay: {
    flex: 1,
    backgroundColor: 'rgba(0,0,0,0.4)',
    justifyContent: 'center',
    padding: 20,
  },
  container: {
    backgroundColor: '#fff',
    borderRadius: 10,
    padding: 24,
    alignItems: 'center',
    elevation: 4,
  },
  title: {
    fontSize: 20,
    fontWeight: '600',
    marginBottom: 20,
    textAlign: 'center',
  },
});
