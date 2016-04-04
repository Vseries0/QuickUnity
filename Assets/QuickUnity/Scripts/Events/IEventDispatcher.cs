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

using System;

namespace QuickUnity.Events
{
    /// <summary>
    /// The IEventDispatcher interface defines methods for adding or removing event listeners, 
    /// checks whether specific types of event listeners are registered, and dispatches events.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IEventDispatcher : IDisposable
    {
        /// <summary>
        /// Registers an event listener object with an EventDispatcher object so that the listener receives notification of an event.
        /// </summary>
        /// <typeparam name="T">The type of event object.</typeparam>
        /// <param name="eventType">The type of event.</param>
        /// <param name="listener">The listener function that processes the event.</param>
        void AddEventListener<T>(string eventType, Action<T> listener) where T : Event;

        /// <summary>
        /// Dispatches the event.
        /// </summary>
        /// <typeparam name="T">The type of event object.</typeparam>
        /// <param name="eventObject">The event object.</param>
        void DispatchEvent<T>(T eventObject) where T : Event;

        /// <summary>
        /// Checks whether the EventDispatcher object has any listeners registered for a specific type of event.
        /// </summary>
        /// <typeparam name="T">The type of event object.</typeparam>
        /// <param name="eventType">The type of event.</param>
        /// <param name="listener">The listener function that processes the event.</param>
        /// <returns>A value of <c>true</c> if a listener of the specified type is registered; <c>false</c> otherwise.</returns>
        bool HasEventListener<T>(string eventType, Action<T> listener) where T : Event;

        /// <summary>
        /// Removes a listener from the EventDispatcher object.
        /// </summary>
        /// <typeparam name="T">The type of event object.</typeparam>
        /// <param name="eventType">The type of event.</param>
        /// <param name="listener">The listener object to remove.</param>
        void RemoveEventListener<T>(string eventType, Action<T> listener) where T : Event;
    }
}
