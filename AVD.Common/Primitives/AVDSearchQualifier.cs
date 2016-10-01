using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TE.Common.Primitives
{
    [Serializable]
    public struct TSearchQualifier<T> where T : struct, IConvertible
    {
        private string _value;
        private string _code;
        private T _provider;

        public TSearchQualifier(string resultId)
        {
            _value = resultId;
            _code = null;
            _provider = default(T);
            PraseCodeAndProviderInfo();
        }

        public TSearchQualifier(string code, T searchprovider)
        {
            _code = code;
            _provider = searchprovider;
            _value = null;
            SetSearchQualifierString();
        }

        /// <summary>
        /// Unique ID generated for each search result
        /// </summary>
        public string AvailabilityResultId
        {
            get
            {
                return _value;
            }
            set {
                _value = value;
                PraseCodeAndProviderInfo();
            }
        }

        /// <summary>
        /// Unique code of the search result. In case of hotel, it will be hotel code.
        /// </summary>
        [JsonIgnore]
        public string ProviderPropertyCode
        {
            get
            {
                return _code;
            }
        }

        /// <summary>
        /// Provider of the search result
        /// </summary>
        [JsonIgnore]
        public T Provider
        {
            get
            {
                return _provider;
            }
        }

        private void PraseCodeAndProviderInfo()
        {
            if (!string.IsNullOrEmpty(_value))
            {
                string[] values = _value.Split('_');
                _code = values.Last();
                _provider = (T)Enum.Parse(typeof(T), values.First(), true);
            }
        }

        private void SetSearchQualifierString()
        {
            if (!string.IsNullOrEmpty(_code))
            {
                int providerId = Convert.ToInt32(_provider as Enum);
                _value = providerId + "_" + _code;
            }
        }
    }


}
