namespace flow_desktop.Helpers
{
    public class RepeatSongManager
    {
        public const int MaxRepeatCount = 3;
        private const int RepeatForAWhileCount = 10;

        public int RepeatCount { get; private set; } = 0;

        public bool IsRepeating => RepeatCount > 0;

        public bool IsRepeatingForAWhile => RepeatCount > MaxRepeatCount;

        public void ToggleRepeat()
        {
            if (IsRepeatingForAWhile) return;

            var isAtMax = RepeatCount == MaxRepeatCount;

            RepeatCount = isAtMax
                ? RepeatForAWhileCount
                : RepeatCount + 1;
        }

        /// <summary>
        /// uses up one repeat, if there is one.
        ///
        /// returns true when the song should play again.
        /// </summary>
        public bool ConsumeRepeatIfActive()
        {
            if (!IsRepeating) return false;

            RepeatCount -= 1;
            return true;
        }

        /// <summary>
        /// repeats the current song for a while,
        /// for when a song is the whole mood.
        /// e.g. Burna Boy's 23
        /// </summary>
        public void RepeatForAWhile()
        {
            RepeatCount = RepeatForAWhileCount;
        }

        public void Reset()
        {
            RepeatCount = 0;
        }
    }
}