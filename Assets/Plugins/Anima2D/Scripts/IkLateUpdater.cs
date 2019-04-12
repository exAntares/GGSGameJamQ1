using System.Collections.Generic;
using UnityEngine;

namespace Anima2D
{
    public class IkLateUpdater : MonoBehaviour
    {
        public List<Ik2D> toUpdate = new List<Ik2D>();

        void LateUpdate()
        {
            for (int i = 0; i < toUpdate.Count; i++)
            {
                toUpdate[i].Ik2dLateUpdate();
            }
        }

        internal void AddLateUpdate(Ik2D ik2D)
        {
            toUpdate.Add(ik2D);
            toUpdate.Sort((x, y) => x.transform.GetSiblingIndex() - y.transform.GetSiblingIndex());
        }
    }
}
