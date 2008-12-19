#region License

/*
 * Copyright (C) 2002-2008 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion

#if NET_2_0

#region Imports

using System;
using System.Collections.Generic;
using Spring.Collections.Generic;

#endregion

namespace Spring.Threading.Collections.Generic
{
	/// <summary> 
	/// A <see cref="IQueue{T}"/> that additionally supports operations
	/// that wait for the queue to become non-empty when retrieving an
	/// element, and wait for space to become available in the queue when
	/// storing an element.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="IBlockingQueue{T}"/> methods come in four forms, with different ways
	/// of handling operations that cannot be satisfied immediately, but may be
	/// satisfied at some point in the future:
	/// <list type="bullet">
	/// <item>
	/// one throws an exception,
	/// </item>
	/// <item>
	/// the second returns a special value (either <c>default(T)</c> or 
	/// <see langword="false"/>, depending on the operation), 
	/// </item>
	/// <item>
	/// the third blocks the current thread indefinitely until the operation can succeed,
	/// and 
	/// </item>
	/// <item>
	/// the fourth blocks for only a given maximum time limit before giving
	/// up.
	/// </item>
	/// </list>
	/// </para>
	/// <para>
	/// A <see cref="IBlockingQueue{T}"/> may be capacity bounded. At any given
	/// time it may have a <see cref="IQueue{T}.RemainingCapacity"/> beyond which no
	/// additional elements can be <see cref="IBlockingQueue{T}.Put(T)"/> without blocking.
	/// A <see cref="IBlockingQueue{T}"/> without any intrinsic capacity constraints always
	/// reports a remaining capacity of <see cref="System.Int32.MaxValue"/>.
	/// </para>
	/// <para>
	/// <see cref="IBlockingQueue{T}"/> implementations are designed to be used
	/// primarily for producer-consumer queues, but additionally support
	/// the <see cref="ICollection{T}"/> interface.  So, for example, it is
	/// possible to remove an arbitrary element from a queue using
	/// <see cref="ICollection{T}.Remove(T)"/>. 
	/// However, such operations are in general
	/// <b>not</b> performed very efficiently, and are intended for only
	/// occasional use, such as when a queued message is cancelled.
	/// </para>
	/// <para>
	/// A <see cref="IBlockingQueue{T}"/> does <b>not</b> intrinsically support
	/// any kind of 'close' or 'shutdown' operation to
	/// indicate that no more items will be added.  The needs and usage of
	/// such features tend to be implementation-dependent. For example, a
	/// common tactic is for producers to insert special
	/// <b>end-of-stream</b> or <b>poison</b> objects, that are
	/// interpreted accordingly when taken by consumers.
	/// </para>
	/// <para>
    /// A <see cref="IBlockingQueue{T}"/> can safely be used with multiple
    /// producers and multiple consumers.
	/// </para>
	/// <example>
	/// Usage example, based on a typical producer-consumer scenario. 
	/// TODO: Convert non-generic example below to using generic version
	/// <code>
	/// class Producer : IRunnable {
	///		private IBlockingQueue queue;
	/// 	Producer(IBlockingQueue q) { queue = q; }
	/// 	public void Run() {
	/// 		try {
	/// 			while (true) { 
	/// 				queue.Put(produce()); 
	/// 			}
	/// 		} catch (InterruptedException ex) { 
	/// 			... handle ...
	/// 		}
	/// 	}
	/// 	Object Produce() { ... }
	/// }
	/// 
	/// class Consumer : IRunnable {
	///		private IBlockingQueue queue;
	/// 	Consumer(IBlockingQueue q) { queue = q; }
	/// 	public void Run() {
	/// 		try {
	/// 			while (true) { Consume(queue.Take()); }
	/// 		} catch (InterruptedException ex) { ... handle ...}
	/// 	}
	/// 	void Consume(object x) { ... }
	/// }
	/// 
	/// class Setup {
	///		void Main() {
	/// 		IBlockingQueue q = new SomeQueueImplementation();
	/// 		Producer p = new Producer(q);
	/// 		Consumer c1 = new Consumer(q);
	/// 		Consumer c2 = new Consumer(q);
	/// 		new Thread(new ThreadStart(p.Run)).Start();
	/// 		new Thread(new ThreadStart(c1.Run)).Start();
	/// 		new Thread(new ThreadStart(c2.Run)).Start();
	/// 	}
	/// }
	/// </code>
	/// </example>
	/// </remarks>
    /// <typeparam name="T">The type of the elements in the queue.</typeparam>
    /// <author>Doug Lea</author>
	/// <author>Griffin Caprio(.NET)</author>
	/// <author>Kenneth Xu</author>
	public interface IBlockingQueue<T> : IQueue<T>
	{
		/// <summary> 
		/// Inserts the specified element into this queue, waiting if necessary
		/// for space to become available.
		/// </summary>
		/// <param name="element">the element to add</param>
		/// <exception cref="System.ArgumentNullException">
		/// If the specified element is <see langword="null"/> and this queue 
		/// does not permit <see langword="null"/> elements.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// If some property of the supplied <paramref name="element"/> prevents
		/// it from being added to this queue.
		/// </exception>
		void Put(T element);

		/// <summary> 
		/// Inserts the specified element into this queue, waiting up to the
		/// specified wait time if necessary for space to become available.
		/// </summary>
        /// <param name="element">The element to add.</param>
		/// <param name="duration">How long to wait before giving up.</param>
		/// <returns>
		/// <see langword="true"/> if successful, or <see langword="false"/> if
		/// the specified waiting time elapses before space is available.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// If the specified element is <see langword="null"/> and this queue 
		/// does not permit <see lang="null"/> elements.
		/// </exception>
		/// <exception cref="System.ArgumentException">
        /// If some property of the supplied <paramref name="element"/> prevents
		/// it from being added to this queue.
		/// </exception>
		bool Offer(T element, TimeSpan duration);

		/// <summary> 
		/// Retrieves and removes the head of this queue, waiting if necessary
		/// until an element becomes available.
		/// </summary>
		/// <returns> the head of this queue</returns>
		T Take();

	    /// <summary> 
	    /// Retrieves and removes the head of this queue, waiting up to the
	    /// specified wait time if necessary for an element to become available.
	    /// </summary>
	    /// <param name="element">
	    /// Set to the head of this queue. <c>default(T)</c> if queue is empty.
	    /// </param>
	    /// <param name="duration">How long to wait before giving up.</param>
	    /// <returns> 
	    /// <c>false</c> if the queue is still empty after waited for the time 
	    /// specified by the <paramref name="duration"/>. Otherwise <c>true</c>.
	    /// </returns>
	    bool Poll(TimeSpan duration, out T element);

		/// <summary> 
		/// Removes all available elements from this queue and adds them to the 
		/// given collection.  
		/// </summary>
		/// <remarks>
		/// This operation may be more efficient than repeatedly polling this 
		/// queue.  A failure encountered while attempting to add elements to 
		/// collection <paramref name="collection"/> may result in elements 
		/// being in neither, either or both collections when the associated 
		/// exception is thrown.  Attempts to drain a queue to itself result in
		/// <see cref="System.ArgumentException"/>. Further, the behavior of
		/// this operation is undefined if the specified collection is
		/// modified while the operation is in progress.
		/// </remarks>
		/// <param name="collection">the collection to transfer elements into</param>
		/// <returns> the number of elements transferred</returns>
		/// <exception cref="System.InvalidOperationException">
		/// If the queue cannot be drained at this time.
		/// </exception>
		/// <exception cref="System.InvalidCastException">
		/// If the class of the supplied <paramref name="collection"/> prevents it
		/// from being used for the elemetns from the queue.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// If the specified collection is <see lang="null"/>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// If <paramref name="collection"/> represents the queue itself.
		/// </exception>
		/// <seealso cref="DrainTo(ICollection{T},int)"/>
		int DrainTo(ICollection<T> collection);

//        / <summary> 
//        / Removes all available elements from this queue and invoke the given
//        / <paramref name="action"/> on each element in order.
//        / </summary>
//        / <remarks>
//        / This operation may be more efficient than repeatedly polling this 
//        / queue.  A failure encountered while attempting to invoke the 
//        / <paramref name="action"/> on the elements may result in elements 
//        / being neither, either or both in the queue or processed when the 
//        / associated exception is thrown.
//        / <example> Drain to a non-generic list.
//        / <code language="c#">
//        / IList c = ...;
//        / int count = DrainTo(delegate(T e) {c.Add(e);});
//        / </code>
//        / </example>
//        / </remarks>
//        / <param name="action">The action to performe on each element.</param>
//        / <returns>The number of elements processed.</returns>
//        / <exception cref="System.InvalidOperationException">
//        / If the queue cannot be drained at this time.
//        / </exception>
//        / <exception cref="System.ArgumentNullException">
//        / If the specified action is <see langword="null"/>.
//        / </exception>
//        / <seealso cref="DrainTo(ICollection{T})"/>
//        / <seealso cref="DrainTo(ICollection{T},int)"/>
//        / <seealso cref="DrainTo(Action{T},int)"/>
//        / 
        // TODO: Implement
//        int DrainTo(Action<T> action);


		/// <summary> 
		/// Removes at most the given number of available elements from
		/// this queue and adds them to the given collection.  
		/// </summary>
		/// <remarks> 
		/// This operation may be more
		/// efficient than repeatedly polling this queue.  A failure
		/// encountered while attempting to add elements to
		/// collection <paramref name="collection"/> may result in elements being in neither,
		/// either or both collections when the associated exception is
		/// thrown.  Attempts to drain a queue to itself result in
		/// <see cref="System.ArgumentException"/>. Further, the behavior of
		/// this operation is undefined if the specified collection is
		/// modified while the operation is in progress.
		/// </remarks>
		/// <param name="collection">the collection to transfer elements into</param>
		/// <param name="maxElements">the maximum number of elements to transfer</param>
		/// <returns> the number of elements transferred</returns>
		/// <exception cref="System.InvalidOperationException">
		/// If the queue cannot be drained at this time.
		/// </exception>
		/// <exception cref="System.InvalidCastException">
		/// If the class of the supplied <paramref name="collection"/> prevents it
		/// from being used for the elemetns from the queue.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// If the specified collection is <see lang="null"/>.
		/// </exception>
		/// <exception cref="System.ArgumentException">
		/// If <paramref name="collection"/> represents the queue itself.
		/// </exception>
        /// <seealso cref="DrainTo(ICollection{T})"/>
        int DrainTo(ICollection<T> collection, int maxElements);

       // /// <summary> 
        ///// Removes at most the given number of available elements from this 
        ///// queue and invoke the given <paramref name="action"/> on each 
        ///// element in order.
        ///// </summary>
        ///// <remarks>
//        / This operation may be more efficient than repeatedly polling this 
//        / queue.  A failure encountered while attempting to invoke the 
//        / <paramref name="action"/> on the elements may result in elements 
//        / being neither, either or both in the queue or processed when the 
//        / associated exception is thrown.
//        / </remarks>
//        / <param name="action">The action to performe on each element.</param>
//        / <param name="maxElements">the maximum number of elements to transfer</param>
//        / <returns>The number of elements processed.</returns>
//        / <exception cref="System.InvalidOperationException">
//        / If the queue cannot be drained at this time.
//        / </exception>
//        / <exception cref="System.ArgumentNullException">
//        / If the specified action is <see langword="null"/>.
//        / </exception>
//        / <seealso cref="DrainTo(ICollection{T})"/>
//        / <seealso cref="DrainTo(Action{T})"/>
//        / <seealso cref="DrainTo(ICollection{T},int)"/>
//         TODO: Implement
        //int DrainTo(Action<T> action, int maxElements);

	}
}

#endif