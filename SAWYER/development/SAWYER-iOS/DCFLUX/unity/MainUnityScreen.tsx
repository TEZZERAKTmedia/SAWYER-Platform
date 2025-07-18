import React from 'react';
import { View, Button, StyleSheet } from 'react-native';
import { useNavigation } from '@react-navigation/native';
import { StackNavigationProp } from '@react-navigation/stack';
import UnityView from 'react-native-unity-view'; 
import { RootStackParamList } from '../App.js';



type MainScreenNavProp = StackNavigationProp<RootStackParamList, 'Main'>;

export default function MainUnityScreen() {
  const navigation = useNavigation<MainScreenNavProp>();

  const goToProvisioning = () => {
    navigation.navigate('Provisioning');
  };

  return (
    <View style={styles.container}>
      <UnityView style={styles.unity} />
      <Button title="Start Provisioning" onPress={goToProvisioning} />
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  unity: {
    flex: 1,
  },
});
