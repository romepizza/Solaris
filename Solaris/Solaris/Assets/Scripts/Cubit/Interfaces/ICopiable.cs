﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICopiable
{
    void onCopy(ICopiable baseScript);
}
