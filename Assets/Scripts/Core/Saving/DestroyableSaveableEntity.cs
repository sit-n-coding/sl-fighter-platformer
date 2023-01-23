using System.Collections.Generic;

namespace MakersWrath.Saving
{
    public class DestroyableSaveableEntity : SaveableEntity
    {
        public override void RestoreState(object state) {
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                string typeString = saveable.GetType().ToString();
                if (stateDict.ContainsKey(typeString))
                {
                    saveable.RestoreState(stateDict[typeString]);
                } else {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}