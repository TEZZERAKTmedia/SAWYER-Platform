# Alembic to Unity (Timeline + Looping) - Step-by-Step Guide

This guide walks you through the **entire process** of exporting an animated Alembic (`.abc`) file from **Blender** and using it in **Unity** via Timeline with full playback and looping. Perfect for high-performance vertex animations that don't rely on bones or shape keys.

---

## ✅ Blender Export Setup

### 1. Prepare Your Animation
- Ensure your object is **fully animated using mesh deformation** (not just modifiers).
- Apply all transforms: `Ctrl + A → All Transforms`.

### 2. Bake Your Animation
- Select the object(s).
- `Object → Animation → Bake Action`
  - ✅ Only Selected
  - ✅ Visual Keying
  - ✅ Clear Parents
  - ✅ Overwrite Current Action
- Bake to keyframes.

### 3. Export as Alembic (.abc)
- `File → Export → Alembic (.abc)`
- Important Export Settings:
  - ✅ **Apply Transform**
  - ✅ **Selected Objects**
  - ✅ **Flatten Hierarchy**
  - ✅ **Only Deform Bones** (optional, if armature-based)
  - ❌ Uncheck 'UVs', 'Normals', etc. unless you need them
  - ✔️ Set frame range appropriately

---

## ✅ Unity Setup (Tested on Unity 6.1)

### 1. Install Unity Alembic Package
- Go to `Window → Package Manager → Unity Registry`
- Install `Alembic` package by Unity Technologies

### 2. Import `.abc` File
- Drag the Alembic `.abc` file into your Unity project
- Unity will auto-generate a `GameObject` and `AlembicStreamPlayer`

### 3. Configure Alembic Import
- Select `.abc` asset
- In the **Inspector**, make sure:
  - Scale Factor = `0.01` (or appropriate for your scene)
  - ✅ Import Meshes
  - ✅ Import Points
  - ✅ Import Curves (if needed)
- Hit **Apply**

---

## ✅ Hook into Timeline

### 1. Create Timeline
- Add an empty GameObject or select the `.abc` GameObject (e.g., `3d_UI`)
- Add a `Playable Director` if not auto-added
- Click `Create` next to Timeline field → Save `New Timeline.asset`

### 2. Create Alembic Track
- Timeline window opens
- Click `Add` → Select `Alembic Track`
- Drag the `.abc` GameObject from Hierarchy into the Alembic Track slot
- Clip appears on the timeline

### 3. Loop the Animation
- Select the clip in the timeline
- In the Inspector:
  - ✅ Enable **Loop**
  - Adjust duration if needed

---

## ✅ Final Tips

- If the clip doesn’t play:
  - Make sure the `Stream Descriptor` is linked in `Alembic Stream Player`
  - Ensure `Playable Director` has `Play On Awake` ✅
  - Use `Game Time` for best results

- If Timeline disappears:
  - Select the Timeline GameObject again
  - Reopen `Timeline` from `Window → Sequencing → Timeline`

---

## 🔁 Why Use Alembic?
- High-performance vertex-level animations
- No bones, no rigging, no blend shape issues
- Great for **3D UI elements**, **fracturing animations**, and **FX passes**

---

Created by [Trentyn Nicholas] – feel free to use and share!
