using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnigiriGenerator : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject onigiriPrefab;
    public List<GameObject> generatedOnigiriList = new List<GameObject>();

    public void GenerateOnigiri()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            //生成するかどうか
            int isCreate = Random.Range(0, 2);

            if (isCreate == 1)
            {
                GameObject onigiri = Instantiate(onigiriPrefab, spawnPoints[i].position, spawnPoints[i].rotation) as GameObject;
                generatedOnigiriList.Add(onigiri);
            }
        }
    }

    public void DestroyAllOnigiri()
    {
        int count = generatedOnigiriList.Count; //長さを保存しないとダメ
        for (int i = 0; i < count; i++)
        {
            GameObject onigiri = generatedOnigiriList[0]; //iでなく0
            generatedOnigiriList.RemoveAt(0); //iでなく0
            Destroy(onigiri);
        }
    }

}
