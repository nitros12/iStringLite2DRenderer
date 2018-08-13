using System;
using System.Diagnostics;

namespace iStringLite2DRenderer.Effects
{
    /// <summary>
    /// An abstract Effect class that holds all the core Effect information and provides methods used for randering new Effects.
    /// </summary>
    public abstract class Effect
    {
        protected Stopwatch Timer { get; set; }       // timer used for rendering effects based on an interval
        protected int Interval { get; set; }          // the interval used to with the timer
        public bool isPaused { get; private set; }    // whether or not the timer is running

        /// <summary>
        /// Used when rendering an Effect that does not rely on the timer.
        /// </summary>
        public Effect()
        {
            this.Timer = new Stopwatch();
            this.Interval = 0;
            this.isPaused = true;
            
            Console.WriteLine("Loaded {0} Effect", this.GetType().Name);
        }
        
        /// <summary>
        /// Used when rendering an Effect that relies on the timer.
        /// </summary>
        /// <param name="interval">The interval between renders</param>
        public Effect(int interval)
        {
            this.Interval = interval;
            this.Timer = new Stopwatch();
            this.play();

            Console.WriteLine("Loaded {0} Effect running every {1}ms", this.GetType().Name, interval);
        }
        
        /// <summary>
        /// Updates the VideoBuffer with the Effect's pixel values.
        /// </summary>
        public abstract void update(ref VideoBuffer videoBuffer);
        
        /// <summary>
        /// Resets the Effect to its original state.
        /// </summary>
        public abstract void reset();

        /// <summary>
        /// Pauses the Effect timer and sets isPaused to true.
        /// </summary>
        public void pause()
        {
            Timer.Stop();
            isPaused = true;
        }

        /// <summary>
        /// Starts the Effect timer and sets isPaused to false.
        /// </summary>
        public void play()
        {
            Timer.Start();
            isPaused = false;
        }

        /// <summary>
        /// Returns true when the interval timer has been reached.
        /// </summary>
        /// <returns>Whether or not the interval has been reached</returns>
        public Boolean intervalReached()
        {
            //TODO: restart the timer if true instead of using reset() override to do it
            return Timer.ElapsedMilliseconds >= Interval;
        }

        /// <summary>
        /// Returns whether or not the Effect has completed its cycle
        /// </summary>
        public abstract bool isComplete { get; set; }
    }
}