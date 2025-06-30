This is not a script that is executable from here,
 but can be executed within Blender

This script takes the selected object and all frames, vertex data and 
created a png map

import bpy
import bmesh
import os
import numpy as np
from mathutils import Vector
from PIL import Image

# -- CONFIG --
object_name = bpy.context.active_object.name  # Must have object selected
start_frame = bpy.context.scene.frame_start
end_frame = bpy.context.scene.frame_end
output_dir = bpy.path.abspath("//VAT_Output")
os.makedirs(output_dir, exist_ok=True)

print(f"▶ Exporting VAT for: {object_name} | Frames {start_frame}-{end_frame}")

obj = bpy.data.objects[object_name]
frame_count = end_frame - start_frame + 1

# Evaluate first frame for vertex count reference
bpy.context.scene.frame_set(start_frame)
depsgraph = bpy.context.evaluated_depsgraph_get()
eval_obj = obj.evaluated_get(depsgraph)
eval_mesh = eval_obj.to_mesh()
vertex_count = len(eval_mesh.vertices)
eval_obj.to_mesh_clear()

print(f"🧠 Vertex count: {vertex_count}")

# Allocate animation array
animation_data = np.zeros((frame_count, vertex_count, 3), dtype=np.float32)

# -- BAKE FRAMES --
for i, frame in enumerate(range(start_frame, end_frame + 1)):
    bpy.context.scene.frame_set(frame)
    depsgraph = bpy.context.evaluated_depsgraph_get()
    eval_obj = obj.evaluated_get(depsgraph)
    eval_mesh = eval_obj.to_mesh()

    if len(eval_mesh.vertices) != vertex_count:
        raise Exception(f"❌ Vertex count mismatch at frame {frame}!")

    for v_idx, v in enumerate(eval_mesh.vertices):
        world_co = eval_obj.matrix_world @ v.co
        animation_data[i][v_idx] = (world_co.x, world_co.y, world_co.z)

    eval_obj.to_mesh_clear()

# -- NORMALIZE FOR TEXTURE --
min_val = animation_data.min()
max_val = animation_data.max()
normalized = (animation_data - min_val) / (max_val - min_val)

# Format for image: vertex (Y) x time (X) x 3 channels
img_data = (normalized * 255).astype(np.uint8)
img_data = img_data.transpose(1, 0, 2)  # (vertex, frame, channel)

# Ensure width is multiple of 3
frame_tex_width = img_data.shape[1]
# Convert to flattened float array (R, G, B) interleaved
flattened = img_data.reshape(-1, 3) / 255.0
flat_pixels = np.zeros((flattened.shape[0] * 4), dtype=np.float32)

for i in range(flattened.shape[0]):
    flat_pixels[i * 4 + 0] = flattened[i][0]
    flat_pixels[i * 4 + 1] = flattened[i][1]
    flat_pixels[i * 4 + 2] = flattened[i][2]
    flat_pixels[i * 4 + 3] = 1.0  # Alpha

# Create image
vat_image = bpy.data.images.new(
    name=f"{object_name}_VAT",
    width=frame_tex_width,
    height=vertex_count
)
vat_image.pixels = flat_pixels.tolist()
vat_image.file_format = 'PNG'
vat_image.filepath_raw = os.path.join(output_dir, f"{object_name}_VAT.png")
vat_image.save()

print(f"✅ VAT texture saved as {object_name}_VAT.png")

# -- EXPORT BASE MESH --
bpy.ops.export_scene.obj(
    filepath=os.path.join(output_dir, f"{object_name}_Mesh.obj"),
    use_selection=True,
    use_animation=False,
    use_mesh_modifiers=True,
    use_edges=False,
    use_normals=True,
    use_uvs=True,
    use_materials=False
)

print(f"✅ Base mesh exported as {object_name}_Mesh.obj")
print(f"📁 Output folder: {output_dir}")

