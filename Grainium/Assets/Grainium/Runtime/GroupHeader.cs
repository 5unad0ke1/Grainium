using UnityEngine;

namespace Grainium
{
    public sealed class GroupHeader : MonoBehaviour
    {
        void Start()
        {
            return;
            int count = transform.childCount;
            for (int i = 0; i < count; i++)
            {
                transform.GetChild(i).SetParent(null);
            }
            Destroy(gameObject);
        }
    }
}