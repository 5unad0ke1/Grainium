using UnityEngine;

namespace Grainium
{
    [System.Serializable]
    public sealed class InterfacePointer<T> where T : class
    {
        [SerializeField] private Object _object;

        private T _value;
        public T Value => _value;
        public void Initialize()
        {
            if (!typeof(T).IsInterface)
            {
                throw new System.InvalidOperationException($"{typeof(T).Name} is not an interface type.");
            }
            if (_object is T t)
            {
                _value = t;
            }
            else
            {
                throw new System.InvalidOperationException($"{_object?.GetType().Name ?? "null"} does not implement {typeof(T).Name}.");
            }
        }
    }
}