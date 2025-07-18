import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createStackNavigator } from '@react-navigation/stack';
import MainUnityScreen from './unity/MainUnityScreen.js';
import {ProvisioningFlow} from './provisioning/ProvisioningFlow.js';
import { navigationRef } from './navigationService.js';




export type RootStackParamList = {
  Main: undefined;
  Provisioning: undefined;

}

const Stack = createStackNavigator<RootStackParamList>();

export default function App() {
  return (
    <NavigationContainer ref={navigationRef}>
      <Stack.Navigator initialRouteName="Main">
        <Stack.Screen name="Main" component={MainUnityScreen} options={{ headerShown: false }} />
        <Stack.Screen name="Provisioning" component={ProvisioningFlow} />
      </Stack.Navigator>
    </NavigationContainer>
  );
}
