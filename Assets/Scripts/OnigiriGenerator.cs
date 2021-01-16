using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnigiriGenerator : MonoBehaviour
{
    public Transform[] m_SpawnPoints;

    public GameObject m_OnigiriPrefab;

    public  List<GameObject> m_GeneratedOnigiriList = new List<GameObject>();

    public void GenerateOnigiri()
    {
        for(int i = 0;i < m_SpawnPoints.Length;i++)
        {
            int isCreate = Random.Range(0,2);

            if(isCreate == 1)
            {
                GameObject onigiri = Instantiate(m_OnigiriPrefab,m_SpawnPoints[i].position,m_SpawnPoints[i].rotation) as GameObject;

                m_GeneratedOnigiriList.Add(onigiri);

            }
        }

    }

    public void DestroyAllOnigiri()
    {
        int count = m_GeneratedOnigiriList.Count; //長さを保存しないとダメ
        for(int i = 0;i < count;i++)
        {
            GameObject onigiri = m_GeneratedOnigiriList[0]; //iでなく0
            m_GeneratedOnigiriList.RemoveAt(0); //iでなく0
            Destroy(onigiri);
        }
    }

}
