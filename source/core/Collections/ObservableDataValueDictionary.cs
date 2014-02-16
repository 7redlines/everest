namespace Se7enRedLines.Collections
{
    public class ObservableDataValueDictionary<TValue> : ObservableDictionary<string, TValue>
    {
        //======================================================
        #region _Public properties_

        public override TValue this[string key]
        {
            get
            {
                TValue obj;
                TryGetValue(key, out obj);
                return obj;
            }
            set { base[key] = value; }
        }

        #endregion
    }
}