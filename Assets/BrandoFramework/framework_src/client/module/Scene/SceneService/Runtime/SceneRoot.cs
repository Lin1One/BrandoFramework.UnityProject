using GameWorld;
using System.Collections.Generic;
using UnityEngine;

public class SceneRoot : MonoBehaviour
{
    public bool isCombineMesh;
    public List<int> testLoadCellIndexs = new List<int>();
    SceneService sceneService;
    void Start()
    {
        sceneService = new SceneService();
        sceneService.Init();
        sceneService.OpenMap(1);
        if(isCombineMesh)
            CombineMesh();
    }

    private void OnGUI()
    {
        if(GUI.Button(new Rect(10,10,100,100),"加载地图块"))
        {
            sceneService.CurSceneUnit.UpdateMap_InEditor(testLoadCellIndexs.ToArray());
            //CombineMesh();
        }
    }


    /// <summary>
    /// 合并网格
    /// </summary>
    private void MergeMesh()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();   //获取 所有子物体的网格
        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length]; //新建一个合并组，长度与 meshfilters一致
        for (int i = 0; i < meshFilters.Length; i++)                                  //遍历
        {
            combineInstances[i].mesh = meshFilters[i].sharedMesh;                   //将共享mesh，赋值
            combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix; //本地坐标转矩阵，赋值
        }
        Mesh newMesh = new Mesh();                                  //声明一个新网格对象
        newMesh.CombineMeshes(combineInstances);                    //将combineInstances数组传入函数
        gameObject.AddComponent<MeshFilter>().sharedMesh = newMesh; //给当前空物体，添加网格组件；将合并后的网格，给到自身网格
        //到这里，新模型的网格就已经生成了。运行模式下，可以点击物体的 MeshFilter 进行查看网格

        #region 以下是对新模型做的一些处理：添加材质，关闭所有子物体，添加自转脚本和控制相机的脚本

        //gameObject.AddComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Koala"); //给当前空物体添加渲染组件，给新模型网格上色;
        //foreach (Transform t in transform)                                                              //禁用掉所有子物体
        //{
        //    t.gameObject.SetActive(false);
        //}
        //gameObject.AddComponent<BallRotate>();
        //Camera.main.gameObject.AddComponent<ChinarCamera>().pivot = transform;

        #endregion
    }

    void MergeMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        MeshRenderer[] meshRenderer = GetComponentsInChildren<MeshRenderer>();  //获取自身和所有子物体中所有MeshRenderer组件
        Dictionary<Material, List<CombineInstance>> metConbineInstances = new Dictionary<Material, List<CombineInstance>>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for (int i = 1; i < meshFilters.Length; i++)
        {
            var met = meshRenderer[i].sharedMaterial;
            //获取材质球列表
            if (!metConbineInstances.ContainsKey(met))
            {
                metConbineInstances.Add(met, new List<CombineInstance>());
            }
            var newConbine = new CombineInstance();
            newConbine.mesh = meshFilters[i].sharedMesh;
            newConbine.transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            metConbineInstances[met].Add(newConbine);
        }

        
        //for (int i = 1; i < meshFilters.Length; i++)
        //{
        //    //获取材质球列表
        //    if (!mats.Contains(meshRenderer[i].sharedMaterial))
        //    {
        //        mats.Add(meshRenderer[i].sharedMaterial);
        //    }
        //}
        foreach(var met in metConbineInstances.Keys)
        {
            GameObject newConbineGameObject = new GameObject(met.ToString());
            newConbineGameObject.transform.SetParent(this.transform);
            newConbineGameObject.AddComponent<MeshFilter>();
            newConbineGameObject.AddComponent<MeshRenderer>();
            newConbineGameObject.GetComponent<MeshFilter>().mesh = new Mesh();
            newConbineGameObject.GetComponent<MeshFilter>().mesh.CombineMeshes(metConbineInstances[met].ToArray(), true);//为mesh.CombineMeshes添加一个 false 参数，表示并不是合并为一个网格，而是一个子网格列表
            newConbineGameObject.GetComponent<MeshRenderer>().material = met;          //为合并后的GameObject指定材质
        }


        transform.gameObject.SetActive(true);
    }

    public Transform Combine(Transform root)
    {
          float startTime = Time.realtimeSinceStartup;
  
         // The SkinnedMeshRenderers that will make up a character will be
          // combined into one SkinnedMeshRenderers using one material.
          // This will speed up rendering the resulting character.
          // note:each SkinnedMeshRenderer must share a same material
          List<CombineInstance> combineInstances = new List<CombineInstance>();
          List<Material> materials = new List<Material>();
          Material material = null;
          List<Transform> bones = new List<Transform>();
          Transform[] transforms = root.GetComponentsInChildren<Transform>();
          List<Texture2D> textures = new List<Texture2D>();
          int width = 0;
          int height = 0;
  
          int uvCount = 0;
  
          List<Vector2[]> uvList = new List<Vector2[]>();
  
          foreach (SkinnedMeshRenderer smr in root.GetComponentsInChildren<SkinnedMeshRenderer>())
          {
              if (material == null)
                  material = Instantiate(smr.sharedMaterial) as Material;
              for (int sub = 0; sub<smr.sharedMesh.subMeshCount; sub++)
              {
                  CombineInstance ci = new CombineInstance();
                  ci.mesh = smr.sharedMesh;
                  ci.subMeshIndex = sub;
                  combineInstances.Add(ci);
              }
  
              uvList.Add(smr.sharedMesh.uv);
              uvCount += smr.sharedMesh.uv.Length;
  
              if (smr.material.mainTexture != null)
              {
                  textures.Add(smr.gameObject.GetComponent<Renderer>().material.mainTexture as Texture2D);
                 width += smr.gameObject.GetComponent<Renderer>().material.mainTexture.width;
                  height += smr.gameObject.GetComponent<Renderer>().material.mainTexture.height;
              }
  
              // we need to recollect references to the bones we are using
              foreach (Transform bone in smr.bones)
              {
                  foreach (Transform transform in transforms)
                  {
                     if (transform.name != bone.name) continue;
                     bones.Add(transform);
                      break;
                  }
              }
              Object.Destroy(smr.gameObject);
          }
  
          // Obtain and configure the SkinnedMeshRenderer attached to
          // the character base.
          SkinnedMeshRenderer r = root.gameObject.GetComponent<SkinnedMeshRenderer>();
          if (!r)
              r = root.gameObject.AddComponent<SkinnedMeshRenderer>();
          
          r.sharedMesh = new Mesh();
  
          //only set mergeSubMeshes true will combine meshs into single submesh
          r.sharedMesh.CombineMeshes(combineInstances.ToArray(), true, false);
          r.bones = bones.ToArray();
          r.material = material;
  
          Texture2D skinnedMeshAtlas = new Texture2D(1024, 512);
          Rect[] packingResult = skinnedMeshAtlas.PackTextures(textures.ToArray(), 0);
          Vector2[] atlasUVs = new Vector2[uvCount];
  
          //as combine textures into single texture,so need recalculate uvs
  
          int j = 0;        
          for (int i = 0; i<uvList.Count; i++)
          {
              foreach (Vector2 uv in uvList[i])
             {
                 atlasUVs[j].x = Mathf.Lerp(packingResult[i].xMin, packingResult[i].xMax, uv.x);
                 atlasUVs[j].y = Mathf.Lerp(packingResult[i].yMin, packingResult[i].yMax, uv.y);
                 j++;
             }
         }
         
         r.material.mainTexture = skinnedMeshAtlas;
         r.sharedMesh.uv = atlasUVs;
 
         Debug.Log("combine meshes takes : " + (Time.realtimeSinceStartup - startTime) * 1000 + " ms");
        return root;
     }

    void CombineMesh()
    {
        //获取所有子物体的网格
        MeshFilter[] mfChildren = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[mfChildren.Length];

        //获取所有子物体的渲染器和材质
        MeshRenderer[] mrChildren = GetComponentsInChildren<MeshRenderer>();
        Material[] materials = new Material[mrChildren.Length];

        //生成新的渲染器和网格组件
        MeshRenderer mrSelf = gameObject.AddComponent<MeshRenderer>();
        MeshFilter mfSelf = gameObject.AddComponent<MeshFilter>();

        //合并子纹理
        Texture2D[] textures = new Texture2D[mrChildren.Length];


        List<Material> materialsList = new List<Material>();
        List<Texture2D> texturesList = new List<Texture2D>();
        for (int i = 0; i < mrChildren.Length; i++)
        {
            if (mrChildren[i].transform == transform)
            {
                continue;
            }
            materials[i] = mrChildren[i].sharedMaterial;
            if (materialsList.Contains(mrChildren[i].sharedMaterial))
            {
                textures[i] = textures[i - 1];
                continue;
            }
            materialsList.Add(mrChildren[i].sharedMaterial);
            materials[i] = mrChildren[i].sharedMaterial;
            Texture2D tx = materials[i].GetTexture("_MainTex") as Texture2D;
            if(tx == null)
            {
                Debug.Log(materials[i].ToString() + "_____" + i);
                continue;
            }
            if(texturesList.Contains(tx))
            {
                continue;
            }
            Texture2D tx2D = new Texture2D(tx.width, tx.height, TextureFormat.ARGB32, false);
            tx2D.SetPixels(tx.GetPixels(0, 0, tx.width, tx.height));
            tx2D.Apply();
            textures[i] = tx2D;
            texturesList.Add(tx2D);
        }

        //设置新材质的主纹理
        Texture2D texture = new Texture2D(2048, 2048);
        
        Rect[] rects = texture.PackTextures(texturesList.ToArray(), 5, 2048);

        foreach (var met in materialsList )
        {
            Material sameMaterial = new Material(met.shader);
            sameMaterial.CopyPropertiesFromMaterial(met);
            sameMaterial.SetTexture("_MainTex", texture);
            GameObject newSameMaterialGo = new GameObject(met.ToString());
            //生成新的渲染器和网格组件
            MeshRenderer mr = newSameMaterialGo.AddComponent<MeshRenderer>();
            MeshFilter mf = newSameMaterialGo.AddComponent<MeshFilter>();
            mr.material = sameMaterial;
            //mr.sharedMaterial = sameMaterial;
            newSameMaterialGo.transform.SetParent(transform);

            var indexOfRect = materialsList.IndexOf(met);
            var combines = new List<CombineInstance>();
            for (int i = 0; i < mfChildren.Length; i++)
            {
                if (materials[i] != met)
                    continue;
                Rect rect = rects[indexOfRect];
                Mesh meshCombine = mfChildren[i].mesh;
                Vector2[] uvs = new Vector2[meshCombine.uv.Length];
                //把网格的uv根据贴图的rect刷一遍  
                for (int j = 0; j < uvs.Length; j++)
                {
                    uvs[j].x = rect.x + meshCombine.uv[j].x * rect.width;
                    uvs[j].y = rect.y + meshCombine.uv[j].y * rect.height;
                }
                meshCombine.uv = uvs;
                CombineInstance newconbine = new CombineInstance();
                newconbine.mesh = meshCombine;
                newconbine.transform = mfChildren[i].transform.localToWorldMatrix;
                mfChildren[i].gameObject.SetActive(false);
                combines.Add(newconbine);
            }

            //生成新的网格，赋值给新的网格渲染组件
            Mesh newMesh = new Mesh();
            newMesh.CombineMeshes(combines.ToArray(), true, true);//合并网格  
            mf.mesh = newMesh;
        }

        //生成新的材质
        //Material materialNew = new Material(materials[0].shader);
        //materialNew.CopyPropertiesFromMaterial(materials[4]);
        //mrSelf.sharedMaterial = materialNew;


        //根据纹理合并的信息刷新子网格UV
        //for (int i = 0; i < mfChildren.Length; i++)
        //{
        //    if (mfChildren[i].transform == transform)
        //    {
        //        continue;
        //    }
        //    Rect rect = rects[i];


        //    Mesh meshCombine = mfChildren[i].mesh;
        //    Vector2[] uvs = new Vector2[meshCombine.uv.Length];
        //    //把网格的uv根据贴图的rect刷一遍  
        //    for (int j = 0; j < uvs.Length; j++)
        //    {
        //        uvs[j].x = rect.x + meshCombine.uv[j].x * rect.width;
        //        uvs[j].y = rect.y + meshCombine.uv[j].y * rect.height;
        //    }
        //    meshCombine.uv = uvs;
        //    combine[i].mesh = meshCombine;
        //    combine[i].transform = mfChildren[i].transform.localToWorldMatrix;
        //    mfChildren[i].gameObject.SetActive(false);
        //}

        ////生成新的网格，赋值给新的网格渲染组件
        //Mesh newMesh = new Mesh();
        //newMesh.CombineMeshes(combine, true, true);//合并网格  
        //mfSelf.mesh = newMesh;
    }

    void CombineMesh2()
    {
        //获取所有子物体的网格
        MeshFilter[] mfChildren = GetComponentsInChildren<MeshFilter>();
        //获取所有子物体的渲染器
        MeshRenderer[] mrChildren = GetComponentsInChildren<MeshRenderer>();
        CombineInstance[] combine = new CombineInstance[mfChildren.Length];


        //生成新的渲染器和网格组件
        MeshRenderer mrSelf = gameObject.AddComponent<MeshRenderer>();
        MeshFilter mfSelf = gameObject.AddComponent<MeshFilter>();

        //材质
        Material[] materials = new Material[mrChildren.Length];
        //合并子纹理
        Texture2D[] textures = new Texture2D[mrChildren.Length];
        List<Material> materialsList = new List<Material>();
        List<Texture2D> texturesList = new List<Texture2D>();

        for (int i = 0; i < mrChildren.Length; i++)
        {
            if (mrChildren[i].transform == transform)
            {
                continue;
            }
            materials[i] = mrChildren[i].sharedMaterial;
            if (materialsList.Contains(mrChildren[i].sharedMaterial))
            {
                textures[i] = textures[i - 1];
                continue;
            }
            materialsList.Add(mrChildren[i].sharedMaterial);
            materials[i] = mrChildren[i].sharedMaterial;
            Texture2D tx = materials[i].GetTexture("_MainTex") as Texture2D;
            if (tx == null)
            {
                Debug.Log(materials[i].ToString() + "_____" + i);
                continue;
            }
            if (texturesList.Contains(tx))
            {
                continue;
            }
            Texture2D tx2D = new Texture2D(tx.width, tx.height, TextureFormat.ARGB32, false);
            tx2D.SetPixels(tx.GetPixels(0, 0, tx.width, tx.height));
            tx2D.Apply();
            textures[i] = tx2D;
            texturesList.Add(tx2D);
        }

        //设置新材质的主纹理
        Texture2D texture = new Texture2D(2048, 2048);

        Rect[] rects = texture.PackTextures(texturesList.ToArray(), 5, 2048);

        foreach (var met in materialsList)
        {
            Material sameMaterial = new Material(met.shader);
            sameMaterial.CopyPropertiesFromMaterial(met);
            sameMaterial.SetTexture("_MainTex", texture);
            GameObject newSameMaterialGo = new GameObject(met.ToString());
            //生成新的渲染器和网格组件
            MeshRenderer mr = newSameMaterialGo.AddComponent<MeshRenderer>();
            MeshFilter mf = newSameMaterialGo.AddComponent<MeshFilter>();
            mr.material = sameMaterial;
            //mr.sharedMaterial = sameMaterial;
            newSameMaterialGo.transform.SetParent(transform);

            var indexOfRect = materialsList.IndexOf(met);
            var combines = new List<CombineInstance>();
            for (int i = 0; i < mfChildren.Length; i++)
            {
                if (materials[i] != met)
                    continue;
                Rect rect = rects[indexOfRect];
                Mesh meshCombine = mfChildren[i].mesh;
                Vector2[] uvs = new Vector2[meshCombine.uv.Length];
                //把网格的uv根据贴图的rect刷一遍  
                for (int j = 0; j < uvs.Length; j++)
                {
                    uvs[j].x = rect.x + meshCombine.uv[j].x * rect.width;
                    uvs[j].y = rect.y + meshCombine.uv[j].y * rect.height;
                }
                meshCombine.uv = uvs;
                CombineInstance newconbine = new CombineInstance();
                newconbine.mesh = meshCombine;
                newconbine.transform = mfChildren[i].transform.localToWorldMatrix;
                mfChildren[i].gameObject.SetActive(false);
                combines.Add(newconbine);
            }

            //生成新的网格，赋值给新的网格渲染组件
            Mesh newMesh = new Mesh();
            newMesh.CombineMeshes(combines.ToArray(), true, true);//合并网格  
            mf.mesh = newMesh;
        }

        //生成新的材质
        //Material materialNew = new Material(materials[0].shader);
        //materialNew.CopyPropertiesFromMaterial(materials[4]);
        //mrSelf.sharedMaterial = materialNew;


        //根据纹理合并的信息刷新子网格UV
        //for (int i = 0; i < mfChildren.Length; i++)
        //{
        //    if (mfChildren[i].transform == transform)
        //    {
        //        continue;
        //    }
        //    Rect rect = rects[i];


        //    Mesh meshCombine = mfChildren[i].mesh;
        //    Vector2[] uvs = new Vector2[meshCombine.uv.Length];
        //    //把网格的uv根据贴图的rect刷一遍  
        //    for (int j = 0; j < uvs.Length; j++)
        //    {
        //        uvs[j].x = rect.x + meshCombine.uv[j].x * rect.width;
        //        uvs[j].y = rect.y + meshCombine.uv[j].y * rect.height;
        //    }
        //    meshCombine.uv = uvs;
        //    combine[i].mesh = meshCombine;
        //    combine[i].transform = mfChildren[i].transform.localToWorldMatrix;
        //    mfChildren[i].gameObject.SetActive(false);
        //}

        ////生成新的网格，赋值给新的网格渲染组件
        //Mesh newMesh = new Mesh();
        //newMesh.CombineMeshes(combine, true, true);//合并网格  
        //mfSelf.mesh = newMesh;
    }

}
