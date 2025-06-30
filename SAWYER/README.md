Hi welocome to the Entire project structure for TECHTONIC-ROBOTICS

Model: SAWYER

Most of the build scripting logic has been created for Unity > iOS 



Architecture Overview


├── Application
│   └── MainUI
├── Builds
│   ├── AR-Mapping
│   ├── Editor
│   └── MainUI
├── development
│   ├── Applion  //Build target for the integrated iOS App (Not finished)
│   ├── Ble  nder//Blender for animation
│   ├── Builds  //This has all builds for modular devlopment
│   ├── docs
│   ├── Rasberry-Pi //SAWYER robotics logic
│   ├── Unit-UI  //handles the App frontend also main hub for openning other unityJECT
│   ├── Unity-AR-Mapp //handles mapping using iOS (LIDAR preferred)ing
│   ├── Unity-Ed  //Will be designed for editting 3d maps 
│   └── web
├── docker
├── Logs
│   ├── Unity-3d-UI-build.log   
│   ├── Unity-AR-Mapping-build.log
│   └── Unity-Editor-build.log
├── run-all-builds.sh
└── scripts
    └── build.sh



UNITY BUILD BUGS


When we build unity > iOS app we are experiencing a change within the directory where we have:

 - Assets/Prefabs 

+ Assets/Resources 

The contents of the Prefab/ folder has been moved to Scenes/



