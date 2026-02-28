using MelonLoader;

namespace PreserveContainerItems
{
    public class PreserveContainerItems : MelonMod
    {
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg($"[NoContainerDecay] v{Info.Version} loaded. Gear in containers will not be destroyed at 0% condition.");
        }
    }
}