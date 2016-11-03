﻿/*
 *	The MIT License (MIT)
 *
 *	Copyright (c) 2016 Jerry Lee
 *
 *	Permission is hereby granted, free of charge, to any person obtaining a copy
 *	of this software and associated documentation files (the "Software"), to deal
 *	in the Software without restriction, including without limitation the rights
 *	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *	copies of the Software, and to permit persons to whom the Software is
 *	furnished to do so, subject to the following conditions:
 *
 *	The above copyright notice and this permission notice shall be included in all
 *	copies or substantial portions of the Software.
 *
 *	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *	SOFTWARE.
 */

using QuickUnity.Events;
using QuickUnity.Extensions;
using System.Collections.Generic;

namespace QuickUnity.Timers
{
    /// <summary>
    /// Class TimerGroup.
    /// </summary>
    /// <seealso cref="QuickUnity.Events.EventDispatcher"/>
    public class TimerGroup : EventDispatcher, ITimerGroup
    {
        /// <summary>
        /// The timers list.
        /// </summary>
        protected List<ITimer> m_timers;

        /// <summary>
        /// The name of the group.
        /// </summary>
        protected string m_groupName;

        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>The name of the group.</value>
        public string groupName
        {
            get
            {
                return m_groupName;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerGroup"/> class.
        /// </summary>
        /// <param name="groupName">Name of the group.</param>
        /// <param name="autoStart">if set to <c>true</c> [automatic start].</param>
        /// <param name="timers">The timers.</param>
        public TimerGroup(string groupName, bool autoStart = false, params ITimer[] timers)
            : base()
        {
            if (string.IsNullOrEmpty(groupName))
            {
                return;
            }

            m_timers = new List<ITimer>();
            AddTimers(timers);
            m_groupName = groupName;
            TimerManager.instance.AddTimerGroup(this);

            if (autoStart)
            {
                Start();
            }
        }

        /// <summary>
        /// Determines whether this timer group contains the specified timer.
        /// </summary>
        /// <param name="timer">The timer.</param>
        /// <returns>
        /// <c>true</c> if this timer group contains the specified timer; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsTimer(ITimer timer)
        {
            if (m_timers != null && timer != null)
            {
                return m_timers.Contains(timer);
            }

            return false;
        }

        /// <summary>
        /// Adds the timer.
        /// </summary>
        /// <param name="timer">The timer.</param>
        /// <param name="autoStart">if set to <c>true</c> [automatic start].</param>
        public void AddTimer(ITimer timer, bool autoStart = false)
        {
            if (m_timers != null && timer != null)
            {
                m_timers.AddUnique(timer);

                if (autoStart)
                {
                    timer.Start();
                }
            }
        }

        /// <summary>
        /// Adds the timers.
        /// </summary>
        /// <param name="timers">The timers.</param>
        public void AddTimers(params ITimer[] timers)
        {
            if (m_timers != null && timers != null)
            {
                m_timers.AddRangeUnique(timers);
            }
        }

        /// <summary>
        /// Removes the timer.
        /// </summary>
        /// <param name="timer">The timer.</param>
        /// <param name="autoStop">if set to <c>true</c> [automatic stop].</param>
        /// <returns>
        /// <c>true</c> if timer is successfully removed; otherwise, <c>false</c>. This method also
        /// returns false if timer was not found.
        /// </returns>
        public bool RemoveTimer(ITimer timer, bool autoStop = false)
        {
            if (m_timers != null && timer != null)
            {
                bool success = m_timers.Remove(timer);

                if (success)
                {
                    if (autoStop)
                    {
                        timer.Stop();
                    }
                }

                return success;
            }

            return false;
        }

        /// <summary>
        /// Removes all timers.
        /// </summary>
        /// <param name="autoStop">if set to <c>true</c> [automatic stop].</param>
        public void RemoveAllTimers(bool autoStop = false)
        {
            if (m_timers != null)
            {
                if (autoStop)
                {
                    Stop();
                }

                m_timers.Clear();
            }
        }

        /// <summary>
        /// This timer group start timing.
        /// </summary>
        public void Start()
        {
            if (m_timers != null)
            {
                for (int i = 0, length = m_timers.Count; i < length; ++i)
                {
                    ITimer timer = m_timers[i];
                    timer.Start();
                }

                DispatchEvent(new TimerGroupEvent(TimerGroupEvent.TimerGroupStart, this));
            }
        }

        /// <summary>
        /// This timer group pause timing.
        /// </summary>
        public void Pause()
        {
            if (m_timers != null)
            {
                for (int i = 0, length = m_timers.Count; i < length; ++i)
                {
                    ITimer timer = m_timers[i];
                    timer.Pause();
                }

                DispatchEvent(new TimerGroupEvent(TimerGroupEvent.TimerGroupPause, this));
            }
        }

        /// <summary>
        /// This timer group resume timing.
        /// </summary>
        public void Resume()
        {
            if (m_timers != null)
            {
                for (int i = 0, length = m_timers.Count; i < length; ++i)
                {
                    ITimer timer = m_timers[i];
                    timer.Resume();
                }

                DispatchEvent(new TimerGroupEvent(TimerGroupEvent.TimerGroupResume, this));
            }
        }

        /// <summary>
        /// This timer group stop timing.
        /// </summary>
        public void Stop()
        {
            if (m_timers != null)
            {
                for (int i = 0, length = m_timers.Count; i < length; ++i)
                {
                    ITimer timer = m_timers[i];
                    timer.Stop();
                }

                DispatchEvent(new TimerGroupEvent(TimerGroupEvent.TimerGroupStop, this));
            }
        }

        /// <summary>
        /// This timer group resets timing.
        /// </summary>
        public void Reset()
        {
            if (m_timers != null)
            {
                for (int i = 0, length = m_timers.Count; i < length; ++i)
                {
                    ITimer timer = m_timers[i];
                    timer.Reset();
                }

                DispatchEvent(new TimerGroupEvent(TimerGroupEvent.TimerGroupReset, this));
            }
        }

        /// <summary>
        /// Destroys this timer group instance.
        /// </summary>
        public void Destroy()
        {
            Stop();

            if (m_timers != null)
            {
                m_timers.Clear();
                m_timers = null;
            }

            TimerManager.instance.RemoveTimerGroup(this);
        }
    }
}