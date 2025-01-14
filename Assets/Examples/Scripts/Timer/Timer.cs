using System;
using UnityEngine;

namespace RuntimeNodeEditor
{
    public class Timer
    {
        #region Public Properties/Fields
        /// <summary>
        /// How long the timer takes to complete from start to finish.
        /// </summary>
        public float duration { get; private set; }

        /// <summary>
        /// Whether the timer will run again after completion.
        /// </summary>
        public bool isLooped { get; set; }

        /// <summary>
        /// Whether or not the timer completed running. This is false if the timer was cancelled.
        /// </summary>
        public bool isCompleted { get; private set; }

        /// <summary>
        /// Whether the timer uses real-time or game-time. Real time is unaffected by changes to the timescale
        /// of the game(e.g. pausing, slow-mo), while game time is affected.
        /// </summary>
        public bool usesRealTime { get; private set; }

        /// <summary>
        /// Whether the timer is currently paused.
        /// </summary>
        public bool isPaused
        {
            get => this.m_TimeElapsedBeforePause.HasValue;
        }

        /// <summary>
        /// Whether or not the timer was cancelled.
        /// </summary>
        public bool isCancelled
        {
            get => this.m_TimeElapsedBeforeCancel.HasValue;
        }

        /// <summary>
        /// Get whether or not the timer has finished running for any reason.
        /// </summary>
        public bool isDone
        {
            get => this.isCompleted || this.isCancelled || this.m_IsOwnerDestroyed;
            set { }
        }

#endregion

        #region Public Static Methods

        /// <summary>
        /// Register a new timer that should fire an event after a certain amount of time
        /// has elapsed.
        ///
        /// Registered timers are destroyed when the scene changes.
        /// </summary>
        /// <param name="duration">The time to wait before the timer should fire, in seconds.</param>
        /// <param name="onComplete">An action to fire when the timer completes.</param>
        /// <param name="onUpdate">An action that should fire each time the timer is updated. Takes the amount
        /// of time passed in seconds since the start of the timer's current loop.</param>
        /// <param name="isLooped">Whether the timer should repeat after executing.</param>
        /// <param name="useRealTime">Whether the timer uses real-time(i.e. not affected by pauses,
        /// slow/fast motion) or game-time(will be affected by pauses and slow/fast-motion).</param>
        /// <param name="autoDestroyOwner">An object to attach this timer to. After the object is destroyed,
        /// the timer will expire and not execute. This allows you to avoid annoying <see cref="NullReferenceException"/>s
        /// by preventing the timer from running and accessessing its parents' components
        /// after the parent has been destroyed.</param>
        /// <returns>A timer object that allows you to examine stats and stop/resume progress.</returns>
        public static Timer Register(float duration, bool isLooped = false, bool useRealTime = false,
            MonoBehaviour autoDestroyOwner = null)
        {
            // create a manager object to update all the timers if one does not already exist.
//            if (m_TimerComponent == null)
//                m_TimerComponent = GameEntry.GetComponent<TimerComponent>();

            Timer timer = new Timer(duration, isLooped, useRealTime, autoDestroyOwner);
//            m_TimerComponent.RegisterTimer(timer);
            return timer;
        }

        /// <summary>
        /// Cancels a timer. The main benefit of this over the method on the instance is that you will not get
        /// a <see cref="NullReferenceException"/> if the timer is null.
        /// </summary>
        /// <param name="timer">The timer to cancel.</param>
        public static void Cancel(Timer timer)
        {
            if (timer != null)
            {
                timer.Cancel();
            }
        }

        /// <summary>
        /// Pause a timer. The main benefit of this over the method on the instance is that you will not get
        /// a <see cref="NullReferenceException"/> if the timer is null.
        /// </summary>
        /// <param name="timer">The timer to pause.</param>
        public static void Pause(Timer timer)
        {
            if (timer != null)
            {
                timer.Pause();
            }
        }

        /// <summary>
        /// Resume a timer. The main benefit of this over the method on the instance is that you will not get
        /// a <see cref="NullReferenceException"/> if the timer is null.
        /// </summary>
        /// <param name="timer">The timer to resume.</param>
        public static void Resume(Timer timer)
        {
            if (timer != null)
            {
                timer.Resume();
            }
        }

        public static void CancelAllRegisteredTimers()
        {
//            if (Timer.m_TimerComponent != null)
//            {
//                Timer.m_TimerComponent.CancelAllTimers();
//            }

            // if the manager doesn't exist, we don't have any registered timers yet, so don't
            // need to do anything in this case
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Stop a timer that is in-progress or paused. The timer's on completion callback will not be called.
        /// </summary>
        public void Cancel()
        {
            if (this.isDone)
            {
                return;
            }

            this.m_TimeElapsedBeforeCancel = this.GetTimeElapsed();
            this.m_TimeElapsedBeforePause = null;
        }

        /// <summary>
        /// Pause a running timer. A paused timer can be resumed from the same point it was paused.
        /// </summary>
        public void Pause()
        {
            if (this.isPaused || this.isDone)
            {
                return;
            }

            this.m_TimeElapsedBeforePause = this.GetTimeElapsed();
        }

        /// <summary>
        /// Continue a paused timer. Does nothing if the timer has not been paused.
        /// </summary>
        public void Resume()
        {
            if (!this.isPaused || this.isDone)
            {
                return;
            }

            this.m_TimeElapsedBeforePause = null;
        }

        /// <summary>
        /// Get how many seconds have elapsed since the start of this timer's current cycle.
        /// </summary>
        /// <returns>The number of seconds that have elapsed since the start of this timer's current cycle, i.e.
        /// the current loop if the timer is looped, or the start if it isn't.
        ///
        /// If the timer has finished running, this is equal to the duration.
        ///
        /// If the timer was cancelled/paused, this is equal to the number of seconds that passed between the timer
        /// starting and when it was cancelled/paused.</returns>
        public float GetTimeElapsed()
        {
            if (this.isCompleted || this.GetWorldTime() >= this.GetFireTime())
            {
                return this.duration;
            }

            return this.m_TimeElapsedBeforeCancel ??
                   this.m_TimeElapsedBeforePause ??
                   this.GetWorldTime() - this.m_StartTime;
        }

        /// <summary>
        /// Get how many seconds remain before the timer completes.
        /// </summary>
        /// <returns>The number of seconds that remain to be elapsed until the timer is completed. A timer
        /// is only elapsing time if it is not paused, cancelled, or completed. This will be equal to zero
        /// if the timer completed.</returns>
        public float GetTimeRemaining()
        {
            return this.duration - this.GetTimeElapsed();
        }

        /// <summary>
        /// Get how much progress the timer has made from start to finish as a ratio.
        /// </summary>
        /// <returns>A value from 0 to 1 indicating how much of the timer's duration has been elapsed.</returns>
        public float GetRatioComplete()
        {
            return this.GetTimeElapsed() / this.duration;
        }

        /// <summary>
        /// Get how much progress the timer has left to make as a ratio.
        /// </summary>
        /// <returns>A value from 0 to 1 indicating how much of the timer's duration remains to be elapsed.</returns>
        public float GetRatioRemaining()
        {
            return this.GetTimeRemaining() / this.duration;
        }

        public void SetDuration(float duration)
        {
            this.duration = duration;
        }

        public void SetUsesRealTime(bool isUsesRealTime)
        {
            this.usesRealTime = isUsesRealTime;
        }

        #endregion

        #region Private Static Properties/Fields

        // responsible for updating all registered timers
        //        private static TimerComponent m_TimerComponent;

        #endregion

        #region Private Properties/Fields

        private bool m_IsOwnerDestroyed
        {
            get { return this.m_HasAutoDestroyOwner && this.m_AutoDestroyOwner == null; }
        }

        private Action m_OnComplete;
        private Action<float> m_OnUpdate;
        private float m_StartTime;
        private float m_LastUpdateTime;

        // for pausing, we push the start time forward by the amount of time that has passed.
        // this will mess with the amount of time that elapsed when we're cancelled or paused if we just
        // check the start time versus the current world time, so we need to cache the time that was elapsed
        // before we paused/cancelled
        private float? m_TimeElapsedBeforeCancel;
        private float? m_TimeElapsedBeforePause;

        // after the auto destroy owner is destroyed, the timer will expire
        // this way you don't run into any annoying bugs with timers running and accessing objects
        // after they have been destroyed
        private readonly MonoBehaviour m_AutoDestroyOwner;
        private readonly bool m_HasAutoDestroyOwner;

        #endregion

        #region Private Constructor (use static Register method to create new timer)

        private Timer(float duration, bool isLooped, bool usesRealTime, MonoBehaviour autoDestroyOwner)
        {
            this.duration = duration;
            //this.m_OnComplete = onComplete;
            //   this.m_OnUpdate = onUpdate;

            this.isLooped = isLooped;
            this.usesRealTime = usesRealTime;

            this.m_AutoDestroyOwner = autoDestroyOwner;
            this.m_HasAutoDestroyOwner = autoDestroyOwner != null;

            this.m_StartTime = this.GetWorldTime();
            this.m_LastUpdateTime = this.m_StartTime;
        }

        #endregion

        #region Private Methods

        private float GetWorldTime()
        {
            return this.usesRealTime ? Time.realtimeSinceStartup : Time.time;
        }

        private float GetFireTime()
        {
            return this.m_StartTime + this.duration;
        }

        private float GetTimeDelta()
        {
            return this.GetWorldTime() - this.m_LastUpdateTime;
        }

        public void Update()
        {
            if (this.isDone)
            {
                return;
            }

            if (this.isPaused)
            {
                this.m_StartTime += this.GetTimeDelta();
                this.m_LastUpdateTime = this.GetWorldTime();
                return;
            }

            this.m_LastUpdateTime = this.GetWorldTime();

            if (this.m_OnUpdate != null)
            {
                this.m_OnUpdate(this.GetTimeElapsed());
            }

            if (this.GetWorldTime() >= this.GetFireTime())
            {

                if (this.m_OnComplete != null)
                {
                    this.m_OnComplete();
                }

                if (this.isLooped)
                {
                    this.m_StartTime = this.GetWorldTime();
                }
                else
                {
                    this.isCompleted = true;
                }
            }
        }

        public void AddEvent(Action completedEvent)
        {
            if (m_OnComplete == null)
                m_OnComplete = completedEvent;
            else
            {
                Delegate[] eventList = m_OnComplete.GetInvocationList();
                if (!Array.Exists(eventList, x => x == (Delegate)completedEvent))
                {
                    m_OnComplete += completedEvent;
                }
            }
        }

        public void AddEvent(Action<float> updateEvent)
        {
            if (m_OnUpdate == null)
                m_OnUpdate = updateEvent;
            else
            {
                Delegate[] eventList = m_OnUpdate.GetInvocationList();
                if (!Array.Exists(eventList, x => x == (Delegate)updateEvent))
                {
                    m_OnUpdate += updateEvent;
                }
            }
        }


        public void ClearCompleteEvent()
        {
            m_OnComplete = null;
        }

        public void ClearUpdateEvent()
        {
            m_OnUpdate = null;
        }

        #endregion
    }
}
