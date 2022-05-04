using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BleGadget
{
    internal class BehaviourProxy : MonoBehaviour
    {
        private static BehaviourProxy s_instance;

        private Action updateFunction;
        private Action destroyFunction;

        public static void Initialize(Action update,Action destroy)
        {
            if(s_instance == null)
            {
                var gmo = new GameObject("mabeeProxy");
                GameObject.DontDestroyOnLoad(gmo);
                s_instance = gmo.AddComponent<BehaviourProxy>();
                s_instance.updateFunction = update;
                s_instance.destroyFunction = destroy;
            }
        }

        private void Update()
        {
            if (this.updateFunction != null)
            {
                this.updateFunction();
            }
        }

        void OnDestroy()
        {
            if (this.destroyFunction != null)
            {
                this.destroyFunction();
            }

        }
    }
}
