declare module 'react-native-unity-view' {
    import * as React from 'react';
    import { ViewProps } from 'react-native';
  
    export interface UnityViewProps extends ViewProps {
      onUnityMessage?: (message: string) => void;
    }
  
    const UnityView: React.ComponentType<UnityViewProps>;
  
    export default UnityView;
  }
  