namespace AutoSearch.Tools
{
    public class KeyTools
    {
        public void SendCtrlV()
        {
            SendKeys.SendWait("^v");
        }
        
        public void SendCtrlA()
        {
            SendKeys.SendWait("^a");
        }
        
        public void SendEnter()
        {
            SendKeys.SendWait("{ENTER}");
        }
        
        public void SendDelete()
        {
            SendKeys.SendWait("{DELETE}");
        }
    }
}
