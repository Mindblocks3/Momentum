using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mirage.Momentum
{

 
    public class Snapshot<T> where T : ObjectState
    {
        
        internal ushort Id;
        // server time when this snapshot got generated
        public double Time;

        public List<T> ObjectsState = new List<T>();

    }
}