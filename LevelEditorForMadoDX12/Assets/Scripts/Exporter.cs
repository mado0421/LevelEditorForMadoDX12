using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Exporter : MonoBehaviour
{
    void Start()
    {
        Export();
    }

    void Export()
    {
        // 환경 오브젝트들을 모두 찾아서 오브젝트의 이름, 사용하는 메쉬, 사용하는 마테리얼 이름을 저장
        // 사용하는 메쉬의 이름이 3ds max에서의 이름으로 들어와서(== 3ds max에서 export할 때 정한 이름과 달라서)
        // import할 때는 오브젝트의 이름으로 해야 할 듯

        // 이 때 순회하면서 material list를 만들고 중복 없이 넣어둔 뒤에 그 list도 export.
        string textForLogPrint, textForExport;
        List<Material> materials = new List<Material>();


        GameObject env = GameObject.Find("Env");
        {
            Transform[] transforms = env.GetComponentsInChildren<Transform>();

            string path = "Assets/Exports/EnvMeshList";
            StreamWriter sw;
            sw = new StreamWriter(path + ".txt");

            // ObjectCount
            textForExport = "Count: " + (transforms.Length - 1);
            sw.WriteLine(textForExport);

            // 구해둔 오브젝트를 전부 출력
            foreach (Transform child in transforms)
            {
                if (child.name == "Env") continue;  // exclude self

                MeshRenderer mr = child.GetComponent<MeshRenderer>();

                textForLogPrint = child.name + "'s Transform value:" + child.position + ", " + child.rotation;
                textForLogPrint += "\n" + child.name + "'s Material name: " + mr.material.name;

                materials.Add(mr.material);

                textForExport = child.name;
                textForExport += " " + child.position.x + "," + child.position.y + "," + child.position.z;
                textForExport += " " + child.rotation.x + "," + child.rotation.y + "," + child.rotation.z + "," + child.rotation.w;
                textForExport += " " + mr.material.name;
                textForExport = textForExport.Replace(" (Instance)", "");
                textForExport = textForExport.Replace(" #", "#");
                sw.WriteLine(textForExport);

                Debug.Log(textForLogPrint);
            }

            sw.Flush();
            sw.Close();
        }

        {
            string path = "Assets/Exports/MaterialList";
            StreamWriter sw;
            sw = new StreamWriter(path + ".txt");

            // ObjectCount
            textForExport = "Count: " + (materials.Count);
            sw.WriteLine(textForExport);

            // 구해둔 마테리얼들을 전부 출력
            foreach (Material mat in materials)
            {
                textForLogPrint = "Material's name: " + mat.name;
                textForExport = mat.name;

                Texture albedo = mat.GetTexture("_MainTex");
                if (albedo)
                {
                    textForLogPrint += "\n - albedo: " + albedo.name;
                    textForExport += " d " + albedo.name;
                }

                Texture normal = mat.GetTexture("_BumpMap");
                if (normal)
                {
                    textForLogPrint += "\n - normal: " + normal.name;
                    textForExport += " n " + normal.name;
                }

                textForExport = textForExport.Replace(" (Instance)", "");
                textForExport = textForExport.Replace(" #", "#");
                sw.WriteLine(textForExport);

                Debug.Log(textForLogPrint);
            }

            sw.Flush();
            sw.Close();
        }

        // 충돌체도 빼야 함.
        // 충돌체의 centerPos, extents, orientation(rotation quaternion)을 빼야 함.
        GameObject collider = GameObject.Find("Collider");
        {
            string path = "Assets/Exports/ColliderList";
            StreamWriter sw;
            sw = new StreamWriter(path + ".txt");


            BoxCollider[] boxColliders = collider.GetComponentsInChildren<BoxCollider>();

            textForExport = "Count: " + boxColliders.Length;

            sw.WriteLine(textForExport);

            foreach (BoxCollider box in boxColliders)
            {
                if (box.name == "Collider") continue;  // exclude self

                


                textForLogPrint = (box.transform.position + box.center) + ", " + box.size + ", " + box.transform.rotation;
                textForExport = 
                    (box.transform.position + box.center).x + "," + (box.transform.position + box.center).y + "," + (box.transform.position + box.center).z + " " + 
                    box.size.x + "," + box.size.y + "," + box.size.z + " " +
                    box.transform.rotation.x + "," + box.transform.rotation.y + "," + box.transform.rotation.z + "," + box.transform.rotation.w;

                sw.WriteLine(textForExport);

                Debug.Log(textForLogPrint);
            }

            sw.Flush();
            sw.Close();
        }

        // 문이랑 과녁판은 어떻게 할 것인지?
        // 나중에 생각

        // 조명도 빼야 함
        GameObject lightObjects = GameObject.Find("Lights");
        {
            string path = "Assets/Exports/LightList";
            StreamWriter sw;
            sw = new StreamWriter(path + ".txt");

            Light[] lights = lightObjects.GetComponentsInChildren<Light>();

            foreach(Light l in lights)
            {
                switch (l.type)
                {
                    case LightType.Spot:
                        // SpotLight가 필요한 정보
                        // { spot color 0.8 0.0 0.0 direction 0 -2 1 position 2 6 -2 falloff 7 9 spotPower 4 shadow true }

                        textForLogPrint = l.type.ToString();
                        textForLogPrint += " " + l.color.r + " " + l.color.g + " " + l.color.b;
                        textForLogPrint += " " + l.transform.forward;
                        textForLogPrint += " " + l.transform.position;
                        textForLogPrint += " " + l.range;
                        textForLogPrint += " " + l.intensity;

                        textForExport = textForLogPrint;

                        sw.WriteLine(textForExport);

                        Debug.Log(textForLogPrint);

                        break;
                    case LightType.Point:
                        // PointLight가 필요한 정보
                        // { point color 0.1 0.1 0.4 position 0 2.5 4 falloff 4 5 shadow true }

                        textForLogPrint = l.type.ToString();
                        textForLogPrint += " " + l.color.r + " " + l.color.g + " " + l.color.b;
                        textForLogPrint += " " + l.transform.position;
                        textForLogPrint += " " + l.range;
                        textForLogPrint += " " + l.intensity;

                        textForExport = textForLogPrint;

                        sw.WriteLine(textForExport);

                        Debug.Log(textForLogPrint);

                        break;
                    case LightType.Directional:
                        // DirectionalLight가 필요한 정보
                        // { dir color 0.65 0.65 0.55 direction - 1 - 2 1 shadow true }

                        textForLogPrint = l.type.ToString();
                        textForLogPrint += " " + l.color.r + " " + l.color.g + " " + l.color.b;
                        textForLogPrint += " " + l.transform.forward;
                        textForLogPrint += " " + l.transform.position;
                        textForLogPrint += " " + l.intensity;

                        textForExport = textForLogPrint;

                        sw.WriteLine(textForExport);

                        Debug.Log(textForLogPrint);

                        break;
                    default: break;
                }
            }

            sw.Flush();
            sw.Close();
        }
    }
}
