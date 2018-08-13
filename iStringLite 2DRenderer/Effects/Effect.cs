using System;

namespace iStringLite_2DRenderer
{
    public abstract class Effect
    {
        public Effect()
        {
            Console.WriteLine("Loaded {0} Effect", this.GetType().Name);
        }
        
        /// <summary>
        /// Updates the VideoBuffer with the Effect's pixel values.
        /// </summary>
        public abstract void update(ref VideoBuffer videoBuffer);
        
        /// <summary>
        /// Resets the Effect to its original state
        /// </summary>
        public abstract void reset();
        
        /// <summary>
        /// Returns whether or not the Effect has completed its cycle
        /// </summary>
        public abstract bool isComplete { get; }
    }
}