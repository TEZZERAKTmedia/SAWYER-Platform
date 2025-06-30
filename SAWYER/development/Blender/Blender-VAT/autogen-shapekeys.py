import bpy
import bmesh
import os
import numpy as np
from mathutils import Vector
from PIL import Image

# --- CONFIG ---
object_name = bpy.context.active_object.name  # Must have object selected
start_frame = bpy.context.scene.frame_start
end_frame = bpy.context.scene.frame_end
output_dir = bpy.path.abspath("//VAT_Output")
os.makedirs(output_dir, exist_ok=True)

print(f"▶ Exporting VAT for: {object_name} | Frames {start_frame}-{end_frame}")

obj = bpy.data.objects[object_name]
frame_count = end_frame - start_frame + 1

# Evaluate first frame to get vertex count
bpy.context.scene.frame_set(start_frame)
depsgraph = bpy.context.evaluated_depsgraph_get()
eval_obj = obj.evaluated_get(depsgraph)
eval_mesh = eval_obj.to_mesh()
vertex_count = len(eval_mesh.vertices)

# --- CREATE POSITION TEXTURE (pixels = vertices x frames) ---
width = vertex_count
height = frame_count
position_data = np.zeros((height, width, 3), dtype=np.float32)

for frame in range(start_frame, end_frame + 1):
    bpy.context.scene.frame_set(frame)
    eval_obj = obj.evaluated_get(depsgraph)
    eval_mesh = eval_obj.to_mesh()
    
    for i, v in enumerate(eval_mesh.vertices):
        pos = obj.matrix_world @ v.co
        position_data[frame - start_frame, i] = (pos.x, pos.y, pos.z)

    obj.to_mesh_clear()

# Normalize and convert to 8-bit image
min_val = np.min(position_data)
max_val = np.max(position_data)
norm_data = (position_data - min_val) / (max_val - min_val)
image_data = (norm_data * 255).astype(np.uint8)

# Save as PNG
image = Image.fromarray(image_data, 'RGB')
image.save(os.path.join(output_dir, "vat_position.png"))

print("✅ Export complete:", os.path.join(output_dir, "vat_position.png"))

