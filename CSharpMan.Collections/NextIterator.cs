using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CSharpMan.Collections
{
    /// <summary>
    /// Wraps an enumeration to support a next item iteration
    /// </summary>
    /// <typeparam name="T">The type enumerated</typeparam>
    public class NextIterator<T>
    {
        private int pointerPosition;

        private readonly IEnumerable<T> source;

        private readonly IQueryable<T> queryableSource; 

        private readonly Expression<Func<T, bool>> predicate;

        private readonly NextIteratorMode mode;

        private bool hasExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="NextIterator{T}"/> class. 
        /// </summary>
        /// <param name="source">
        /// The enumeration to be wrapped
        /// </param>
        /// <param name="mode">
        /// The way to react when iterate after last item
        /// </param>
        public NextIterator(IEnumerable<T> source, NextIteratorMode mode = NextIteratorMode.StopAtEnd)
        {
            this.source = source;
            this.mode = mode;
            this.hasExpression = true;
            this.pointerPosition = 0;
            this.queryableSource = source.AsQueryable();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NextIterator{T}"/> class. 
        /// Wraps an enumeration to support a next item iteration
        /// </summary>
        /// <param name="source">
        /// The enumeration to be wrapped
        /// </param>
        /// <param name="predicate">
        /// The expression to be checked
        /// </param>
        /// <param name="mode">
        /// The way to react when iterate after last item
        /// </param>
        public NextIterator(IEnumerable<T> source, Expression<Func<T, bool>> predicate, NextIteratorMode mode = NextIteratorMode.StopAtEnd)
        {
            this.source = source;
            this.predicate = predicate;
            this.mode = mode;
            this.hasExpression = true;
            this.pointerPosition = 0;
            this.queryableSource = source.AsQueryable();
        }

        /// <summary>
        /// Gets the next element in the enumeration
        /// </summary>
        /// <returns>
        /// The next element in the enumeration or null if the enumeration is at the end.<see cref="T"/>.
        /// </returns>
        public T Next()
        {
            var resultedEnumeration = this.queryableSource.Where(this.predicate).ToArray();
            T next;
            if (this.CanContinue())
            {
                if (this.IsAtEnd(resultedEnumeration) && this.mode == NextIteratorMode.Repeat)
                {
                    this.Repeat();
                }

                next = resultedEnumeration.Skip(this.pointerPosition).Take(1).First();
                this.pointerPosition++;
            }
            else
            {
                next = default(T);
            }

            return next;
        }

        /// <summary>
        /// Checks if can iterate one item more.
        /// </summary>
        /// <returns>Returns true if can get one item more, returns false if no more items are available.</returns>
        public bool CanContinue()
        {
            return this.source.Any();
        }

        private void Repeat()
        {
            this.pointerPosition = 0;
        }

        private bool IsAtEnd(IEnumerable<T> resultedEnumeration)
        {
            return resultedEnumeration.Count() <= this.pointerPosition;
        }

    }
}
