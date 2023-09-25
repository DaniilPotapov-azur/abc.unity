﻿using abc.unity.Core;
using UnityEngine;

namespace abc.unity.ExampleMonobehaviour
{
    public class JumpComponent : BaseComponent<IJumpComponent>, IJumpComponent
    {
       [field: SerializeField] public float Force { get; private set; }

        protected override IJumpComponent _component => this;
    }
}