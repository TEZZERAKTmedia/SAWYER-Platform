/**
 * @format
 */

import { AppRegistry } from 'react-native';
import App from './App';
import { name as appName } from './app.json';

global.ReactBridge = {
    OpenProvisioning: () => {
        console.log('[ReactBridge] OpenProvisioning called from Unity.');


        if (global.navigationRef?.navigate) {
            global.navigationRef.navigate('Provisioning');
        }
    },
};

AppRegistry.registerComponent(appName, () => App);
