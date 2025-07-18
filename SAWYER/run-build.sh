#!/bin/bash

# SAWYER Unified Unity iOS Build Script

UNITY_PATH="/Applications/Unity/Hub/Editor/6000.1.4f1/Unity.app/Contents/MacOS/Unity"
PROJECT_PATH="$(cd "$(dirname "${BASH_SOURCE[0]}")/development/Unity" && pwd)"
BUILD_METHOD="BuildConfigRN_Unity.BuildiOSProject"
LOG_FILE="$(dirname "${BASH_SOURCE[0]}")/Logs/unity-ios-build.log"

mkdir -p "$(dirname "$LOG_FILE")"

echo "🚀 Starting iOS build from unified Unity project"
echo "📁 Project Path: $PROJECT_PATH"
echo "🔧 Build Method: $BUILD_METHOD"
echo "📝 Log File: $LOG_FILE"

"$UNITY_PATH" \
  -batchmode -nographics -quit \
  -projectPath "$PROJECT_PATH" \
  -executeMethod "$BUILD_METHOD" \
  -logFile "$LOG_FILE"

EXIT_CODE=$?

if [[ $EXIT_CODE -ne 0 ]]; then
  echo "❌ Build failed (exit $EXIT_CODE)"
  echo "➡️  Check log: $LOG_FILE"
  exit $EXIT_CODE
else
  echo "✅ Build succeeded"
  echo "📦 Xcode project should be in: development/SAWYER-iOS/DCFLUX/ios"
fi

echo "🎉 Build complete!"
echo
echo "📄 Summary of post-build actions:"
grep "✅" "$LOG_FILE" | sed 's/^/   /'

