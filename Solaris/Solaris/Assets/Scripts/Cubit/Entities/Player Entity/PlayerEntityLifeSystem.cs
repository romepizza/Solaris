using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntityLifeSystem : MonoBehaviour
{
    [Header("----- SETTINGS -----")]
    public int m_maxLife;

    [Header("----- DEBUG -----")]
    public int m_currentMaxLife;
    public int m_currentLife;

	// Use this for initialization
	void Start ()
    {
        m_currentMaxLife = m_maxLife;
        reInitializeLife();
	}
	

    // management
    void manageLife()
    {
        if (m_currentLife <= 0)
        {
            die();
        }
        else if (m_currentLife > m_currentMaxLife)
            Debug.Log("Warning: Player has more than currentMaxLife");
    }

    // change life
    public void gainLife(int lifeGain)
    {
        for(int i = 0; i < lifeGain; i++)
        {
            if(m_currentLife < m_currentMaxLife)
            {
                m_currentLife++;
                manageLife();
            }
        }
    }
        
    public void loseHp(int damage)
    {
        for(int i = 0; i < damage; i++)
        {
            m_currentLife--;
            manageLife();
        }
    }

    // die
    void die()
    {

    }

    public void reInitializeLife()
    {
        m_currentLife = m_currentMaxLife;
    }
}
