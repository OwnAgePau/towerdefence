using UnityEngine;
using System.Collections.Generic;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

// /////////////////////////////////////////////////////////////////////////////////////////
//                              More Effective Coroutines
//                                        v2.00.0
// 
// This is an improved implementation of coroutines that boasts zero per-frame memory allocations,
// runs about twice as fast as Unity's built in coroutines and has a range of extra features.
// 
// This is the free version. MEC also has a pro version, which can be found here:
// https://www.assetstore.unity3d.com/en/#!/content/68480
// The pro version contains exactly the same core that the free version uses, but also
// contains additional features.
// 
// For manual, support, or upgrade guide visit http://trinary.tech/
//
// Created by Teal Rogers
// Trinary Software
// All rights preserved
// /////////////////////////////////////////////////////////////////////////////////////////

namespace MovementEffects
{
    public class Timing : MonoBehaviour
    {
        public enum DebugInfoType
        {
            None,
            SeperateCoroutines,
            SeperateTags
        }

        /// <summary>
        /// The time between calls to SlowUpdate.
        /// </summary>
        public float TimeBetweenSlowUpdateCalls = 1f / 7f;
        /// <summary>
        /// The amount that each coroutine should be seperated inside the Unity profiler. NOTE: When the profiler window
        /// is not open this value is ignored and all coroutines behave as if "None" is selected.
        /// </summary>
        public DebugInfoType ProfilerDebugAmount = DebugInfoType.SeperateCoroutines;
        /// <summary>
        /// The number of coroutines that are being run in the Update segment.
        /// </summary>
        [Space(12)]
        public int UpdateCoroutines;
        /// <summary>
        /// The number of coroutines that are being run in the FixedUpdate segment.
        /// </summary>
        public int FixedUpdateCoroutines;
        /// <summary>
        /// The number of coroutines that are being run in the LateUpdate segment.
        /// </summary>
        public int LateUpdateCoroutines;
        /// <summary>
        /// The number of coroutines that are being run in the SlowUpdate segment.
        /// </summary>
        public int SlowUpdateCoroutines;

        [HideInInspector]
        public double localTime;
        public static float LocalTime { get { return (float)Instance.localTime; } }
        [HideInInspector]
        public float deltaTime;
        public static float DeltaTime { get { return Instance.deltaTime; } }

        private bool _runningUpdate;
        private bool _runningLateUpdate;
        private bool _runningFixedUpdate;
        private bool _runningSlowUpdate;
        private int _nextUpdateProcessSlot;
        private int _nextLateUpdateProcessSlot;
        private int _nextFixedUpdateProcessSlot;
        private int _nextSlowUpdateProcessSlot;
        private double _lastUpdateTime;
        private double _lastLateUpdateTime;
        private double _lastFixedUpdateTime;
        private double _lastSlowUpdateTime;
        private ushort _framesSinceUpdate;
        private ushort _expansions = 1;

        private const ushort FramesUntilMaintenance = 64;
        private const int ProcessArrayChunkSize = 64;
        private const int InitialBufferSizeLarge = 256;
        private const int InitialBufferSizeMedium = 64;
        private const int InitialBufferSizeSmall = 8;

        public System.Action<System.Exception> OnError;
        public static System.Func<IEnumerator<float>, Timing, CoroutineHandle, IEnumerator<float>> ReplacementFunction;
        private readonly CoroutineHandle _nullHandle = new CoroutineHandle();
        private readonly Dictionary<CoroutineHandle, WaitingProcess> _waitingTriggers = new Dictionary<CoroutineHandle, WaitingProcess>();
        private readonly Dictionary<CoroutineHandle, WaitingProcess> _pausedProcessBuckets = new Dictionary<CoroutineHandle, WaitingProcess>();
        private readonly Queue<System.Exception> _exceptions = new Queue<System.Exception>();
        private readonly Dictionary<CoroutineHandle, ProcessIndex> _handleToIndex = new Dictionary<CoroutineHandle, ProcessIndex>();
        private readonly Dictionary<ProcessIndex, CoroutineHandle> _indexToHandle = new Dictionary<ProcessIndex, CoroutineHandle>();
        private readonly Dictionary<ProcessIndex, string> _processTags = new Dictionary<ProcessIndex, string>();
        private readonly Dictionary<string, HashSet<ProcessIndex>> _taggedProcesses = new Dictionary<string, HashSet<ProcessIndex>>();

        private IEnumerator<float>[] UpdateProcesses = new IEnumerator<float>[InitialBufferSizeLarge];
        private IEnumerator<float>[] LateUpdateProcesses = new IEnumerator<float>[InitialBufferSizeSmall];
        private IEnumerator<float>[] FixedUpdateProcesses = new IEnumerator<float>[InitialBufferSizeMedium];
        private IEnumerator<float>[] SlowUpdateProcesses = new IEnumerator<float>[InitialBufferSizeMedium];

        private static Timing _instance;
        public static Timing Instance
        {
            get
            {
                if (_instance == null || !_instance.gameObject)
                {
                    GameObject instanceHome = GameObject.Find("Movement Effects");
                    System.Type movementType =
                        System.Type.GetType("MovementEffects.Movement, MovementOverTime, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");

                    if(instanceHome == null)
                    {
                        instanceHome = new GameObject { name = "Movement Effects" };
                        DontDestroyOnLoad(instanceHome);

                        if (movementType != null)
                            instanceHome.AddComponent(movementType);

                        _instance = instanceHome.AddComponent<Timing>();
                    }
                    else
                    {
                         if (movementType != null && instanceHome.GetComponent(movementType) == null) 
                            instanceHome.AddComponent(movementType);

                        _instance = instanceHome.GetComponent<Timing>() ?? instanceHome.AddComponent<Timing>();
                    }
                }

                return _instance;
            }

            set { _instance = value; }
        }

        void Awake()
        {
            if(_instance == null)
                _instance = this;
            else
                deltaTime = _instance.deltaTime;
        }

        void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        private void Update()
        {
            if(_lastSlowUpdateTime + TimeBetweenSlowUpdateCalls < Time.realtimeSinceStartup && _nextSlowUpdateProcessSlot > 0)
            {
                ProcessIndex coindex = new ProcessIndex { seg = Segment.SlowUpdate };
                _runningSlowUpdate = true;
                UpdateTimeValues(coindex.seg);

                for (coindex.i = 0; coindex.i < _nextSlowUpdateProcessSlot; coindex.i++)
                {
                    if (SlowUpdateProcesses[coindex.i] != null && !(Time.realtimeSinceStartup < SlowUpdateProcesses[coindex.i].Current))
                    {
                        if (ProfilerDebugAmount != DebugInfoType.None)
                        {
                            Profiler.BeginSample(ProfilerDebugAmount == DebugInfoType.SeperateTags
                                                     ? ("Processing Coroutine (Slow Update)" +
                                                        (_processTags.ContainsKey(coindex) ? ", tag " + _processTags[coindex] : ", no tag"))
                                                     : "Processing Coroutine (Slow Update)");
                        }

                        try
                        {
                            if (!SlowUpdateProcesses[coindex.i].MoveNext())
                            {
                                SlowUpdateProcesses[coindex.i] = null;
                            }
                            else if (SlowUpdateProcesses[coindex.i] != null && float.IsNaN(SlowUpdateProcesses[coindex.i].Current))
                            {
                                if(ReplacementFunction == null)
                                {
                                    SlowUpdateProcesses[coindex.i] = null;
                                }
                                else
                                {
                                    SlowUpdateProcesses[coindex.i] = ReplacementFunction(SlowUpdateProcesses[coindex.i], this, _indexToHandle[coindex]);

                                    ReplacementFunction = null;
                                    coindex.i--;
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            if (OnError == null)
                                _exceptions.Enqueue(ex);
                            else
                                OnError(ex);

                            SlowUpdateProcesses[coindex.i] = null;
                        }

                        if (ProfilerDebugAmount != DebugInfoType.None)
                            Profiler.EndSample();
                    }
                }

                _runningSlowUpdate = false;
            }

            if (_nextUpdateProcessSlot > 0)
            {
                ProcessIndex coindex = new ProcessIndex { seg = Segment.Update };
                _runningUpdate = true;
                UpdateTimeValues(coindex.seg);

                for (coindex.i = 0; coindex.i < _nextUpdateProcessSlot; coindex.i++)
                {
                    if (UpdateProcesses[coindex.i] != null && !(localTime < UpdateProcesses[coindex.i].Current))
                    {
                        if (ProfilerDebugAmount != DebugInfoType.None)
                        {
                            Profiler.BeginSample(ProfilerDebugAmount == DebugInfoType.SeperateTags
                                                     ? ("Processing Coroutine" +
                                                        (_processTags.ContainsKey(coindex) ? ", tag " + _processTags[coindex] : ", no tag"))
                                                     : "Processing Coroutine");
                        }

                        try
                        {
                            if (!UpdateProcesses[coindex.i].MoveNext())
                            {
                                UpdateProcesses[coindex.i] = null;
                            }
                            else if (UpdateProcesses[coindex.i] != null && float.IsNaN(UpdateProcesses[coindex.i].Current))
                            {
                                if(ReplacementFunction == null)
                                {
                                    UpdateProcesses[coindex.i] = null;
                                }
                                else
                                {
                                    UpdateProcesses[coindex.i] = ReplacementFunction(UpdateProcesses[coindex.i], this, _indexToHandle[coindex]);

                                    ReplacementFunction = null;
                                    coindex.i--;
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            if (OnError == null)
                                _exceptions.Enqueue(ex);
                            else
                                OnError(ex);

                            UpdateProcesses[coindex.i] = null;
                        }

                        if (ProfilerDebugAmount != DebugInfoType.None)
                            Profiler.EndSample();
                    }
                }

                _runningUpdate = false;
            }

            if(++_framesSinceUpdate > FramesUntilMaintenance)
            {
                _framesSinceUpdate = 0;

                if (ProfilerDebugAmount != DebugInfoType.None)
                    Profiler.BeginSample("Maintenance Task");

                RemoveUnused();

                if (ProfilerDebugAmount != DebugInfoType.None)
                    Profiler.EndSample();
            }

            if (_exceptions.Count > 0)
                 throw _exceptions.Dequeue();
        }

        private void FixedUpdate()
        {
            if(_nextFixedUpdateProcessSlot > 0)
            {
                ProcessIndex coindex = new ProcessIndex { seg = Segment.FixedUpdate };
                _runningFixedUpdate = true;
                UpdateTimeValues(coindex.seg);

                for (coindex.i = 0; coindex.i < _nextFixedUpdateProcessSlot; coindex.i++)
                {
                    if (FixedUpdateProcesses[coindex.i] != null && !(localTime < FixedUpdateProcesses[coindex.i].Current))
                    {
                        if (ProfilerDebugAmount != DebugInfoType.None)
                        {
                            Profiler.BeginSample(ProfilerDebugAmount == DebugInfoType.SeperateTags
                                                     ? ("Processing Coroutine" +
                                                        (_processTags.ContainsKey(coindex) ? ", tag " + _processTags[coindex] : ", no tag"))
                                                     : "Processing Coroutine");
                        }

                        try
                        {
                            if (!FixedUpdateProcesses[coindex.i].MoveNext())
                            {
                                FixedUpdateProcesses[coindex.i] = null;
                            }
                            else if (FixedUpdateProcesses[coindex.i] != null && float.IsNaN(FixedUpdateProcesses[coindex.i].Current))
                            {
                                if(ReplacementFunction == null)
                                {
                                    FixedUpdateProcesses[coindex.i] = null;
                                }
                                else
                                {
                                    FixedUpdateProcesses[coindex.i] = ReplacementFunction(FixedUpdateProcesses[coindex.i], this, _indexToHandle[coindex]);

                                    ReplacementFunction = null;
                                    coindex.i--;
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            if (OnError == null)
                                _exceptions.Enqueue(ex);
                            else
                                OnError(ex);

                            FixedUpdateProcesses[coindex.i] = null;
                        }

                        if (ProfilerDebugAmount != DebugInfoType.None)
                            Profiler.EndSample();
                    }
                }

                _runningFixedUpdate = false;
            }

            if (_exceptions.Count > 0)
                throw _exceptions.Dequeue();
        }

        private void LateUpdate()
        {
            if(_nextLateUpdateProcessSlot > 0)
            {
                ProcessIndex coindex = new ProcessIndex { seg = Segment.LateUpdate };
                _runningLateUpdate = true;
                UpdateTimeValues(coindex.seg);

                for (coindex.i = 0; coindex.i < _nextLateUpdateProcessSlot; coindex.i++)
                {
                    if (LateUpdateProcesses[coindex.i] != null && !(localTime < LateUpdateProcesses[coindex.i].Current))
                    {
                        if (ProfilerDebugAmount != DebugInfoType.None)
                        {
                            Profiler.BeginSample(ProfilerDebugAmount == DebugInfoType.SeperateTags
                                                     ? ("Processing Coroutine" +
                                                        (_processTags.ContainsKey(coindex) ? ", tag " + _processTags[coindex] : ", no tag"))
                                                     : "Processing Coroutine");
                        }

                        try
                        {
                            if (!LateUpdateProcesses[coindex.i].MoveNext())
                            {
                                LateUpdateProcesses[coindex.i] = null;
                            }
                            else if (LateUpdateProcesses[coindex.i] != null && float.IsNaN(LateUpdateProcesses[coindex.i].Current))
                            {
                                if(ReplacementFunction == null)
                                {
                                    LateUpdateProcesses[coindex.i] = null;
                                }
                                else
                                {
                                    LateUpdateProcesses[coindex.i] = ReplacementFunction(LateUpdateProcesses[coindex.i], this, _indexToHandle[coindex]);

                                    ReplacementFunction = null;
                                    coindex.i--;
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            if (OnError == null)
                                _exceptions.Enqueue(ex);
                            else
                                OnError(ex);

                            LateUpdateProcesses[coindex.i] = null;
                        }

                        if (ProfilerDebugAmount != DebugInfoType.None)
                            Profiler.EndSample();
                    }
                }

                _runningLateUpdate = false;
            }

            if (_exceptions.Count > 0)
                throw _exceptions.Dequeue();
        }

        private void UpdateTimeValues(Segment segment)
        {
            switch(segment)
            {
                case Segment.Update:
                    deltaTime = Time.deltaTime;
                    _lastUpdateTime += deltaTime;
                    localTime = _lastUpdateTime;
                    break;
                case Segment.LateUpdate:
                    deltaTime = Time.deltaTime;
                    _lastLateUpdateTime += deltaTime;
                    localTime = _lastLateUpdateTime;
                    break;
                case Segment.FixedUpdate:
                    deltaTime = Time.deltaTime;
                    _lastFixedUpdateTime += deltaTime;
                    localTime = _lastFixedUpdateTime;
                    break;
                case Segment.SlowUpdate:
                    if(_lastSlowUpdateTime == 0d)
                        deltaTime = TimeBetweenSlowUpdateCalls;
                    else
                        deltaTime = Time.realtimeSinceStartup - (float)_lastSlowUpdateTime;

                    localTime = _lastSlowUpdateTime = Time.realtimeSinceStartup;
                    break;
            }
        }

        private void SetTimeValues(Segment segment)
        {
            switch (segment)
            {
                case Segment.Update:
                    deltaTime = Time.deltaTime;
                    localTime = _lastUpdateTime;
                    break;
                case Segment.LateUpdate:
                    deltaTime = Time.deltaTime;
                    localTime = _lastLateUpdateTime;
                    break;
                case Segment.FixedUpdate:
                    deltaTime = Time.deltaTime;
                    localTime = _lastFixedUpdateTime;
                    break;
                case Segment.SlowUpdate:
                    deltaTime = Time.realtimeSinceStartup - (float)_lastSlowUpdateTime;
                    localTime = _lastSlowUpdateTime = Time.realtimeSinceStartup;
                    break;
            }
        }

        private double GetSegmentTime(Segment segment)
        {
            switch (segment)
            {
                case Segment.Update:
                    return _lastUpdateTime;
                case Segment.LateUpdate:
                    return _lastLateUpdateTime;
                case Segment.FixedUpdate:
                    return _lastFixedUpdateTime;
                case Segment.SlowUpdate:
                    return _lastSlowUpdateTime;
                default:
                    return 0d;
            }
        }
        /// <summary>
        /// Resets the value of LocalTime to zero (only for the Update, LateUpdate, and FixedUpdate segments).
        /// </summary>
        public void ResetTimeCountOnInstance()
        {
            localTime = 0d;

            _lastUpdateTime = 0d;
            _lastLateUpdateTime = 0d;
            _lastFixedUpdateTime = 0d;
        }

        /// <summary>
        /// This will pause all coroutines running on the current MEC instance until ResumeCoroutines is called.
        /// </summary>
        /// <returns>The number of coroutines that were paused.</returns>
        public static int PauseCoroutines()
        {
            return _instance == null ? 0 : _instance.PauseCoroutinesOnInstance();
        }

        /// <summary>
        /// This will pause all coroutines running on this MEC instance until ResumeCoroutinesOnInstance is called.
        /// </summary>
        /// <returns>The number of coroutines that were paused.</returns>
        public int PauseCoroutinesOnInstance()
        {
            enabled = false;

            return _nextUpdateProcessSlot + _nextLateUpdateProcessSlot + _nextFixedUpdateProcessSlot + _nextSlowUpdateProcessSlot;
        }

        /// <summary>
        /// This will pause any matching coroutines running on the current MEC instance until ResumeCoroutines is called.
        /// </summary>
        /// <param name="tag">Any coroutines with a matching tag will be paused.</param>
        /// <returns>The number of coroutines that were paused.</returns>
        public static int PauseCoroutines(string tag)
        {
            return _instance == null ? 0 : _instance.PauseCoroutinesOnInstance(tag);
        }

        /// <summary>
        /// This will pause any matching coroutines running on this MEC instance until ResumeCoroutinesOnInstance is called.
        /// </summary>
        /// <param name="tag">Any coroutines with a matching tag will be paused.</param>
        /// <returns>The number of coroutines that were paused.</returns>
        public int PauseCoroutinesOnInstance(string tag)
        {
            if (tag == null)
                return 0;

            HashSet<ProcessIndex> matches;
            if (!_taggedProcesses.TryGetValue(tag, out matches))
                return 0;

            WaitingProcess pausedProcs;
            if (_waitingTriggers.ContainsKey(_nullHandle))
                pausedProcs = _waitingTriggers[_nullHandle];
            else
                _waitingTriggers.Add(_nullHandle, pausedProcs = new WaitingProcess());

            int count = 0;

            var matchesEnum = matches.GetEnumerator();
            while (matchesEnum.MoveNext() && _taggedProcesses.ContainsKey(tag))
            {
                CoroutineHandle handle;
                _indexToHandle.TryGetValue(matchesEnum.Current, out handle);
                IEnumerator<float> task = CoindexExtract(matchesEnum.Current);

                if (task == null)
                {
                    RemoveTag(matchesEnum.Current);
                    if (_indexToHandle.ContainsKey(matchesEnum.Current))
                    {
                        _handleToIndex.Remove(_indexToHandle[matchesEnum.Current]);
                        _indexToHandle.Remove(matchesEnum.Current);
                    }

                    matchesEnum = matches.GetEnumerator();
                    continue;
                }

                WaitingProcess.ProcessData procData = new WaitingProcess.ProcessData
                {
                    Segment = matchesEnum.Current.seg,
                    Tag = RemoveTag(matchesEnum.Current),
                    Coroutine = task,
                    Handle = handle,
                    PauseTime = task.Current > GetSegmentTime(matchesEnum.Current.seg) ? task.Current - GetSegmentTime(matchesEnum.Current.seg) : 0d
                };

                _handleToIndex.Remove(handle);
                _indexToHandle.Remove(matchesEnum.Current);
                _pausedProcessBuckets.Add(handle, pausedProcs);

                pausedProcs.Tasks.Add(procData);
                matchesEnum = matches.GetEnumerator();
                count++;
            }

            return count;
        }

        /// <summary>
        /// This resumes all coroutines on the current MEC instance if they are currently paused, otherwise it has
        /// no effect.
        /// </summary>
        /// <returns>The number of coroutines that were resumed.</returns>
        public static int ResumeCoroutines()
        {
            return _instance == null ? 0 : _instance.ResumeCoroutinesOnInstance();
        }

        /// <summary>
        /// This resumes all coroutines on this MEC instance if they are currently paused, otherwise it has no effect.
        /// </summary>
        /// <returns>The number of coroutines that were resumed.</returns>
        public int ResumeCoroutinesOnInstance()
        {
            enabled = true;

            int count = _nextUpdateProcessSlot + _nextLateUpdateProcessSlot + _nextFixedUpdateProcessSlot + _nextSlowUpdateProcessSlot;

            if (_waitingTriggers.ContainsKey(_nullHandle))
            {
                var tasksEnum = _waitingTriggers[_nullHandle].Tasks.GetEnumerator();
                while (tasksEnum.MoveNext())
                {
                    if (tasksEnum.Current != null)
                    {
                        RunCoroutineInternal(tasksEnum.Current.PauseTime > 0d ? _InjectDelay(tasksEnum.Current.Coroutine,
                            (float)(GetSegmentTime(tasksEnum.Current.Segment) + tasksEnum.Current.PauseTime)) : tasksEnum.Current.Coroutine,
                            tasksEnum.Current.Segment, tasksEnum.Current.Tag, tasksEnum.Current.Handle);

                        _pausedProcessBuckets.Remove(tasksEnum.Current.Handle);
                        count++;

                    }
                }

                _waitingTriggers.Remove(_nullHandle);
            }

            return count;
        }

        /// <summary>
        /// This resumes any matching coroutines on the current MEC instance if they are currently paused, otherwise it has
        /// no effect.
        /// </summary>
        /// <param name="tag">Any coroutines previously paused with a matching tag will be resumend.</param>
        /// <returns>The number of coroutines that were resumed.</returns>
        public static int ResumeCoroutines(string tag)
        {
            return _instance == null ? 0 : _instance.ResumeCoroutinesOnInstance(tag);
        }

        /// <summary>
        /// This resumes any matching coroutines on this MEC instance if they are currently paused, otherwise it has no effect.
        /// </summary>
        /// <param name="tag">Any coroutines previously paused with a matching tag will be resumend.</param>
        /// <returns>The number of coroutines that were resumed.</returns>
        public int ResumeCoroutinesOnInstance(string tag)
        {
            if (tag == null)
                return 0;
            int count = 0;

            if (_waitingTriggers.ContainsKey(_nullHandle))
            {
                var tasksEnum = _waitingTriggers[_nullHandle].Tasks.GetEnumerator();
                while (tasksEnum.MoveNext())
                {
                    if (tasksEnum.Current == null)
                    {
                        _waitingTriggers[_nullHandle].Tasks.Remove(tasksEnum.Current);
                        tasksEnum = _waitingTriggers[_nullHandle].Tasks.GetEnumerator();
                        continue;
                    }

                    if (tasksEnum.Current.Tag == tag)
                    {
                        RunCoroutineInternal(tasksEnum.Current.PauseTime > 0d ? _InjectDelay(tasksEnum.Current.Coroutine,
                            (float)(GetSegmentTime(tasksEnum.Current.Segment) + tasksEnum.Current.PauseTime)) : tasksEnum.Current.Coroutine,
                            tasksEnum.Current.Segment, tasksEnum.Current.Tag, tasksEnum.Current.Handle);

                        tasksEnum = _waitingTriggers[_nullHandle].Tasks.GetEnumerator();
                        count++;
                    }
                }

                if (_waitingTriggers[_nullHandle].Tasks.Count == 0)
                    _waitingTriggers.Remove(_nullHandle);
            }

            return count;
        }

        private void RemoveUnused()
        {
            var waitProcsEnum = _waitingTriggers.GetEnumerator();
            while (waitProcsEnum.MoveNext())
            {
                if (waitProcsEnum.Current.Value.Tasks.Count == 0)
                {
                    if (_handleToIndex.ContainsKey(waitProcsEnum.Current.Key))
                        CoindexReplace(_handleToIndex[waitProcsEnum.Current.Key], waitProcsEnum.Current.Value.Coroutine);
                    _waitingTriggers.Remove(waitProcsEnum.Current.Key);

                    waitProcsEnum = _waitingTriggers.GetEnumerator();
                    continue;
                }

                if (_handleToIndex.ContainsKey(waitProcsEnum.Current.Key) && CoindexIsNull(_handleToIndex[waitProcsEnum.Current.Key]))
                {
                    CloseWaitingProcess(waitProcsEnum.Current.Key);
                    waitProcsEnum = _waitingTriggers.GetEnumerator();
                }
            }

            ProcessIndex outer, inner;
            outer.seg = inner.seg = Segment.Update;
            for (outer.i = inner.i = 0; outer.i < _nextUpdateProcessSlot; outer.i++)
            {
                if (UpdateProcesses[outer.i] != null)
                {
                    if(outer.i != inner.i)
                    {
                        UpdateProcesses[inner.i] = UpdateProcesses[outer.i];
                        MoveTag(outer, inner);

                        if (_indexToHandle.ContainsKey(inner))
                        {
                            _handleToIndex.Remove(_indexToHandle[inner]);
                            _indexToHandle.Remove(inner);
                        }

                        _handleToIndex[_indexToHandle[outer]] = inner;
                        _indexToHandle.Add(inner, _indexToHandle[outer]);
                        _indexToHandle.Remove(outer);
                    }
                    inner.i++;
                }
            }
            for(outer.i = inner.i;outer.i < _nextUpdateProcessSlot;outer.i++)
            {
                UpdateProcesses[outer.i] = null;
                RemoveTag(outer);

                if (_indexToHandle.ContainsKey(outer))
                {
                    _handleToIndex.Remove(_indexToHandle[outer]);
                    _indexToHandle.Remove(outer);
                }
            }

            UpdateCoroutines = _nextUpdateProcessSlot = inner.i;

            outer.seg = inner.seg = Segment.FixedUpdate;
            for (outer.i = inner.i = 0; outer.i < _nextFixedUpdateProcessSlot; outer.i++)
            {
                if(FixedUpdateProcesses[outer.i] != null)
                {
                    if(outer.i != inner.i)
                    {
                        FixedUpdateProcesses[inner.i] = FixedUpdateProcesses[outer.i];
                        MoveTag(outer, inner);

                        if (_indexToHandle.ContainsKey(inner))
                        {
                            _handleToIndex.Remove(_indexToHandle[inner]);
                            _indexToHandle.Remove(inner);
                        }

                        _handleToIndex[_indexToHandle[outer]] = inner;
                        _indexToHandle.Add(inner, _indexToHandle[outer]);
                        _indexToHandle.Remove(outer);
                    }
                    inner.i++;
                }
            }
            for(outer.i = inner.i;outer.i < _nextFixedUpdateProcessSlot;outer.i++)
            {
                FixedUpdateProcesses[outer.i] = null;
                RemoveTag(outer);

                if (_indexToHandle.ContainsKey(outer))
                {
                    _handleToIndex.Remove(_indexToHandle[outer]);
                    _indexToHandle.Remove(outer);
                }
            }

            FixedUpdateCoroutines = _nextFixedUpdateProcessSlot = inner.i;

            outer.seg = inner.seg = Segment.LateUpdate;
            for (outer.i = inner.i = 0; outer.i < _nextLateUpdateProcessSlot; outer.i++)
            {
                if(LateUpdateProcesses[outer.i] != null)
                {
                    if(outer.i != inner.i)
                    {
                        LateUpdateProcesses[inner.i] = LateUpdateProcesses[outer.i];
                        MoveTag(outer, inner);

                        if (_indexToHandle.ContainsKey(inner))
                        {
                            _handleToIndex.Remove(_indexToHandle[inner]);
                            _indexToHandle.Remove(inner);
                        }

                        _handleToIndex[_indexToHandle[outer]] = inner;
                        _indexToHandle.Add(inner, _indexToHandle[outer]);
                        _indexToHandle.Remove(outer);
                    }
                    inner.i++;
                }
            }
            for(outer.i = inner.i;outer.i < _nextLateUpdateProcessSlot;outer.i++)
            {
                LateUpdateProcesses[outer.i] = null;
                RemoveTag(outer);

                if (_indexToHandle.ContainsKey(outer))
                {
                    _handleToIndex.Remove(_indexToHandle[outer]);
                    _indexToHandle.Remove(outer);
                }
            }

            LateUpdateCoroutines = _nextLateUpdateProcessSlot = inner.i;

            outer.seg = inner.seg = Segment.SlowUpdate;
            for (outer.i = inner.i = 0; outer.i < _nextSlowUpdateProcessSlot; outer.i++)
            {
                if (SlowUpdateProcesses[outer.i] != null)
                {
                    if (outer.i != inner.i)
                    {
                        SlowUpdateProcesses[inner.i] = SlowUpdateProcesses[outer.i];
                        MoveTag(outer, inner);

                        if (_indexToHandle.ContainsKey(inner))
                        {
                            _handleToIndex.Remove(_indexToHandle[inner]);
                            _indexToHandle.Remove(inner);
                        }

                        _handleToIndex[_indexToHandle[outer]] = inner;
                        _indexToHandle.Add(inner, _indexToHandle[outer]);
                        _indexToHandle.Remove(outer);
                    }
                    inner.i++;
                }
            }
            for (outer.i = inner.i; outer.i < _nextSlowUpdateProcessSlot; outer.i++)
            { 
                SlowUpdateProcesses[outer.i] = null;
                RemoveTag(outer);

                if (_indexToHandle.ContainsKey(outer))
                {
                    _handleToIndex.Remove(_indexToHandle[outer]);
                    _indexToHandle.Remove(outer);
                }
            }

            SlowUpdateCoroutines = _nextSlowUpdateProcessSlot = inner.i;
        }

        private void AddTag(string tag, ProcessIndex coindex)
        {
            _processTags.Add(coindex, tag);

            if (_taggedProcesses.ContainsKey(tag))
                _taggedProcesses[tag].Add(coindex);
            else
                _taggedProcesses.Add(tag, new HashSet<ProcessIndex> { coindex });
        }

        private string RemoveTag(ProcessIndex coindex)
        {
            if (_processTags.ContainsKey(coindex))
            {
                string tag = _processTags[coindex];

                if (_taggedProcesses[tag].Count > 1)
                    _taggedProcesses[tag].Remove(coindex);
                else
                    _taggedProcesses.Remove(tag);

                _processTags.Remove(coindex);

                return tag;
            }

            return null;
        }

        private void MoveTag(ProcessIndex coindexFrom, ProcessIndex coindexTo)
        {
            RemoveTag(coindexTo);

            if (_processTags.ContainsKey(coindexFrom))
            {
                _taggedProcesses[_processTags[coindexFrom]].Remove(coindexFrom);
                _taggedProcesses[_processTags[coindexFrom]].Add(coindexTo);

                _processTags.Add(coindexTo, _processTags[coindexFrom]);
                _processTags.Remove(coindexFrom);
            }
        }

        /// <summary>
        /// Run a new coroutine in the Update segment.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public static CoroutineHandle RunCoroutine(IEnumerator<float> coroutine)
        {
            return coroutine == null ? null : Instance.RunCoroutineInternal(coroutine, Segment.Update, null, null);
        }

        /// <summary>
        /// Run a new coroutine in the Update segment.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <param name="tag">An optional tag to attach to the coroutine which can later be used for Kill operations.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public static CoroutineHandle RunCoroutine(IEnumerator<float> coroutine, string tag)
        {
            return coroutine == null ? null : Instance.RunCoroutineInternal(coroutine, Segment.Update, tag, null);
        }

        /// <summary>
        /// Run a new coroutine.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <param name="timing">The segment that the coroutine should run in.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public static CoroutineHandle RunCoroutine(IEnumerator<float> coroutine, Segment timing)
        {
            return coroutine == null ? null : Instance.RunCoroutineInternal(coroutine, timing, null, null);
        }

        /// <summary>
        /// Run a new coroutine.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <param name="timing">The segment that the coroutine should run in.</param>
        /// <param name="tag">An optional tag to attach to the coroutine which can later be used for Kill operations.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public static CoroutineHandle RunCoroutine(IEnumerator<float> coroutine, Segment timing, string tag)
        {
            return coroutine == null ? null : Instance.RunCoroutineInternal(coroutine, timing, tag, null);
        }

        /// <summary>
        /// Run a new coroutine on this Timing instance in the Update segment.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public CoroutineHandle RunCoroutineOnInstance(IEnumerator<float> coroutine) 
        {
            return coroutine == null ? null : RunCoroutineInternal(coroutine, Segment.Update, null, null);
        }

        /// <summary>
        /// Run a new coroutine on this Timing instance in the Update segment.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <param name="tag">An optional tag to attach to the coroutine which can later be used for Kill operations.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public CoroutineHandle RunCoroutineOnInstance(IEnumerator<float> coroutine, string tag)
        {
            return coroutine == null ? null : RunCoroutineInternal(coroutine, Segment.Update, tag, null);
        }

        /// <summary>
        /// Run a new coroutine on this Timing instance.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <param name="timing">The segment that the coroutine should run in.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public CoroutineHandle RunCoroutineOnInstance(IEnumerator<float> coroutine, Segment timing)
        {
            return coroutine == null ? null : RunCoroutineInternal(coroutine, timing, null, null);
        }

        /// <summary>
        /// Run a new coroutine on this Timing instance.
        /// </summary>
        /// <param name="coroutine">The new coroutine's handle.</param>
        /// <param name="timing">The segment that the coroutine should run in.</param>
        /// <param name="tag">An optional tag to attach to the coroutine which can later be used for Kill operations.</param>
        /// <returns>The coroutine's handle, which can be used for Wait and Kill operations.</returns>
        public CoroutineHandle RunCoroutineOnInstance(IEnumerator<float> coroutine, Segment timing, string tag)
        {
            return coroutine == null ? null : RunCoroutineInternal(coroutine, timing, tag, null);
        }

        private CoroutineHandle RunCoroutineInternal(IEnumerator<float> coroutine, Segment timing, string tag, CoroutineHandle handle)
        {
            ProcessIndex slot = new ProcessIndex {seg = timing};
            bool prewarm;
            if (handle == null)
            {
                prewarm = true;
                handle = new CoroutineHandle();
            }
            else
            {
                prewarm = false;

                if (_handleToIndex.ContainsKey(handle))
                {
                    _indexToHandle.Remove(_handleToIndex[handle]);
                    _handleToIndex.Remove(handle);
                }

                if (_pausedProcessBuckets.ContainsKey(handle))
                {
                    var bucketEnum = _pausedProcessBuckets[handle].Tasks.GetEnumerator();
                    while (bucketEnum.MoveNext())
                    {
                        if (bucketEnum.Current.Handle == handle)
                        {
                            _pausedProcessBuckets[handle].Tasks.Remove(bucketEnum.Current);
                            _pausedProcessBuckets.Remove(handle);
                            break;
                        }
                    }
                }
            }

            switch (timing)
            {
                case Segment.Update:

                    if (_nextUpdateProcessSlot >= UpdateProcesses.Length)
                    {
                        IEnumerator<float>[] oldArray = UpdateProcesses;
                        UpdateProcesses = new IEnumerator<float>[UpdateProcesses.Length + (ProcessArrayChunkSize * _expansions++)];
                        for(int i = 0;i < oldArray.Length;i++)
                            UpdateProcesses[i] = oldArray[i];
                    }

                    slot.i = _nextUpdateProcessSlot++;
                    UpdateProcesses[slot.i] = coroutine;

                    if (null != tag)
                        AddTag(tag, slot);

                    _indexToHandle.Add(slot, handle);
                    _handleToIndex.Add(handle, slot);

                    if (!_runningUpdate && prewarm)
                    {
                        try
                        {
                            _runningUpdate = true;
                            SetTimeValues(slot.seg);

                            if (!UpdateProcesses[slot.i].MoveNext())
                            {
                                UpdateProcesses[slot.i] = null;
                            }
                            else if (UpdateProcesses[slot.i] != null && float.IsNaN(UpdateProcesses[slot.i].Current))
                            {
                                if(ReplacementFunction == null)
                                {
                                    UpdateProcesses[slot.i] = null;
                                }
                                else
                                {
                                    UpdateProcesses[slot.i] = ReplacementFunction(UpdateProcesses[slot.i], this, _indexToHandle[slot]);

                                    ReplacementFunction = null;

                                    if (UpdateProcesses[slot.i] != null)
                                        UpdateProcesses[slot.i].MoveNext();
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            if (OnError == null)
                                _exceptions.Enqueue(ex);
                            else
                                OnError(ex);

                            UpdateProcesses[slot.i] = null;
                        }
                        finally
                        {
                            _runningUpdate = false;
                        }
                    }

                    return handle;

                case Segment.FixedUpdate:

                    if (_nextFixedUpdateProcessSlot >= FixedUpdateProcesses.Length)
                    {
                        IEnumerator<float>[] oldArray = FixedUpdateProcesses;
                        FixedUpdateProcesses = new IEnumerator<float>[FixedUpdateProcesses.Length + (ProcessArrayChunkSize * _expansions++)];
                        for(int i = 0;i < oldArray.Length;i++)
                            FixedUpdateProcesses[i] = oldArray[i];
                    }

                    slot.i = _nextFixedUpdateProcessSlot++;
                    FixedUpdateProcesses[slot.i] = coroutine;

                    if (null != tag)
                        AddTag(tag, slot);

                    _indexToHandle.Add(slot, handle);
                    _handleToIndex.Add(handle, slot);

                    if (!_runningFixedUpdate && prewarm)
                    {
                        try
                        {
                            _runningFixedUpdate = true;
                            SetTimeValues(slot.seg);

                            if (!FixedUpdateProcesses[slot.i].MoveNext())
                            {
                                FixedUpdateProcesses[slot.i] = null;
                            }
                            else if (FixedUpdateProcesses[slot.i] != null && float.IsNaN(FixedUpdateProcesses[slot.i].Current))
                            {
                                if(ReplacementFunction == null)
                                {
                                    FixedUpdateProcesses[slot.i] = null;
                                }
                                else
                                {
                                    FixedUpdateProcesses[slot.i] = ReplacementFunction(FixedUpdateProcesses[slot.i], this, _indexToHandle[slot]);

                                    ReplacementFunction = null;

                                    if (FixedUpdateProcesses[slot.i] != null)
                                        FixedUpdateProcesses[slot.i].MoveNext();
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            if (OnError == null)
                                _exceptions.Enqueue(ex);
                            else
                                OnError(ex);

                            FixedUpdateProcesses[slot.i] = null;
                        }
                        finally
                        {
                            _runningFixedUpdate = false;
                        }
                    }

                    return handle;

                case Segment.LateUpdate:

                    if (_nextLateUpdateProcessSlot >= LateUpdateProcesses.Length)
                    {
                        IEnumerator<float>[] oldArray = LateUpdateProcesses;
                        LateUpdateProcesses = new IEnumerator<float>[LateUpdateProcesses.Length + (ProcessArrayChunkSize * _expansions++)];
                        for(int i = 0;i < oldArray.Length;i++)
                            LateUpdateProcesses[i] = oldArray[i];
                    }

                    slot.i = _nextLateUpdateProcessSlot++;
                    LateUpdateProcesses[slot.i] = coroutine;

                    if (tag != null)
                        AddTag(tag, slot);

                    _indexToHandle.Add(slot, handle);
                    _handleToIndex.Add(handle, slot);

                    if(!_runningLateUpdate && prewarm)
                    {
                        try
                        {
                            _runningLateUpdate = true;
                            SetTimeValues(slot.seg);

                            if(!LateUpdateProcesses[slot.i].MoveNext())
                            {
                                LateUpdateProcesses[slot.i] = null;
                            }
                            else if (LateUpdateProcesses[slot.i] != null && float.IsNaN(LateUpdateProcesses[slot.i].Current))
                            {
                                if(ReplacementFunction == null)
                                {
                                    LateUpdateProcesses[slot.i] = null;
                                }
                                else
                                {
                                    LateUpdateProcesses[slot.i] = ReplacementFunction(LateUpdateProcesses[slot.i], this, _indexToHandle[slot]);

                                    ReplacementFunction = null;

                                    if (LateUpdateProcesses[slot.i] != null)
                                        LateUpdateProcesses[slot.i].MoveNext();
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            if (OnError == null)
                                _exceptions.Enqueue(ex);
                            else
                                OnError(ex);

                            LateUpdateProcesses[slot.i] = null;
                        }
                        finally
                        {
                            _runningLateUpdate = false;
                        }
                    }

                    return handle;

                case Segment.SlowUpdate:

                    if (_nextSlowUpdateProcessSlot >= SlowUpdateProcesses.Length)
                    {
                        IEnumerator<float>[] oldArray = SlowUpdateProcesses;
                        SlowUpdateProcesses = new IEnumerator<float>[SlowUpdateProcesses.Length + (ProcessArrayChunkSize * _expansions++)];
                        for(int i = 0;i < oldArray.Length;i++)
                            SlowUpdateProcesses[i] = oldArray[i];
                    }

                    slot.i = _nextSlowUpdateProcessSlot++;
                    SlowUpdateProcesses[slot.i] = coroutine;

                    if (tag != null)
                        AddTag(tag, slot);

                    _indexToHandle.Add(slot, handle);
                    _handleToIndex.Add(handle, slot);

                    if (!_runningSlowUpdate && prewarm)
                    {
                        try
                        {
                            _runningSlowUpdate = true;
                            SetTimeValues(slot.seg);

                            if(!SlowUpdateProcesses[slot.i].MoveNext())
                            {
                                SlowUpdateProcesses[slot.i] = null;
                            }
                            else if (SlowUpdateProcesses[slot.i] != null && float.IsNaN(SlowUpdateProcesses[slot.i].Current))
                            {
                                if(ReplacementFunction == null)
                                {
                                    SlowUpdateProcesses[slot.i] = null;
                                }
                                else
                                {
                                    SlowUpdateProcesses[slot.i] = ReplacementFunction(SlowUpdateProcesses[slot.i], this, _indexToHandle[slot]);

                                    ReplacementFunction = null;

                                    if (SlowUpdateProcesses[slot.i] != null)
                                        SlowUpdateProcesses[slot.i].MoveNext();
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            if (OnError == null)
                                _exceptions.Enqueue(ex);
                            else
                                OnError(ex);

                            SlowUpdateProcesses[slot.i] = null;
                        }
                        finally
                        {
                            _runningSlowUpdate = false;
                        }
                    }

                    return handle;

                default:
                    return null;
            }
        }

        private bool CoindexKill(ProcessIndex coindex)
        {
            bool retVal;

            switch(coindex.seg)
            {
                case Segment.Update:
                    retVal = UpdateProcesses[coindex.i] != null;
                    UpdateProcesses[coindex.i] = null;
                    return retVal;
                case Segment.FixedUpdate:
                    retVal = FixedUpdateProcesses[coindex.i] != null;
                    FixedUpdateProcesses[coindex.i] = null;
                    return retVal;
                case Segment.LateUpdate:
                    retVal = LateUpdateProcesses[coindex.i] != null;
                    LateUpdateProcesses[coindex.i] = null;
                    return retVal;
                case Segment.SlowUpdate:
                    retVal = SlowUpdateProcesses[coindex.i] != null;
                    SlowUpdateProcesses[coindex.i] = null;
                    return retVal;
            }

            return false;
        }

        private
            IEnumerator<float> CoindexExtract(ProcessIndex coindex)
        {
            IEnumerator<float> retVal;

            switch (coindex.seg)
            {
                case Segment.Update:
                    retVal = UpdateProcesses[coindex.i];
                    UpdateProcesses[coindex.i] = null;
                    return retVal;
                case Segment.FixedUpdate:
                    retVal = FixedUpdateProcesses[coindex.i];
                    FixedUpdateProcesses[coindex.i] = null;
                    return retVal;
                case Segment.LateUpdate:
                    retVal = LateUpdateProcesses[coindex.i];
                    LateUpdateProcesses[coindex.i] = null;
                    return retVal;
                case Segment.SlowUpdate:
                    retVal = SlowUpdateProcesses[coindex.i];
                    SlowUpdateProcesses[coindex.i] = null;
                    return retVal;
                default:
                    return null;
            }
        }

        private bool CoindexIsNull(ProcessIndex coindex)
        {
            switch (coindex.seg)
            {
                case Segment.Update:
                    return UpdateProcesses[coindex.i] == null;
                case Segment.FixedUpdate:
                    return FixedUpdateProcesses[coindex.i] == null;
                case Segment.LateUpdate:
                    return LateUpdateProcesses[coindex.i] == null;
                case Segment.SlowUpdate:
                    return SlowUpdateProcesses[coindex.i] == null;
                default:
                    return true;
            }
        }

        private IEnumerator<float> CoindexReplace(ProcessIndex coindex, IEnumerator<float> replacement)
        {
            IEnumerator<float> retVal;

            switch (coindex.seg)
            {
                case Segment.Update:
                    retVal = UpdateProcesses[coindex.i];
                    UpdateProcesses[coindex.i] = replacement;
                    return retVal;
                case Segment.FixedUpdate:
                    retVal = FixedUpdateProcesses[coindex.i];
                    FixedUpdateProcesses[coindex.i] = replacement;
                    return retVal;
                case Segment.LateUpdate:
                    retVal = LateUpdateProcesses[coindex.i];
                    LateUpdateProcesses[coindex.i] = replacement;
                    return retVal;
                case Segment.SlowUpdate:
                    retVal = SlowUpdateProcesses[coindex.i];
                    SlowUpdateProcesses[coindex.i] = replacement;
                    return retVal;
                default:
                    return null;
            }
        }

        private static IEnumerator<float> _InjectDelay(IEnumerator<float> proc, double returnAt)
        {
            yield return (float)returnAt;

            ReplacementFunction = delegate { return proc; };
            yield return float.NaN;
        }

        private static IEnumerator<float> _InjectYield(IEnumerator<float> proc, Timing instance, CoroutineHandle yieldCoroutine, bool warnOnIssue)
        {
            yield return 0f;
            yield return instance.WaitUntilDoneOnInstance(yieldCoroutine, warnOnIssue);

            ReplacementFunction = delegate { return proc; };
            yield return float.NaN;
        }

        /// <summary>
        /// This will kill all coroutines running on the main MEC instance.
        /// </summary>
        /// <returns>The number of coroutines that were killed.</returns>
        public static void KillAllCoroutines()
        {
            if(_instance != null)
                _instance.KillAllCoroutinesOnInstance();
        }

        /// <summary>
        /// This will kill all coroutines running on the current MEC instance.
        /// </summary>
        /// <returns>The number of coroutines that were killed.</returns>
        public void KillAllCoroutinesOnInstance()
        {
            UpdateProcesses = new IEnumerator<float>[InitialBufferSizeLarge];
            UpdateCoroutines = 0;
            _nextUpdateProcessSlot = 0;

            LateUpdateProcesses = new IEnumerator<float>[InitialBufferSizeSmall];
            LateUpdateCoroutines = 0;
            _nextLateUpdateProcessSlot = 0;

            FixedUpdateProcesses = new IEnumerator<float>[InitialBufferSizeMedium];
            FixedUpdateCoroutines = 0;
            _nextFixedUpdateProcessSlot = 0;

            SlowUpdateProcesses = new IEnumerator<float>[InitialBufferSizeMedium];
            SlowUpdateCoroutines = 0;
            _nextSlowUpdateProcessSlot = 0;

            _processTags.Clear();
            _taggedProcesses.Clear();
            _waitingTriggers.Clear();
            _pausedProcessBuckets.Clear();
            _expansions = (ushort)((_expansions / 2) + 1);

            ResetTimeCountOnInstance();
        }

        /// <summary>
        /// Kills all instances of the coroutine handle on the main Timing instance.
        /// </summary>
        /// <param name="handle">The handle of the coroutine to kill.</param>
        /// <returns>The number of coroutines that were found and killed (0 or 1).</returns>
        public static int KillCoroutines(CoroutineHandle handle)
        {
            return _instance == null ? 0 : _instance.KillCoroutinesOnInstance(handle);
        }

        /// <summary>
        /// Kills all instances of the coroutine handle on this Timing instance.
        /// </summary>
        /// <param name="handle">The handle of the coroutine to kill.</param>
        /// <returns>The number of coroutines that were found and killed (0 or 1).</returns>
        public int KillCoroutinesOnInstance(CoroutineHandle handle)
        {
            if (handle == null)
                return 0;

            bool foundOne = false;


            var waitProcsEnum = _waitingTriggers.GetEnumerator();
            while (waitProcsEnum.MoveNext())
            {
                if (waitProcsEnum.Current.Key == handle)
                {
                    CloseWaitingProcess(waitProcsEnum.Current.Key);
                    break;
                }
            }

            if (_handleToIndex.ContainsKey(handle))
            {
                foundOne = CoindexExtract(_handleToIndex[handle]) != null;
                RemoveTag(_handleToIndex[handle]);

                _indexToHandle.Remove(_handleToIndex[handle]);
                _handleToIndex.Remove(handle);
            }

            if (_pausedProcessBuckets.ContainsKey(handle))
            {
                var taskEnum = waitProcsEnum.Current.Value.Tasks.GetEnumerator();
                while (taskEnum.MoveNext())
                {
                    if (taskEnum.Current.Handle == handle)
                    {
                        waitProcsEnum.Current.Value.Tasks.Remove(taskEnum.Current);
                        _pausedProcessBuckets.Remove(handle);
                        foundOne = true;
                        break;
                    }
                }
            }

            return foundOne ? 1 : 0;
        }

        /// <summary>
        /// Kills all coroutines that have the given tag.
        /// </summary>
        /// <param name="tag">All coroutines with this tag will be killed.</param>
        /// <returns>The number of coroutines that were found and killed.</returns>
        public static int KillCoroutines(string tag)
        {
            return _instance == null ? 0 : _instance.KillCoroutinesOnInstance(tag);
        }

        /// <summary> 
        /// Kills all coroutines that have the given tag.
        /// </summary>
        /// <param name="tag">All coroutines with this tag will be killed.</param>
        /// <returns>The number of coroutines that were found and killed.</returns>
        public int KillCoroutinesOnInstance(string tag)
        {
            int numberFound = 0;

            var waitProcsEnum = _waitingTriggers.GetEnumerator();
            while (waitProcsEnum.MoveNext())
            {
                if (_processTags.ContainsKey(_handleToIndex[waitProcsEnum.Current.Key]) &&
                    _processTags[_handleToIndex[waitProcsEnum.Current.Key]] == tag)
                {
                    CloseWaitingProcess(waitProcsEnum.Current.Key);
                    waitProcsEnum = _waitingTriggers.GetEnumerator();
                    continue;
                }

                var taskEnum = waitProcsEnum.Current.Value.Tasks.GetEnumerator();
                while (taskEnum.MoveNext())
                {
                    if (taskEnum.Current.Tag == tag)
                    {
                        _pausedProcessBuckets.Remove(taskEnum.Current.Handle);
                        waitProcsEnum.Current.Value.Tasks.Remove(taskEnum.Current);
                        taskEnum = waitProcsEnum.Current.Value.Tasks.GetEnumerator();
                        numberFound++;
                    }
                }
            }

            while (_taggedProcesses.ContainsKey(tag))
            {
                var matchEnum = _taggedProcesses[tag].GetEnumerator();
                matchEnum.MoveNext();

                if (CoindexKill(matchEnum.Current))
                    numberFound++;

                RemoveTag(matchEnum.Current);
                _handleToIndex.Remove(_indexToHandle[matchEnum.Current]);
                _indexToHandle.Remove(matchEnum.Current);
            }

            return numberFound;
        }

        /// <summary>
        /// Kills all instances that match both the coroutine handle and the tag on the main Timing instance.
        /// </summary>
        /// <param name="handle">The handle of the coroutine to kill.</param>
        /// <param name="tag">The tag to also match for.</param>
        /// <returns>The number of coroutines that were found and killed (0 or 1).</returns>
        public static int KillCoroutines(CoroutineHandle handle, string tag)
        {
            return _instance == null ? 0 : _instance.KillAllCoroutinesOnInstance(handle, tag);
        }

        /// <summary>
        /// Kills all instances that match both the coroutine handle and the tag on this Timing instance.
        /// </summary>
        /// <param name="handle">The handle of the coroutine to kill.</param>
        /// <param name="tag">The tag to also match for.</param>
        /// <returns>The number of coroutines that were found and killed (0 or 1).</returns>
        public int KillAllCoroutinesOnInstance(CoroutineHandle handle, string tag)
        {
            if (handle == null)
                return 0;
            if (tag == null)
                return KillCoroutinesOnInstance(handle);

            bool foundOne = false;
            var waitProcsEnum = _waitingTriggers.GetEnumerator();
            while (waitProcsEnum.MoveNext())
            {
                if (waitProcsEnum.Current.Key == handle && _processTags.ContainsKey(_handleToIndex[handle]) &&
                    _processTags[_handleToIndex[handle]] == tag)
                {
                    CloseWaitingProcess(waitProcsEnum.Current.Key);
                    break;
                }
            }

            if (_handleToIndex.ContainsKey(handle) && _processTags.ContainsKey(_handleToIndex[handle]) && _processTags[_handleToIndex[handle]] == tag)
            {
                foundOne = CoindexExtract(_handleToIndex[handle]) != null;
                RemoveTag(_handleToIndex[handle]);

                _indexToHandle.Remove(_handleToIndex[handle]);
                _handleToIndex.Remove(handle);
            }

            if (_pausedProcessBuckets.ContainsKey(handle))
            {
                var taskEnum = waitProcsEnum.Current.Value.Tasks.GetEnumerator();
                while (taskEnum.MoveNext())
                {
                    if (taskEnum.Current.Handle == handle && taskEnum.Current.Tag == tag)
                    {
                        waitProcsEnum.Current.Value.Tasks.Remove(taskEnum.Current);
                        _pausedProcessBuckets.Remove(handle);
                        foundOne = true;
                        break;
                    }
                }
            }

            return foundOne ? 1 : 0;
        }

        /// <summary>
        /// Use the command "yield return Timing.WaitUntilDone(otherCoroutine);" to pause the current 
        /// coroutine until otherCoroutine is done.
        /// </summary>
        /// <param name="otherCoroutine">The coroutine to pause for.</param>
        public static float WaitUntilDone(CoroutineHandle otherCoroutine)
        {
            return Instance.WaitUntilDoneOnInstance(otherCoroutine, true);
        }

        /// <summary>
        /// Use the command "yield return Timing.WaitUntilDone(otherCoroutine);" to pause the current 
        /// coroutine until otherCoroutine is done.
        /// </summary>
        /// <param name="otherCoroutine">The coroutine to pause for.</param>
        /// <param name="warnOnIssue">Post a warning to the console if no hold action was actually performed.</param>
        public static float WaitUntilDone(CoroutineHandle otherCoroutine, bool warnOnIssue)
        {
            return Instance.WaitUntilDoneOnInstance(otherCoroutine, warnOnIssue);
        }

        /// <summary>
        /// Use the command "yield return timingInstance.WaitUntilDoneOnInstance(otherCoroutine);" to pause the current 
        /// coroutine until the otherCoroutine is done.
        /// </summary>
        /// <param name="otherCoroutine">The coroutine to pause for.</param>
        /// <param name="warnOnIssue">Post a warning to the console if no hold action was actually performed.</param>
        public float WaitUntilDoneOnInstance(CoroutineHandle otherCoroutine, bool warnOnIssue)
        {
            {
                if (otherCoroutine == null)
                {
                    if (warnOnIssue)
                        Debug.LogWarning("A null handle was passed into WaitUntilDone.");

                    return 0f;
                }

                if (!_waitingTriggers.ContainsKey(otherCoroutine) && _handleToIndex.ContainsKey(otherCoroutine))
                {
                    if (CoindexIsNull(_handleToIndex[otherCoroutine]))
                        return 0f;

                    WaitingProcess newWaitingProcess = new WaitingProcess();
                    newWaitingProcess.Coroutine = CoindexReplace(_handleToIndex[otherCoroutine], _StartWhenDone(otherCoroutine));
                    _waitingTriggers.Add(otherCoroutine, newWaitingProcess);
                }

                if (_waitingTriggers.ContainsKey(otherCoroutine))
                {
                    ReplacementFunction = (coroutine, instance, handle) =>
                    {
                        if (handle == otherCoroutine)
                        {
                            if (warnOnIssue)
                                Debug.LogWarning("A coroutine attempted to wait for itself.");

                            return coroutine;
                        }

                        ProcessIndex index = _handleToIndex[handle];
                        WaitingProcess.ProcessData procData = new WaitingProcess.ProcessData
                        {
                            Handle = handle,
                            Tag = _processTags.ContainsKey(index) ? _processTags[index] : null,
                            Coroutine = coroutine,
                            Segment = index.seg,
                            PauseTime = coroutine.Current > GetSegmentTime(index.seg) ? coroutine.Current - GetSegmentTime(index.seg) : 0d
                        };

                        _waitingTriggers[otherCoroutine].Tasks.Add(procData);
                        _pausedProcessBuckets.Add(handle, _waitingTriggers[otherCoroutine]);
                        return null;
                    };

                    return float.NaN;
                }

                if (_pausedProcessBuckets.ContainsKey(otherCoroutine))
                {
                    var tasksEnum = _pausedProcessBuckets[otherCoroutine].Tasks.GetEnumerator();
                    while (tasksEnum.MoveNext())
                    {
                        if (tasksEnum.Current.Handle == otherCoroutine)
                        {
                            ReplacementFunction = (coroutine, instance, handle) =>
                            {
                                ProcessIndex index = _handleToIndex[handle];
                                WaitingProcess.ProcessData procData = new WaitingProcess.ProcessData
                                {
                                    Handle = handle,
                                    Tag = _processTags.ContainsKey(index) ? _processTags[index] : null,
                                    Coroutine = _InjectYield(coroutine, instance, otherCoroutine, warnOnIssue),
                                    Segment = index.seg,
                                    PauseTime = coroutine.Current > GetSegmentTime(index.seg) ? coroutine.Current - GetSegmentTime(index.seg) : 0d
                                };

                                _pausedProcessBuckets[otherCoroutine].Tasks.Add(procData);
                                _pausedProcessBuckets.Add(handle, _pausedProcessBuckets[otherCoroutine]);

                                return null;
                            };

                            return float.NaN;
                        }
                    }
                }

                if (warnOnIssue)
                    Debug.LogWarning("WaitUntilDone cannot hold: The coroutine handle that was passed in is invalid.\n" + otherCoroutine);

                return 0f;
            }
        }

        private IEnumerator<float> _StartWhenDone(CoroutineHandle handle)
        {
            try
            {
                if (_waitingTriggers[handle].Coroutine.Current > localTime)
                    yield return _waitingTriggers[handle].Coroutine.Current;

                while (_waitingTriggers[handle].Coroutine.MoveNext())
                {
                    yield return _waitingTriggers[handle].Coroutine.Current;
                }
            }
            finally
            {
                CloseWaitingProcess(handle);
            }
        }

        private void CloseWaitingProcess(CoroutineHandle handle)
        {
            if (_waitingTriggers.ContainsKey(handle))
            {
                WaitingProcess processData = _waitingTriggers[handle];
                _waitingTriggers.Remove(handle);
                _pausedProcessBuckets.Remove(handle);

                var tasksEnum = processData.Tasks.GetEnumerator();
                while (tasksEnum.MoveNext())
                {
                    if (tasksEnum.Current == null)
                        continue;

                    RunCoroutineInternal(tasksEnum.Current.PauseTime > 0d ? _InjectDelay(tasksEnum.Current.Coroutine,
                        (float)(GetSegmentTime(tasksEnum.Current.Segment) + tasksEnum.Current.PauseTime)) : tasksEnum.Current.Coroutine,
                        tasksEnum.Current.Segment, tasksEnum.Current.Tag, handle);
                }
            }
        }

        /// <summary>
        /// Use the command "yield return Timing.WaitUntilDone(wwwObject);" to pause the current 
        /// coroutine until the wwwObject is done.
        /// </summary>
        /// <param name="wwwObject">The www object to pause for.</param>
        public static float WaitUntilDone(WWW wwwObject)
        {
            if (wwwObject == null || wwwObject.isDone) return 0f;
            ReplacementFunction = (input, timing, tag) => _StartWhenDone(wwwObject, input);
            return float.NaN;
        }

        private static IEnumerator<float> _StartWhenDone(WWW www, IEnumerator<float> pausedProc)
        {
            while (!www.isDone)
                yield return 0f;

            ReplacementFunction = delegate { return pausedProc; };
            yield return float.NaN;
        }

        /// <summary>
        /// Use the command "yield return Timing.WaitUntilDone(operation);" to pause the current 
        /// coroutine until the operation is done.
        /// </summary>
        /// <param name="operation">The operation variable returned.</param>
        public static float WaitUntilDone(AsyncOperation operation)
        {
            if (operation == null || operation.isDone) return 0f;
            ReplacementFunction = (input, timing, tag) => _StartWhenDone(operation, input);
            return float.NaN;
        }

        private static IEnumerator<float> _StartWhenDone(AsyncOperation operation, IEnumerator<float> pausedProc)
        {
            while (!operation.isDone)
                yield return 0f;

            ReplacementFunction = delegate { return pausedProc; };
            yield return float.NaN;
        }

#if !UNITY_4_6 && !UNITY_4_7 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
        /// <summary>
        /// Use the command "yield return Timing.WaitUntilDone(operation);" to pause the current 
        /// coroutine until the operation is done.
        /// </summary>
        /// <param name="operation">The operation variable returned.</param>
        public static float WaitUntilDone(CustomYieldInstruction operation)
        {
            if (operation == null || !operation.keepWaiting) return 0f;
            ReplacementFunction = (input, timing, tag) => _StartWhenDone(operation, input);
            return float.NaN;
        }

        private static IEnumerator<float> _StartWhenDone(CustomYieldInstruction operation, IEnumerator<float> pausedProc)
        {
            while (operation.keepWaiting)
                yield return 0f;

            ReplacementFunction = delegate { return pausedProc; };
            yield return float.NaN;
        }
#endif

        /// <summary>
        /// Use in a yield return statement to wait for the specified number of seconds.
        /// </summary>
        /// <param name="waitTime">Number of seconds to wait.</param>
        public static float WaitForSeconds(float waitTime)
        {
            if (float.IsNaN(waitTime)) waitTime = 0f;
            return LocalTime + waitTime;
        }

        /// <summary>
        /// Use in a yield return statement to wait for the specified number of seconds.
        /// </summary>
        /// <param name="waitTime">Number of seconds to wait.</param>
        public float WaitForSecondsOnInstance(float waitTime)
        {
            if (float.IsNaN(waitTime)) waitTime = 0f;
            return (float)localTime + waitTime;
        }

        /// <summary>
        /// Calls the specified action after a specified number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action.</param>
        /// <param name="delay">The number of seconds to wait before calling the action.</param>
        /// <param name="action">The action to call.</param>
        public static void CallDelayed<TRef>(TRef reference, float delay, System.Action<TRef> action)
        {
            if (action == null) return;

            if (delay >= -0.001f)
                RunCoroutine(Instance._CallDelayBack(reference, delay, action));
            else
                action(reference);
        }
        /// <summary>
        /// Calls the specified action after a specified number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action.</param>
        /// <param name="delay">The number of seconds to wait before calling the action.</param>
        /// <param name="action">The action to call.</param>
        public void CallDelayedOnInstance<TRef>(TRef reference, float delay, System.Action<TRef> action)
        {
            if (action == null) return;

            if (delay >= -0.001f)
                RunCoroutineOnInstance(_CallDelayBack(reference, delay, action));
            else
                action(reference);
        }

        private IEnumerator<float> _CallDelayBack<TRef>(TRef reference, float delay, System.Action<TRef> action)
        {
            yield return (float)localTime + delay;

            CallDelayed(reference, -1f, action);
        }

        /// <summary>
        /// Calls the specified action after a specified number of seconds.
        /// </summary>
        /// <param name="delay">The number of seconds to wait before calling the action.</param>
        /// <param name="action">The action to call.</param>
        public static void CallDelayed(float delay, System.Action action)
        {
            if (action == null) return;

            if (delay >= -0.0001f)
                RunCoroutine(Instance._CallDelayBack(delay, action));
            else
                action();
        }

        /// <summary>
        /// Calls the specified action after a specified number of seconds.
        /// </summary>
        /// <param name="delay">The number of seconds to wait before calling the action.</param>
        /// <param name="action">The action to call.</param>
        public void CallDelayedOnInstance(float delay, System.Action action)
        {
            if (action == null) return;

            if (delay >= -0.0001f)
                RunCoroutineOnInstance(_CallDelayBack(delay, action));
            else
                action();
        }

        private IEnumerator<float> _CallDelayBack(float delay, System.Action action)
        {
            yield return (float)localTime + delay;

            CallDelayed(-1f, action);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        public static void CallPeriodically(float timeframe, float period, System.Action action, System.Action onDone = null)
        {
            if (action != null)
                RunCoroutine(Instance._CallContinuously(timeframe, period, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        public void CallPeriodicallyOnInstance(float timeframe, float period, System.Action action, System.Action onDone = null)
        {
            if (action != null)
                RunCoroutineOnInstance(_CallContinuously(timeframe, period, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        public static void CallPeriodically(float timeframe, float period, System.Action action, Segment timing, System.Action onDone = null)
        {
            if (action != null)
                RunCoroutine(Instance._CallContinuously(timeframe, period, action, onDone), timing);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        public void CallPeriodicallyOnInstance(float timeframe, float period, System.Action action, Segment timing, System.Action onDone = null)
        {
            if (action != null)
                RunCoroutineOnInstance(_CallContinuously(timeframe, period, action, onDone), timing);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        public static void CallContinuously(float timeframe, System.Action action, System.Action onDone = null)
        {
            if (action != null)
                RunCoroutine(Instance._CallContinuously(timeframe, 0f, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        public void CallContinuouslyOnInstance(float timeframe, System.Action action, System.Action onDone = null)
        {
            if (action != null)
                RunCoroutineOnInstance(_CallContinuously(timeframe, 0f, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action every frame for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        public static void CallContinuously(float timeframe, System.Action action, Segment timing, System.Action onDone = null)
        {
            if (action != null)
                RunCoroutine(Instance._CallContinuously(timeframe, 0f, action, onDone), timing);
        }

        /// <summary>
        /// Calls the supplied action every frame for a given number of seconds.
        /// </summary>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        public void CallContinuouslyOnInstance(float timeframe, System.Action action, Segment timing, System.Action onDone = null)
        {
            if (action != null)
                RunCoroutineOnInstance(_CallContinuously(timeframe, 0f, action, onDone), timing);
        }

        private IEnumerator<float> _CallContinuously(float timeframe, float period, System.Action action, System.Action onDone)
        {
            double startTime = localTime;
            while (localTime <= startTime + timeframe)
            {
                yield return WaitForSeconds(period);

                action();
            }

            if (onDone != null)
                onDone();
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each period.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        public static void CallPeriodically<T>(T reference, float timeframe, float period, System.Action<T> action, System.Action<T> onDone = null)
        {
            if (action != null)
                RunCoroutine(Instance._CallContinuously(reference, timeframe, period, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each period.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        public void CallPeriodicallyOnInstance<T>(T reference, float timeframe, float period, System.Action<T> action, System.Action<T> onDone = null)
        {
            if (action != null)
                RunCoroutineOnInstance(_CallContinuously(reference, timeframe, period, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each period.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        public static void CallPeriodically<T>(T reference, float timeframe, float period, System.Action<T> action, 
            Segment timing, System.Action<T> onDone = null)
        {
            if (action != null)
                RunCoroutine(Instance._CallContinuously(reference, timeframe, period, action, onDone), timing);
        }

        /// <summary>
        /// Calls the supplied action at the given rate for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each period.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="period">The amount of time between calls.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        public void CallPeriodicallyOnInstance<T>(T reference, float timeframe, float period, System.Action<T> action,
            Segment timing, System.Action<T> onDone = null)
        {
            if (action != null)
                RunCoroutineOnInstance(_CallContinuously(reference, timeframe, period, action, onDone), timing);
        }

        /// <summary>
        /// Calls the supplied action every frame for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each frame.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        public static void CallContinuously<T>(T reference, float timeframe, System.Action<T> action, System.Action<T> onDone = null)
        {
            if (action != null)
                RunCoroutine(Instance._CallContinuously(reference, timeframe, 0f, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action every frame for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each frame.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        public void CallContinuouslyOnInstance<T>(T reference, float timeframe, System.Action<T> action, System.Action<T> onDone = null)
        {
            if (action != null)
                RunCoroutineOnInstance(_CallContinuously(reference, timeframe, 0f, action, onDone), Segment.Update);
        }

        /// <summary>
        /// Calls the supplied action every frame for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each frame.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        public static void CallContinuously<T>(T reference, float timeframe, System.Action<T> action, 
            Segment timing, System.Action<T> onDone = null)
        {
            if(action != null)
                RunCoroutine(Instance._CallContinuously(reference, timeframe, 0f, action, onDone), timing);
        }

        /// <summary>
        /// Calls the supplied action every frame for a given number of seconds.
        /// </summary>
        /// <param name="reference">A value that will be passed in to the supplied action each frame.</param>
        /// <param name="timeframe">The number of seconds that this function should run.</param>
        /// <param name="action">The action to call every frame.</param>
        /// <param name="timing">The timing segment to run in.</param>
        /// <param name="onDone">An optional action to call when this function finishes.</param>
        public void CallContinuouslyOnInstance<T>(T reference, float timeframe, System.Action<T> action,
            Segment timing, System.Action<T> onDone = null)
        {
            if (action != null)
                RunCoroutineOnInstance(_CallContinuously(reference, timeframe, 0f, action, onDone), timing);
        }

        private IEnumerator<float> _CallContinuously<T>(T reference, float timeframe, float period,
            System.Action<T> action, System.Action<T> onDone = null)
        {
            double startTime = localTime;
            while (localTime <= startTime + timeframe)
            {
                yield return WaitForSeconds(period);

                action(reference);
            }

            if (onDone != null)
                onDone(reference);
        }

        private class WaitingProcess
        {
            public class ProcessData
            {
                public CoroutineHandle Handle;
                public IEnumerator<float> Coroutine;
                public string Tag;
                public Segment Segment;
                public double PauseTime;
            }

            public IEnumerator<float> Coroutine;
            public readonly HashSet<ProcessData> Tasks = new HashSet<ProcessData>();
        }

        private struct ProcessIndex : System.IEquatable<ProcessIndex>
        {
            public Segment seg;
            public int i;

            public bool Equals(ProcessIndex other)
            {
                return seg == other.seg && i == other.i;
            }

            public override bool Equals(object other)
            {
                if (other is ProcessIndex)
                    return Equals((ProcessIndex)other);
                return false;
            }

            public static bool operator ==(ProcessIndex a, ProcessIndex b)
            {
                return a.seg == b.seg && a.i == b.i;
            }

            public static bool operator !=(ProcessIndex a, ProcessIndex b)
            {
                return a.seg != b.seg || a.i != b.i;
            }

            public override int GetHashCode()
            {
                return (((int)seg - 2) * (int.MaxValue / 3)) + i;
            }
        }

        [System.Obsolete("Unity coroutine function, use RunCoroutine instead.", true)]
        public new Coroutine StartCoroutine(System.Collections.IEnumerator routine) { return null; }

        [System.Obsolete("Unity coroutine function, use RunCoroutine instead.", true)]
        public new Coroutine StartCoroutine(string methodName, object value) { return null; }

        [System.Obsolete("Unity coroutine function, use RunCoroutine instead.", true)]
        public new Coroutine StartCoroutine(string methodName) { return null; }

        [System.Obsolete("Unity coroutine function, use RunCoroutine instead.", true)]
        public new Coroutine StartCoroutine_Auto(System.Collections.IEnumerator routine) { return null; }

        [System.Obsolete("Unity coroutine function, use KillCoroutine instead.", true)]
        public new void StopCoroutine(string methodName) { }

        [System.Obsolete("Unity coroutine function, use KillCoroutine instead.", true)]
        public new void StopCoroutine(System.Collections.IEnumerator routine) { }

        [System.Obsolete("Unity coroutine function, use KillCoroutine instead.", true)]
        public new void StopCoroutine(Coroutine routine) { }

        [System.Obsolete("Unity coroutine function, use KillAllCoroutines instead.", true)]
        public new void StopAllCoroutines() { }

        [System.Obsolete("Use your own GameObject for this.", true)]
        public new static void Destroy(Object obj) { }

        [System.Obsolete("Use your own GameObject for this.", true)]
        public new static void Destroy(Object obj, float f) { }

        [System.Obsolete("Use your own GameObject for this.", true)]
        public new static void DestroyObject(Object obj) { }

        [System.Obsolete("Use your own GameObject for this.", true)]
        public new static void DestroyObject(Object obj, float f) { }

        [System.Obsolete("Use your own GameObject for this.", true)]
        public new static void DestroyImmediate(Object obj) { }

        [System.Obsolete("Use your own GameObject for this.", true)]
        public new static void DestroyImmediate(Object obj, bool b) { }

        [System.Obsolete("Just.. no.", true)]
        public new static T FindObjectOfType<T>() where T : Object { return null; }

        [System.Obsolete("Just.. no.", true)]
        public new static Object FindObjectOfType(System.Type t) { return null; }

        [System.Obsolete("Just.. no.", true)]
        public new static T[] FindObjectsOfType<T>() where T : Object { return null; }

        [System.Obsolete("Just.. no.", true)]
        public new static Object[] FindObjectsOfType(System.Type t) { return null; }

        [System.Obsolete("Just.. no.", true)]
        public new static void print(object message) { }
    }

    public enum Segment
    {
        Update,
        FixedUpdate,
        LateUpdate,
        SlowUpdate,
    }

    public sealed class CoroutineHandle { }
}

public static class MECExtensionMethods
{
    /// <summary>
    /// Cancels this coroutine when the supplied game object is destroyed or made inactive.
    /// </summary>
    /// <param name="coroutine">The coroutine handle to act upon.</param>
    /// <param name="gameObject">The GameObject to test.</param>
    /// <returns>The modified coroutine handle.</returns>
    public static IEnumerator<float> CancelWith(this IEnumerator<float> coroutine, GameObject gameObject)
    {
        while (gameObject && gameObject.activeInHierarchy && coroutine.MoveNext())
            yield return coroutine.Current;
    }

    /// <summary>
    /// Cancels this coroutine when the supplied game objects are destroyed or made inactive.
    /// </summary>
    /// <param name="coroutine">The coroutine handle to act upon.</param>
    /// <param name="gameObject1">The first GameObject to test.</param>
    /// <param name="gameObject2">The second GameObject to test</param>
    /// <returns>The modified coroutine handle.</returns>
    public static IEnumerator<float> CancelWith(this IEnumerator<float> coroutine, GameObject gameObject1, GameObject gameObject2)
    {
        while (gameObject1 && gameObject1.activeInHierarchy && gameObject2 && gameObject2.activeInHierarchy && coroutine.MoveNext())
            yield return coroutine.Current;
    }

    /// <summary>
    /// Cancels this coroutine when the supplied game objects are destroyed or made inactive.
    /// </summary>
    /// <param name="coroutine">The coroutine handle to act upon.</param>
    /// <param name="gameObject1">The first GameObject to test.</param>
    /// <param name="gameObject2">The second GameObject to test</param>
    /// <param name="gameObject3">The third GameObject to test.</param>
    /// <returns>The modified coroutine handle.</returns>
    public static IEnumerator<float> CancelWith(this IEnumerator<float> coroutine,
        GameObject gameObject1, GameObject gameObject2, GameObject gameObject3)
    {
        while (gameObject1 && gameObject1.activeInHierarchy && gameObject2 && gameObject2.activeInHierarchy &&
                gameObject3 && gameObject3.activeInHierarchy && coroutine.MoveNext())
            yield return coroutine.Current;
    }
}
