using GameWorld;
using UnityEngine;

public class SwitchBakedObjectsTexture : MonoBehaviour
{
    // The target renderer where we will switch materials.
    public MeshRenderer targetRenderer;

    // The list of materials to cycle through.
    public Material[] materials;

    // The Mesh Baker that will do the baking
    public MeshCombinerEntrance meshBaker;

    public void Start()
    {
        // Bake the mesh.
        meshBaker.AddDeleteGameObjects(meshBaker.GetObjectsToCombine().ToArray(), null, true);
        meshBaker.Apply();
    }

    public void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 100), "切换材质"))
        {
            Material mat = targetRenderer.sharedMaterial;
            int materialIdx = -1;
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] == mat)
                {
                    materialIdx = i;
                }
            }
            materialIdx++;
            if (materialIdx >= materials.Length)
                materialIdx = 0;

            if (materialIdx != -1)
            {
                targetRenderer.sharedMaterial = materials[materialIdx];
                Debug.Log("更换材质: " + targetRenderer.sharedMaterial);

                // Update the Mesh Baker combined mesh
                GameObject[] gameObjects = new GameObject[] { targetRenderer.gameObject };
                meshBaker.UpdateGameObjects(gameObjects, false, false, false, false, true, false, false, false, false);

                // 可以使用AddDelteGameObjects 代替 UpdateGameObjects。
                // UpdateGameObjects速度更快，
                // 但如果 material 发生变化，则无法正常工作
                meshBaker.Apply();
            }
        }
    }



}
