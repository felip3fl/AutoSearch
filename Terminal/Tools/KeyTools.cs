namespace AutoSearch.Tools
{
    public class KeyTools
    {
        const int DefaultTimeSleepBefore = 500;
        const int DefaulttimeSleepAfter = 0;
        private void SendKeyCombination(string keyCombination, int timeSleepBefore, int timeSleepAfter)
        {
            TimeSleep(timeSleepBefore);
            SendKeys.SendWait(keyCombination);
            TimeSleep(timeSleepAfter);
        }

        public void SendCtrlV(int timeSleepBefore = DefaultTimeSleepBefore, int timeSleepAfter = DefaulttimeSleepAfter)
        {
            SendKeyCombination("^v", timeSleepBefore, timeSleepAfter);
        }

        public void SendCtrlA(int timeSleepBefore = DefaultTimeSleepBefore, int timeSleepAfter = DefaulttimeSleepAfter)
        {
            SendKeyCombination("^a", timeSleepBefore, timeSleepAfter);
        }

        public void SendEnter(int timeSleepBefore = DefaultTimeSleepBefore, int timeSleepAfter = DefaulttimeSleepAfter)
        {
            SendKeyCombination("{ENTER}", timeSleepBefore, timeSleepAfter);
        }

        public void SendDelete(int timeSleepBefore = DefaultTimeSleepBefore, int timeSleepAfter = DefaulttimeSleepAfter)
        {
            SendKeyCombination("{DELETE}", timeSleepBefore, timeSleepAfter);
        }

        public void  SendHome(int timeSleepBefore = DefaultTimeSleepBefore, int timeSleepAfter = DefaulttimeSleepAfter)
        {
            SendKeyCombination("{HOME}", timeSleepBefore, timeSleepAfter);
        }

        private void TimeSleep(int timeInMillisecond)
        {
            Thread.Sleep(timeInMillisecond);
        }

        private void DefaultTimeSleep()
        {
            TimeSleep(500);
        }
    }
}
