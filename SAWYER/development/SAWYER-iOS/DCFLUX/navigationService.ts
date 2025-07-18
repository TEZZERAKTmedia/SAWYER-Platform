// navigationService.ts
import { createNavigationContainerRef } from '@react-navigation/native';
import type { RootStackParamList } from './App.js';

export const navigationRef = createNavigationContainerRef<RootStackParamList>();

export function navigate(name: keyof RootStackParamList) {
  if (navigationRef.isReady()) {
    navigationRef.navigate(name);
  }
}
