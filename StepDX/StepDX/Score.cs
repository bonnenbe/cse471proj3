using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StepDX
{
    class Score
    {
        private DateTime startTime;
        private DateTime stopTime;
        private Boolean running;
        public Boolean Running { set { running = value; } get { return running; } }
        private double score;
        public void start()
        {
            running = true;
            startTime = DateTime.Now;
        }
        public void stop()
        {
            stopTime = DateTime.Now;
            running = false;
        }
        public double time()
        {
            if (running)
            {
                score = 30000 - (DateTime.Now - startTime).TotalMilliseconds;
            }
            else
            {
                score = 30000 - (stopTime - startTime).TotalMilliseconds;
            }
            return score;
        }
    }
}
