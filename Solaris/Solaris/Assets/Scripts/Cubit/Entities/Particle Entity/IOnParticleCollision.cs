using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOnParticleCollision
{
    void onParticleCollision(GameObject other, int otherAffiliation);
}
