// types/global.d.ts

import { NavigationContainerRef } from '@react-navigation/native';
import { RootStackParamList } from '../App';

declare global {
  var navigationRef: NavigationContainerRef<RootStackParamList> | undefined;
  var ReactBridge: {
    OpenProvisioning: () => void;
  };
}
