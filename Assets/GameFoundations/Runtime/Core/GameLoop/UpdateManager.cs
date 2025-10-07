using System;
using System.Collections.Generic;
using Savya.GameFoundations.GameLoop.Tasks;
using UnityEngine;

namespace Savya.GameFoundations.GameLoop
{
    [DefaultExecutionOrder(-32000)]
    public sealed class UpdateManager : MonoBehaviour
    {
        #region Singleton
        private static UpdateManager _instance;
        private static bool _isQuitting = false;

        public static UpdateManager Instance
        {
            get
            {
                if (_isQuitting) return null;

                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<UpdateManager>();
                    if (_instance == null)
                    {
                        var go = new GameObject("UpdateManager");
                        _instance = go.AddComponent<UpdateManager>();
                    }
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }
        #endregion

        #region State
        private readonly List<UpdateTask> _updatables = new();
        private readonly List<FixedUpdateTask> _fixedUpdatables = new();
        private readonly List<LateUpdateTask> _lateUpdatables = new();

        // Per-instance queues (no statics → no cross-scene leaks)
        private readonly List<UpdateTask> _toAddUpdate = new();
        private readonly List<FixedUpdateTask> _toAddFixed = new();
        private readonly List<LateUpdateTask> _toAddLate = new();
        private readonly List<UpdateTask> _toRemoveUpdate = new();
        private readonly List<FixedUpdateTask> _toRemoveFixed = new();
        private readonly List<LateUpdateTask> _toRemoveLate = new();
        // Track registered updatables to prevent duplicates
        private readonly HashSet<IUpdatable> _registeredUpdatables = new();
        private readonly HashSet<IFixedUpdatable> _registeredFixedUpdatables = new();
        private readonly HashSet<ILateUpdatable> _registeredLateUpdatables = new();

        // Reentrancy flags
        private bool _isUpdating, _isFixedUpdating, _isLateUpdating;

        // Optional: priority sorting
        [SerializeField] private bool _sortByPriority = true;
        #endregion

        #region Unity
        private void Awake()
        {
            if (_instance != null && _instance != this) { Destroy(gameObject); return; }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
                _isQuitting = true; 
            }
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }
        
        private void Update()
        {
            FlushAdds(_updatables, _toAddUpdate);
            _isUpdating = true;
            for (int i = 0; i < _updatables.Count; i++)
            {
                var u = _updatables[i];
                if (u == null || u.Updatable == null)
                {
                    var updatable = u?.Updatable;
                    RemoveAtSwapBack(_updatables, i);
                    if (updatable != null)
                    {
                        _registeredUpdatables.Remove(updatable);
                    }
                    i--;
                    continue;
                }
                try { u.Execute(); }
                catch (Exception e) { Debug.LogException(e); }
            }
            _isUpdating = false;
            FlushRemovals(_updatables, _toRemoveUpdate);
        }

        private void FixedUpdate()
        {
            FlushAdds(_fixedUpdatables, _toAddFixed);
            _isFixedUpdating = true;
            for (int i = 0; i < _fixedUpdatables.Count; i++)
            {
                var u = _fixedUpdatables[i];
                if (u == null || u.FixedUpdatable == null)
                {
                    var fixedUpdatable = u?.FixedUpdatable;
                    RemoveAtSwapBack(_fixedUpdatables, i);
                    if (fixedUpdatable != null)
                    {
                        _registeredFixedUpdatables.Remove(fixedUpdatable);
                    }
                    i--;
                    continue;
                }
                try { u.Execute(); }
                catch (Exception e) { Debug.LogException(e); }
            }
            _isFixedUpdating = false;
            FlushRemovals(_fixedUpdatables, _toRemoveFixed);
        }

        private void LateUpdate()
        {
            FlushAdds(_lateUpdatables, _toAddLate);
            _isLateUpdating = true;
            for (int i = 0; i < _lateUpdatables.Count; i++)
            {
                var u = _lateUpdatables[i];
                if (u == null || u.LateUpdatable == null)
                {
                    var lateUpdatable = u?.LateUpdatable;
                    RemoveAtSwapBack(_lateUpdatables, i);
                    if (lateUpdatable != null)
                    {
                        _registeredLateUpdatables.Remove(lateUpdatable);
                    }
                    i--;
                    continue;
                }
                try { u.Execute(); }
                catch (Exception e) { Debug.LogException(e); }
            }
            _isLateUpdating = false;
            FlushRemovals(_lateUpdatables, _toRemoveLate);
        }

        #endregion

        #region Public API
        public void RegisterUpdatable(IUpdatable u, int priority = 0)
        {
            if (u == null) return;
            if (!_registeredUpdatables.Add(u)) return;
            _toAddUpdate.Add(new UpdateTask(u, priority));
        }
        public void UnregisterUpdatable(IUpdatable u)
        {
            if (u == null) return;
            if (!_registeredUpdatables.Remove(u)) return;

            if (_isUpdating)
            {
                var task = _updatables.Find(t => t?.Updatable == u);
                if (task != null && !Contains(_toRemoveUpdate, task))
                {
                    _toRemoveUpdate.Add(task);
                }
            }
            else
            {
                var task = _updatables.Find(t => t?.Updatable == u);
                if (task != null)
                {
                    RemoveNow(_updatables, task);
                }
            }
        }

        public void RegisterFixedUpdatable(IFixedUpdatable u, int priority = 0)
        {
            if (u == null) return;
            var task = new FixedUpdateTask(u, priority);
            if (!_registeredFixedUpdatables.Add(u)) return;
            _toAddFixed.Add(task);
        }

        public void UnregisterFixedUpdatable(IFixedUpdatable u)
        {
            if (u == null) return;
            if (!_registeredFixedUpdatables.Remove(u)) return;
            if (_isFixedUpdating)
            {
                var task = _fixedUpdatables.Find(t => t?.FixedUpdatable == u);
                if (task != null && !Contains(_toRemoveFixed, task))
                {
                    _toRemoveFixed.Add(task);
                }
            }
            else
            {
                var task = _fixedUpdatables.Find(t => t?.FixedUpdatable == u);
                if (task != null)
                {
                    RemoveNow(_fixedUpdatables, task);
                }
            }
        }

        public void RegisterLateUpdatable(ILateUpdatable u, int priority = 0)
        {
            if (u == null) return;
            var task = new LateUpdateTask(u, priority);
            if (!_registeredLateUpdatables.Add(u)) return;
            _toAddLate.Add(task);
        }
        public void UnregisterLateUpdatable(ILateUpdatable u)
        {
            if (u == null) return;
            if (!_registeredLateUpdatables.Remove(u)) return;
            if (_isLateUpdating)
            {
                var task = _lateUpdatables.Find(t => t?.LateUpdatable == u);
                if (task != null && !Contains(_toRemoveLate, task))
                {
                    _toRemoveLate.Add(task);
                }
            }
            else
            {
                var task = _lateUpdatables.Find(t => t?.LateUpdatable == u);
                if (task != null)
                {
                    RemoveNow(_lateUpdatables, task);
                }
            }
        }

        public void ClearAll()
        {
            _updatables.Clear(); _fixedUpdatables.Clear(); _lateUpdatables.Clear();
            _toAddUpdate.Clear(); _toAddFixed.Clear(); _toAddLate.Clear();
            _toRemoveUpdate.Clear(); _toRemoveFixed.Clear(); _toRemoveLate.Clear();
        }
        #endregion

        #region Helpers
        private static void FlushAdds<T>(List<T> target, List<T> toAdd) where T : class
        {
            if (toAdd.Count == 0) return;
            for (int i = 0; i < toAdd.Count; i++)
            {
                var item = toAdd[i];
                if (item != null && !Contains(target, item)) target.Add(item);
            }
            toAdd.Clear();
        }

        private void FlushRemovals<T>(List<T> target, List<T> toRemove) where T : class
        {
            if (toRemove.Count == 0) return;
            for (int i = 0; i < toRemove.Count; i++) RemoveNow(target, toRemove[i]);
            toRemove.Clear();

            if (_sortByPriority)
            {
                // Stable-ish sort by optional Priority property if implemented
                target.Sort((a, b) => GetPriority(b).CompareTo(GetPriority(a))); // high → low
            }
        }

        private static void RemoveNow<T>(List<T> list, T item) where T : class
        {
            int idx = list.IndexOf(item);
            if (idx >= 0) RemoveAtSwapBack(list, idx);
        }

        private static void RemoveAtSwapBack<T>(List<T> list, int index)
        {
            int last = list.Count - 1;
            if (index < 0 || index > last) return;
            list[index] = list[last];
            list.RemoveAt(last);
        }

        private static bool Contains<T>(List<T> a, T item) => a.IndexOf(item) >= 0;
        private static bool Contains<T>(List<T> a, List<T> b, T item) => Contains(a, item) || Contains(b, item);
        private static int GetPriority(object obj)
        {
            // Uses default interface member `Priority` if present; otherwise 0.
            return obj switch
            {
                UpdateTask u => u.Priority,
                FixedUpdateTask fu => fu.Priority,
                LateUpdateTask lu => lu.Priority,
                _ => 0
            };
        }
        #endregion
    }
}