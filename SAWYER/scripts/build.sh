# scripts/build.sh
#!/bin/bash

case "$1" in
  pi)
    echo "🔧 Building Pi Server..."
    docker build -t sawyer-pi-server ./apps/pi-server
    ;;
  unity)
    echo "🎮 Building Unity AR App..."
    bash ./scripts/build-unity.sh
    ;;
  ios)
    echo "📱 Building iOS App..."
    bash ./scripts/build-ios.sh
    ;;
  all)
    bash $0 pi
    bash $0 unity
    bash $0 ios
    ;;
  *)
    echo "Usage: build.sh [pi|unity|ios|all]"
    exit 1
    ;;
esac

