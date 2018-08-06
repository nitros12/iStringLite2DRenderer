namespace iStringLite_2DRenderer
{
    public abstract class Effect
    {
        protected VideoBuffer videoBuffer;
        
        public Effect(ref VideoBuffer videoBuffer)
        {
            this.videoBuffer = videoBuffer;
        }
        
        /// <summary>
        /// Updates the VideoBuffer with the Effect's pixel values.
        /// </summary>
        public abstract void update();
        
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