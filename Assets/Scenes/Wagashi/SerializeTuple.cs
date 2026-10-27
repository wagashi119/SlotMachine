using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WagashiTool.Types
{
    [System.Serializable]
    public struct SerializeTuple<T1, T2>
    {
        [SerializeField]
        public T1 item1;
        [SerializeField]
        public T2 item2;

        public SerializeTuple(T1 item1, T2 item2)
        {
            this.item1 = item1;
            this.item2 = item2;
        }

        public ValueTuple<T1, T2> ToValueTuple()
        {
            return new ValueTuple<T1, T2>(item1, item2);
        }
    }
}