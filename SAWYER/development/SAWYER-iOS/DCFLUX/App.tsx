import React, { useEffect } from 'react';
import { DeviceEventEmitter } from 'react-native';
import { NavigationContainer } from '@react-navigation/native';
import { createStackNavigator } from '@react-navigation/stack';
import MainUnityScreen from './unity/MainUnityScreen.js';
import { ProvisioningFlow } from './provisioning/ProvisioningFlow.js';
import { navigationRef } from './navigationService.js';

export type RootStackParamList = {
  Main: undefined;
  Provisioning: undefined;
};

const Stack = createStackNavigator<RootStackParamList>();

export default function App() {
  useEffect(() => {
    const sub = DeviceEventEmitter.addListener('OpenProvisioningFlow', () => {
      console.log('[React] Received OpenProvisioningFlow event');
      navigationRef.navigate('Provisioning');
    });
    return () => sub.remove();
  }, []);

  return (
    <NavigationContainer ref={navigationRef}>
      <Stack.Navigator initialRouteName="Main">
        <Stack.Screen name="Main" component={MainUnityScreen} options={{ headerShown: false }} />
        <Stack.Screen name="Provisioning" component={ProvisioningFlow} />
      </Stack.Navigator>
    </NavigationContainer>
  );
}
