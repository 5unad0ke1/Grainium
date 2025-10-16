using System;
using System.Collections.Generic;
using Unity.Collections;

namespace Grainium
{
    public class PauseSystem
    {
        private readonly HashSet<IPausable> _hashSet = new();

        private bool _isCanRegister = true;

        public event Action OnPaused;
        public event Action OnResumed;
        public void Register(IPausable pausable)
        {
            if (!_isCanRegister)
            {
                throw new InvalidOperationException("Register() cannot be used during Pause() or Resume()");
            }
            if (_hashSet.Contains(pausable))
                return;
            _hashSet.Add(pausable);
        }
        public void Unregister(IPausable pausable)
        {
            if (!_isCanRegister)
            {
                throw new InvalidOperationException("Unregister() cannot be used during Pause() or Resume()");
            }

            _hashSet.Remove(pausable);
        }
        public void Pause()
        {
            _isCanRegister = false;

            foreach (var pausable in _hashSet)
            {
                pausable.OnPause();
            }
            OnPaused?.Invoke();
            _isCanRegister = true;
        }
        public void Resume()
        {
            _isCanRegister = false;
            foreach (var pausable in _hashSet)
            {
                pausable.OnResume();
            }
            OnResumed?.Invoke();
            _isCanRegister = true;
        }
        public void Clear()
        {
            _hashSet.Clear();
        }
    }
    public interface IPausable
    {
        void OnPause();
        void OnResume();
    }
}