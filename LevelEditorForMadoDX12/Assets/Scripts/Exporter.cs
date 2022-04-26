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
        // ȯ�� ������Ʈ���� ��� ã�Ƽ� ������Ʈ�� �̸�, ����ϴ� �޽�, ����ϴ� ���׸��� �̸��� ����
        // ����ϴ� �޽��� �̸��� 3ds max������ �̸����� ���ͼ�(== 3ds max���� export�� �� ���� �̸��� �޶�)
        // import�� ���� ������Ʈ�� �̸����� �ؾ� �� ��

        // �� �� ��ȸ�ϸ鼭 material list�� ����� �ߺ� ���� �־�� �ڿ� �� list�� export.
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

            // ���ص� ������Ʈ�� ���� ���
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

            // ���ص� ���׸������ ���� ���
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

        // �浹ü�� ���� ��.
        // �浹ü�� centerPos, extents, orientation(rotation quaternion)�� ���� ��.
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

        // ���̶� �������� ��� �� ������?
        // ���߿� ����

        // ���� ���� ��
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
                        // SpotLight�� �ʿ��� ����
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
                        // PointLight�� �ʿ��� ����
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
                        // DirectionalLight�� �ʿ��� ����
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
