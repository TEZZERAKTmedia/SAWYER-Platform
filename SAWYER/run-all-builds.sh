#!/bin/bash

# SAWYER Unity Modular Build Script

UNITY_PATH="/Applications/Unity/Hub/Editor/6000.1.4f1/Unity.app/Contents/MacOS/Unity"

declare -a builds=(
  "development/Unity-3d-UI:AutoBuildHookUI.BuildIOSApp"
  "development/Unity-AR-Mapping:AutoBuildHookARMapping.BuildIOSApp"
  "development/Unity-Editor:AutoBuildHookEditor.BuildIOSApp"
)

mkdir -p Logs

for entry in "${builds[@]}"; do 
  IFS=":" read -r project method <<< "$entry"

  if [[ -z "$project" || -z "$method" ]]; then
    echo "⚠️  Skipping invalid entry: '$entry'"
    continue
  fi

  LOG_FILE="Logs/$(basename "$project")-build.log"

  echo "🚀 Building: $project"
  echo "🔧 Method:  $method"
  echo "📝 Log: $LOG_FILE"

  "$UNITY_PATH" \
    -batchmode -nographics -quit \
    -projectPath "$project" \
    -executeMethod "$method" \
    -logFile "$LOG_FILE"

  EXIT_CODE=$?
  if [[ $EXIT_CODE -ne 0 ]]; then
    echo "❌ Build failed for $project (exit $EXIT_CODE)"
    echo "➡️  Check log: $LOG_FILE"
    exit $EXIT_CODE
  else
    echo "✅ Build succeeded for $project"
  fi

  echo "----------------------------------------"
done

echo "🎉 All Unity builds complete!"
