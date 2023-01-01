using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetPonies.Test
{
    public class SecretData
    {
        private Dictionary<string, string> _data;

        public SecretData() {
            _data = new Dictionary<string, string>();
        }

        public SecretData(string filename) : this()
        {
            Load(filename);
        }

        public SecretData Load(string filename)
        {
            if(File.Exists(filename))
                _data = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(filename)) ?? new Dictionary<string, string>();
            return this;
        }

        public string? GetSecret(string key, string? _default = null)
        {
            if(_data.ContainsKey(key))
                return _data[key];
            return _default;
        }

        public string GetSecretNotNull(string key, string? _default = null)
            => GetSecret(key, _default) ?? string.Empty;
    }
}
