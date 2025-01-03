namespace AutoSearch.Tools
{
    public class KeyTools
    {
        public void SendCtrlV()
        {
            SendKeys.SendWait("^v");
            DefaultTimeSleep();
        }
        
        public void SendCtrlA()
        {
            SendKeys.SendWait("^a");
            DefaultTimeSleep();
        }
        
        public void SendEnter()
        {
            SendKeys.SendWait("{ENTER}");
            DefaultTimeSleep();
        }
        
        public void SendDelete()
        {
            SendKeys.SendWait("{DELETE}");
            DefaultTimeSleep();
        }

        private void DefaultTimeSleep()
        {
            System.Threading.Thread.Sleep(500);
        }
    }
}
