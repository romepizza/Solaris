using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public bool m_test;
    [Header("------- Settings Base -------")]
    public List<Zone> m_subZones;


    [Header("--- Editor Base ---")]
    public bool m_drawGizmos;
    public bool m_drawGizmosSub;
    public bool m_calculateStuff;

    [Header("------- Debug Base -------")]
    public float m_volumeTotal;
    public float m_volumeSingle;

    public bool m_isDrawGizmosSub;
    public Vector3 m_randomVector;


    // points
    public Vector3 getRandomPointSub()
    {
        if (m_subZones == null || m_subZones.Count == 0)
            return getRandomPoint();

        Vector3 randomPoint = Vector3.zero;

        float[] volumes = new float[m_subZones.Count + 1];
        volumes[volumes.Length - 1] = calculateVolumeSingle();
        float totalVolume = volumes[volumes.Length - 1];
        for(int i = 0; i < m_subZones.Count; i++)
        {
            if (m_subZones[i] == null)
                continue;

            float volume = m_subZones[i].calculateVolumeTotal();
            volumes[i] = volume;
            totalVolume += volume;
        }

        float randomValue = Random.Range(0, totalVolume);
        float done = 0;
        for(int i = 0; i < volumes.Length; i++)
        {
            done += volumes[i];
            if(randomValue < done)
            {
                if (i == volumes.Length - 1)
                    randomPoint = getRandomPoint();
                else
                    randomPoint = m_subZones[i].getRandomPointSub();
                break;
            }
        }

        return randomPoint;
    }
    virtual public Vector3 getRandomPoint()
    {
        return Vector3.zero;
    }
    // calculation
     public float calculateVolumeTotal()
    {
        float volume = calculateVolumeSingle();

        for(int i = 0; i < m_subZones.Count; i++)
        {
            if (m_subZones[i] == null)
                continue;

            volume += m_subZones[i].calculateVolumeTotal();
        }

        m_volumeTotal = volume;
        return volume;
    }
    virtual public float calculateVolumeSingle()
    {
        m_volumeSingle = 0;
        return 0;
    }


    // Gizmos
    void OnDrawGizmos()
    {
        setDrawGizmosSub(m_drawGizmosSub);

        if(m_calculateStuff)
        {
            calculateVolumeTotal();
            m_calculateStuff = false;
        }

        if(m_test)
        {
            m_randomVector = getRandomPointSub();
            m_test = false;
        }
        //Gizmos.color = Color.yellow;
       // Gizmos.DrawWireSphere(m_randomVector, 0.5f);
    }
    public void setDrawGizmosSub(bool value)
    {
        m_isDrawGizmosSub = value;
        for (int i = 0; i < m_subZones.Count; i++)
        {
            m_subZones[i].setDrawGizmosSub(value);
        }
    }
}
